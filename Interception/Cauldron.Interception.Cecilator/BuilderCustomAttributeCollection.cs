using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderCustomAttributeCollection : CecilatorBase
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly FieldDefinition fieldDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<BuilderCustomAttribute> innerCollection = new List<BuilderCustomAttribute>();

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MethodDefinition methodDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PropertyDefinition propertyDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly BuilderType type;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TypeDefinition typeDefinition;

        internal BuilderCustomAttributeCollection(BuilderType builder, MethodDefinition methodDefinition) : base(builder)
        {
            this.type = builder;
            this.methodDefinition = methodDefinition;

            this.innerCollection.AddRange(this.methodDefinition.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, this.methodDefinition, x)));
        }

        internal BuilderCustomAttributeCollection(BuilderType builder, FieldDefinition fieldDefinition) : base(builder)
        {
            this.type = builder;
            this.fieldDefinition = fieldDefinition;

            this.innerCollection.AddRange(this.fieldDefinition.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, this.fieldDefinition, x)));
        }

        internal BuilderCustomAttributeCollection(BuilderType builder, TypeDefinition typeDefinition) : base(builder)
        {
            this.type = builder;
            this.typeDefinition = typeDefinition;

            this.innerCollection.AddRange(this.typeDefinition.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, this.typeDefinition, x)));
        }

        internal BuilderCustomAttributeCollection(BuilderType builder, PropertyDefinition propertyDefinition) : base(builder)
        {
            this.type = builder;
            this.propertyDefinition = propertyDefinition;

            if (propertyDefinition.GetMethod != null)
                this.innerCollection.AddRange(propertyDefinition.GetMethod.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, propertyDefinition.GetMethod, x)));

            if (propertyDefinition.SetMethod != null)
                this.innerCollection.AddRange(propertyDefinition.SetMethod.CustomAttributes.Select(x => new BuilderCustomAttribute(builder, propertyDefinition.SetMethod, x)));
        }

        public void Add(Type customAttributeType, params object[] parameters)
        {
            MethodReference ctor = null;
            var type = this.moduleDefinition.Import(customAttributeType);

            if (parameters == null || parameters.Length == 0)
                ctor = type.Resolve().Methods.FirstOrDefault(x => x.Name == ".ctor" && x.Parameters.Count == 0);
            else
            {
                ctor = type.Resolve().Methods.FirstOrDefault(x =>
                {
                    if (x.Name != ".ctor")
                        return false;

                    if (x.Parameters.Count != parameters.Length)
                        return false;

                    for (int i = 0; i < x.Parameters.Count; i++)
                    {
                        var parameterType = parameters[i] == null ? null : this.moduleDefinition.Import(parameters[i].GetType());

                        if (!this.AreReferenceAssignable(x.Parameters[i].ParameterType, parameterType))
                            return false;
                    }

                    return true;
                });
            }

            if (ctor == null)
                throw new ArgumentException("Unable to find matching ctor.");

            var attrib = new CustomAttribute(this.moduleDefinition.Import(ctor));

            if (ctor.Parameters.Count > 0)
                for (int i = 0; i < ctor.Parameters.Count; i++)
                    attrib.ConstructorArguments.Add(new CustomAttributeArgument(ctor.Parameters[i].ParameterType, parameters[i]));

            if (this.propertyDefinition != null && this.propertyDefinition.GetMethod != null)
            {
                this.propertyDefinition.GetMethod.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.type, this.propertyDefinition.GetMethod, attrib));
            }

            if (this.propertyDefinition != null && this.propertyDefinition.SetMethod != null)
            {
                this.propertyDefinition.SetMethod.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.type, this.propertyDefinition.SetMethod, attrib));
            }

            if (this.typeDefinition != null)
            {
                this.typeDefinition.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.type, this.typeDefinition, attrib));
            }

            if (this.fieldDefinition != null)
            {
                this.fieldDefinition.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.type, this.fieldDefinition, attrib));
            }

            if (this.methodDefinition != null)
            {
                this.methodDefinition.CustomAttributes.Add(attrib);
                this.innerCollection.Add(new BuilderCustomAttribute(this.type, this.methodDefinition, attrib));
            }
        }

        public void AddCompilerGeneratedAttribute() => this.Add(typeof(CompilerGeneratedAttribute));

        public void AddDebuggerBrowsableAttribute(DebuggerBrowsableState state) => this.Add(typeof(DebuggerBrowsableAttribute), state);

        public void AddEditorBrowsableAttribute(EditorBrowsableAttribute state) => this.Add(typeof(EditorBrowsableAttribute), state);

        public void Remove(Type type)
        {
            var attributesToRemove = this.innerCollection.Where(x => x.Fullname == type.FullName).ToArray();

            foreach (var item in attributesToRemove)
            {
                this.innerCollection.Remove(item);

                if (this.propertyDefinition != null && this.propertyDefinition.GetMethod != null)
                    this.propertyDefinition.GetMethod.CustomAttributes.Remove(item.attribute);

                if (this.propertyDefinition != null && this.propertyDefinition.SetMethod != null)
                    this.propertyDefinition.SetMethod.CustomAttributes.Remove(item.attribute);

                if (this.typeDefinition != null)
                    this.typeDefinition.CustomAttributes.Remove(item.attribute);

                if (this.fieldDefinition != null)
                    this.fieldDefinition.CustomAttributes.Remove(item.attribute);

                if (this.methodDefinition != null)
                    this.methodDefinition.CustomAttributes.Remove(item.attribute);
            }
        }
    }
}