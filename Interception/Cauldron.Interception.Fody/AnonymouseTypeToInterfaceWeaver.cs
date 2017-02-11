using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Interception.Fody
{
    public class AnonymouseTypeToInterfaceWeaver : ModuleWeaverBase
    {
        public AnonymouseTypeToInterfaceWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            if (!this.ModuleDefinition.AllReferencedAssemblies().Any(x => x.Name.Name == "Cauldron.Activator"))
            {
                this.LogInfo("Skipping implementation of Cauldron.Activator.AnonymousTypeWithInterfaceExtension.CreateObject<>. Cauldron.Activator is not referenced in the project");
                return;
            }

            this.LogInfo("Implementing Cauldron.Activator.AnonymousTypeWithInterfaceExtension.CreateObject<>");

            var allMethodsWithMethodOfCalls = this.GetMethodsWhere(x =>
            {
                if (x.OpCode != OpCodes.Call)
                    return false;

                var methodReference = x.Operand as MethodReference;

                if (methodReference.Name == "CreateObject" && methodReference.DeclaringType.Namespace == "Cauldron.Activator")
                    return true;

                return false;
            });
            var knownTypes = new Dictionary<TypeReference, TypeDefinition>();

            foreach (var t in allMethodsWithMethodOfCalls)
            {
                foreach (var instruction in t.Instruction)
                {
                    this.LogInfo("Implementing anonymouse type CreateObject: " + t.Method.FullName);
                    TypeDefinition anonymousType = null;
                    TypeReference declaringType = null;
                    var previousInstruction = instruction.Previous;

                    if (previousInstruction.Operand is MethodReference)
                    {
                        declaringType = (previousInstruction.Operand as MethodReference).DeclaringType;
                        anonymousType = declaringType.Resolve();
                    }
                    else if (previousInstruction.OpCode == OpCodes.Ldarg_1 ||
                       previousInstruction.OpCode == OpCodes.Ldarg_2 ||
                       previousInstruction.OpCode == OpCodes.Ldarg_3 ||
                       previousInstruction.OpCode == OpCodes.Ldarg_S)
                    {
                        TypeReference parameter;

                        if (previousInstruction.OpCode == OpCodes.Ldloc_0)
                            parameter = t.Method.Parameters[0].ParameterType;
                        else if (previousInstruction.OpCode == OpCodes.Ldloc_1)
                            parameter = t.Method.Parameters[1].ParameterType;
                        else if (previousInstruction.OpCode == OpCodes.Ldloc_2)
                            parameter = t.Method.Parameters[2].ParameterType;
                        else if (previousInstruction.OpCode == OpCodes.Ldloc_3)
                            parameter = t.Method.Parameters[3].ParameterType;
                        else
                            parameter = t.Method.Parameters[(int)previousInstruction.Operand].ParameterType;

                        declaringType = parameter;
                        anonymousType = declaringType.Resolve();
                    }
                    else if (previousInstruction.OpCode == OpCodes.Ldloc_0 ||
                        previousInstruction.OpCode == OpCodes.Ldloc_1 ||
                        previousInstruction.OpCode == OpCodes.Ldloc_2 ||
                        previousInstruction.OpCode == OpCodes.Ldloc_3 ||
                        previousInstruction.OpCode == OpCodes.Ldloc_S)
                    {
                        VariableReference local;

                        if (previousInstruction.OpCode == OpCodes.Ldloc_0)
                            local = t.Method.Body.Variables[0];
                        else if (previousInstruction.OpCode == OpCodes.Ldloc_1)
                            local = t.Method.Body.Variables[1];
                        else if (previousInstruction.OpCode == OpCodes.Ldloc_2)
                            local = t.Method.Body.Variables[2];
                        else if (previousInstruction.OpCode == OpCodes.Ldloc_3)
                            local = t.Method.Body.Variables[3];
                        else
                            local = previousInstruction.Operand as VariableReference;

                        declaringType = local.VariableType;
                        anonymousType = declaringType.Resolve();
                    }
                    else if (previousInstruction.OpCode == OpCodes.Ldfld || previousInstruction.OpCode == OpCodes.Ldsfld)
                    {
                        var field = previousInstruction.Operand as FieldReference;

                        declaringType = field.FieldType;
                        anonymousType = declaringType.Resolve();
                    }
                    else
                    {
                        this.LogWarning($"Unable to implement CreateObject<> in '{t.Method.Name}'. The anonymouse type was not found.");
                        continue;
                    }

                    var interfaceType = GetInterface(instruction);

                    if (!knownTypes.ContainsKey(interfaceType))
                        knownTypes.Add(interfaceType, this.CreateAnonymouseType(anonymousType.Namespace, $"<{interfaceType.Name}>Cauldron_AnonymousType" + knownTypes.Count + 1, interfaceType));

                    instruction.Operand = this.CreateAssignMethod(t.Method, declaringType, instruction.Operand as MethodReference, knownTypes[interfaceType]);
                }
            }
        }

        protected override void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            throw new NotImplementedException();
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            throw new NotImplementedException();
        }

        private TypeDefinition CreateAnonymouseType(string namespaceName, string name, TypeReference interfaceType)
        {
            var newAnonymousType = new TypeDefinition(namespaceName, name, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable, typeof(object).GetTypeReference().Import());
            newAnonymousType.Interfaces.Add(interfaceType.Import());
            this.ModuleDefinition.Types.Add(newAnonymousType);

            newAnonymousType.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

            var newAnonymousTypeCtor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, this.ModuleDefinition.TypeSystem.Void);
            var newAnonymousTypeCtorProcessor = newAnonymousTypeCtor.Body.GetILProcessor();
            newAnonymousTypeCtorProcessor.Append(newAnonymousTypeCtorProcessor.Create(OpCodes.Ldarg_0));
            newAnonymousTypeCtorProcessor.Append(newAnonymousTypeCtorProcessor.Create(OpCodes.Call, typeof(object).GetTypeDefinition().GetDefaultConstructor().Import()));
            newAnonymousTypeCtorProcessor.Append(newAnonymousTypeCtorProcessor.Create(OpCodes.Ret));

            newAnonymousType.Methods.Add(newAnonymousTypeCtor);

            foreach (var property in interfaceType.Resolve().GetProperties())
            {
                var type = property.PropertyType.Import();
                var backingFieldName = $"<{property.Name}>k_BackingField";
                var backingField = new FieldDefinition(backingFieldName, FieldAttributes.Private, type);
                var propertyMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot;
                var newProperty = new PropertyDefinition(property.Name, PropertyAttributes.None, type);
                newAnonymousType.Fields.Add(backingField);
                backingField.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);

                newProperty.GetMethod = new MethodDefinition("get_" + property.Name, propertyMethodAttributes, type);
                newProperty.SetMethod = new MethodDefinition("set_" + property.Name, propertyMethodAttributes, this.ModuleDefinition.TypeSystem.Void);
                newProperty.SetMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, type));

                newProperty.GetMethod.CustomAttributes.Add(new CustomAttribute(typeof(CompilerGeneratedAttribute).GetMethodReference(".ctor", 0).Import()));
                newProperty.SetMethod.CustomAttributes.Add(new CustomAttribute(typeof(CompilerGeneratedAttribute).GetMethodReference(".ctor", 0).Import()));

                var getterProcessor = newProperty.GetMethod.Body.GetILProcessor();
                var setterProcessor = newProperty.SetMethod.Body.GetILProcessor();

                getterProcessor.Append(getterProcessor.Create(OpCodes.Ldarg_0));
                getterProcessor.Append(getterProcessor.Create(OpCodes.Ldfld, backingField));
                getterProcessor.Append(getterProcessor.Create(OpCodes.Ret));

                setterProcessor.Append(setterProcessor.Create(OpCodes.Ldarg_0));
                setterProcessor.Append(setterProcessor.Create(OpCodes.Ldarg_1));
                setterProcessor.Append(setterProcessor.Create(OpCodes.Stfld, backingField));
                setterProcessor.Append(setterProcessor.Create(OpCodes.Ret));

                newAnonymousType.Properties.Add(newProperty);
                newAnonymousType.Methods.Add(newProperty.GetMethod);
                newAnonymousType.Methods.Add(newProperty.SetMethod);

                if (property.GetMethod != null)
                    newProperty.GetMethod.Overrides.Add(property.GetMethod);
                if (property.SetMethod != null)
                    newProperty.SetMethod.Overrides.Add(property.SetMethod);
            }

            // Implement NotImplementedException
            foreach (var method in interfaceType.Resolve().GetMethods().Where(x => !x.Name.StartsWith("get_") && !x.Name.StartsWith("set_")))
            {
                var newMethod = new MethodDefinition(method.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final, method.ReturnType.Import());
                foreach (var item in method.Parameters)
                    newMethod.Parameters.Add(new ParameterDefinition(item.ParameterType));
                newAnonymousType.Methods.Add(newMethod);
                newMethod.Overrides.Add(method);

                var processor = newMethod.Body.GetILProcessor();
                processor.Append(processor.Create(OpCodes.Ldstr, "Generated by Cauldron Toolkit"));
                processor.Append(processor.Create(OpCodes.Newobj, typeof(NotImplementedException).GetMethodReference(".ctor", new Type[] { typeof(string) }).Import()));
                processor.Append(processor.Create(OpCodes.Throw));
            }

            return newAnonymousType;
        }

        private MethodReference CreateAssignMethod(MethodDefinition containingMethod, TypeReference anonType, MethodReference createObject, TypeDefinition newTypeDef)
        {
            var containingMethodDeclaringTypeDef = containingMethod.DeclaringType.Resolve();
            var targetInterfaceTypeRef = (createObject as GenericInstanceMethod).GenericArguments[0].Import();
            var assignMethod = new MethodDefinition("<>anon_assign_" + containingMethod.DeclaringType.Methods.Count, MethodAttributes.Static | MethodAttributes.Private, targetInterfaceTypeRef);
            assignMethod.Parameters.Add(new ParameterDefinition(anonType));
            containingMethod.DeclaringType.Methods.Add(assignMethod);

            var targetVariable = new VariableDefinition(newTypeDef);
            var processor = assignMethod.Body.GetILProcessor();
            assignMethod.Body.Variables.Add(targetVariable);
            assignMethod.Body.InitLocals = true;

            processor.Append(processor.Create(OpCodes.Newobj, newTypeDef.GetDefaultConstructor()));
            processor.Append(processor.Create(OpCodes.Stloc, targetVariable));

            var genericTypes = (anonType as GenericInstanceType).GenericArguments.ToArray();
            var genericParameters = anonType.Resolve().GenericParameters;
            var sourceTypeDef = anonType.Resolve();

            foreach (var propertyDef in sourceTypeDef.Properties)
            {
                var returnType = genericTypes[genericParameters.FirstOrDefault(x => x.FullName == propertyDef.PropertyType.FullName).Position];
                var propertyGetter = propertyDef.GetMethod.MakeHostInstanceGeneric(genericTypes);
                var targetProperty = newTypeDef.Properties.FirstOrDefault(x => x.Name == propertyDef.Name);

                if (targetProperty == null)
                {
                    this.LogError($"The anonymous type in '{containingMethod.Name}' has a property '{propertyDef.Name}' that does not exist in the interface '{targetInterfaceTypeRef.Name}'.");
                    continue;
                }

                if (targetProperty.PropertyType.FullName != returnType.FullName)
                {
                    this.LogError($"The anonymous type in '{containingMethod.Name}' has a property '{propertyDef.Name}' with a type '{returnType.Name}' that does not match with the interface '{targetInterfaceTypeRef.Name}'. Expected: {targetProperty.PropertyType.Name}");
                    continue;
                }

                processor.Append(processor.Create(OpCodes.Ldloc, targetVariable));
                processor.Append(processor.Create(OpCodes.Ldarg_0));
                processor.Append(processor.Create(OpCodes.Callvirt, propertyGetter));
                processor.Append(processor.Create(OpCodes.Callvirt, targetProperty.SetMethod));
            }

            processor.Append(processor.Create(OpCodes.Ldloc, targetVariable));
            processor.Append(processor.Create(OpCodes.Isinst, targetInterfaceTypeRef.Import()));
            processor.Append(processor.Create(OpCodes.Ret));

            return assignMethod;
        }

        private TypeReference GetInterface(Instruction instruction)
        {
            var method = instruction.Operand as GenericInstanceMethod;
            return method.GenericArguments[0];
        }
    }
}