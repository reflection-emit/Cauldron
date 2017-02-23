using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cauldron.Interception.Cecilator
{
    public sealed class Builder : CecilatorBase
    {
        internal Builder(IWeaver weaver) : base(weaver)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => this.ToString().Equals(obj.ToString());

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => this.moduleDefinition.Assembly.FullName.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => this.moduleDefinition.Assembly.FullName;

        #region Type Finders

        public IEnumerable<BuilderType> FindTypes(string regexPattern) => this.FindTypes(SearchContext.Module, regexPattern);

        public IEnumerable<BuilderType> FindTypes(SearchContext searchContext, string regexPattern) =>
            this.GetTypesInternal(searchContext)
                .Where(x => Regex.IsMatch(x.FullName, regexPattern, RegexOptions.Singleline))
                .Select(x => new BuilderType(this, x));

        public IEnumerable<BuilderType> FindTypesByBaseClass(string baseClassName) => this.FindTypesByBaseClass(SearchContext.Module, baseClassName);

        public IEnumerable<BuilderType> FindTypesByBaseClass(SearchContext searchContext, string baseClassName) => this.GetTypes(searchContext).Where(x => x.Inherits(baseClassName));

        public IEnumerable<BuilderType> FindTypesByBaseClass(Type baseClassType) => this.FindTypesByBaseClass(SearchContext.Module, baseClassType);

        public IEnumerable<BuilderType> FindTypesByBaseClass(SearchContext searchContext, Type baseClassType)
        {
            if (!baseClassType.IsInterface)
                throw new ArgumentException("Argument 'interfaceType' is not an interface");

            return this.FindTypesByBaseClass(searchContext, baseClassType.FullName);
        }

        public IEnumerable<BuilderType> FindTypesByInterface(string interfaceName) => this.FindTypesByInterface(SearchContext.Module, interfaceName);

        public IEnumerable<BuilderType> FindTypesByInterface(SearchContext searchContext, string interfaceName) => this.GetTypes(searchContext).Where(x => x.Implements(interfaceName));

        public IEnumerable<BuilderType> FindTypesByInterface(Type interfaceType) => this.FindTypesByInterface(SearchContext.Module, interfaceType);

        public IEnumerable<BuilderType> FindTypesByInterface(SearchContext searchContext, Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("Argument 'interfaceType' is not an interface");

            return this.FindTypesByInterface(searchContext, interfaceType.FullName);
        }

        public IEnumerable<BuilderType> FindTypesByInterfaces(params string[] interfaceNames) => this.FindTypesByInterfaces(SearchContext.Module, interfaceNames);

        public IEnumerable<BuilderType> FindTypesByInterfaces(SearchContext searchContext, params string[] interfaceNames) => this.GetTypes(searchContext).Where(x => interfaceNames.Any(y => x.Implements(y)));

        public IEnumerable<BuilderType> FindTypesByInterfaces(params Type[] interfaceTypes) => this.FindTypesByInterfaces(SearchContext.Module, interfaceTypes);

        public IEnumerable<BuilderType> FindTypesByInterfaces(SearchContext searchContext, params Type[] interfaceTypes) => this.FindTypesByInterfaces(searchContext, interfaceTypes.Select(x => x.FullName).ToArray());

        #endregion Type Finders

        #region Field Finders

        public IEnumerable<Field> FindFields(string regexPattern) => this.FindFields(SearchContext.Module, regexPattern);

        public IEnumerable<Field> FindFields(SearchContext searchContext, string regexPattern) => this.GetTypes(searchContext).SelectMany(x => x.Fields).Where(x => Regex.IsMatch(x.Name, regexPattern, RegexOptions.Singleline));

        public IEnumerable<AttributedField> FindFieldsByAttribute(Type attributeType) => this.FindFieldsByAttribute(SearchContext.Module, attributeType);

        public IEnumerable<AttributedField> FindFieldsByAttribute(SearchContext searchContext, Type attributeType) => FindFieldsByAttribute(searchContext, attributeType.FullName);

        public IEnumerable<AttributedField> FindFieldsByAttribute(string attributeName) => this.FindFieldsByAttribute(SearchContext.Module, attributeName);

        public IEnumerable<AttributedField> FindFieldsByAttribute(SearchContext searchContext, string attributeName)
        {
            var result = this.GetTypes(searchContext)
                .SelectMany(x => x.Fields)
                .Where(x => x.fieldDef.HasCustomAttributes)
                .Select(x => new { Field = x, CustomAttributes = x.fieldDef.CustomAttributes.Where(y => y.AttributeType.FullName == attributeName) })
                .Where(x => x.CustomAttributes.Any());

            foreach (var item in result)
                foreach (var attrib in item.CustomAttributes)
                    yield return new AttributedField(item.Field, attrib);
        }

        public IEnumerable<AttributedField> FindFieldsByAttributes(IEnumerable<BuilderType> types) => this.FindFieldsByAttributes(SearchContext.Module, types);

        public IEnumerable<AttributedField> FindFieldsByAttributes(SearchContext searchContext, IEnumerable<BuilderType> types)
        {
            var result = this.GetTypes(searchContext)
                .SelectMany(x => x.Fields)
                .Where(x => x.fieldDef.HasCustomAttributes)
                .Select(x => new { Field = x, CustomAttributes = x.fieldDef.CustomAttributes.Where(y => types.Any(t => t.typeDefinition.FullName == y.AttributeType.Resolve().FullName)) })
                .Where(x => x.CustomAttributes.Any());

            foreach (var item in result)
                foreach (var attrib in item.CustomAttributes)
                    yield return new AttributedField(item.Field, attrib);
        }

        public IEnumerable<Field> FindFieldsByName(string fieldName) => this.FindFieldsByName(SearchContext.Module, fieldName);

        public IEnumerable<Field> FindFieldsByName(SearchContext searchContext, string fieldName) => this.GetTypes(searchContext).SelectMany(x => x.Fields).Where(x => x.Name == fieldName);

        #endregion Field Finders

        #region Method Finders

        public IEnumerable<Method> FindMethods(string regexPattern) => this.FindMethods(SearchContext.Module, regexPattern);

        public IEnumerable<Method> FindMethods(SearchContext searchContext, string regexPattern) => this.GetTypes(searchContext).SelectMany(x => x.Methods).Where(x => Regex.IsMatch(x.Name, regexPattern, RegexOptions.Singleline));

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(Type attributeType) => this.FindMethodsByAttribute(SearchContext.Module, attributeType);

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(SearchContext searchContext, Type attributeType) => this.FindMethodsByAttribute(searchContext, attributeType.FullName);

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(string attributeName) => this.FindMethodsByAttribute(SearchContext.Module, attributeName);

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(SearchContext searchContext, string attributeName)
        {
            var result = this.GetTypes(searchContext)
                .SelectMany(x => x.Methods)
                .Where(x => x.methodDefinition.HasCustomAttributes)
                .Select(x => new { Method = x, CustomAttributes = x.methodDefinition.CustomAttributes.Where(y => y.AttributeType.FullName == attributeName) })
                .Where(x => x.CustomAttributes.Any() && x.Method != null);

            foreach (var item in result)
                foreach (var attrib in item.CustomAttributes)
                    yield return new AttributedMethod(item.Method, attrib);
        }

        public IEnumerable<AttributedMethod> FindMethodsByAttributes(IEnumerable<BuilderType> types) => this.FindMethodsByAttributes(SearchContext.Module, types);

        public IEnumerable<AttributedMethod> FindMethodsByAttributes(SearchContext searchContext, IEnumerable<BuilderType> types)
        {
            var result = this.GetTypes(searchContext)
                .SelectMany(x => x.Methods)
                .Where(x => x.methodDefinition.HasCustomAttributes)
                .Select(x => new { Method = x, CustomAttributes = x.methodDefinition.CustomAttributes.Where(y => types.Any(t => t.typeDefinition == y.AttributeType.Resolve())) })
                .Where(x => x.CustomAttributes.Any() && x.Method != null);

            foreach (var item in result)
                foreach (var attrib in item.CustomAttributes)
                    yield return new AttributedMethod(item.Method, attrib);
        }

        public IEnumerable<Method> FindMethodsByName(string methodName, int parameterCount) => this.FindMethodsByName(SearchContext.Module, methodName, parameterCount);

        public IEnumerable<Method> FindMethodsByName(SearchContext searchContext, string methodName, int parameterCount) => this.GetTypes(searchContext).SelectMany(x => x.GetMethods(methodName, parameterCount));

        public IEnumerable<Method> FindMethodsByName(string methodName) => this.FindMethodsByName(SearchContext.Module, methodName);

        public IEnumerable<Method> FindMethodsByName(SearchContext searchContext, string methodName) => this.GetTypes(searchContext).SelectMany(x => x.GetMethods(methodName, 0));

        #endregion Method Finders

        #region Attribute Finders

        public IEnumerable<BuilderType> FindAttributesByInterfaces(Type[] interfaceTypes) => this.FindAttributesByInterfaces(interfaceTypes.Select(x => x.FullName).ToArray());

        public IEnumerable<BuilderType> FindAttributesByInterfaces(IEnumerable<BuilderType> interfaceTypes) => this.FindAttributesInModule().Where(x => interfaceTypes.Any(y => x.Implements(y)));

        public IEnumerable<BuilderType> FindAttributesByInterfaces(params string[] interfaceName) => this.FindAttributesInModule().Where(x => interfaceName.Any(y => x.Implements(y)));

        public IEnumerable<BuilderType> FindAttributesInModule() =>
                    this.GetTypesInternal(SearchContext.Module)
                .SelectMany(x => x.Resolve().Methods)
                .Where(x => x.HasCustomAttributes)
                .SelectMany(x => x.CustomAttributes)
                .Concat(
                        this.GetTypesInternal(SearchContext.Module)
                            .Select(x => x.Resolve())
                            .Where(x => x.HasCustomAttributes)
                            .SelectMany(x => x.CustomAttributes)
                    )
                .Distinct(new CustomAttributeEqualityComparer())
                .Select(x => new BuilderType(this, x.AttributeType));

        #endregion Attribute Finders

        #region Getting types

        private IEnumerable<TypeReference> typeCache;

        public BuilderType GetType(Type type)
        {
            if (type.IsArray)
            {
                var child = type.GetElementType();
                var bt = this.GetType(child.FullName);
                return new BuilderType(this, new ArrayType(this.moduleDefinition.Import(bt.typeReference)));
            }

            return this.GetType(type.FullName);
        }

        public BuilderType GetType(string typeName)
        {
            var nameSpace = typeName.Substring(0, typeName.LastIndexOf('.'));
            var result = this.allTypes.FirstOrDefault(x => x.FullName == typeName || (x.Name == typeName && x.Namespace == nameSpace));

            if (result == null)
                throw new TypeNotFoundException($"The type '{typeName}' does not exist in any of the referenced assemblies.");

            return new BuilderType(this, result);
        }

        public IEnumerable<BuilderType> GetTypes() => this.GetTypesInternal().Select(x => new BuilderType(this, x)).ToArray();

        public IEnumerable<BuilderType> GetTypes(SearchContext searchContext) => this.GetTypesInternal(searchContext).Select(x => new BuilderType(this, x)).ToArray();

        public IEnumerable<BuilderType> GetTypesInNamespace(string namespaceName) => this.GetTypesInNamespace(SearchContext.Module, namespaceName);

        public IEnumerable<BuilderType> GetTypesInNamespace(SearchContext searchContext, string namespaceName) => this.GetTypes(searchContext).Where(x => x.Namespace == namespaceName);

        internal IEnumerable<TypeReference> GetTypesInternal() => GetTypesInternal(SearchContext.Module);

        internal IEnumerable<TypeReference> GetTypesInternal(SearchContext searchContext)
        {
            if (searchContext == SearchContext.Module)
                return this.moduleDefinition.Types
                    .SelectMany(x => GetInterfaces(x).Concat(GetBaseClasses(x)).Concat(GetNestedTypes(x).Concat(new TypeReference[] { x })))
                    .Where(x => x.Module.Assembly == this.moduleDefinition.Assembly)
                    .Distinct(new TypeReferenceEqualityComparer());
            else
            {
                if (typeCache == null)
                    typeCache = this.allTypes
                    .SelectMany(x => GetNestedTypes(x).Concat(new TypeReference[] { x }))
                    .Where(x => !x.Module.Assembly.FullName.StartsWith("System."))
                    .Distinct(new TypeReferenceEqualityComparer());

                return typeCache;
            }
        }

        #endregion Getting types
    }
}