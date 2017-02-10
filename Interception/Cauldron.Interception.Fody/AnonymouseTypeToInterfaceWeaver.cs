using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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
                    var newObjCall = instruction.Previous;
                    var ctor = newObjCall.Operand as MethodReference;

                    // TODO - !!!
                    if (ctor == null)
                        continue;

                    var interfaceType = GetInterface(instruction);
                    var anonymousType = ctor.DeclaringType.Resolve();

                    if (!knownTypes.ContainsKey(interfaceType))
                    {
                        var newAnonymousType = new TypeDefinition(anonymousType.Namespace, "Cauldron_AnonymousType" + knownTypes.Count + 1, TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, typeof(object).GetTypeReference().Import());
                        newAnonymousType.Interfaces.Add(interfaceType.Import());

                        knownTypes.Add(interfaceType, newAnonymousType);
                        this.ModuleDefinition.Types.Add(newAnonymousType);

                        var newAnonymousTypeCtor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, this.ModuleDefinition.TypeSystem.Void);
                        var newAnonymousTypeCtorProcessor = newAnonymousTypeCtor.Body.GetILProcessor();
                        newAnonymousTypeCtorProcessor.Append(newAnonymousTypeCtorProcessor.Create(OpCodes.Ldarg_0));
                        newAnonymousTypeCtorProcessor.Append(newAnonymousTypeCtorProcessor.Create(OpCodes.Call, typeof(object).GetTypeDefinition().GetDefaultConstructor().Import()));
                        newAnonymousTypeCtorProcessor.Append(newAnonymousTypeCtorProcessor.Create(OpCodes.Ret));

                        newAnonymousType.Methods.Add(newAnonymousTypeCtor);

                        foreach (var property in interfaceType.Resolve().Properties)
                        {
                            var type = property.PropertyType;
                            var backingFieldName = $"<{property.Name}>i__Field";
                            var backingField = newAnonymousType.Fields.FirstOrDefault(x => x.Name == backingFieldName);

                            if (backingField == null)
                            {
                                backingField = new FieldDefinition(backingFieldName, FieldAttributes.Private, type.Import());
                                newAnonymousType.Fields.Add(backingField);
                            }
                            else
                                backingField.FieldType = type.Import();

                            var propertyMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot;
                            var newProperty = new PropertyDefinition(property.Name, PropertyAttributes.None, type.Import());
                            newProperty.GetMethod = new MethodDefinition("get_" + property.Name, propertyMethodAttributes, type.Import());
                            newProperty.SetMethod = new MethodDefinition("set_" + property.Name, propertyMethodAttributes, this.ModuleDefinition.TypeSystem.Void);
                            newProperty.SetMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, type.Import()));

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

                            newProperty.GetMethod.Overrides.Add(property.GetMethod);
                            newProperty.SetMethod.Overrides.Add(property.SetMethod);
                        }
                    }

                    var targetTypeDef = knownTypes[interfaceType];
                    var sourceVariable = new VariableDefinition(ctor.DeclaringType.Import());
                    VariableDefinition targetVariable = null;

                    if (instruction.Next.OpCode == OpCodes.Stloc)
                        targetVariable = instruction.Next.Operand as VariableDefinition;
                    else
                        targetVariable = new VariableDefinition(targetTypeDef.Import());

                    t.Method.Body.Variables.Add(sourceVariable);
                    t.Method.Body.Variables.Add(targetVariable);
                    t.Method.Body.InitLocals = true;

                    var processor = t.Method.Body.GetILProcessor();
                    var createAndStoreTarget = new Instruction[]
                    {
                        processor.Create(OpCodes.Newobj, targetTypeDef.GetDefaultConstructor()),
                        processor.Create(OpCodes.Stloc, targetVariable)
                    };
                    processor.InsertBefore(t.Method.Body.Instructions.First(), createAndStoreTarget);
                    processor.InsertBefore(instruction, processor.Create(OpCodes.Stloc, sourceVariable));
                    processor.InsertAfter(instruction, processor.Create(OpCodes.Ldloc, targetVariable));

                    var genericTypes = (ctor.DeclaringType as GenericInstanceType).GenericArguments.ToArray();
                    var genericParameters = anonymousType.GenericParameters;

                    foreach (var propertyDef in anonymousType.Properties)
                    {
                        var returnType = genericTypes[genericParameters.FirstOrDefault(x => x.FullName == propertyDef.PropertyType.FullName).Position];
                        var propertyGetter = propertyDef.GetMethod.MakeHostInstanceGeneric(genericTypes);
                        var targetProperty = targetTypeDef.Properties.FirstOrDefault(x => x.Name == propertyDef.Name);

                        if (targetProperty == null)
                        {
                            this.LogError($"The anonymous type in '{t.Method.FullName}' has a property '{propertyDef.Name}' that does not exist in the interface '{interfaceType.FullName}'.");
                            continue;
                        }

                        if (targetProperty.PropertyType.FullName != returnType.FullName)
                        {
                            this.LogError($"The anonymous type in '{t.Method.FullName}' has a property '{propertyDef.Name}' with a type '{returnType.FullName}' that does not match with the interface '{interfaceType.FullName}'. Expected: {targetProperty.PropertyType.FullName}");
                            continue;
                        }

                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, targetVariable));
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldloc, sourceVariable));
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Callvirt, propertyGetter));
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Callvirt, targetProperty.SetMethod));
                    }

                    processor.Remove(instruction);
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

        private TypeReference GetInterface(Instruction instruction)
        {
            var method = instruction.Operand as GenericInstanceMethod;
            return method.GenericArguments[0];
        }
    }
}