using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public class Method : CecilatorBase, IEquatable<Method>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal MethodDefinition methodDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal MethodReference methodReference;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal BuilderType type;

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

        public ICode Code { get { return new InstructionsSet(this.type, this); } }

        public BuilderType DeclaringType { get { return this.type; } }

        public bool IsCCtor { get { return this.methodDefinition.Name == ".cctor"; } }

        public bool IsCtor { get { return this.methodDefinition.Name == ".ctor"; } }

        public bool IsStatic { get { return this.methodDefinition.IsStatic; } }

        public bool IsVoid
        {
            get
            {
                return this.methodDefinition.ReturnType == this.moduleDefinition.TypeSystem.Void ||
                        this.methodDefinition.MethodReturnType.ReturnType == this.moduleDefinition.TypeSystem.Void ||
                        this.methodReference.ReturnType == this.moduleDefinition.TypeSystem.Void;
            }
        }

        public string Name { get { return this.methodDefinition.Name; } }

        public BuilderType ReturnType { get { return new BuilderType(this.type, this.methodReference.ReturnType); } }

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

        #region Variables

        public LocalVariableCollection LocalVariables { get { return new LocalVariableCollection(this.type, this.methodDefinition.Body.Variables); } }

        public LocalVariable CreateVariable(string name, Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(name, method.DeclaringType);

            return this.CreateVariable(name, method.ReturnType);
        }

        public LocalVariable CreateVariable(string name, BuilderType type)
        {
            var isInitialized = this.methodDefinition.Body.InitLocals;

            var existingVariable = this.methodDefinition.Body.Variables.FirstOrDefault(x => x.Name == name);

            if (existingVariable != null)
                return new LocalVariable(this.type, existingVariable);

            var newVariable = new VariableDefinition(name, this.moduleDefinition.Import(type.typeReference));
            this.methodDefinition.Body.Variables.Add(newVariable);

            if (!isInitialized)
                this.methodDefinition.Body.InitLocals = true;

            return new LocalVariable(this.type, newVariable);
        }

        public LocalVariable CreateVariable(Type type)
        {
            var isInitialized = this.methodDefinition.Body.InitLocals;
            var newVariable = new VariableDefinition("var_" + Guid.NewGuid().ToString().Replace('-', 'x'), this.moduleDefinition.Import(GetTypeDefinition(type)));
            this.methodDefinition.Body.Variables.Add(newVariable);

            if (!isInitialized)
                this.methodDefinition.Body.InitLocals = true;

            return new LocalVariable(this.type, newVariable);
        }

        public LocalVariable CreateVariable(Method method)
        {
            if (method.IsCtor)
                return this.CreateVariable(method.DeclaringType);

            return this.CreateVariable(method.ReturnType);
        }

        public LocalVariable CreateVariable(BuilderType type)
        {
            var isInitialized = this.methodDefinition.Body.InitLocals;
            var newVariable = new VariableDefinition("var_" + Guid.NewGuid().ToString().Replace('-', 'x'), this.moduleDefinition.Import(type.typeReference));
            this.methodDefinition.Body.Variables.Add(newVariable);

            if (!isInitialized)
                this.methodDefinition.Body.InitLocals = true;

            return new LocalVariable(this.type, newVariable);
        }

        #endregion Variables

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