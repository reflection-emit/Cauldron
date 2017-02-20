using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class LocalVariable : CecilatorBase, IEquatable<LocalVariable>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal VariableDefinition variable;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BuilderType type;

        internal LocalVariable(BuilderType type, VariableDefinition variable) : base(type)
        {
            this.variable = variable;
            this.type = type;
        }

        public int Index { get { return this.variable.Index; } }
        public bool IsPinned { get { return this.variable.IsPinned; } }
        public string Name { get { return this.variable.Name; } }
        public BuilderType Type { get { return new BuilderType(this.type, this.variable.VariableType); } }

        #region Equitable stuff

        public static implicit operator string(LocalVariable localVariable) => localVariable.variable.Name;

        public static bool operator !=(LocalVariable a, LocalVariable b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(LocalVariable a, LocalVariable b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is LocalVariable)
                return this.Equals(obj as LocalVariable);

            return false;
        }

        public bool Equals(LocalVariable other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.variable == this.variable));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.variable.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.variable.ToString();

        #endregion Equitable stuff
    }
}