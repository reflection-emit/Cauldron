using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderAttribute : CecilatorBase, IEquatable<BuilderAttribute>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly CustomAttribute attribute;

        internal BuilderAttribute(BuilderType type, CustomAttribute attribute) : base(type)
        {
            this.attribute = attribute;
        }

        #region Equitable stuff

        public static implicit operator string(BuilderAttribute value) => value.ToString();

        public static bool operator !=(BuilderAttribute a, BuilderAttribute b) => !(a == b);

        public static bool operator ==(BuilderAttribute a, BuilderAttribute b)
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

            if (obj is BuilderAttribute)
                return this.Equals(obj as BuilderAttribute);

            if (obj is CustomAttribute)
                return this.attribute == obj as CustomAttribute;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(BuilderAttribute other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.attribute == other.attribute;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.attribute.AttributeType.FullName;

        #endregion Equitable stuff
    }
}