using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Cauldron.Interception.Fody
{
    public class PropertyInterceptorWeaver : ModuleWeaverBase
    {
        private string lockablePropertyInterceptor = "Cauldron.Core.Interceptors.ILockablePropertyInterceptor";

        private string propertyInterceptor = "Cauldron.Core.Interceptors.IPropertyInterceptor";

        public PropertyInterceptorWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            var propertyInterceptorInterface = this.GetType(this.propertyInterceptor);
            if (propertyInterceptorInterface == null)
                throw new Exception($"Unable to find the interface {this.propertyInterceptor}.");

            var propertyInterceptors = propertyInterceptorInterface.GetTypesThatImplementsInterface()
                .Concat(this.GetType(this.lockablePropertyInterceptor).GetTypesThatImplementsInterface());

            // find all types with methods that are decorated with any of the found property interceptors
            var propertiesAndAttributes = this.ModuleDefinition.Types.SelectMany(x => x.Properties).Where(x => x.HasCustomAttributes)
                .Select(x => new { Property = x, Attributes = x.CustomAttributes.Where(y => propertyInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0)
                .ToArray();

            foreach (var method in propertiesAndAttributes)
                this.ImplementProperty(method.Property, method.Attributes);
        }

        protected override VariableDefinition CreateParameterObject(MethodDefinition method, ILProcessor processor, List<Instruction> attributeInitialization, TypeReference objectReference, ArrayType parametersArrayTypeRef) => null;

        protected override void ImplementLockableOnEnter(ILProcessor processor, List<Instruction> onEnterInstructions, MethodDefinition method, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition methodBaseReference, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            throw new NotImplementedException();
        }

        protected override void ImplementOnEnter(ILProcessor processor, List<Instruction> onEnterInstructions, MethodDefinition method, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition methodBaseReference, VariableDefinition parametersArrayVariable, List<Instruction> originalBody)
        {
            var property = method.GetPropertyDefinition();
            var backingField = originalBody.FirstOrDefault(x => x.OpCode == OpCodes.Ldfld).Operand as FieldDefinition;

            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, attributeVariable));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldtoken, method.DeclaringType));
            onEnterInstructions.Add(processor.Create(OpCodes.Call, this.ModuleDefinition.Import(typeof(Type).GetMethodReference("GetTypeFromHandle", 1))));
            onEnterInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldstr, property.Name));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldloc_S, methodBaseReference));
            onEnterInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldfld, backingField));

            if (backingField.FieldType.IsValueType)
                onEnterInstructions.Add(processor.Create(OpCodes.Box, backingField.FieldType));

            onEnterInstructions.Add(processor.Create(method.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            onEnterInstructions.Add(processor.Create(OpCodes.Ldftn, method.DeclaringType.GetMethodReference($"<{property.Name}>_BackingField_Setter", 1)));
            onEnterInstructions.Add(processor.Create(OpCodes.Newobj, this.ModuleDefinition.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }))));

            onEnterInstructions.Add(processor.Create(OpCodes.Callvirt, interceptorOnEnter));
        }

        private MethodDefinition CreateSetterDelegate(PropertyDefinition property, bool isStatic, FieldDefinition variable)
        {
            var methodAttributes = Mono.Cecil.MethodAttributes.Private | Mono.Cecil.MethodAttributes.HideBySig | (isStatic ? Mono.Cecil.MethodAttributes.Static : (Mono.Cecil.MethodAttributes)0);
            var method = new MethodDefinition($"<{property.Name}>_BackingField_Setter", methodAttributes, this.ModuleDefinition.TypeSystem.Void);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, this.ModuleDefinition.Import(typeof(object).GetTypeReference())));

            var processor = method.Body.GetILProcessor();

            processor.Append(processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_1));
            processor.Append(processor.Create(variable.FieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, this.ModuleDefinition.Import(variable.FieldType)));
            processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, variable));
            processor.Append(processor.Create(OpCodes.Ret));

            property.DeclaringType.Methods.Add(method);

            return method;
        }

        private void ImplementProperty(PropertyDefinition property, CustomAttribute[] attributes)
        {
            // Before we proceed we have to check if this is an auto property...
            // The first version of our property interceptor only works for auto properties...
            // Other features should come later... TODO Dariusz

            if (!property.GetMethod.HasCustomAttributes || !property.GetMethod.CustomAttributes.Any(x => x.AttributeType.Name == "CompilerGeneratedAttribute"))
            {
                this.LogWarning($"{property.Name}: The current version of the property interceptor only supports auto-properties.");
                return;
            }

            // If the getter is static... then the property itself is static
            var isStatic = property.GetMethod.IsStatic;
            var field = property.GetMethod.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Ldfld).Operand as FieldDefinition;

            this.CreateSetterDelegate(property, isStatic, field);
            this.ImplementMethod(property.GetMethod, attributes, (r, isLockable) => isLockable ? r.GetMethodReference("OnGet", 7) : r.GetMethodReference("OnGet", 6));

            // Remove compiler generated attribute
        }
    }
}