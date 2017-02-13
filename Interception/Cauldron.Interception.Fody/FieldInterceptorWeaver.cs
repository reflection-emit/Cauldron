using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Diagnostics;
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
            var methodsWithLoadStoreFields = this.GetMethodsWhere(field.DeclaringType, x => (x.OpCode == OpCodes.Ldfld || x.OpCode == OpCodes.Ldsfld || x.OpCode == OpCodes.Stsfld || x.OpCode == OpCodes.Stfld) && (x.Operand as FieldReference).Resolve().FullName == field.FullName);
            var propertyMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Private;

            var fieldType = field.FieldType.IsGenericParameter ? field.FieldType : field.FieldType.Import();
            var declaringType = field.DeclaringType;
            declaringType.Fields.Remove(field);
            var newField = new FieldDefinition($"<{field.Name}>k__BackingField", field.Attributes, fieldType);
            newField.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
            declaringType.Fields.Add(newField);

            if (newField.IsStatic)
                propertyMethodAttributes |= MethodAttributes.Static;
            var propertyName = field.Name;
            var property = new PropertyDefinition(propertyName, PropertyAttributes.None, fieldType);
            property.GetMethod = new MethodDefinition("get_" + propertyName, propertyMethodAttributes, fieldType);
            property.SetMethod = new MethodDefinition("set_" + propertyName, propertyMethodAttributes, this.ModuleDefinition.TypeSystem.Void);
            property.SetMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, fieldType));

            foreach (var customAttribute in attributes)
                property.CustomAttributes.Add(customAttribute);

            property.GetMethod.CustomAttributes.Add(new CustomAttribute(typeof(CompilerGeneratedAttribute).GetMethodReference(".ctor", 0).Import()));
            property.SetMethod.CustomAttributes.Add(new CustomAttribute(typeof(CompilerGeneratedAttribute).GetMethodReference(".ctor", 0).Import()));

            var getterProcessor = property.GetMethod.Body.GetILProcessor();
            var setterProcessor = property.SetMethod.Body.GetILProcessor();
            var fieldReference = newField.CreateFieldReference();

            if (!newField.IsStatic)
                getterProcessor.Append(getterProcessor.Create(OpCodes.Ldarg_0));
            getterProcessor.Append(getterProcessor.Create(newField.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, fieldReference));
            getterProcessor.Append(getterProcessor.Create(OpCodes.Ret));

            if (!newField.IsStatic)
                setterProcessor.Append(setterProcessor.Create(OpCodes.Ldarg_0));
            setterProcessor.Append(setterProcessor.Create(newField.IsStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
            setterProcessor.Append(setterProcessor.Create(newField.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, fieldReference));
            setterProcessor.Append(setterProcessor.Create(OpCodes.Ret));

            declaringType.Properties.Add(property);
            declaringType.Methods.Add(property.GetMethod);
            declaringType.Methods.Add(property.SetMethod);

            foreach (var method in methodsWithLoadStoreFields.Where(x => x.Method.Name != ".ctor" && x.Method.Name != ".cctor"))
            {
                var processor = method.Method.Body.GetILProcessor();

                foreach (var instruction in method.Instruction)
                {
                    if (instruction.OpCode == OpCodes.Ldsfld || instruction.OpCode == OpCodes.Ldfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Call, property.GetMethod.CreateMethodReference()));
                    else if (instruction.OpCode == OpCodes.Stsfld || instruction.OpCode == OpCodes.Stfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Call, property.SetMethod.CreateMethodReference()));

                    processor.Remove(instruction);
                }
            }

            foreach (var method in methodsWithLoadStoreFields.Where(x => x.Method.Name == ".ctor" || x.Method.Name == ".cctor"))
            {
                var processor = method.Method.Body.GetILProcessor();

                foreach (var instruction in method.Instruction)
                {
                    if (instruction.OpCode == OpCodes.Ldsfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldsfld, fieldReference));
                    if (instruction.OpCode == OpCodes.Ldfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Ldfld, fieldReference));
                    else if (instruction.OpCode == OpCodes.Stsfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Stsfld, fieldReference));
                    else if (instruction.OpCode == OpCodes.Stfld)
                        processor.InsertBefore(instruction, processor.Create(OpCodes.Stfld, fieldReference));

                    processor.Remove(instruction);
                }
            }
        }
    }
}