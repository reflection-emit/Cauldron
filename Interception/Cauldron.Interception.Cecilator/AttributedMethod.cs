using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class AttributedMethod : CecilatorBase, IEquatable<AttributedMethod>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal CustomAttribute customAttribute;

        internal AttributedMethod(Method method, CustomAttribute customAttribute) : base(method)
        {
            this.customAttribute = customAttribute;
            this.Method = method;
            this.Attribute = new BuilderType(method.type, customAttribute.AttributeType);
        }

        public BuilderType Attribute { get; private set; }
        public Method Method { get; private set; }

        #region Equitable stuff

        public static implicit operator string(AttributedMethod field) => field.ToString();

        public static bool operator !=(AttributedMethod a, AttributedMethod b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(AttributedMethod a, AttributedMethod b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is AttributedMethod)
                return this.Equals(obj as AttributedMethod);

            return false;
        }

        public bool Equals(AttributedMethod other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) ||
            (other.Method.methodDefinition.FullName == this.Method.methodDefinition.FullName && other.Attribute.typeDefinition.FullName == this.Attribute.typeDefinition.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Method.GetHashCode() ^ this.Attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{this.Method} >> {this.Attribute}";

        #endregion Equitable stuff
    }
}