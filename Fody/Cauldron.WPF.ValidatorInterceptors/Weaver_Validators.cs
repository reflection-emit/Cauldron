using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using System.Linq;

public static class Weaver_Validators
{
    public const string Name = "WPF/XAML Validator Interceptors";
    public const int Priority = 50;

    [Display("WPF/XAML Validators implementation")]
    public static void AddValidatorInits(Builder builder)
    {
        var attributes = builder.FindAttributesByBaseClass("Cauldron.XAML.Validation.ValidatorAttributeBase");
        var propertiesWithAttributes = builder.FindPropertiesByAttributes(attributes)
            .Where(x => !x.Property.IsStatic)
            .GroupBy(x => x.Property)
            .Select(x => new
            {
                x.Key,
                Validators = x.ToArray()
            });

        foreach (var item in propertiesWithAttributes)
        {
            var addValidatorGroup = item.Key.OriginType.GetMethod("AddValidatorGroup", false, typeof(string));
            if (addValidatorGroup == null)
                continue;
            var addValidatorAttribute = item.Key.OriginType.GetMethod("AddValidator", false, typeof(string).FullName, "Cauldron.XAML.Validation.ValidatorAttributeBase");

            builder.Log(LogTypes.Info, $"Adding initializer for validators ({item.Validators.Length}) of property '{item.Key.Name}' in type '{item.Key.OriginType.Fullname}'");

            foreach (var ctors in item.Key.OriginType.GetRelevantConstructors())
            {
                ctors.NewCoder().Context(context =>
                {
                    context.Call(addValidatorGroup, item.Key.Name);

                    for (int i = 0; i < item.Validators.Length; i++)
                        context.Call(addValidatorAttribute, x => item.Key.Name, x => x.NewObj(item.Validators[i]));

                    return context;
                })
                .Insert(InsertionPosition.Beginning);
            }
        }
    }
}