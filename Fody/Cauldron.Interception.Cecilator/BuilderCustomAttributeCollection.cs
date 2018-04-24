
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderCustomAttributeCollection : CecilatorBase, IEnumerable<BuilderCustomAttribute>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Builder builder;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ICustomAttributeProvider customAttributeProvider;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<BuilderCustomAttribute> innerCollection = new List<BuilderCustomAttribute>();

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PropertyDefinition propertyDefinition;

        internal BuilderCustomAttributeCollection(Builder builder, ICustomAttributeProvider customAttributeProvider) : base(builder)
        {
            this.builder = builder;
            this.customAttributeProvider = customAttributeProvider;

            this.innerCollection.AddRange(customAttributeProvider.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, customAttributeProvider, x)));
        }

        internal BuilderCustomAttributeCollection(Builder builder, PropertyDefinition propertyDefinition) : base(builder)
        {
            this.builder = builder;
            this.propertyDefinition = propertyDefinition;
            this.customAttributeProvider = propertyDefinition;

            this.innerCollection.AddRange(customAttributeProvider.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, customAttributeProvider, x)));

            if (propertyDefinition.GetMethod != null)
                this.innerCollection.AddRange(propertyDefinition.GetMethod.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, propertyDefinition.GetMethod, x)));

            if (propertyDefinition.SetMethod != null)
                this.innerCollection.AddRange(propertyDefinition.SetMethod.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, propertyDefinition.SetMethod, x)));
        }

        public bool Add(Type customAttributeType, params object[] parameters) => this.Add(this.moduleDefinition.ImportReference(customAttributeType), parameters);

        public bool Add(BuilderType customAttributeType, params object[] parameters) => this.Add(customAttributeType.typeReference, parameters);

        public bool Add(TypeReference customAttributeType, params object[] parameters)
        {
            if (this.customAttributeProvider is FieldDefinition fieldDefinition &&
                (customAttributeType.FullName == "System.NonSerializedAttribute" || customAttributeType.FullName == "System.Runtime.Serialization.IgnoreDataMemberAttribute"))
            {
                fieldDefinition.Attributes |= FieldAttributes.NotSerialized;
                return true;
            }

            if (this.DonotApply(customAttributeType))
                return false;

            MethodReference ctor = null;
            var type = this.moduleDefinition.ImportReference(customAttributeType);

            if (parameters == null || parameters.Length == 0)
                ctor = (type.Resolve() ?? this.allTypes.Get(type.FullName)).Methods.FirstOrDefault(x => x.Name == ".ctor" && x.Parameters.Count == 0);
            else
            {
                ctor = (type.Resolve() ?? this.allTypes.Get(type.FullName)).Methods.FirstOrDefault(x =>
                {
                    if (x.Name != ".ctor")
                        return false;

                    if (x.Parameters.Count != parameters.Length)
                        return false;

                    for (int i = 0; i < x.Parameters.Count; i++)
                    {
                        var parameterType = parameters[i] == null ? null : this.moduleDefinition.ImportReference(parameters[i].GetType());

                        if (!x.Parameters[i].ParameterType.AreReferenceAssignable(parameterType))
                            return false;
                    }

                    return true;
                });
            }

            if (ctor == null)
                throw new ArgumentException("Unable to find matching ctor.");

            var attrib = new CustomAttribute(this.moduleDefinition.ImportReference(ctor));

            if (ctor.Parameters.Count > 0)
                for (int i = 0; i < ctor.Parameters.Count; i++)
                    attrib.ConstructorArguments.Add(new CustomAttributeArgument(ctor.Parameters[i].ParameterType, ConvertToAttributeParameter(parameters[i])));

            if (this.customAttributeProvider != null)
            {
                this.customAttributeProvider.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.builder, this.customAttributeProvider, attrib));
            }

            return true;
        }

        public void AddCompilerGeneratedAttribute() => this.Add(this.builder.GetType("System.Runtime.CompilerServices.CompilerGeneratedAttribute"));

        public void AddDebuggerBrowsableAttribute(DebuggerBrowsableState state) => this.Add(this.builder.GetType("System.Diagnostics.DebuggerBrowsableAttribute"), state);

        public void AddEditorBrowsableAttribute(EditorBrowsableState state) => this.Add(this.builder.GetType("System.ComponentModel.EditorBrowsableAttribute"), state);

        public void AddNonSerializedAttribute()
        {
            if (this.builder.IsUWP)
                this.Add(this.builder.GetType("System.Runtime.Serialization.IgnoreDataMemberAttribute"));
            else if (this.builder.TypeExists("System.NonSerializedAttribute"))
            {
                if (this.propertyDefinition != null)
                {
                    if (this.builder.TypeExists("System.Xml.Serialization.XmlIgnoreAttribute"))
                        this.Add(this.builder.GetType("System.Xml.Serialization.XmlIgnoreAttribute"));
                }
                else if (this.customAttributeProvider is FieldDefinition fieldDefinition)
                    this.Add(this.builder.GetType("System.NonSerializedAttribute"));
            }

            if (this.builder.TypeExists("Newtonsoft.Json.JsonIgnoreAttribute"))
                this.Add(this.builder.GetType("Newtonsoft.Json.JsonIgnoreAttribute"));
        }

        public void Copy(BuilderCustomAttribute attribute)
        {
            if (this.DonotApply(attribute.attribute.AttributeType))
                return;

            if (this.customAttributeProvider != null)
                this.customAttributeProvider.CustomAttributes.Add(attribute.attribute);
            else if (this.propertyDefinition != null)
                this.propertyDefinition.CustomAttributes.Add(attribute.attribute);

            this.innerCollection.Add(attribute);
        }

        public IEnumerator<BuilderCustomAttribute> GetEnumerator() => this.innerCollection.GetEnumerator();

        public bool HasAttribute(BuilderType type)
        {
            if (this.customAttributeProvider != null)
                for (int i = 0; i < this.customAttributeProvider.CustomAttributes.Count; i++)
                {
                    var item = this.customAttributeProvider.CustomAttributes[i];

                    if (item.AttributeType.FullName.GetHashCode() == type.typeReference.FullName.GetHashCode() &&
                        item.AttributeType.FullName == type.typeReference.FullName)
                        return true;
                }
            else if (this.propertyDefinition != null)
                for (int i = 0; i < this.propertyDefinition.CustomAttributes.Count; i++)
                {
                    var item = this.propertyDefinition.CustomAttributes[i];

                    if (item.AttributeType.FullName.GetHashCode() == type.typeReference.FullName.GetHashCode() &&
                        item.AttributeType.FullName == type.typeReference.FullName)
                        return true;
                }

            return false;
        }

        public void Remove(Type type)
        {
            var attributesToRemove = this.innerCollection
                .Where(x => x.Fullname.GetHashCode() == type.FullName.GetHashCode() && x.Fullname == type.FullName)
                .ToArray();
            this.Remove(attributesToRemove);
        }

        public void Remove(BuilderType type)
        {
            var attributesToRemove = this.innerCollection
                .Where(x => x.Fullname.GetHashCode() == type.typeReference.FullName.GetHashCode() && x.Fullname == type.typeReference.FullName)
                .ToArray();
            this.Remove(attributesToRemove);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.innerCollection.GetEnumerator();

        private object ConvertToAttributeParameter(object value)
        {
            switch (value)
            {
                case Type type: return type.ToBuilderType().typeReference;
                default: return value;
            }
        }

        private bool DonotApply(TypeReference customAttributeType)
        {
            var attributeTarget = customAttributeType.Resolve().CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.AttributeUsageAttribute");
            var attributeTargets = AttributeTargets.All;
            var allowMultiple = false;

            if (attributeTarget != null)
            {
                Enum.TryParse(attributeTarget.ConstructorArguments[0].Value?.ToString(), out attributeTargets);
                for (int i = 0; i < attributeTarget.Properties.Count; i++)
                    if (attributeTarget.Properties[i].Name == "AllowMultiple")
                    {
                        allowMultiple = (bool)attributeTarget.Properties[i].Argument.Value;
                        break;
                    }
            }

            bool DonotApply(AttributeTargets target)
            {
                if (!attributeTargets.HasFlag(target))
                    return true;

                if (allowMultiple)
                    return false;

                return this.customAttributeProvider.CustomAttributes?.Any(x => x.AttributeType.FullName == customAttributeType.FullName) ?? false;
            }

            switch (this.customAttributeProvider)
            {
                case PropertyDefinition propertyDefinition when (propertyDefinition.GetMethod ?? propertyDefinition.SetMethod).IsAbstract || DonotApply(AttributeTargets.Property):
                case ModuleDefinition moduleDefinition when DonotApply(AttributeTargets.Module):
                case TypeDefinition typeDefinition when typeDefinition.IsInterface /* We don't have any implementation for interfaces */ || DonotApply(AttributeTargets.Class):
                case FieldDefinition fieldDefinition when DonotApply(AttributeTargets.Field):
                    return true;

                case MethodDefinition methodDefinition:
                    {
                        if (methodDefinition.IsAbstract)
                            return true;

                        if ((methodDefinition.Name == ".ctor" || methodDefinition.Name == ".cctor") /* constructor */)
                            return DonotApply(AttributeTargets.Constructor);

                        return DonotApply(AttributeTargets.Method);
                    }
            }

            return false;
        }

        private void Remove(BuilderCustomAttribute[] attributesToRemove)
        {
            foreach (var item in attributesToRemove)
            {
                this.innerCollection.Remove(item);

                if (this.propertyDefinition != null && this.propertyDefinition.GetMethod != null)
                    this.propertyDefinition.GetMethod.CustomAttributes.Remove(item.attribute);

                if (this.propertyDefinition != null && this.propertyDefinition.SetMethod != null)
                    this.propertyDefinition.SetMethod.CustomAttributes.Remove(item.attribute);

                this.customAttributeProvider?.CustomAttributes.Remove(item.attribute);
            }
        }
    }
}