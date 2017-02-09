using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Cauldron.Interception.Fody
{
    public class PropertyInterceptorWeaver : ModuleWeaverBase
    {
        private string lockablePropertyGetterInterceptor = "Cauldron.Interception.ILockablePropertyGetterInterceptor";
        private string lockablePropertySetterInterceptor = "Cauldron.Interception.ILockablePropertySetterInterceptor";

        private string propertyGetterInterceptor = "Cauldron.Interception.IPropertyGetterInterceptor";
        private TypeReference propertyInterceptionInfoReference;
        private string propertySetterInterceptor = "Cauldron.Interception.IPropertySetterInterceptor";

        public PropertyInterceptorWeaver(ModuleWeaver weaver) : base(weaver)
        {
        }

        public override void Implement()
        {
            var propertyInterceptors = GetPropertyInterceptorTypes();
            this.propertyInterceptionInfoReference = "Cauldron.Interception.PropertyInterceptionInfo".ToTypeDefinition().Import();

            // find all types with methods that are decorated with any of the found property interceptors
            var propertiesAndAttributes = this.ModuleDefinition.Types.SelectMany(x => x.Properties).Where(x => x.HasCustomAttributes)
                .Select(x => new { Property = x, Attributes = x.CustomAttributes.Where(y => propertyInterceptors.Any(t => y.AttributeType.FullName == t.FullName)).ToArray() })
                .Where(x => x.Attributes != null && x.Attributes.Length > 0)
                .ToArray();

            foreach (var method in propertiesAndAttributes)
                this.ImplementProperty(method.Property, method.Attributes);
        }

        protected override VariableDefinition CreateParameterObject(MethodWeaverInfo methodWeaverInfo, TypeReference objectReference, ArrayType parametersArrayTypeRef) => null;

        protected IEnumerable<TypeDefinition> GetPropertyInterceptorTypes()
        {
            var propertyInterceptorInterface = this.propertyGetterInterceptor.ToTypeDefinition();
            if (propertyInterceptorInterface == null)
                throw new Exception($"Unable to find the interface {this.propertyGetterInterceptor}.");

            return propertyInterceptorInterface.GetTypesThatImplementsInterface()
                .Concat(this.lockablePropertyGetterInterceptor.ToTypeDefinition().GetTypesThatImplementsInterface())
                .Concat(this.propertySetterInterceptor.ToTypeDefinition().GetTypesThatImplementsInterface())
                .Concat(this.lockablePropertySetterInterceptor.ToTypeDefinition().GetTypesThatImplementsInterface());
        }

        protected override void ImplementLockableOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable, FieldDefinition semaphoreSlim)
        {
            // Implement fields
            if (methodWeaverInfo.MethodDefinition.IsStatic)
            {
                var cctor = this.GetOrCreateStaticConstructor(methodWeaverInfo.MethodDefinition.DeclaringType);

                var body = cctor.Body;
                var ctorProcessor = body.GetILProcessor();
                var first = body.Instructions.FirstOrDefault();

                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Newobj, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference(".ctor", new Type[] { typeof(int), typeof(int) }))));
                ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Stsfld, semaphoreSlim));
            }
            else
            {
                foreach (var constructor in this.GetConstructors(methodWeaverInfo.MethodDefinition.DeclaringType))
                {
                    var body = constructor.Resolve().Body;
                    var ctorProcessor = body.GetILProcessor();
                    var first = body.Instructions.First();

                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldarg_0));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Ldc_I4_1));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Newobj, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference(".ctor", new Type[] { typeof(int), typeof(int) }))));
                    ctorProcessor.InsertBefore(first, ctorProcessor.Create(OpCodes.Stfld, semaphoreSlim));
                }
            }

            var isStatic = methodWeaverInfo.MethodDefinition.IsStatic;
            var property = methodWeaverInfo.MethodDefinition.GetPropertyDefinition();
            var backingField = methodWeaverInfo.AutoPropertyBackingField;
            var propertyWeaverInfo = methodWeaverInfo.GetOrCreateField(this.propertyInterceptionInfoReference);

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, attributeVariable));
            if (!methodWeaverInfo.MethodDefinition.IsStatic)
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, semaphoreSlim));
            }
            else
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, semaphoreSlim));
            if (isStatic)
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, propertyWeaverInfo));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, backingField));
            }
            else
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, propertyWeaverInfo));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, backingField));
            }

            if (backingField.FieldType.IsValueType)
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Box, backingField.FieldType));

            if (interceptorOnEnter.Name.StartsWith("OnSet"))
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
                if (backingField.FieldType.IsValueType)
                    methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Box, backingField.FieldType));
            }

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Callvirt, interceptorOnEnter));

            if (interceptorOnEnter.Name.StartsWith("OnSet"))
            {
                var endif = methodWeaverInfo.Processor.Create(OpCodes.Nop);
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldc_I4_1));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Pop));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Brfalse_S, endif));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Leave_S, methodWeaverInfo.LastReturn));
                methodWeaverInfo.OnEnterInstructions.Add(endif);
            }

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Nop));
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            var isStatic = methodWeaverInfo.MethodDefinition.IsStatic;
            var fieldAttributes = FieldAttributes.Private;
            if (isStatic)
                fieldAttributes |= FieldAttributes.Static;

            var property = methodWeaverInfo.MethodDefinition.GetPropertyDefinition();
            var backingField = methodWeaverInfo.AutoPropertyBackingField;
            var propertyWeaverInfo = methodWeaverInfo.GetOrCreateField(this.propertyInterceptionInfoReference);

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldloc_S, attributeVariable));
            if (isStatic)
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, propertyWeaverInfo));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldsfld, backingField));
            }
            else
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, propertyWeaverInfo));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldarg_0));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldfld, backingField));
            }

            if (backingField.FieldType.IsValueType)
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Box, backingField.FieldType));

            if (interceptorOnEnter.Name.StartsWith("OnSet"))
            {
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));
                if (backingField.FieldType.IsValueType)
                    methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Box, backingField.FieldType));
            }

            methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Callvirt, interceptorOnEnter));

            if (interceptorOnEnter.Name.StartsWith("OnSet"))
            {
                var endif = methodWeaverInfo.Processor.Create(OpCodes.Nop);
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Ldc_I4_1));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Pop));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Brfalse_S, endif));
                methodWeaverInfo.OnEnterInstructions.Add(methodWeaverInfo.Processor.Create(OpCodes.Leave_S, methodWeaverInfo.LastReturn));
                methodWeaverInfo.OnEnterInstructions.Add(endif);
            }

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
            var field = methodWeaverInfo.AutoPropertyBackingField;

            var methodParams = new ParameterDefinition("value", ParameterAttributes.None, typeof(object).GetTypeReference().Import());
            var method = methodWeaverInfo.GetMethod("setterAction", methodParams);

            if (method == null)
            {
                method = methodWeaverInfo.GetOrCreateMethod("setterAction", methodParams);
                this.ImplementFieldSetterDelegate(method, field, isStatic);
            }

            var fieldDefinition = methodWeaverInfo.GetField(this.propertyInterceptionInfoReference);

            if (fieldDefinition == null)
            {
                // Implement interceptor info instance
                fieldDefinition = methodWeaverInfo.GetOrCreateField(this.propertyInterceptionInfoReference);
                var getMethodFromHandleRef = typeof(System.Reflection.MethodBase).Import().GetMethodReference("GetMethodFromHandle", 2).Import();
                var intitalizePropertyInfo = new Func<ILProcessor, IEnumerable<Instruction>>(processor =>
                   {
                       var result = new List<Instruction>();

                       if (!isStatic)
                           result.Add(processor.Create(OpCodes.Ldarg_0));

                       result.Add(processor.Create(OpCodes.Ldtoken, property.GetMethod.Import()));
                       result.Add(processor.Create(OpCodes.Ldtoken, property.GetMethod.DeclaringType.Import()));
                       result.Add(processor.Create(OpCodes.Call, getMethodFromHandleRef));

                       result.Add(processor.Create(OpCodes.Ldtoken, property.SetMethod.Import()));
                       result.Add(processor.Create(OpCodes.Ldtoken, property.SetMethod.DeclaringType.Import()));
                       result.Add(processor.Create(OpCodes.Call, getMethodFromHandleRef));

                       result.Add(processor.Create(OpCodes.Ldstr, property.Name));
                       result.AddRange(processor.TypeOf(property.PropertyType.Import()));
                       result.Add(processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));

                       if (property.PropertyType.IsIEnumerable())
                           result.AddRange(processor.TypeOf(property.PropertyType.GetChildrenType().Import()));
                       else
                           result.Add(processor.Create(OpCodes.Ldnull));

                       result.Add(processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0));
                       result.Add(processor.Create(OpCodes.Ldftn, method));
                       result.Add(processor.Create(OpCodes.Newobj, typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }).Import()));
                       result.Add(processor.Create(OpCodes.Newobj, this.propertyInterceptionInfoReference.GetMethodReference(".ctor", 7).Import()));
                       result.Add(processor.Create(isStatic ? OpCodes.Stsfld : OpCodes.Stfld, fieldDefinition));

                       return result;
                   });

                if (methodWeaverInfo.MethodDefinition.IsStatic)
                {
                    var cctor = this.GetOrCreateStaticConstructor(methodWeaverInfo.MethodDefinition.DeclaringType);

                    var body = cctor.Body;
                    var ctorProcessor = body.GetILProcessor();
                    var first = body.Instructions.FirstOrDefault();

                    ctorProcessor.InsertBefore(first, intitalizePropertyInfo(ctorProcessor));
                }
                else
                {
                    foreach (var constructor in this.GetConstructors(methodWeaverInfo.MethodDefinition.DeclaringType))
                    {
                        var body = constructor.Resolve().Body;
                        var ctorProcessor = body.GetILProcessor();
                        var first = body.Instructions.First();

                        ctorProcessor.InsertBefore(first, intitalizePropertyInfo(ctorProcessor));
                    }
                }
            }
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

            this.ImplementMethod(property.GetMethod,
                attributes.Where(x => x.AttributeType.Resolve().Interfaces.Any(y => y.FullName == propertyGetterInterceptor || y.FullName == lockablePropertyGetterInterceptor)).ToArray(),
                (r, isLockable) => isLockable ? r.GetMethodReference("OnGet", 3) : r.GetMethodReference("OnGet", 2));
            this.ImplementMethod(property.SetMethod,
                attributes.Where(x => x.AttributeType.Resolve().Interfaces.Any(y => y.FullName == propertySetterInterceptor || y.FullName == lockablePropertySetterInterceptor)).ToArray(),
                (r, isLockable) => isLockable ? r.GetMethodReference("OnSet", 4) : r.GetMethodReference("OnSet", 3));

            var propertyTypeDefinition = property.PropertyType.Import();

            // Remove compiler generated attribute
            property.GetMethod.CustomAttributes.Remove(compilerGeneratedAttribute);
            property.SetMethod.CustomAttributes.Remove(property.SetMethod.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "CompilerGeneratedAttribute"));

            foreach (var attr in attributes)
                property.CustomAttributes.Remove(attr);
        }
    }
}