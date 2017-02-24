using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class Property : CecilatorBase, IEquatable<Property>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly PropertyDefinition propertyDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly BuilderType type;

        internal Property(BuilderType type, PropertyDefinition propertyDefinition) : base(type)
        {
            this.type = type;
            this.propertyDefinition = propertyDefinition;
            this.Getter = propertyDefinition.GetMethod == null ? null : new Method(type, propertyDefinition.GetMethod);
            this.Setter = propertyDefinition.SetMethod == null ? null : new Method(type, propertyDefinition.SetMethod);
        }

        public BuilderType DeclaringType { get { return this.type; } }

        public Method Getter { get; private set; }
        public bool IsAbstract { get { return this.propertyDefinition.GetMethod?.IsAbstract ?? false | this.propertyDefinition.SetMethod?.IsAbstract ?? false; } }

        public bool IsAutoProperty
        {
            get
            {
                return (this.propertyDefinition.GetMethod ?? this.propertyDefinition.SetMethod).CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "CompilerGeneratedAttribute") != null;
            }
        }

        public bool IsPublic { get { return this.Getter?.IsPublic ?? false | this.Setter?.IsPublic ?? false; } }
        public bool IsStatic { get { return this.propertyDefinition.GetMethod?.IsStatic ?? false | this.propertyDefinition.SetMethod?.IsStatic ?? false; } }
        public string Name { get { return this.propertyDefinition.Name; } }
        public BuilderType ReturnType { get { return new BuilderType(this.type, this.propertyDefinition.PropertyType); } }
        public Method Setter { get; private set; }

        public Field CreateField(Type fieldType, string name) =>
            this.CreateField(this.moduleDefinition.Import(this.GetTypeDefinition(fieldType).ResolveType(this.DeclaringType.typeReference)), name);

        public Field CreateField(Field field, string name) => this.CreateField(field.fieldRef.FieldType, name);

        public Field CreateField(TypeReference typeReference, string name) =>
            this.IsStatic ? this.DeclaringType.CreateField(Modifiers.PrivateStatic, typeReference, name) : this.DeclaringType.CreateField(Modifiers.Private, typeReference, name);

        #region Equitable stuff

        public static implicit operator string(Property property) => property.propertyDefinition.FullName;

        public static bool operator !=(Property a, Property b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(Property a, Property b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is Property)
                return this.Equals(obj as Property);

            if (obj is PropertyDefinition)
                return this.propertyDefinition.FullName == (obj as PropertyDefinition).FullName;

            return false;
        }

        public bool Equals(Property other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.propertyDefinition.FullName == this.propertyDefinition.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.propertyDefinition.FullName.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.propertyDefinition.FullName;

        #endregion Equitable stuff
    }
}