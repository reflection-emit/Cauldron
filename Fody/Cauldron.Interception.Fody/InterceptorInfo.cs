using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System;
using System.Linq;

namespace Cauldron.Interception.Fody
{
    public class InterceptorInfo
    {
        public InterceptorInfo(BuilderType attributedType)
        {
            var interceptorRule = __InterceptionRuleAttribute.Type;

            this.CustomAttributes = attributedType.CustomAttributes.ToArray();
            this.HasSuppressRule = this.CustomAttributes.Any(x =>
                    x.Fullname == interceptorRule.Fullname &&
                    x.ConstructorArguments.Length > 0 &&
                    (byte)x.ConstructorArguments[0].Value == 0);
            this.SuppressRuleAttributeTypes = this.CustomAttributes.Where(x =>
                    x.Fullname == interceptorRule.Fullname &&
                    x.ConstructorArguments.Length == 2 &&
                    x.GetConstructorArgumentType(1).Fullname == "System.Type")
                .Select(x => x.ConstructorArguments[1].Value as TypeReference)
                .ToArray();
        }

        public BuilderCustomAttribute[] CustomAttributes { get; private set; }
        public bool HasSuppressRule { get; private set; }
        public TypeReference[] SuppressRuleAttributeTypes { get; private set; }
    }
}