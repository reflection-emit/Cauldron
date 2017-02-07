using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public class PropertyInterceptorWeaver : ModuleWeaverBase
    {
        private string lockablePropertyInterceptor = "Cauldron.Core.Interceptors.ILockablePropertyInterceptor";

        private TypeReference propertyInterceptionInfoReference;
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

            this.propertyInterceptionInfoReference = this.GetType("Cauldron.Core.Interceptors.PropertyInterceptionInfo").Import();

            // find all types with methods that are decorated with any of the found property interceptors
            var propertiesAndAttributes = this.ModuleDefinition.Types.SelectMany(x => x.Properties).Where(x => x.HasCustomAttributes)
                .Select(x => new { Property = x, Attributes = x.CustomAttributes.Where(y => propertyInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0)
                .ToArray();

            foreach (var method in propertiesAndAttributes)
                this.ImplementProperty(method.Property, method.Attributes);
        }

        protected override void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            throw new NotImplementedException();
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            var isStatic = methodWeaverInfo.MethodDefinition.IsStatic;
            var fieldAttributes = FieldAttributes.Private;
            if (isStatic)
                fieldAttributes |= FieldAttributes.Static;

            //var property = methodWeaverInfo.MethodDefinition.GetPropertyDefinition();
            //var propertyTypeDefinition = property.PropertyType.Import();
            var backingField = methodWeaverInfo.OriginalBody.FirstOrDefault(x => x.OpCode == OpCodes.Ldfld).Operand as FieldDefinition;
            //var propertyWeaverInfo = methodWeaverInfo.MethodDefinition.DeclaringType.Fields.FirstOrDefault(x => x.Name == $"<{property.Name}>_interceptor_info_{methodWeaverInfo.Id}");

            //methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(methodWeaverInfo.MethodDefinition.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            //methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, propertyWeaverInfo));
            //methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(methodWeaverInfo.MethodDefinition.IsStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            //methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, backingField));

            //if (backingField.FieldType.IsValueType)
            //    methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Box, backingField.FieldType));

            //methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Callvirt, interceptorOnEnter));

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, attributeVariable));
            if (isStatic)
            {
                methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, methodWeaverInfo.MethodBaseField));
                methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, backingField));
            }
            else
            {
                methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, methodWeaverInfo.MethodBaseField));
                methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, backingField));
            }

            if (backingField.FieldType.IsValueType)
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Box, backingField.FieldType));
            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Callvirt, interceptorOnEnter));

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Nop));
        }

        protected override void OnImplementMethod(MethodWeaverInfo methodWeaverInfo)
        {
            // Implement the Setter delegate
            var methodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig;
            var isStatic = methodWeaverInfo.MethodDefinition.IsStatic;

            if (isStatic)
                methodAttributes |= MethodAttributes.Static;

            var property = methodWeaverInfo.MethodDefinition.GetPropertyDefinition();
            var field = methodWeaverInfo.OriginalBody.FirstOrDefault(x => x.OpCode == OpCodes.Ldfld).Operand as FieldDefinition;
            var method = new MethodDefinition($"<{property.Name}>_BackingField_Setter_{methodWeaverInfo.Id}", methodAttributes, this.ModuleDefinition.TypeSystem.Void);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, typeof(object).GetTypeReference().Import()));

            var processor = method.Body.GetILProcessor();

            processor.Append(processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            processor.Append(processor.Create(OpCodes.Ldarg_1));

            if (field.FieldType.FullName == typeof(int).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToInt32", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(uint).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToUInt32", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(bool).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToBoolean", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(byte).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToByte", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(char).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToChar", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(DateTime).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToDateTime", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(decimal).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToDecimal", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(double).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToDouble", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(short).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToInt16", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(long).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToInt64", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(sbyte).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToSByte", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(float).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToSingle", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(string).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToString", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(ushort).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToUInt16", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.FullName == typeof(ulong).FullName) processor.Append(processor.Create(OpCodes.Call, typeof(Convert).GetMethodReference("ToUInt64", new Type[] { typeof(object) }).Import()));
            else if (field.FieldType.Resolve().IsInterface) processor.Append(processor.Create(OpCodes.Isinst, field.FieldType.Import()));
            else processor.Append(processor.Create(field.FieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, field.FieldType.Import()));

            processor.Append(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, field));
            processor.Append(processor.Create(OpCodes.Ret));

            property.DeclaringType.Methods.Add(method);

            // Implement interceptor info instance
            var fieldAttributes = FieldAttributes.Private;
            if (isStatic)
                fieldAttributes |= FieldAttributes.Static;

            var propertyTypeDefinition = property.PropertyType.Import();
            var fieldDefinition = new FieldDefinition($"<{property.Name}>_interceptor_info_{methodWeaverInfo.Id}_field", fieldAttributes, this.propertyInterceptionInfoReference);
            property.DeclaringType.Fields.Add(fieldDefinition);

            var endIf = processor.Create(OpCodes.Nop);

            if (isStatic)
                methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldsfld, fieldDefinition));
            else
            {
                methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldfld, fieldDefinition));
            }

            methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldnull));
            methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ceq));
            methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Brfalse_S, endIf));

            methodWeaverInfo.Initializations.Add(processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            if (isStatic)
                methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldsfld, methodWeaverInfo.MethodBaseField));
            else
            {
                methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldfld, methodWeaverInfo.MethodBaseField));
            }

            methodWeaverInfo.Initializations.Add(processor.Create(OpCodes.Ldstr, property.Name));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldtoken, property.PropertyType.Import()));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Call, typeof(Type).GetMethodReference("GetTypeFromHandle", 1).Import()));
            methodWeaverInfo.Initializations.Add(processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            methodWeaverInfo.Initializations.Add(processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldftn, methodWeaverInfo.MethodDefinition.DeclaringType.GetMethodReference($"<{property.Name}>_BackingField_Setter_{methodWeaverInfo.Id}", 1)));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Newobj, typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }).Import()));
            methodWeaverInfo.Initializations.Add(methodWeaverInfo.Processor.Create(OpCodes.Newobj, this.propertyInterceptionInfoReference.GetMethodReference(".ctor", 5).Import()));
            methodWeaverInfo.Initializations.Add(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, fieldDefinition));
            methodWeaverInfo.Initializations.Add(endIf);
        }

        private void ImplementProperty(PropertyDefinition property, CustomAttribute[] attributes)
        {
            // Before we proceed we have to check if this is an auto property...
            // The first version of our property interceptor only works for auto properties...
            // Other features should come later... TODO Dariusz

            var compilerGeneratedAttribute = property.GetMethod.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "CompilerGeneratedAttribute");

            if (compilerGeneratedAttribute == null)
            {
                this.LogWarning($"{property.Name}: The current version of the property interceptor only supports auto-properties.");
                return;
            }

            this.ImplementMethod(property.GetMethod, attributes, (r, isLockable) => isLockable ? r.GetMethodReference("OnGet", 3) : r.GetMethodReference("OnGet", 2));

            // Remove compiler generated attribute
            property.GetMethod.CustomAttributes.Remove(compilerGeneratedAttribute);
        }
    }
}