using Cauldron.Interception.Cecilator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        private const string ModuleName = "<Module>";

        public void AddAssemblyWideAttributes(Builder builder)
        {
            /*
                 We will allow multiple configuration files to allow different nuget packages to add its own conifg
                 without bothering to edit the existing one.
            */

            using (new StopwatchLog(this, "decorator"))
            {
                var inDebugMode = this.DefineConstants.Contains("DEBUG");
                IEnumerable<AssemblyWideAttributeConfig> GetConfigs(string path) =>
                    Directory.GetFiles(path, "*cauldron.fody.json", SearchOption.TopDirectoryOnly)
                        .Select(x => JsonConvert.DeserializeObject<IEnumerable<AssemblyWideAttributeConfig>>(File.ReadAllText(x)))
                        .Where(x => x != null)
                        .SelectMany(x => x)
                        .Where(x => x.IsActive && ((x.DebugOnly && inDebugMode) || !x.DebugOnly));

                var configurations = GetConfigs(this.ProjectDirectoryPath).Concat(GetConfigs(this.SolutionDirectoryPath));

                foreach (var config in configurations)
                {
                    if (config.Decorator == null)
                        continue;

                    ImplementDecorator(builder, config);
                }
            }
        }

        private static object ConvertPropertyType(Builder builder, string typeName, string value)
        {
            switch (typeName)
            {
                case "System.String": return value;
                case "System.Type": return builder.GetType(value);
                case "System.Int16": return Convert.ToInt16(value);
                case "System.Int32": return Convert.ToInt32(value);
                case "System.Int64": return Convert.ToInt64(value);
                case "System.UInt16": return Convert.ToUInt16(value);
                case "System.UInt32": return Convert.ToUInt32(value);
                case "System.UInt64": return Convert.ToUInt64(value);
                case "System.Single": return Convert.ToSingle(value);
                case "System.Double": return Convert.ToDouble(value);
                case "System.Boolean": return Convert.ToBoolean(value);
                case "System.Byte": return Convert.ToByte(value);
                case "System.Char": return Convert.ToChar(value);
                case "System.SByte": return Convert.ToSByte(value);

                default: return null;
            }
        }

        private static Type GetTypeFromString(string typeName)
        {
            switch (typeName)
            {
                case "System.String": return typeof(string);
                case "System.Type": return typeof(Type);
                case "System.Int16": return typeof(short);
                case "System.Int32": return typeof(int);
                case "System.Int64": return typeof(long);
                case "System.UInt16": return typeof(ushort);
                case "System.UInt32": return typeof(uint);
                case "System.UInt64": return typeof(ulong);
                case "System.Single": return typeof(float);
                case "System.Double": return typeof(double);
                case "System.Boolean": return typeof(bool);
                case "System.Byte": return typeof(byte);
                case "System.Char": return typeof(char);
                case "System.SByte": return typeof(sbyte);

                default: return null;
            }
        }

        private static IEnumerable<Method> GetValidMethods(DecoratorDecription decorator, BuilderType[] methodParameters, BuilderType @class)
        {
            return @class.Methods.Where(x =>
            {
                if (!decorator.TargetMethodFilter.Static && x.IsStatic)
                    return false;

                if (!decorator.TargetMethodFilter.Instanced && !x.IsStatic)
                    return false;

                if (
                    decorator.TargetMethodFilter.Protected && !x.IsProtected &&
                    decorator.TargetMethodFilter.Public && !x.IsPublic &&
                    decorator.TargetMethodFilter.Private && !x.IsPrivate &&
                    decorator.TargetMethodFilter.Internal && !x.IsInternal)
                    return false;

                if (decorator.TargetMethodFilter.ReturnTypeNames.Length > 0 && !decorator.TargetMethodFilter.ReturnTypeNames.Any(y => y == x.Fullname))
                    return false;

                if (decorator.TargetMethodFilter.ParameterMatch)
                {
                    if (!decorator.TargetMethodFilter.ParameterStrict && decorator.TargetMethodFilter.Parameters.Length > 0 && !x.Parameters.IsAssignableFrom(methodParameters))
                        return false;
                    else if (decorator.TargetMethodFilter.ParameterStrict && decorator.TargetMethodFilter.Parameters.Length > 0 && !x.Parameters.SequenceEqual(methodParameters))
                        return false;
                }

                if (x.Fullname.IndexOf('<') >= 0 || x.Fullname.IndexOf('>') >= 0)
                    return false;

                if (!string.IsNullOrEmpty(decorator.TargetMethodFilter.Name) && !Regex.IsMatch(x.Name, decorator.TargetMethodFilter.Name == "*" ? "\\w*" : decorator.TargetMethodFilter.Name, RegexOptions.Singleline))
                    return false;

                return true;
            });
        }

        private static IEnumerable<Property> GetValidProperties(DecoratorDecription decorator, BuilderType @class)
        {
            return @class.Properties.Where(x =>
            {
                if (!decorator.TargetPropertyFilter.Static && x.IsStatic)
                    return false;

                if (!decorator.TargetPropertyFilter.Instanced && !x.IsStatic)
                    return false;

                if (
                    decorator.TargetPropertyFilter.Protected && !x.IsProtected &&
                    decorator.TargetPropertyFilter.Public && !x.IsPublic &&
                    decorator.TargetPropertyFilter.Private && !x.IsPrivate &&
                    decorator.TargetPropertyFilter.Internal && !x.IsInternal)
                    return false;

                if (decorator.TargetPropertyFilter.ReturnTypeNames.Length > 0 && !decorator.TargetPropertyFilter.ReturnTypeNames.Any(y => y == x.ReturnType.Fullname))
                    return false;

                if (x.Fullname.IndexOf('<') >= 0 || x.Fullname.IndexOf('>') >= 0)
                    return false;

                if (!string.IsNullOrEmpty(decorator.TargetPropertyFilter.Name) && !Regex.IsMatch(x.Name, decorator.TargetMethodFilter.Name == "*" ? "\\w*" : decorator.TargetPropertyFilter.Name, RegexOptions.Singleline))
                    return false;

                return true;
            });
        }

        private static void ImplementDecorator(Builder builder, AssemblyWideAttributeConfig config)
        {
            foreach (var decorator in config.Decorator)
            {
                if (string.IsNullOrEmpty(decorator.TypeName))
                    continue;

                if (!builder.TypeExists(decorator.TypeName))
                {
                    builder.Log(LogTypes.Error, $"Unable to find defined attribute: {decorator.TypeName}. Please add a reference to be able to continue.");
                    continue;
                }

                builder.Log(LogTypes.Info, $"Adding attributes: '{decorator.TypeName}'");

                var classes = builder.GetTypes(SearchContext.Module).Where(x =>
                {
                    if (x.Name.GetHashCode() == ModuleName.GetHashCode() && x.Name == ModuleName)
                        return false;

                    if (!decorator.TargetClassFilter.Static && x.IsStatic)
                        return false;

                    if (!decorator.TargetClassFilter.Instanced && !x.IsStatic)
                        return false;

                    if (decorator.TargetClassFilter.Public && !x.IsPublic && decorator.TargetClassFilter.Private && !x.IsPrivate && decorator.TargetClassFilter.Internal && !x.IsInternal)
                        return false;

                    if (!string.IsNullOrEmpty(decorator.TargetClassFilter.Name) && !Regex.IsMatch(x.Fullname, decorator.TargetClassFilter.Name == "*" ? "\\w*" : decorator.TargetClassFilter.Name, RegexOptions.Singleline))
                        return false;

                    if (x.Fullname.IndexOf('<') >= 0 || x.Fullname.IndexOf('>') >= 0)
                        return false;

                    if (decorator.TargetClassFilter.ReguiresInterfaces.Length > 0)
                        if (!x.Interfaces.Any(y => decorator.TargetClassFilter.ReguiresInterfaces.Contains(y.Fullname)))
                            return false;

                    if (x.Inherits("System.Attribute"))
                        return false;

                    return true;
                });

                ImplementDecorator(builder, decorator, classes);
            }
        }

        private static void ImplementDecorator(Builder builder, DecoratorDecription decorator, IEnumerable<BuilderType> classes)
        {
            var attribute = builder.GetType(decorator.TypeName);
            var attributeTarget = attribute.CustomAttributes.FirstOrDefault(x => x.Fullname == "System.AttributeUsageAttribute");
            var parameterValues = decorator.Parameters.Select<DecoratorDecriptionParameter, object>(x =>
            {
                var result = ConvertPropertyType(builder, x.TypeName, x.Value);
                if (result != null)
                    return result;

                if (x.TypeName.EndsWith("[]"))
                    throw new NotSupportedException($"Arrays are not supported by the attribute decorator: " + x.TypeName);

                var type = builder.GetType(x.TypeName);

                if (type.IsEnum)
                {
                    var underlyingType = type.EnumUnderlyingType;
                    return ConvertPropertyType(builder, underlyingType.Fullname, x.Value);
                }

                throw new NotSupportedException($"The attribute decorator does not support this type: " + x.TypeName);
            })
            .ToArray();
            var methodParameters = decorator.TargetMethodFilter.Parameters.Select(x => builder.GetType(x)).ToArray();

            AttributeTargets attributeTargets = AttributeTargets.All;
            if (attributeTarget != null)
                Enum.TryParse(attributeTarget.ConstructorArguments[0].Value?.ToString(), out attributeTargets);

            foreach (var @class in classes)
            {
                if (decorator.DecorateClass && attributeTargets.HasFlag(AttributeTargets.Class))
                {
                    builder.Log(LogTypes.Info, "- Decorating Class " + @class);
                    @class.CustomAttributes.Add(attribute, parameterValues);
                }

                if (decorator.DecorateMethod && (attributeTargets.HasFlag(AttributeTargets.Method) || attributeTargets.HasFlag(AttributeTargets.Constructor)))
                    foreach (var method in GetValidMethods(decorator, methodParameters, @class))
                    {
                        builder.Log(LogTypes.Info, "- Decorating Method " + method.Name);
                        method.CustomAttributes.Add(attribute, parameterValues);
                    }

                if (decorator.DecorateProperty && attributeTargets.HasFlag(AttributeTargets.Property))
                    foreach (var property in GetValidProperties(decorator, @class))
                    {
                        builder.Log(LogTypes.Info, "- Decorating Properties " + property.Name);
                        property.CustomAttributes.Add(attribute, parameterValues);
                    }
            }
        }
    }
}