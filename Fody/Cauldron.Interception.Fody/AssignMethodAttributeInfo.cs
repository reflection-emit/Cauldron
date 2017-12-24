using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class AssignMethodAttributeInfo
    {
        public Method AttributedMethod { get; private set; }
        public Field AttributeField { get; private set; }
        public BuilderType[] ParameterTypes { get; private set; }

        public Method TargetMethod => this.Type
                                        .GetMethods(this.TargetMethodName, this.ParameterTypes.Length, false)
                                        .FirstOrDefault(x =>
                                        {
                                            if (x.IsPrivate && x.DeclaringType.Fullname != this.Type.Fullname)
                                                return false;

                                            if (x.IsInternal && x.DeclaringType.Assembly.FullName != this.Type.Assembly.FullName)
                                                return false;

                                            if (!x.Parameters.Select(y => y.Fullname).SequenceEqual(this.ParameterTypes.Select(y => y.Fullname)))
                                                return false;

                                            var returnType = GetDelegateType(this.AttributeField.FieldType);
                                            return x.ReturnType.Fullname == returnType;
                                        });

        public bool TargetMethodIsVoid => this.TargetMethodReturnType == "System.Void";
        public string TargetMethodName { get; private set; }
        public string TargetMethodReturnType { get; private set; }
        public bool ThrowError { get; private set; }
        public BuilderType Type { get; private set; }

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedProperty attributedProperty) =>
            GetAllAssignMethodAttributedFields(attributedProperty.Property.Setter ?? attributedProperty.Property.Getter, attributedProperty.Attribute, attributedProperty.Property.OriginType, attributedProperty.Property.Name, GetDelegateType(attributedProperty.Property.ReturnType));

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedField attributedField) =>
            GetAllAssignMethodAttributedFields(null, attributedField.Attribute, attributedField.Field.OriginType, attributedField.Field.Name, GetDelegateType(attributedField.Field.FieldType));

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedMethod attributedMethod) =>
            GetAllAssignMethodAttributedFields(attributedMethod.Method, attributedMethod.Attribute, attributedMethod.Method.OriginType, attributedMethod.Method.Name, GetDelegateType(attributedMethod.Method.ReturnType));

        private static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(Method attributedMethod, BuilderCustomAttribute builderCustomAttribute, BuilderType targetType, string name, string returnTypeName)
        {
            var fields = builderCustomAttribute.Type
                .GetAttributedFields()
                .Where(x => x.Field.IsPublic && !x.Field.IsStatic && x.Attribute.Fullname == __AssignMethodAttribute.TypeName);

            return fields
                .Select(x => new AssignMethodAttributeInfo
                {
                    AttributeField = x.Field,
                    TargetMethodName = ReplacePlaceHolder(x.Attribute.ConstructorArguments[0].Value as string ?? "", name, returnTypeName),
                    TargetMethodReturnType = GetDelegateType(x.Field.FieldType),
                    ThrowError = !(bool)x.Attribute.ConstructorArguments[1].Value,
                    Type = targetType,
                    ParameterTypes = GetParameters(x.Field.FieldType),
                    AttributedMethod = attributedMethod
                }).ToArray();
        }

        private static string GetDelegateType(BuilderType type)
        {
            if (type != null && type.IsGenericInstance && type.Fullname.StartsWith("System.Func"))
                return type.GetGenericArgument(0);

            return "System.Void";
        }

        private static BuilderType[] GetParameters(BuilderType type)
        {
            if (type != null && type.IsGenericInstance && type.Fullname.StartsWith("System.Func"))
                return type.GenericArguments().Skip(1).ToArray();
            else if (type != null && type.IsGenericInstance)
                return type.GenericArguments().ToArray();

            return new BuilderType[0];
        }

        private static string ReplacePlaceHolder(string argument, string propertyOrMethodName, string returnTypeName)
        {
            if (string.IsNullOrEmpty(argument))
                return null;

            return argument
                .Replace("{Name}", propertyOrMethodName)
                .Replace("{ReturnType}", returnTypeName ?? "void");
        }
    }
}