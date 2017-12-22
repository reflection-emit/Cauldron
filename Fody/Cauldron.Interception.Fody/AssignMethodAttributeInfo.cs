using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public sealed class AssignMethodAttributeInfo
    {
        public Field AttributeField { get; private set; }
        public bool TargetMethodIsVoid => this.TargetMethodReturnType == "System.Void";
        public string TargetMethodName { get; private set; }
        public string TargetMethodReturnType { get; private set; }

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedProperty attributedProperty) =>
            GetAllAssignMethodAttributedFields(attributedProperty.Attribute, attributedProperty.Property.Name, GetDelegateType(attributedProperty.Property.ReturnType));

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedField attributedField) =>
            GetAllAssignMethodAttributedFields(attributedField.Attribute, attributedField.Field.Name, GetDelegateType(attributedField.Field.FieldType));

        public static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(AttributedMethod attributedMethod) =>
            GetAllAssignMethodAttributedFields(attributedMethod.Attribute, attributedMethod.Method.Name, GetDelegateType(attributedMethod.Method.ReturnType));

        public Method GetToBeAssignedMethod() =>
                                    this.AttributeField
                                                .DeclaringType
                                                .GetMethods(this.TargetMethodName, 0, false)
                                                .FirstOrDefault(x =>
                                                {
                                                    var returnType = GetDelegateType(this.AttributeField.FieldType);
                                                    return x.ReturnType.Fullname == returnType;
                                                });

        private static AssignMethodAttributeInfo[] GetAllAssignMethodAttributedFields(BuilderCustomAttribute builderCustomAttribute, string name, string returnTypeName)
        {
            var fields = builderCustomAttribute.Type
                .GetAttributedFields()
                .Where(x => x.Field.IsPublic && x.Attribute.Fullname == __AssignMethodAttribute.TypeName);

            return fields
                .Select(x => new AssignMethodAttributeInfo
                {
                    AttributeField = x.Field,
                    TargetMethodName = ReplacePlaceHolder(x.Attribute.ConstructorArguments[0].Value as string ?? "", name, returnTypeName),
                    TargetMethodReturnType = GetDelegateType(x.Field.FieldType)
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