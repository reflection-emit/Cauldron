using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class AssignMethodAttributeInfo
    {
        public Field AttributeField { get; private set; }

        public Method TargetMethod =>
                                    this.Type
                                        .GetMethods(this.TargetMethodName, 0, false)
                                        .FirstOrDefault(x =>
                                        {
                                            if (x.IsPrivate && x.DeclaringType.Fullname != this.Type.Fullname)
                                                return false;

                                            if (x.IsInternal && x.DeclaringType.Assembly.FullName != this.Type.Assembly.FullName)
                                                return false;

                                            var returnType = GetDelegateType(this.AttributeField.FieldType);
                                            return x.ReturnType.Fullname == returnType;
                                        });

        public bool TargetMethodIsVoid => this.TargetMethodReturnType == "System.Void";
        public string TargetMethodName { get; private set; }
        public string TargetMethodReturnType { get; private set; }
        public BuilderType Type { get; private set; }

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedProperty attributedProperty) =>
            GetAllAssignMethodAttributedFields(attributedProperty.Attribute, attributedProperty.Property.OriginType, attributedProperty.Property.Name, GetDelegateType(attributedProperty.Property.ReturnType));

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedField attributedField) =>
            GetAllAssignMethodAttributedFields(attributedField.Attribute, attributedField.Field.OriginType, attributedField.Field.Name, GetDelegateType(attributedField.Field.FieldType));

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedMethod attributedMethod) =>
            GetAllAssignMethodAttributedFields(attributedMethod.Attribute, attributedMethod.Method.OriginType, attributedMethod.Method.Name, GetDelegateType(attributedMethod.Method.ReturnType));

        private static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(BuilderCustomAttribute builderCustomAttribute, BuilderType targetType, string name, string returnTypeName)
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
                    Type = targetType
                }).ToArray();
        }

        private static string GetDelegateType(BuilderType type)
        {
            if (type != null && type.IsGenericInstance)
                return type.GetGenericArgument(0);

            return "System.Void";
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