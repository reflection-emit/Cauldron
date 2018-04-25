using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System;
using System.Linq;

public class InterceptorInfo
{
    public InterceptorInfo(BuilderType attributedType)
    {
        var interceptorRule = __InterceptionRuleAttribute.Type;

        this.CustomAttributes = attributedType.CustomAttributes.ToArray();
        var interceptorRules = this.CustomAttributes.Where(x => x.Fullname == interceptorRule.Fullname);

        this.HasSuppressRule = interceptorRules.Any(x =>
                x.ConstructorArguments.Length > 0 &&
                (byte)x.ConstructorArguments[0].Value == 0);
        this.SuppressRuleAttributeTypes = interceptorRules.Where(x =>
                x.ConstructorArguments.Length >= 2 &&
                (byte)x.ConstructorArguments[0].Value == 0 &&
                x.GetConstructorArgumentType(1).Fullname == "System.Type")
            .Select(x => x.ConstructorArguments[1].Value as TypeReference)
            .ToArray();

        this.HasInterfaceOrBaseClassRequirement = interceptorRules.Any(x =>
                x.ConstructorArguments.Length > 0 &&
                (byte)x.ConstructorArguments[0].Value == 1);
        this.InterfaceOrBaseClassRequirement = interceptorRules.Where(x =>
                x.ConstructorArguments.Length >= 2 &&
                (byte)x.ConstructorArguments[0].Value == 1 &&
                x.GetConstructorArgumentType(1).Fullname == "System.Type")
            .Select(x =>
            {
                if (x.ConstructorArguments.Length == 2)
                    return new Tuple<TypeReference, byte>(x.ConstructorArguments[1].Value as TypeReference, (byte)0);

                if (x.ConstructorArguments.Length > 2)
                    return new Tuple<TypeReference, byte>(x.ConstructorArguments[1].Value as TypeReference, (byte)x.ConstructorArguments[2].Value);

                throw new Exception(" OMG ");
            })
            .ToArray();
    }

    public BuilderCustomAttribute[] CustomAttributes { get; private set; }

    public bool HasInterfaceOrBaseClassRequirement { get; private set; }

    public bool HasSuppressRule { get; private set; }

    public Tuple<TypeReference, byte>[] InterfaceOrBaseClassRequirement { get; private set; }

    public TypeReference[] SuppressRuleAttributeTypes { get; private set; }

    public static bool GetIsSupressed(InterceptorInfo interceptorInfo,
        BuilderType declaringType,
        BuilderCustomAttributeCollection appliedCustomAttributes,
        BuilderCustomAttribute thisAttribute,
        string methodOrPropertyName,
        bool isMethod)
    {
        if (interceptorInfo.HasSuppressRule)
        {
            var attributes = appliedCustomAttributes.Select(x => x.Fullname);
            var result = interceptorInfo.SuppressRuleAttributeTypes.Any(x => attributes.Contains(x.FullName));

            if (result)
            {
                declaringType.Log(LogTypes.Info, $"Suppressed - The { (isMethod ? "method" : "property") } '{methodOrPropertyName}' is not intercepted by '{thisAttribute.Type.Name}' because of a rule.");
                thisAttribute.Remove();
                return true;
            }
        }

        if (interceptorInfo.HasInterfaceOrBaseClassRequirement)
        {
            var interfacesAndBaseClasses = declaringType.Interfaces.Concat(declaringType.BaseClasses).Select(x => x.Fullname);
            var hasRequired = interceptorInfo.InterfaceOrBaseClassRequirement.Any(x => x.Item2 == 1);
            var requiredInterfacesAndBaseClasses = interceptorInfo.InterfaceOrBaseClassRequirement.Any(x => x.Item2 == 1 && interfacesAndBaseClasses.Any(y => y.StartsWith(x.Item1.FullName)));
            var optionalInterfacesAnsBaseClasses = interceptorInfo.InterfaceOrBaseClassRequirement.Any(x => x.Item2 == 0 && interfacesAndBaseClasses.Any(y => y.StartsWith(x.Item1.FullName)));

            if (hasRequired && !requiredInterfacesAndBaseClasses)
            {
                thisAttribute.Remove();
                return true;
            }

            if (!optionalInterfacesAnsBaseClasses)
            {
                thisAttribute.Remove();
                return true;
            }
        }

        return false;
    }
}