using Mono.Cecil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class AttributedMethod : CecilatorBase, IEquatable<AttributedMethod>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly CustomAttribute customAttribute;

        internal AttributedMethod(Method method, CustomAttribute customAttribute) : base(method)
        {
            this.customAttribute = customAttribute;
            this.Method = method;
            this.Attribute = new BuilderType(method.type, customAttribute.AttributeType);
        }

        public BuilderType Attribute { get; private set; }

        public Method Method { get; private set; }

        public void Remove() => this.Method.methodDefinition.CustomAttributes.Remove(this.customAttribute);

        #region Equitable stuff

        public static implicit operator string(AttributedMethod value) => value.ToString();

        public static bool operator !=(AttributedMethod a, AttributedMethod b) => !(a == b);

        public static bool operator ==(AttributedMethod a, AttributedMethod b)
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

            if (obj is AttributedMethod)
                return this.Equals(obj as AttributedMethod);

            if (obj is MethodDefinition)
                return this.Method.methodDefinition == obj as MethodDefinition;

            if (obj is MethodReference)
                return this.Method.methodReference == obj as MethodReference;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(AttributedMethod other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.customAttribute == other.customAttribute && this.Method.methodReference == other.Method.methodReference;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.Method.GetHashCode() ^ this.Attribute.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => $"{this.Method} >> {this.Attribute}";

        #endregion Equitable stuff
    }
}