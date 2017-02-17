using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public sealed class BuilderType : CecilatorBase, IEquatable<BuilderType>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal TypeDefinition typeDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal TypeReference typeReference;

        public string Fullname { get { return this.typeReference.FullName; } }
        public string Namespace { get { return this.typeDefinition.Namespace; } }

        public bool Implements(Type interfaceType) => this.Implements(interfaceType.FullName);

        public bool Implements(string interfaceName) => this.Interfaces.Any(x => x.typeReference.FullName == interfaceName || x.typeDefinition.FullName == interfaceName);

        public bool Inherits(Type type) => this.Inherits(typeDefinition.FullName);

        public bool Inherits(string typename) => this.BaseClasses.Any(x => x.typeReference.FullName == typename || x.typeDefinition.FullName == typename);

        #region Constructors

        internal BuilderType(IWeaver weaver, TypeReference typeReference, TypeDefinition typeDefinition) : base(weaver)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeReference;
        }

        internal BuilderType(Builder builder, TypeDefinition typeDefinition) : base(builder)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeDefinition.ResolveType(typeDefinition);
        }

        internal BuilderType(Builder builder, TypeReference typeReference) : base(builder)
        {
            this.typeDefinition = typeReference.Resolve();
            this.typeReference = typeReference;
        }

        internal BuilderType(BuilderType builderType, TypeDefinition typeDefinition) : base(builderType)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeDefinition.ResolveType(typeDefinition);
        }

        internal BuilderType(BuilderType builderType, TypeReference typeReference) : base(builderType)
        {
            this.typeReference = typeReference;
            this.typeDefinition = typeReference.Resolve();
        }

        #endregion Constructors

        #region Interfaces Base classes and nested types

        public IEnumerable<BuilderType> BaseClasses
        {
            get
            {
                return this.GetBaseClasses(this.typeReference).Select(x => new BuilderType(this, x)).Distinct(new BuilderTypeEqualityComparer());
            }
        }

        public IEnumerable<BuilderType> Interfaces
        {
            get
            {
                return this.GetInterfaces(this.typeReference).Select(x => new BuilderType(this, x)).Distinct(new BuilderTypeEqualityComparer());
            }
        }

        public IEnumerable<BuilderType> NestedTypes
        {
            get
            {
                return this.GetNestedTypes(this.typeReference).Select(x => new BuilderType(this, x)).Distinct(new BuilderTypeEqualityComparer());
            }
        }

        #endregion Interfaces Base classes and nested types

        #region Actions

        public void Remove() => this.moduleDefinition.Types.Remove(this.typeDefinition);

        #endregion Actions

        #region Fields

        public FieldCollection Fields { get { return new FieldCollection(this, this.typeDefinition.Fields); } }

        #endregion Fields

        #region Equitable stuff

        public static implicit operator string(BuilderType type) => type.typeReference.FullName;

        public static bool operator !=(BuilderType a, BuilderType b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator !=(TypeReference a, BuilderType b) => !object.Equals(b, null) && !b.Equals(a);

        public static bool operator !=(BuilderType a, TypeReference b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(BuilderType a, BuilderType b) => !object.Equals(a, null) && a.Equals(b);

        public static bool operator ==(TypeReference a, BuilderType b) => !object.Equals(b, null) && b.Equals(a);

        public static bool operator ==(BuilderType a, TypeReference b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is BuilderType)
                return this.Equals(obj as BuilderType);

            if (obj is TypeReference)
                return this.typeDefinition.FullName == (obj as TypeReference).FullName;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(BuilderType other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.typeDefinition.FullName == this.typeDefinition.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.typeDefinition.FullName.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.typeReference.FullName;

        #endregion Equitable stuff
    }
}