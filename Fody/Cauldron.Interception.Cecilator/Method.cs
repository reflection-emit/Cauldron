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
        internal readonly static Dictionary<string, Dictionary<string, VariableDefinition>> variableDictionary = new Dictionary<string, Dictionary<string, VariableDefinition>>();

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly MethodDefinition methodDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly MethodReference methodReference;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal readonly BuilderType type;

        private Method _asyncMethod;

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

        public Method AsyncMethod
        {
            get
            {
                if (_asyncMethod == null)
                    _asyncMethod = this.type.Builder.GetAsyncMethod(this.methodDefinition);

                return _asyncMethod;
            }
        }

        public AsyncMethodHelper AsyncMethodHelper => new AsyncMethodHelper(this);

        public BuilderType AsyncOriginType
        {
            get
            {
                if (this.IsAsync)
                    return this.AsyncMethod == null ? this.type : this.AsyncMethod.type;
                else
                    return this.type;
            }
        }

        public BuilderCustomAttributeCollection CustomAttributes => new BuilderCustomAttributeCollection(this.type.Builder, this.methodDefinition);

        /// <summary>
        /// Gets the type that contains the method.
        /// </summary>
        public BuilderType DeclaringType => new BuilderType(this.type.Builder, this.methodReference.DeclaringType);

        public string Fullname => this.methodReference.FullName;
        public bool IsAbstract => this.methodDefinition.IsAbstract;
        public bool IsAsync => this.methodDefinition.ReturnType.FullName.EqualsEx("System.Threading.Tasks.Task") || this.methodDefinition.ReturnType.Resolve().FullName.EqualsEx("System.Threading.Tasks.Task`1");
        public bool IsCCtor => this.methodDefinition.Name == ".cctor";

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

        public bool IsCtor => this.methodDefinition.Name == ".ctor";
        public bool IsInternal => this.methodDefinition.Attributes.HasFlag(MethodAttributes.Assembly);
        public bool IsPrivate => this.methodDefinition.IsPrivate;
        public bool IsProtected => this.methodDefinition.Attributes.HasFlag(MethodAttributes.Family);
        public bool IsPublic => this.methodDefinition.IsPublic;
        public bool IsPublicOrInternal => this.IsPublic || this.IsInternal;
        public bool IsStatic => this.methodDefinition.IsStatic;
        public bool IsVoid => this.methodDefinition.ReturnType.FullName == "System.Void";

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

        public string Name => this.methodDefinition.Name;

        /// <summary>
        /// Gets the type that inherited the method.
        /// </summary>
        public BuilderType OriginType => this.type;

        public BuilderType[] Parameters =>
            this.methodReference.Parameters.Select(x => new BuilderType(this.OriginType.Builder, x.ParameterType.ResolveType(this.type.typeReference, this.methodReference))).ToArray();

        public BuilderType ReturnType => new BuilderType(this.type, this.methodReference.ReturnType);

        public VariableDefinition AddLocalVariable(string name, VariableDefinition variable)
        {
            if (variableDictionary.TryGetValue(this.methodDefinition.FullName, out Dictionary<string, VariableDefinition> methodsDictionary))
            {
                if (methodsDictionary.ContainsKey(name))
                    throw new ArgumentException($"The variable with the name '{name}' already exist in '{this.Name}'");
            }
            else
            {
                methodsDictionary = new Dictionary<string, VariableDefinition>();
                variableDictionary.Add(this.methodDefinition.FullName, methodsDictionary);
            }

            methodsDictionary.Add(name, variable);
            this.methodDefinition.Body.Variables.Add(variable);

            return variable;
        }

        public Method Copy() => this.NewCode().Copy(Modifiers.Private, $"<{this.Name}>m__original");

        public Field CreateField(Type fieldType, string name) =>
            this.CreateField(this.moduleDefinition.ImportReference(this.GetTypeDefinition(fieldType).ResolveType(this.OriginType.typeReference)), name);

        public Field CreateField(Field field, string name) => this.CreateField(field.fieldRef.FieldType, name);

        public Field CreateField(TypeReference typeReference, string name) =>
            this.IsStatic ? this.OriginType.CreateField(Modifiers.PrivateStatic, typeReference, name) : this.OriginType.CreateField(Modifiers.Private, typeReference, name);

        public IEnumerable<MethodUsage> FindUsages()
        {
            var result = this.type.Builder.GetTypes()
                .SelectMany(x => x.Methods)
                .SelectMany(x => this.GetMethodUsage(x));

            return result;
        }

        public IEnumerable<string> GetLoadStrings()
        {
            foreach (var items in this.methodDefinition.Body.Instructions)
            {
                if (items.OpCode != OpCodes.Ldstr)
                    continue;

                yield return items.Operand as string;
            }
        }

        public VariableDefinition GetLocalVariable(string name)
        {
            Dictionary<string, VariableDefinition> methodsDictionary;

            if (variableDictionary.TryGetValue(this.methodDefinition.FullName, out methodsDictionary))
            {
                VariableDefinition variableDefinition;

                if (methodsDictionary.TryGetValue(name, out variableDefinition))
                    return variableDefinition;
            }

            return null;
        }

        public IEnumerable<TypeReference> GetTokens()
        {
            foreach (var items in this.methodDefinition.Body.Instructions)
            {
                if (items.OpCode != OpCodes.Ldtoken)
                    continue;

                yield return items.Operand as TypeReference;
            }
        }

        public bool HasMethodBaseCall()
        {
            var first = this.methodDefinition.Body.Instructions
                .Where(x => x.OpCode == OpCodes.Call)
                .FirstOrDefault(x =>
                {
                    var o = x.Operand as MethodReference;
                    return o.Name == this.Name && o.Parameters.Count == this.methodDefinition.Parameters.Count &&
                        o.Parameters.Select(y => y.ParameterType).SequenceEqual(this.methodDefinition.Parameters.Select(y => y.ParameterType), new TypeReferenceEqualityComparer());
                });

            if (first == null)
                return false;

            var operand = first.Operand as MethodReference;

            if (operand.DeclaringType.FullName == this.methodDefinition.DeclaringType.BaseType.FullName)
                return true;

            return false;
        }

        public bool HasMethodCall(Method method)
        {
            return this.methodDefinition.Body.Instructions
                 .Where(x => x.OpCode == OpCodes.Call || x.OpCode == OpCodes.Calli || x.OpCode == OpCodes.Callvirt)
                 .Any(x => x.Operand == method.methodReference);
        }

        public Method Import() => new Method(this.type, this.moduleDefinition.ImportReference(this.methodReference), this.methodDefinition);

        public Method MakeGeneric(params Type[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.ImportReference(x)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(x)).ToArray()), this.methodDefinition);
        }

        public Method MakeGeneric(params BuilderType[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.ImportReference(x.typeReference)).ToArray()), this.methodDefinition);
            else if (this.methodDefinition.ContainsGenericParameter)
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(x.typeReference)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(x.typeReference)).ToArray()), this.methodDefinition);
        }

        public Method MakeGeneric(params string[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.ImportReference(this.type.Builder.GetType(x).typeReference)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(this.type.Builder.GetType(x).typeReference)).ToArray()), this.methodDefinition);
        }

        public ICode NewCode() => new InstructionsSet(this.type, this);

        public void Overrides(Method method) => this.methodDefinition.Overrides.Add(method.methodReference);

        internal ILProcessor GetILProcessor() => this.methodDefinition.Body.GetILProcessor();

        private IEnumerable<MethodUsage> GetMethodUsage(Method method)
        {
            if (method.methodDefinition.Body != null)
                for (int i = 0; i < (method.methodDefinition.Body?.Instructions.Count ?? 0); i++)
                {
                    var instruction = method.methodDefinition.Body.Instructions[i];
                    if ((instruction.OpCode == OpCodes.Call ||
                        instruction.OpCode == OpCodes.Callvirt) &&
                        (instruction.Operand as MethodDefinition ?? instruction.Operand as MethodReference).Resolve() == this.methodDefinition)
                        yield return new MethodUsage(this, method, instruction);
                }
        }

        #region Equitable stuff

        public static implicit operator string(Method value) => value?.ToString();

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