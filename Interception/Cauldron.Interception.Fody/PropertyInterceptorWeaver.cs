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
            methodWeaverInfo.DeclaringType.InsertToCtorOrCctor(methodWeaverInfo.IsStatic, (processor, instructions) =>
            {
                instructions.AddRange(processor.Newobj(semaphoreSlim, this.ModuleDefinition.Import(typeof(SemaphoreSlim).GetMethodReference(".ctor", new Type[] { typeof(int), typeof(int) })), new object[] { 1, 1 }));
                return InsertionPosition.InsertBeforeBaseCall;
            });

            var backingField = methodWeaverInfo.AutoPropertyBackingField;
            var propertyWeaverInfo = methodWeaverInfo.GetOrCreateField(this.propertyInterceptionInfoReference).CreateFieldReference();
            var onSet = new Action<List<Instruction>>(x =>
            {
                if (interceptorOnEnter.Name.StartsWith("OnSet"))
                    x.AddRange(
                        methodWeaverInfo.Processor.Callvirt(attributeVariable, interceptorOnEnter, new object[] {
                        semaphoreSlim,
                        propertyWeaverInfo,
                        backingField,
                        methodWeaverInfo.MethodDefinition.Parameters[0]
                        }));
                else
                    x.AddRange(
                        methodWeaverInfo.Processor.Callvirt(attributeVariable, interceptorOnEnter, new object[] {
                        semaphoreSlim,
                        propertyWeaverInfo,
                        backingField
                        }));
            });

            if (interceptorOnEnter.Name.StartsWith("OnSet"))
                methodWeaverInfo.OnEnterInstructions.AddRange(methodWeaverInfo.Processor.If(onSet, methodWeaverInfo.LastReturn));
            else
                onSet(methodWeaverInfo.OnEnterInstructions);
        }

        protected override void ImplementOnEnter(MethodWeaverInfo methodWeaverInfo, VariableDefinition attributeVariable, MethodReference interceptorOnEnter, VariableDefinition parametersArrayVariable)
        {
            var backingField = methodWeaverInfo.AutoPropertyBackingField;
            var propertyWeaverInfo = methodWeaverInfo.GetOrCreateField(this.propertyInterceptionInfoReference).CreateFieldReference();
            var onSet = new Action<List<Instruction>>(x =>
            {
                if (interceptorOnEnter.Name.StartsWith("OnSet"))
                    x.AddRange(
                        methodWeaverInfo.Processor.Callvirt(attributeVariable, interceptorOnEnter, new object[] {
                        propertyWeaverInfo,
                        backingField,
                        methodWeaverInfo.MethodDefinition.Parameters[0]
                    }));
                else
                    x.AddRange(
                            methodWeaverInfo.Processor.Callvirt(attributeVariable, interceptorOnEnter, new object[] {
                            propertyWeaverInfo,
                            backingField
                    }));
            });

            if (interceptorOnEnter.Name.StartsWith("OnSet"))
                methodWeaverInfo.OnEnterInstructions.AddRange(methodWeaverInfo.Processor.If(onSet, methodWeaverInfo.LastReturn));
            else
                onSet(methodWeaverInfo.OnEnterInstructions);
        }

        protected override void OnImplementMethod(MethodWeaverInfo methodWeaverInfo)
        {
            // Implement the Setter delegate
            var methodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig;
            var isStatic = methodWeaverInfo.IsStatic;

            if (isStatic)
                methodAttributes |= MethodAttributes.Static;

            var property = methodWeaverInfo.Property;
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

                methodWeaverInfo.DeclaringType.InsertToCtorOrCctor(methodWeaverInfo.IsStatic, (processor, instructions) =>
                {
                    instructions.AddRange(processor.Newobj(fieldDefinition, this.propertyInterceptionInfoReference.GetMethodReference(".ctor", 7).Import(),
                        processor.MethodOf(property.GetMethod),
                        processor.MethodOf(property.SetMethod),
                        property.Name,
                        processor.TypeOf(property.PropertyType.Import()),
                        new This(),
                        property.PropertyType.IsIEnumerable() ? processor.TypeOf(property.PropertyType.GetChildrenType().Import()) : new Instruction[] { processor.Create(OpCodes.Ldnull) },
                        processor.Newobj(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }).Import(), new object[] {
                            new Instruction[] {
                                processor.Create(isStatic ? OpCodes.Ldnull : OpCodes.Ldarg_0),
                                processor.Create(OpCodes.Ldftn, method.CreateMethodReference())
                            } })));
                    return methodWeaverInfo.IsStatic ? InsertionPosition.InsertBeforeBaseCall : InsertionPosition.InsertAfterBaseCall;
                });
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