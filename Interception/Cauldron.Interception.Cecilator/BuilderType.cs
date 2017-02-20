using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public sealed class BuilderType : CecilatorBase, IEquatable<BuilderType>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal TypeDefinition typeDefinition;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal TypeReference typeReference;

        public Builder Builder { get; private set; }
        public string Fullname { get { return this.typeReference.FullName; } }
        public bool IsForeign { get { return this.moduleDefinition.Assembly == this.typeDefinition.Module.Assembly; } }
        public string Namespace { get { return this.typeDefinition.Namespace; } }

        public bool Implements(Type interfaceType) => this.Implements(interfaceType.FullName);

        public bool Implements(string interfaceName) => this.Interfaces.Any(x => x.typeReference.FullName == interfaceName || x.typeDefinition.FullName == interfaceName);

        public bool Inherits(Type type) => this.Inherits(typeDefinition.FullName);

        public bool Inherits(string typename) => this.BaseClasses.Any(x => x.typeReference.FullName == typename || x.typeDefinition.FullName == typename);

        #region Constructors

        //internal BuilderType(IWeaver weaver, TypeReference typeReference, TypeDefinition typeDefinition) : base(weaver)
        //{
        //    this.typeDefinition = typeDefinition;
        //    this.typeReference = typeReference;
        //}

        internal BuilderType(Builder builder, TypeDefinition typeDefinition) : base(builder)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeDefinition.ResolveType(typeDefinition);
            this.Builder = builder;
        }

        internal BuilderType(Builder builder, TypeReference typeReference) : base(builder)
        {
            this.typeDefinition = typeReference.Resolve();
            this.typeReference = typeReference;
            this.Builder = builder;
        }

        internal BuilderType(BuilderType builderType, TypeDefinition typeDefinition) : base(builderType)
        {
            this.typeDefinition = typeDefinition;
            this.typeReference = typeDefinition.ResolveType(typeDefinition);
            this.Builder = builderType.Builder;
        }

        internal BuilderType(BuilderType builderType, TypeReference typeReference) : base(builderType)
        {
            this.typeReference = typeReference;
            this.typeDefinition = typeReference.Resolve();
            this.Builder = builderType.Builder;
        }

        #endregion Constructors

        #region Interfaces Base classes and nested types

        public IEnumerable<BuilderType> BaseClasses
        {
            get
            {
                return this.GetBaseClasses(this.typeReference).Select(x => new BuilderType(this, x)).Distinct(new BuilderTypeEqualityComparer());
            }
        }

        public IEnumerable<BuilderType> Interfaces
        {
            get
            {
                return this.GetInterfaces(this.typeReference).Select(x => new BuilderType(this, x)).Distinct(new BuilderTypeEqualityComparer());
            }
        }

        public IEnumerable<BuilderType> NestedTypes
        {
            get
            {
                return this.GetNestedTypes(this.typeReference).Select(x => new BuilderType(this, x)).Distinct(new BuilderTypeEqualityComparer());
            }
        }

        #endregion Interfaces Base classes and nested types

        #region Actions

        public Method ParameterlessContructor
        {
            get
            {
                if (!this.typeDefinition.HasMethods)
                    return null;

                var ctor = this.typeDefinition.Methods.FirstOrDefault(x => x.Name == ".ctor" && x.Parameters.Count == 0);

                if (ctor == null)
                    return null;

                return new Method(this, ctor);
            }
        }

        public Method StaticConstructor
        {
            get
            {
                if (!this.typeDefinition.HasMethods)
                    return null;

                var ctor = this.typeDefinition.Methods.FirstOrDefault(x => x.Name == ".cctor");

                if (ctor == null)
                    return null;

                return new Method(this, ctor);
            }
        }

        public Method CreateStaticConstructor()
        {
            var cctor = this.StaticConstructor;

            if (cctor != null)
                return cctor;

            var method = new MethodDefinition(".cctor", MethodAttributes.Static | MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, this.moduleDefinition.TypeSystem.Void);

            var processor = method.Body.GetILProcessor();
            processor.Append(processor.Create(OpCodes.Ret));
            this.typeDefinition.Methods.Add(method);

            return new Method(this, method);
        }

        /// <summary>
        /// Returns all constructors that does call the base class constructor. All constructors that calls this() is excluded
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Method> GetRelevantConstructors()
        {
            if (this.typeDefinition.HasMethods)
            {
                var ctors = this.typeDefinition.Methods.Where(ctor =>
                {
                    if (ctor.Name != ".ctor")
                        return false;

                    var body = ctor.Body;
                    var first = body.Instructions.FirstOrDefault(x => x.OpCode == OpCodes.Call && (x.Operand as MethodReference).Name == ".ctor");

                    if (first == null)
                        return false;

                    var operand = first.Operand as MethodReference;

                    if (operand.DeclaringType.FullName == this.typeDefinition.BaseType.FullName)
                        return true;

                    return false;
                });

                foreach (var item in ctors)
                    yield return new Method(this, item);

                var cctor = this.StaticConstructor;

                if (cctor != null)
                    yield return cctor;
            }
        }

        public void Remove() => this.moduleDefinition.Types.Remove(this.typeDefinition);

        #endregion Actions

        #region Fields

        public FieldCollection Fields { get { return new FieldCollection(this, this.typeDefinition.Fields); } }

        public Field CreateField(Modifiers modifier, Type fieldType, string name) => this.CreateField(modifier, this.moduleDefinition.Import(this.GetTypeDefinition(fieldType).ResolveType(this.typeReference)), name);

        public Field CreateField(Modifiers modifier, Field field, string name) => this.CreateField(modifier, field.fieldRef.FieldType, name);

        public Field CreateField(Modifiers modifier, TypeReference typeReference, string name)
        {
            var attributes = FieldAttributes.CompilerControlled;

            if (modifier.HasFlag(Modifiers.Private)) attributes |= FieldAttributes.Private;
            if (modifier.HasFlag(Modifiers.Static)) attributes |= FieldAttributes.Static;
            if (modifier.HasFlag(Modifiers.Public)) attributes |= FieldAttributes.Public;

            var field = new FieldDefinition(name, attributes, this.moduleDefinition.Import(typeReference));
            this.typeDefinition.Fields.Add(field);

            return new Field(this, field);
        }

        #endregion Fields

        #region Methods

        public IEnumerable<Method> Methods { get { return this.typeDefinition.Methods.Where(x => x.Body != null).Select(x => new Method(this, x)); } }

        public Method GetMethod(string name)
        {
            var result = this.typeDefinition.Methods
                .Concat(this.BaseClasses.SelectMany(x => x.typeDefinition.Methods))
                .FirstOrDefault(x => x.Name == name && x.Parameters.Count == 0);

            if (result == null)
                throw new MethodNotFoundException($"Unable to proceed. The type '{this.typeDefinition.FullName}' does not contain a method '{name}'");

            return new Method(this, result);
        }

        public Method GetMethod(string name, int parameterCount)
        {
            var result = this.typeDefinition.Methods
                .Concat(this.BaseClasses.SelectMany(x => x.typeDefinition.Methods))
                .FirstOrDefault(x => x.Name == name && x.Parameters.Count == parameterCount);

            if (result == null)
                throw new MethodNotFoundException($"Unable to proceed. The type '{this.typeDefinition.FullName}' does not contain a method '{name}'");

            return new Method(this, result);
        }

        public Method GetMethod(string name, params Type[] parameters)
        {
            var result = this.typeDefinition.Methods
                .Concat(this.BaseClasses.SelectMany(x => x.typeDefinition.Methods))
                .Where(x => x.Name == name && x.Parameters.Count == parameters.Length)
                .FirstOrDefault(x =>
                {
                    var p1 = x.Parameters.Select(y => y.ParameterType.FullName);
                    var p2 = parameters.Select(y => y.FullName);

                    return p1.SequenceEqual(p2);
                });

            if (result == null)
                throw new MethodNotFoundException($"Unable to proceed. The type '{this.typeDefinition.FullName}' does not contain a method '{name}'");

            return new Method(this, result);
        }

        public IEnumerable<Method> GetMethods(string name, int parameterCount)
        {
            var result = this.typeDefinition.Methods
                .Concat(this.BaseClasses.SelectMany(x => x.typeDefinition.Methods))
                .Where(x => x.Name == name && x.Parameters.Count == parameterCount).Select(x => new Method(this, x));

            if (!result.Any())
                throw new MethodNotFoundException($"Unable to proceed. The type '{this.typeDefinition.FullName}' does not contain a method '{name}'");

            return result;
        }

        #endregion Methods

        #region Equitable stuff

        public static implicit operator string(BuilderType type) => type.typeReference.FullName;

        public static bool operator !=(BuilderType a, BuilderType b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator !=(TypeReference a, BuilderType b) => !object.Equals(b, null) && !b.Equals(a);

        public static bool operator !=(BuilderType a, TypeReference b) => !object.Equals(a, null) && !a.Equals(b);

        public static bool operator ==(BuilderType a, BuilderType b) => !object.Equals(a, null) && a.Equals(b);

        public static bool operator ==(TypeReference a, BuilderType b) => !object.Equals(b, null) && b.Equals(a);

        public static bool operator ==(BuilderType a, TypeReference b) => !object.Equals(a, null) && a.Equals(b);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            if (object.Equals(obj, null))
                return false;

            if (object.ReferenceEquals(obj, this))
                return true;

            if (obj is BuilderType)
                return this.Equals(obj as BuilderType);

            if (obj is TypeReference)
                return this.typeDefinition.FullName == (obj as TypeReference).FullName;

            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Equals(BuilderType other) => !object.Equals(other, null) && (object.ReferenceEquals(other, this) || (other.typeDefinition.FullName == this.typeDefinition.FullName));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.typeDefinition.FullName.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.typeReference.FullName + " in " + this.typeDefinition.Module.Assembly.Name.Name;

        #endregion Equitable stuff
    }
}