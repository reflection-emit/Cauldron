using Mono.Cecil;
using Mono.Cecil.Cil;
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

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Field backingField;

        internal Property(BuilderType type, PropertyDefinition propertyDefinition) : base(type)
        {
            this.type = type;
            this.propertyDefinition = propertyDefinition;
            this.Getter = propertyDefinition.GetMethod == null ? null : new Method(type, propertyDefinition.GetMethod);
            this.Setter = propertyDefinition.SetMethod == null ? null : new Method(type, propertyDefinition.SetMethod);
        }

        public Field BackingField
        {
            get
            {
                if (this.backingField == null)
                {
                    var instruction =
                        this.propertyDefinition.GetMethod?.Body.Instructions.LastOrDefault(x => x.OpCode == OpCodes.Ldfld || x.OpCode == OpCodes.Ldsfld) ??
                        this.propertyDefinition.SetMethod?.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Stfld || x.OpCode == OpCodes.Stsfld);
                    var operand = instruction?.Operand;
                    var field = operand as FieldDefinition ?? operand as FieldReference;
                    if (field != null)
                        this.backingField = new Field(this.type, field.Resolve(), field);
                }

                return this.backingField;
            }
        }

        public BuilderCustomAttributeCollection CustomAttributes { get { return new BuilderCustomAttributeCollection(this.type, this.propertyDefinition); } }
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

        public Modifiers Modifiers
        {
            get
            {
                Modifiers modifiers = 0;

                if (this.Getter != null)
                {
                    if (this.Getter.methodDefinition.Attributes.HasFlag(MethodAttributes.Private)) modifiers |= Modifiers.Private;
                    if (this.Getter.methodDefinition.Attributes.HasFlag(MethodAttributes.Static)) modifiers |= Modifiers.Static;
                    if (this.Getter.methodDefinition.Attributes.HasFlag(MethodAttributes.Public)) modifiers |= Modifiers.Public;
                    if (this.Getter.methodDefinition.Attributes.HasFlag(MethodAttributes.Virtual) &&
                        this.Getter.methodDefinition.Attributes.HasFlag(MethodAttributes.NewSlot)) modifiers |= Modifiers.Overrrides;
                }

                if (this.Setter != null)
                {
                    if (this.Setter.methodDefinition.Attributes.HasFlag(MethodAttributes.Private)) modifiers |= Modifiers.Private;
                    if (this.Setter.methodDefinition.Attributes.HasFlag(MethodAttributes.Static)) modifiers |= Modifiers.Static;
                    if (this.Setter.methodDefinition.Attributes.HasFlag(MethodAttributes.Public)) modifiers |= Modifiers.Public;
                    if (this.Setter.methodDefinition.Attributes.HasFlag(MethodAttributes.Virtual) &&
                        this.Setter.methodDefinition.Attributes.HasFlag(MethodAttributes.NewSlot)) modifiers |= Modifiers.Overrrides;
                }

                if (modifiers.HasFlag(Modifiers.Public) && modifiers.HasFlag(Modifiers.Private))
                    modifiers &= ~Modifiers.Private;

                return modifiers;
            }
        }

        public string Name { get { return this.propertyDefinition.Name; } }

        public BuilderType ReturnType
        {
            get
            {
                var type = this.Getter?.ReturnType.typeReference ?? this.propertyDefinition.PropertyType;

                if (type.HasGenericParameters && !type.IsGenericInstance)
                    return new BuilderType(this.type, type.ResolveType(this.propertyDefinition.PropertyType));

                return new BuilderType(this.type, type);
            }
        }

        public Method Setter { get; private set; }

        public void AddSetter()
        {
            this.propertyDefinition.SetMethod = new MethodDefinition("set_" + this.Name, this.propertyDefinition.GetMethod.Attributes, this.moduleDefinition.TypeSystem.Void);
            this.propertyDefinition.SetMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, this.ReturnType.typeReference));
            this.type.typeDefinition.Methods.Add(this.propertyDefinition.SetMethod);

            this.Setter = new Method(this.type, this.propertyDefinition.SetMethod);
            this.Setter.NewCode().Assign(this.BackingField).Set(this.Setter.NewCode().Parameters[0]).Replace();
        }

        public Field CreateField(Type fieldType, string name) =>
                    this.CreateField(this.moduleDefinition.Import(this.GetTypeDefinition(fieldType).ResolveType(this.DeclaringType.typeReference)), name);

        public Field CreateField(Field field, string name) => this.CreateField(field.fieldRef.FieldType, name);

        public Field CreateField(BuilderType type, string name) => this.CreateField(type.typeReference, name);

        public Field CreateField(TypeReference typeReference, string name) =>
            this.IsStatic ? this.DeclaringType.CreateField(Modifiers.PrivateStatic, typeReference, name) : this.DeclaringType.CreateField(Modifiers.Private, typeReference, name);

        public void Overrides(Property property)
        {
            this.Getter?.methodDefinition.Overrides.Add(property.Getter.methodReference);
            this.Setter?.methodDefinition.Overrides.Add(property.Setter.methodReference);
        }

        #region Equitable stuff

        public static implicit operator string(Property value) => value.ToString();

        public static bool operator !=(Property a, Property b) => !(a == b);

        public static bool operator ==(Property a, Property b)
        {
            if (object.Equals(a, null) && object.Equals(b, null))
                return true;

            if (object.Equals(a, null))
                return false;

            return a.Equals(b);
        }

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
                return this.propertyDefinition == obj as PropertyDefinition;

            if (obj is PropertyReference)
                return this.propertyDefinition == obj as PropertyReference;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(Property other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.propertyDefinition == other.propertyDefinition;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.propertyDefinition.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.propertyDefinition.FullName;

        #endregion Equitable stuff
    }
}