using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections;

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

            if (propertyDefinition.GetMethod != null)
                this.innerCollection.AddRange(propertyDefinition.GetMethod.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, propertyDefinition.GetMethod, x)));

            if (propertyDefinition.SetMethod != null)
                this.innerCollection.AddRange(propertyDefinition.SetMethod.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, propertyDefinition.SetMethod, x)));
        }

        public void Add(Type customAttributeType, params object[] parameters) => this.Add(this.moduleDefinition.ImportReference(customAttributeType), parameters);

        public void Add(BuilderType customAttributeType, params object[] parameters) => this.Add(customAttributeType.typeReference, parameters);

        public void Add(TypeReference customAttributeType, params object[] parameters)
        {
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

                        if (!this.AreReferenceAssignable(x.Parameters[i].ParameterType, parameterType))
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
                    attrib.ConstructorArguments.Add(new CustomAttributeArgument(ctor.Parameters[i].ParameterType, parameters[i]));

            if (this.propertyDefinition != null && this.propertyDefinition.GetMethod != null)
            {
                this.propertyDefinition.GetMethod.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.builder, this.propertyDefinition.GetMethod, attrib));
            }

            if (this.propertyDefinition != null && this.propertyDefinition.SetMethod != null)
            {
                this.propertyDefinition.SetMethod.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.builder, this.propertyDefinition.SetMethod, attrib));
            }

            if (this.customAttributeProvider != null)
            {
                this.customAttributeProvider.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.builder, this.customAttributeProvider, attrib));
            }
        }

        public void AddCompilerGeneratedAttribute() => this.Add(this.builder.GetType("System.Runtime.CompilerServices.CompilerGeneratedAttribute"));

        public void AddDebuggerBrowsableAttribute(DebuggerBrowsableState state) => this.Add(this.builder.GetType("System.Diagnostics.DebuggerBrowsableAttribute"), state);

        public void AddEditorBrowsableAttribute(EditorBrowsableState state) => this.Add(this.builder.GetType("System.ComponentModel.EditorBrowsableAttribute"), state);

        public void AddNonSerializedAttribute()
        {
            if (this.builder.IsUWP)
                this.Add(this.builder.GetType("System.Runtime.Serialization.IgnoreDataMemberAttribute"));
            else if (this.builder.TypeExists("System.NonSerializedAttribute"))
                this.Add(this.builder.GetType("System.NonSerializedAttribute"));

            if (this.builder.TypeExists("Newtonsoft.Json.JsonIgnoreAttribute"))
                this.Add(this.builder.GetType("Newtonsoft.Json.JsonIgnoreAttribute"));
        }

        public void Copy(BuilderCustomAttribute attribute)
        {
            if (this.customAttributeProvider != null)
                this.customAttributeProvider.CustomAttributes.Add(attribute.attribute);
            else if (this.propertyDefinition != null)
                this.propertyDefinition.CustomAttributes.Add(attribute.attribute);

            this.innerCollection.Add(attribute);
        }

        public IEnumerator<BuilderCustomAttribute> GetEnumerator() => this.innerCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.innerCollection.GetEnumerator();

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