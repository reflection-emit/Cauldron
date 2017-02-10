using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Interception.Fody
{
    public class FieldInterceptorWeaver : PropertyInterceptorWeaver
    {
        public FieldInterceptorWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            this.LogInfo("Implementing Field interceptors");
            var fieldInterceptors = GetPropertyInterceptorTypes();

            var fieldsAndAttributes = this.ModuleDefinition.Types.Where(x => x.HasFields).SelectMany(x => x.Fields).Where(x => x.HasCustomAttributes)
                .Select(x => new { Field = x, Attributes = x.CustomAttributes.Where(y => fieldInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0)
                .ToArray();

            foreach (var nonPrivateFields in fieldsAndAttributes.Where(x => !x.Field.IsPrivate))
                this.LogError($"The FieldInterceptor does not support fields that are not private. '{nonPrivateFields.Field.FullName}'");

            foreach (var field in fieldsAndAttributes.Where(x => x.Field.IsPrivate))
                this.ImplementField(field.Field, field.Attributes);
        }

        private void ImplementField(FieldDefinition field, CustomAttribute[] attributes)
        {
            this.LogInfo($"Implementing Field interception: {field.FullName} with {string.Join(", ", attributes.Select(x => x.AttributeType.FullName))}");

            var methodsWithLoadStoreFields = this.GetMethodsWhere(x => (x.OpCode == OpCodes.Ldfld || x.OpCode == OpCodes.Ldsfld || x.OpCode == OpCodes.Stsfld || x.OpCode == OpCodes.Stfld) && (x.Operand as FieldDefinition)?.FullName == field.FullName);
            var propertyMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Private;
            if (field.IsStatic)
                propertyMethodAttributes |= MethodAttributes.Static;
            var propertyName = field.Name;
            var property = new PropertyDefinition(propertyName, PropertyAttributes.None, field.FieldType.Import());
            property.GetMethod = new MethodDefinition("get_" + propertyName, propertyMethodAttributes, field.FieldType.Import());
            property.SetMethod = new MethodDefinition("set_" + propertyName, propertyMethodAttributes, this.ModuleDefinition.TypeSystem.Void);
            property.SetMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, field.FieldType.Import()));

            foreach (var customAttribute in attributes)
            {
                property.CustomAttributes.Add(customAttribute);
                field.CustomAttributes.Remove(customAttribute);
            }

            property.GetMethod.CustomAttributes.Add(new CustomAttribute(typeof(CompilerGeneratedAttribute).GetMethodReference(".ctor", 0).Import()));
            property.SetMethod.CustomAttributes.Add(new CustomAttribute(typeof(CompilerGeneratedAttribute).GetMethodReference(".ctor", 0).Import()));

            var getterProcessor = property.GetMethod.Body.GetILProcessor();
            var setterProcessor = property.SetMethod.Body.GetILProcessor();

            if (!field.IsStatic)
                getterProcessor.Append(getterProcessor.Create(OpCodes.Ldarg_0));
            getterProcessor.Append(getterProcessor.Create(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field));
            getterProcessor.Append(getterProcessor.Create(OpCodes.Ret));

            if (!field.IsStatic)
                setterProcessor.Append(setterProcessor.Create(OpCodes.Ldarg_0));
            setterProcessor.Append(setterProcessor.Create(field.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
            setterProcessor.Append(setterProcessor.Create(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));
            setterProcessor.Append(setterProcessor.Create(OpCodes.Ret));

            field.Name = $"<{field.Name}>k_BackingField";

            field.DeclaringType.Properties.Add(property);
            field.DeclaringType.Methods.Add(property.GetMethod);
            field.DeclaringType.Methods.Add(property.SetMethod);

            foreach (var method in methodsWithLoadStoreFields.Where(x => x.Method.Name != ".ctor" && x.Method.Name != ".cctor"))
            {
                var processor = method.Method.Body.GetILProcessor();

                foreach (var instruction in method.Instruction)
                {
                    if (instruction.OpCode == OpCodes.Ldsfld || instruction.OpCode == OpCodes.Ldfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Call, property.GetMethod));
                    else if (instruction.OpCode == OpCodes.Stsfld || instruction.OpCode == OpCodes.Stfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Call, property.SetMethod));

                    processor.Remove(instruction);
                }
            }
        }
    }
}