using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Cecilator.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

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

        public string Fullname => this.methodReference.FullName;

        public override string Identification => $"{this.methodDefinition.DeclaringType.Name}-{this.methodDefinition.Name}-{this.methodDefinition.DeclaringType.MetadataToken.RID}-{this.methodDefinition.MetadataToken.RID}";

        public bool IsAsync => this.methodDefinition.ReturnType.FullName.EqualsEx("System.Threading.Tasks.Task") || (this.methodDefinition.ReturnType.Resolve()?.FullName.EqualsEx("System.Threading.Tasks.Task`1") ?? false);

        /// <summary>
        /// True if the method is a .ctor or .cctor
        /// </summary>
        public bool IsConstructor => this.IsCCtor || this.IsCtor;

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

        public bool IsGenerated => this.methodDefinition.Name.IndexOf('<') >= 0 ||
                    this.methodDefinition.Name.IndexOf('>') >= 0 ||
                    this.type.typeDefinition.FullName.IndexOf('<') >= 0 ||
                    this.type.typeDefinition.FullName.IndexOf('>') >= 0;

        public bool IsPropertyGetterSetter => (this.methodDefinition.Name.StartsWith("get_") || this.methodDefinition.Name.StartsWith("set_")) && this.methodDefinition.IsSpecialName;
        public bool IsProtected => this.methodDefinition.Attributes.HasFlag(MethodAttributes.Family);
        public bool IsPublicOrInternal => this.IsPublic || this.IsInternal;
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

        /// <summary>
        /// Gets the type that contains the method.
        /// </summary>
        public BuilderType DeclaringType => new BuilderType(this.type.Builder, this.methodReference.DeclaringType);

        public bool IsAbstract => this.methodDefinition.IsAbstract;
        public bool IsCCtor => this.methodDefinition.Name == ".cctor";
        public bool IsInternal => this.methodDefinition.Attributes.HasFlag(MethodAttributes.Assembly);
        public bool IsPrivate => this.methodDefinition.IsPrivate;
        public bool IsPublic => this.methodDefinition.IsPublic;
        public bool IsSpecialName => this.methodDefinition.IsSpecialName;
        public bool IsStatic => this.methodDefinition.IsStatic;
        public string Name => this.methodDefinition.Name;

        /// <summary>
        /// Gets the type that inherited the method.
        /// </summary>
        public BuilderType OriginType => this.type;

        public BuilderType[] Parameters =>
            this.methodReference.Parameters.Select(x => new BuilderType(this.OriginType.Builder, x.ParameterType.ResolveType(this.type.typeReference, this.methodReference))).ToArray();

        public BuilderType ReturnType => new BuilderType(this.type, this.methodReference.ReturnType);

        public Method Copy() => this.NewCoder().Copy(Modifiers.Private, $"<{this.Name}>m__original");

        public Field CreateField(Type fieldType, string name) =>
            this.CreateField(this.moduleDefinition.ImportReference(fieldType.GetTypeDefinition().ResolveType(this.OriginType.typeReference)), name);

        public Field CreateField(Field field, string name) => this.CreateField(field.fieldRef.FieldType, name);

        public Field CreateField(BuilderType type, string name) => this.CreateField(type.typeReference, name);

        public Field CreateField(TypeReference typeReference, string name) =>
            this.IsStatic ? this.OriginType.CreateField(Modifiers.PrivateStatic, typeReference, name) : this.OriginType.CreateField(Modifiers.Private, typeReference, name);

        public IEnumerable<MethodUsage> FindUsages()
        {
            var result = this.type.Builder.GetTypes()
                .SelectMany(x => x.Methods)
                .SelectMany(x => this.GetMethodUsage(x));

            return result;
        }

        /// <summary>
        /// Returns all constant strings in the instruction body
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLoadStrings()
        {
            foreach (var items in this.methodDefinition.Body.Instructions)
            {
                if (items.OpCode != OpCodes.Ldstr)
                    continue;

                yield return items.Operand as string;
            }
        }

        /// <summary>
        /// Gets or creates a local variable
        /// </summary>
        /// <param name="type">The type of the variable to create</param>
        /// <param name="name">The name of the variable</param>
        /// <returns></returns>
        public LocalVariable GetOrCreateVariable(Type type, string name = null) =>
            this.GetOrCreateVariable(this.moduleDefinition.ImportReference(type.GetTypeDefinition().ResolveType(this.OriginType.typeReference)), name);

        /// <summary>
        /// Gets or creates a local variable
        /// </summary>
        /// <param name="type">The type of the variable to create</param>
        /// <param name="name">The name of the variable</param>
        /// <returns></returns>
        public LocalVariable GetOrCreateVariable(BuilderType type, string name = null) =>
            this.GetOrCreateVariable(type.typeReference, name);

        /// <summary>
        /// Gets or creates a local variable
        /// </summary>
        /// <param name="type">The type of the variable to create</param>
        /// <param name="name">The name of the variable</param>
        /// <returns></returns>
        public LocalVariable GetOrCreateVariable(TypeReference type, string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = CodeBlocks.VariablePrefix + CodeBlocks.GenerateName();
            else
            {
                var existingVariable = this.GetVariable(name);

                if (existingVariable != null)
                    return new LocalVariable(this.type, existingVariable.variable, name);
            }

            var newVariable = new VariableDefinition(this.moduleDefinition.ImportReference(type));
            this.AddLocalVariable(name, newVariable);

            return new LocalVariable(this.type, newVariable, name);
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

        public LocalVariable GetVariable(int index)
        {
            if (this.methodDefinition.Body.Variables.Count < index)
                throw new ArgumentException($"Variable with the index {index} does not exist.");

            var variable = this.methodDefinition.Body.Variables[index];
            return new LocalVariable(variable.VariableType.ToBuilderType(), variable);
        }

        public LocalVariable GetVariable(string name)
        {
            if (this.methodDefinition.DebugInformation.Scope == null)
                this.methodDefinition.DebugInformation.Scope = new ScopeDebugInformation(this.methodDefinition.Body.Instructions.First(), this.methodDefinition.Body.Instructions.Last());

            var variableIndex = this.methodDefinition.DebugInformation.Scope.Variables?.FirstOrDefault(x => x.Name == name)?.Index;

            if (variableIndex.HasValue)
            {
                var result = this.methodDefinition.Body.Variables[variableIndex.Value];
                return new LocalVariable(result.VariableType.ToBuilderType(), result);
            }

            return null;
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

        public Method Import()
        {
            MethodReference result = null;

            try
            {
                result = this.moduleDefinition.ImportReference(this.methodReference);
            }
            catch (NullReferenceException)
            {
                result = this.moduleDefinition.ImportReference(this.methodReference, this.type.typeReference);
            }

            return new Method(this.type, result, this.methodDefinition);
        }

        public Method MakeGeneric(params Type[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.ImportReference(x)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(x)).ToArray()), this.methodDefinition);
        }

        public Method MakeGeneric(params TypeReference[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.ImportReference(x)).ToArray()), this.methodDefinition);
            else if (this.methodDefinition.ContainsGenericParameter)
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(x)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(x)).ToArray()), this.methodDefinition);
        }

        public Method MakeGeneric(params BuilderType[] types) => this.MakeGeneric(types.Select(x => x.typeReference).ToArray());

        public Method MakeGeneric(params string[] types)
        {
            if (this.methodDefinition.GenericParameters.Count == 0)
                return new Method(this.type.Builder, this.methodDefinition.MakeHostInstanceGeneric(types.Select(x => this.moduleDefinition.ImportReference(this.type.Builder.GetType(x).typeReference)).ToArray()), this.methodDefinition);
            else
                return new Method(this.type.Builder, this.methodDefinition.MakeGeneric(null, types.Select(x => this.moduleDefinition.ImportReference(this.type.Builder.GetType(x).typeReference)).ToArray()), this.methodDefinition);
        }

        //public ICode NewCode() => new InstructionsSet(this.type, this);

        public void Overrides(Method method) => this.methodDefinition.Overrides.Add(method.methodReference);

        internal VariableDefinition AddLocalVariable(string name, VariableDefinition variable)
        {
            if (this.methodDefinition.DebugInformation.Scope == null)
                this.methodDefinition.DebugInformation.Scope = new ScopeDebugInformation(this.methodDefinition.Body.Instructions.First(), this.methodDefinition.Body.Instructions.Last());

            if (this.methodDefinition.DebugInformation.Scope.Variables.Any(x => x.Name == name))
                throw new ArgumentException($"The variable with the name '{name}' already exist in '{this.Name}'");

            this.methodDefinition.DebugInformation.Scope.Variables.Add(new VariableDebugInformation(variable, name));
            this.methodDefinition.Body.Variables.Add(variable);

            return variable;
        }

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