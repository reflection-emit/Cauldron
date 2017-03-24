using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cauldron.Interception.Cecilator
{
    public class Method : CecilatorBase, IEquatable<Method>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly MethodDefinition methodDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly MethodReference methodReference;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly BuilderType type;

        internal Method(BuilderType type, MethodReference methodReference, MethodDefinition methodDefinition) : base(type)
        {
            this.type = type;
            this.methodDefinition = methodDefinition;
            this.methodReference = methodReference;
        }

        internal Method(Builder builder, MethodReference methodReference, MethodDefinition methodDefinition) : base(builder)
        {
            this.type = new BuilderType(builder, methodReference.DeclaringType);
            this.methodDefinition = methodDefinition;
            this.methodReference = methodReference;
        }

        internal Method(BuilderType type, MethodDefinition methodDefinition) : base(type)
        {
            this.type = type;
            this.methodDefinition = methodDefinition;
            this.methodReference = methodDefinition.CreateMethodReference();
        }

        public BuilderCustomAttributeCollection CustomAttributes { get { return new BuilderCustomAttributeCollection(this.type.Builder, this.methodDefinition); } }

        public BuilderType DeclaringType { get { return this.type; } }

        public string Fullname { get { return this.methodReference.FullName; } }

        public bool IsAbstract { get { return this.methodDefinition.IsAbstract; } }

        public bool IsCCtor { get { return this.methodDefinition.Name == ".cctor"; } }

        public bool IsConstructorWithBaseCall
        {
            get
            {
                if (this.Name != ".ctor")
                    return false;

                var first = this.methodDefinition.Body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");

                if (first == null)
                    return false;

                var operand = first.Operand as MethodReference;

                if (operand.DeclaringType.FullName == this.methodDefinition.DeclaringType.BaseType.FullName)
                    return true;

                return false;
            }
        }

        public bool IsCtor { get { return this.methodDefinition.Name == ".ctor"; } }

        public bool IsPublic { get { return this.methodDefinition.Attributes.HasFlag(MethodAttributes.Public); } }

        public bool IsStatic { get { return this.methodDefinition.IsStatic; } }

        public bool IsVoid { get { return this.methodDefinition.ReturnType.FullName == "System.Void"; } }

        public Modifiers Modifiers
        {
            get
            {
                Modifiers modifiers = 0;

                if (this.methodDefinition.Attributes.HasFlag(MethodAttributes.Private)) modifiers |= Modifiers.Private;
                if (this.methodDefinition.Attributes.HasFlag(MethodAttributes.Static)) modifiers |= Modifiers.Static;
                if (this.methodDefinition.Attributes.HasFlag(MethodAttributes.Public)) modifiers |= Modifiers.Public;

                return modifiers;
            }
        }

        public string Name { get { return this.methodDefinition.Name; } }

        public BuilderType[] Parameters { get { return this.methodReference.Parameters.Select(x => new BuilderType(this.DeclaringType.Builder, x.ParameterType)).ToArray(); } }

        public BuilderType ReturnType { get { return new BuilderType(this.type, this.methodReference.ReturnType); } }

        public Field CreateField(Type fieldType, string name) =>
            this.CreateField(this.moduleDefinition.Import(this.GetTypeDefinition(fieldType).ResolveType(this.DeclaringType.typeReference)), name);

        public Field CreateField(Field field, string name) => this.CreateField(field.fieldRef.FieldType, name);

        public Field CreateField(TypeReference typeReference, string name) =>
            this.IsStatic ? this.DeclaringType.CreateField(Modifiers.PrivateStatic, typeReference, name) : this.DeclaringType.CreateField(Modifiers.Private, typeReference, name);

        public IEnumerable<MethodUsage> FindUsages()
        {
            var result = this.type.Builder.GetTypes()
                .SelectMany(x => x.Methods)
                .SelectMany(x => this.GetMethodUsage(x));

            return result;
        }

        public Method MakeGeneric(params Type[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.Import(x)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.Import(x)).ToArray()), this.methodDefinition);
        }

        public Method MakeGeneric(params BuilderType[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.Import(x.typeReference)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.Import(x.typeReference)).ToArray()), this.methodDefinition);
        }

        public Method MakeGeneric(params string[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.Import(this.type.Builder.GetType(x).typeReference)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.Import(this.type.Builder.GetType(x).typeReference)).ToArray()), this.methodDefinition);
        }

        public ICode NewCode() => new InstructionsSet(this.type, this);

        public void Overrides(Method method) => this.methodDefinition.Overrides.Add(method.methodReference);

        internal ILProcessor GetILProcessor() => this.methodDefinition.Body.GetILProcessor();

        private IEnumerable<MethodUsage> GetMethodUsage(Method method)
        {
            if (method.methodDefinition.Body != null)
                for (int i = 0; i < method.methodDefinition.Body.Instructions.Count; i++)
                {
                    var instruction = method.methodDefinition.Body.Instructions[i];
                    if ((instruction.OpCode == OpCodes.Call ||
                        instruction.OpCode == OpCodes.Callvirt) &&
                        (instruction.Operand as MethodDefinition ?? instruction.Operand as MethodReference).Resolve() == this.methodDefinition)
                        yield return new MethodUsage(this, method, instruction);
                }
        }

        #region Variables

        public LocalVariableCollection LocalVariables { get { return new LocalVariableCollection(this.type, this.methodDefinition.Body.Variables); } }

        #endregion Variables

        #region Equitable stuff

        public static implicit operator string(Method value) => value.ToString();

        public static bool operator !=(Method a, Method b) => !(a == b);

        public static bool operator ==(Method a, Method b)
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

            if (obj is Method)
                return this.Equals(obj as Method);

            if (obj is MethodDefinition)
                return this.methodDefinition == obj as MethodDefinition;

            if (obj is MethodReference)
                return this.methodReference == obj as MethodReference;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(Method other)
        {
            if (object.Equals(other, null))
                return false;

            if (object.ReferenceEquals(other, this))
                return true;

            return this.methodDefinition == other.methodDefinition;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.methodDefinition.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.methodReference.FullName;

        #endregion Equitable stuff
    }
}