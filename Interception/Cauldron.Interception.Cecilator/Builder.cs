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

        public IEnumerable<BuilderType> FindTypes(string regexPattern) => this.Types.Where(x => Regex.IsMatch(x.Fullname, regexPattern, RegexOptions.Singleline));

        public IEnumerable<BuilderType> FindTypesByBaseClass(string baseClassName) => this.Types.Where(x => x.Inherits(baseClassName));

        public IEnumerable<BuilderType> FindTypesByBaseClass(Type baseClassType)
        {
            if (!baseClassType.IsInterface)
                throw new ArgumentException("Argument 'interfaceType' is not an interface");

            return this.FindTypesByBaseClass(baseClassType.FullName);
        }

        public IEnumerable<BuilderType> FindTypesByInterface(string interfaceName) => this.Types.Where(x => x.Implements(interfaceName));

        public IEnumerable<BuilderType> FindTypesByInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException("Argument 'interfaceType' is not an interface");

            return this.FindTypesByInterface(interfaceType.FullName);
        }

        #endregion Type Finders

        #region Field Finders

        public IEnumerable<Field> FindFields(string regexPattern) => this.Types.SelectMany(x => x.Fields).Where(x => Regex.IsMatch(x.Name, regexPattern, RegexOptions.Singleline));

        public IEnumerable<Field> FindFieldsByName(string fieldName) => this.Types.SelectMany(x => x.Fields).Where(x => x.Name == fieldName);

        #endregion Field Finders

        #region Getting types

        public IEnumerable<BuilderType> Types
        {
            get
            {
                return this.moduleDefinition.Types
                    .SelectMany(x => GetInterfaces(x).Concat(GetBaseClasses(x)).Concat(GetNestedTypes(x).Concat(new TypeReference[] { x })))
                    .Where(x => x.Module.Assembly == this.moduleDefinition.Assembly)
                    .Distinct(new TypeReferenceEqualityComparer())
                    .Select(x => new BuilderType(this, x))
                    .ToArray();
            }
        }

        public BuilderType GetType(string typeName)
        {
            var nameSpace = typeName.Substring(0, typeName.LastIndexOf('.'));
            var result = this.allTypes.FirstOrDefault(x => x.FullName == typeName || (x.Name == typeName && x.Namespace == nameSpace));

            if (result == null)
                throw new TypeNotFoundException($"The type '{typeName}' does not exist in any of the referenced assemblies.");

            return new BuilderType(this, result);
        }

        public IEnumerable<BuilderType> GetTypesInNamespace(string namespaceName) => this.Types.Where(x => x.Namespace == namespaceName);

        #endregion Getting types
    }
}