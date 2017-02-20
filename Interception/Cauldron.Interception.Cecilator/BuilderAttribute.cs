using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class BuilderAttribute : CecilatorBase, IEquatable<BuilderAttribute>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CustomAttribute attribute;

        internal BuilderAttribute(BuilderType type, CustomAttribute attribute) : base(type)
        {
            this.attribute = attribute;
        }

        #region Equitable stuff

        public static implicit operator string(BuilderAttribute attribute) => attribute.attribute.ToString();

        public static bool operator !=(BuilderAttribute a, BuilderAttribute b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(BuilderAttribute a, BuilderAttribute b) => !object.Equals(a, null) && a.Equals(b);

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
                return this.attribute == (obj as CustomAttribute);

            return false;
        }

        public bool Equals(BuilderAttribute other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.attribute == this.attribute));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.attribute.ToString();

        #endregion Equitable stuff
    }
}