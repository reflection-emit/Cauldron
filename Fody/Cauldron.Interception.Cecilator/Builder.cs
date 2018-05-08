using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cauldron.Interception.Cecilator
{
    public sealed class Builder : CecilatorBase
    {
        internal Builder(WeaverBase weaver) : base(weaver)
        {
        }

        public static Builder Current { get; internal set; }

        public BuilderCustomAttributeCollection CustomAttributes => new BuilderCustomAttributeCollection(this, this.moduleDefinition);

        public string Name => this.moduleDefinition.Name;

        public override bool Equals(object obj) => this.moduleDefinition.Assembly.FullName.Equals(obj?.ToString());

        public TypeReference GetChildrenType(TypeReference type) => this.moduleDefinition.GetChildrenType(type);

        public override int GetHashCode() => this.moduleDefinition.Assembly.FullName.GetHashCode();

        /// <summary>
        /// Returns the void Main method of a program. Returns null if there is non.
        /// </summary>
        /// <returns>Retruns an instance of <see cref="Method"/> representing the static void Main of the program; otherwise null</returns>
        public Method GetMain() =>
            this.FindMethodsByName(SearchContext.Module_NoGenerated, "Main", 1)
                    .FirstOrDefault(x => x.ReturnType == BuilderType.Void && x.Parameters[0].ChildType == BuilderType.String);

        public Method Import(System.Reflection.MethodBase value)
        {
            var result = this.moduleDefinition.ImportReference(value);
            return new Method(new BuilderType(this, result.DeclaringType), result, result.Resolve());
        }

        public MethodReference Import(MethodReference methodReference) => this.moduleDefinition.ImportReference(methodReference);

        public FieldReference Import(FieldReference fieldReference) => this.moduleDefinition.ImportReference(fieldReference);

        public MethodReference Import(MethodReference method, IGenericParameterProvider context) => this.moduleDefinition.ImportReference(method, context);

        public TypeReference Import(Type type) => this.moduleDefinition.ImportReference(type);

        public TypeReference Import(TypeReference typeReference) => this.moduleDefinition.ImportReference(typeReference);

        public Method Import(Method method)
        {
            var result = this.moduleDefinition.ImportReference(method.methodReference);
            return new Method(new BuilderType(this, result.DeclaringType), result, result.Resolve());
        }

        public override string ToString() => this.moduleDefinition.Assembly.FullName;

        #region Type Finders

        public IEnumerable<BuilderType> FindTypes(string regexPattern) => this.FindTypes(SearchContext.Module, regexPattern);

        public IEnumerable<BuilderType> FindTypes(SearchContext searchContext, string regexPattern) =>
            this.GetTypesInternal(searchContext)
                .Where(x => Regex.IsMatch(x.FullName, regexPattern, RegexOptions.Singleline))
                .Select(x => new BuilderType(this, x));

        public IEnumerable<AttributedType> FindTypesByAttribute(BuilderType attributeType) => this.FindTypesByAttribute(SearchContext.Module, attributeType);

        public IEnumerable<AttributedType> FindTypesByAttribute(SearchContext searchContext, BuilderType attributeType)
        {
            var result = new ConcurrentBag<AttributedType>();

            Parallel.ForEach(this.GetTypes(searchContext), type =>
            {
                for (int i = 0; i < type.typeDefinition.CustomAttributes.Count; i++)
                {
                    var name = type.typeDefinition.CustomAttributes[i].AttributeType.Resolve().FullName;
                    if (attributeType.Fullname.GetHashCode() == name.GetHashCode() && attributeType.Fullname == name)
                        result.Add(new AttributedType(type, type.typeDefinition.CustomAttributes[i]));
                }
            });

            return result;
        }

        public IEnumerable<AttributedType> FindTypesByAttributes(IEnumerable<BuilderType> types) => this.FindTypesByAttributes(SearchContext.Module, types);

        public IEnumerable<AttributedType> FindTypesByAttributes(SearchContext searchContext, IEnumerable<BuilderType> types)
        {
            var result = new ConcurrentBag<AttributedType>();
            var attributes = types.Select(x => x.Fullname).ToList();

            Parallel.ForEach(this.GetTypes(searchContext), type =>
            {
                for (int i = 0; i < type.typeDefinition.CustomAttributes.Count; i++)
                {
                    if (attributes.Contains(type.typeDefinition.CustomAttributes[i].AttributeType.Resolve().FullName))
                        result.Add(new AttributedType(type, type.typeDefinition.CustomAttributes[i]));
                }
            });

            return result;
        }

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
            var result = new ConcurrentBag<AttributedField>();

            Parallel.ForEach(this.GetTypes(searchContext), type =>
            {
                foreach (var field in type.Fields.Where(x => x.fieldDef.HasCustomAttributes))
                {
                    for (int i = 0; i < field.fieldDef.CustomAttributes.Count; i++)
                    {
                        var fullname = field.fieldDef.CustomAttributes[i].AttributeType.Resolve().FullName;
                        if (fullname.GetHashCode() == attributeName.GetHashCode() && fullname == attributeName)
                            result.Add(new AttributedField(field, field.fieldDef.CustomAttributes[i]));
                    }
                }
            });

            return result;
        }

        public IEnumerable<AttributedField> FindFieldsByAttributes(IEnumerable<BuilderType> types) => this.FindFieldsByAttributes(SearchContext.Module, types);

        public IEnumerable<AttributedField> FindFieldsByAttributes(SearchContext searchContext, IEnumerable<BuilderType> types)
        {
            var result = new ConcurrentBag<AttributedField>();
            var attributes = types.Select(x => x.Fullname).ToList();

            Parallel.ForEach(this.GetTypes(searchContext), type =>
            {
                foreach (var field in type.Fields.Where(x => x.fieldDef.HasCustomAttributes))
                {
                    for (int i = 0; i < field.fieldDef.CustomAttributes.Count; i++)
                    {
                        if (attributes.Contains(field.fieldDef.CustomAttributes[i].AttributeType.Resolve().FullName))
                            result.Add(new AttributedField(field, field.fieldDef.CustomAttributes[i]));
                    }
                }
            });

            return result;
        }

        public IEnumerable<Field> FindFieldsByName(string fieldName) => this.FindFieldsByName(SearchContext.Module, fieldName);

        public IEnumerable<Field> FindFieldsByName(SearchContext searchContext, string fieldName) => this.GetTypes(searchContext).SelectMany(x => x.Fields).Where(x => x.Name == fieldName);

        #endregion Field Finders

        #region Property Finders

        public IEnumerable<AttributedProperty> FindPropertiesByAttributes(IEnumerable<BuilderType> types) => this.FindPropertiesByAttributes(SearchContext.Module, types);

        public IEnumerable<AttributedProperty> FindPropertiesByAttributes(SearchContext searchContext, IEnumerable<BuilderType> types)
        {
            var result = new ConcurrentBag<AttributedProperty>();
            var attributes = types.Select(x => x.Fullname).ToList();

            Parallel.ForEach(this.GetTypes(searchContext), type =>
            {
                PropertyDefinition propertyDefinition = null;
                CustomAttribute customAttribute = null;

                try
                {
                    foreach (var property in type.Properties.Where(x => x.propertyDefinition.HasCustomAttributes))
                    {
                        propertyDefinition = property.propertyDefinition;
                        for (int i = 0; i < propertyDefinition.CustomAttributes.Count; i++)
                        {
                            customAttribute = propertyDefinition.CustomAttributes[i];

                            if (attributes.Contains((customAttribute.AttributeType.Resolve() ?? customAttribute.AttributeType).FullName))
                                result.Add(new AttributedProperty(property, customAttribute));
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"An error has occured while searching for attributes: {e.Message}\r\nProperty: {propertyDefinition?.FullName}\r\nAttribute: {customAttribute?.AttributeType.FullName}", e);
                }
            });

            return result;
        }

        #endregion Property Finders

        #region Method Finders

        public IEnumerable<Method> FindMethods(string regexPattern) => this.FindMethods(SearchContext.Module, regexPattern);

        public IEnumerable<Method> FindMethods(SearchContext searchContext, string regexPattern) => this.GetTypes(searchContext).SelectMany(x => x.Methods).Where(x => Regex.IsMatch(x.Name, regexPattern, RegexOptions.Singleline));

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(Type attributeType) => this.FindMethodsByAttribute(SearchContext.Module, attributeType);

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(SearchContext searchContext, Type attributeType) => this.FindMethodsByAttribute(searchContext, attributeType.FullName);

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(string attributeName) => this.FindMethodsByAttribute(SearchContext.Module, attributeName);

        public IEnumerable<AttributedMethod> FindMethodsByAttribute(SearchContext searchContext, string attributeName)
        {
            var result = new ConcurrentBag<AttributedMethod>();

            Parallel.ForEach(this.GetTypes(searchContext), type =>
            {
                foreach (var method in type.Methods.Where(x => x.methodDefinition.HasCustomAttributes))
                {
                    var asyncResult = this.GetAsyncMethod(method.methodDefinition);

                    for (int i = 0; i < method.methodDefinition.CustomAttributes.Count; i++)
                    {
                        var fullname = method.methodDefinition.CustomAttributes[i].AttributeType.Resolve().FullName;
                        if (attributeName.GetHashCode() == fullname.GetHashCode() && fullname == attributeName)
                        {
                            if (asyncResult == null)
                                result.Add(new AttributedMethod(method, method.methodDefinition.CustomAttributes[i], asyncResult));
                            else
                                result.Add(new AttributedMethod(method, method.methodDefinition.CustomAttributes[i], asyncResult));
                        }
                    }
                }
            });

            return result;
        }

        public IEnumerable<AttributedMethod> FindMethodsByAttributes(IEnumerable<BuilderType> types) => this.FindMethodsByAttributes(SearchContext.Module, types);

        public IEnumerable<AttributedMethod> FindMethodsByAttributes(SearchContext searchContext, IEnumerable<BuilderType> types)
        {
            var result = new ConcurrentBag<AttributedMethod>();
            var attributes = types.Select(x => x.Fullname).ToList();

            Parallel.ForEach(this.GetTypes(searchContext), type =>
            {
                foreach (var method in type.Methods.Where(x => x.methodDefinition.HasCustomAttributes))
                {
                    var asyncResult = this.GetAsyncMethod(method.methodDefinition);

                    for (int i = 0; i < method.methodDefinition.CustomAttributes.Count; i++)
                    {
                        if (attributes.Contains(method.methodDefinition.CustomAttributes[i].AttributeType.Resolve().FullName))
                        {
                            if (asyncResult == null)
                                result.Add(new AttributedMethod(method, method.methodDefinition.CustomAttributes[i], asyncResult));
                            else
                                result.Add(new AttributedMethod(method, method.methodDefinition.CustomAttributes[i], asyncResult));
                        }
                    }
                }
            });

            return result;
        }

        public IEnumerable<Method> FindMethodsByName(string methodName, int parameterCount) => this.FindMethodsByName(SearchContext.Module, methodName, parameterCount);

        public IEnumerable<Method> FindMethodsByName(SearchContext searchContext, string methodName, int parameterCount) => this.GetTypes(searchContext).SelectMany(x => x.GetMethods(methodName, parameterCount, false));

        public IEnumerable<Method> FindMethodsByName(string methodName) => this.FindMethodsByName(SearchContext.Module, methodName);

        public IEnumerable<Method> FindMethodsByName(SearchContext searchContext, string methodName) => this.GetTypes(searchContext).SelectMany(x => x.GetMethods(methodName, 0));

        #endregion Method Finders

        #region Attribute Finders

        private IEnumerable<BuilderType> findAttributesInModuleCache;

        public IEnumerable<BuilderType> FindAttributesByBaseClass(string baseClassName) => this.FindAttributesInModule().Where(x => x.BaseClasses.Any(y => y.Fullname.GetHashCode() == baseClassName.GetHashCode() && y.Fullname == baseClassName));

        public IEnumerable<BuilderType> FindAttributesByInterfaces(Type[] interfaceTypes) => this.FindAttributesByInterfaces(interfaceTypes.Select(x => x.FullName).ToArray());

        public IEnumerable<BuilderType> FindAttributesByInterfaces(IEnumerable<BuilderType> interfaceTypes) => this.FindAttributesInModule().Where(x => interfaceTypes.Any(y => x.Implements(y)));

        public IEnumerable<BuilderType> FindAttributesByInterfaces(params string[] interfaceName) => this.FindAttributesInModule().Where(x => x != null && interfaceName.Any(y => x.Implements(y)));

        public IEnumerable<BuilderType> FindAttributesInModule()
        {
            if (findAttributesInModuleCache == null)
            {
                var stopwatch = Stopwatch.StartNew();

                findAttributesInModuleCache = this.GetTypesInternal(SearchContext.Module)
                   .SelectMany(x =>
                   {
                       var type = x.Resolve();
                       return type
                           .CustomAttributes
                               .Concat(type.Methods.SelectMany(y => y.CustomAttributes))
                               .Concat(type.Fields.SelectMany(y => y.CustomAttributes))
                               .Concat(type.Properties.SelectMany(y => y.CustomAttributes));
                   })
                   .Concat(this.moduleDefinition.CustomAttributes)
                   .Concat(this.moduleDefinition.Assembly.CustomAttributes)
                   .Distinct(new CustomAttributeEqualityComparer())
                   .Select(x => new BuilderType(this, x.AttributeType));

                stopwatch.Stop();
                this.Log($"Finding attributes took {stopwatch.Elapsed.TotalMilliseconds}ms");

                findAttributesInModuleCache = findAttributesInModuleCache.OrderBy(x => x.ToString()).Distinct(new BuilderTypeEqualityComparer()).ToArray();
            }

            return findAttributesInModuleCache;
        }

        #endregion Attribute Finders

        #region Getting types

        public BuilderType GetType(Type type)
        {
            if (type.IsArray)
            {
                var child = type.GetElementType();
                var bt = this.GetType(child.FullName);
                return new BuilderType(this, new ArrayType(this.moduleDefinition.ImportReference(bt.typeReference)));
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() != type)
            {
                var builder = Builder.Current;
                var definition = type.GetGenericTypeDefinition();
                var typeDefinition = this.GetType(definition.FullName);

                return typeDefinition.MakeGeneric(type.GetGenericArguments().Select(x => x.ToBuilderType()).ToArray());
            }

            return this.GetType(type.FullName);
        }

        /// <summary>
        /// Gets a type from its name. Default search context is <see cref="SearchContext.AllReferencedModules"/>
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public BuilderType GetType(string typeName) => this.GetType(typeName, SearchContext.AllReferencedModules);

        public BuilderType GetType(string typeName, SearchContext searchContext)
        {
            var result = this.GetTypesInternal(searchContext).Get(typeName);

            if (result == null)
                throw new TypeNotFoundException($"The type '{typeName}' does not exist in any of the referenced assemblies.");

            if (result.Module.FileName.GetHashCode() != this.moduleDefinition.FileName.GetHashCode() && result.Module.FileName != this.moduleDefinition.FileName)
                this.moduleDefinition.ImportReference(result);

            return new BuilderType(this, result);
        }

        public IEnumerable<BuilderType> GetTypes() => this.GetTypesInternal().Select(x => new BuilderType(this, x)).ToArray();

        public IEnumerable<BuilderType> GetTypes(SearchContext searchContext) => this.GetTypesInternal(searchContext).Select(x => new BuilderType(this, x)).ToArray();

        public IEnumerable<BuilderType> GetTypesInNamespace(string namespaceName) => this.GetTypesInNamespace(SearchContext.Module, namespaceName);

        public IEnumerable<BuilderType> GetTypesInNamespace(SearchContext searchContext, string namespaceName) => this.GetTypes(searchContext).Where(x => x.Namespace == namespaceName);

        public BuilderType MakeArray(BuilderType type) => new BuilderType(this, new ArrayType(this.moduleDefinition.ImportReference(type.typeReference)));

        public bool TypeExists(string typeName) => this.allTypes.Get(typeName) != null;

        public bool TypeExists(string typeName, SearchContext searchContext) => searchContext == SearchContext.AllReferencedModules ? this.allTypes.Get(typeName) != null : this.moduleDefinition.Types.Get(typeName) != null;

        internal IEnumerable<TypeReference> GetTypesInternal() => GetTypesInternal(SearchContext.Module);

        internal IEnumerable<TypeReference> GetTypesInternal(SearchContext searchContext)
        {
            IEnumerable<TypeDefinition> types = null;
            var result = new ConcurrentBag<TypeReference>();

            switch (searchContext)
            {
                case SearchContext.Module:
                    types = this.moduleDefinition.Types;
                    break;

                case SearchContext.Module_NoGenerated:
                    types = this.moduleDefinition.Types.Where(x =>
                                x.FullName?.IndexOf('<') < 0 &&
                                x.FullName?.IndexOf('>') < 0);
                    break;

                case SearchContext.AllReferencedModules:
                    types = this.allTypes;
                    break;
            }

            Parallel.ForEach(types, type =>
            {
                if (type.HasNestedTypes)
                {
                    foreach (var t in type.Recursive(x => x.NestedTypes))
                        result.Add(t);
                }
                else
                    result.Add(type);
            });

            return result;
        }

        #endregion Getting types

        #region Create Type

        public BuilderType CreateType(string namespaceName, string typeName) => this.CreateType(namespaceName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable, typeName, this.moduleDefinition.TypeSystem.Object);

        public BuilderType CreateType(string namespaceName, TypeAttributes attributes, string typeName) => this.CreateType(namespaceName, attributes, typeName, this.moduleDefinition.TypeSystem.Object);

        public BuilderType CreateType(string namespaceName, TypeAttributes attributes, string typeName, BuilderType baseType) => this.CreateType(namespaceName, attributes, typeName, baseType.typeReference);

        private BuilderType CreateType(string namespaceName, TypeAttributes attributes, string typeName, TypeReference baseType)
        {
            var newType = new TypeDefinition(namespaceName, typeName, attributes, this.moduleDefinition.ImportReference(baseType));
            this.moduleDefinition.Types.Add(newType);

            return new BuilderType(this, newType);
        }

        #endregion Create Type
    }
}