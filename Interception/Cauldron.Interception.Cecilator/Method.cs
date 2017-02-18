using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class Method : CecilatorBase, IEquatable<Method>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal MethodDefinition methodDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MethodReference methodReference;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BuilderType type;

        internal Method(BuilderType type, MethodReference methodReference, MethodDefinition methodDefinition) : base(type)
        {
            this.type = type;
            this.methodDefinition = methodDefinition;
            this.methodReference = methodReference;
        }

        internal Method(BuilderType type, MethodDefinition methodDefinition) : base(type)
        {
            this.type = type;
            this.methodDefinition = methodDefinition;
            this.methodReference = methodDefinition.CreateMethodReference();
        }

        public Method Clear(MethodClearOptions options)
        {
            if (options.HasFlag(MethodClearOptions.Body))
                this.methodDefinition.Body.Instructions.Clear();

            if (options.HasFlag(MethodClearOptions.LocalVariables))
                this.methodDefinition.Body.Variables.Clear();

            return this;
        }

        public Method Clear() => this.Clear(MethodClearOptions.Body);

        internal ILProcessor GetILProcessor() => this.methodDefinition.Body.GetILProcessor();

        #region Equitable stuff

        public static implicit operator string(Method method) => method.methodReference.FullName;

        public static bool operator !=(Method a, Method b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(Method a, Method b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is Method)
                return this.Equals(obj as Method);

            if (obj is MethodReference)
                return this.methodDefinition.FullName == (obj as MethodReference).FullName;

            return false;
        }

        public bool Equals(Method other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.methodDefinition.FullName == this.methodDefinition.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.methodDefinition.FullName.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.methodReference.FullName;

        #endregion Equitable stuff
    }
}