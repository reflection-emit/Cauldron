using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody;
using System.Diagnostics;
using System.Linq;

public static class Weaver_Field
{
    public static string Name = "Field Interceptors";
    public static int Priority = 0;

    [Display("Field Interception")]
    public static void InterceptFields(Builder builder)
    {
        if (!Weaver_Property.PropertyInterceptingAttributes.Any())
            return;

        var fields = builder.FindFieldsByAttributes(Weaver_Property.PropertyInterceptingAttributes).GroupBy(x => x.Field).ToArray();

        foreach (var field in fields)
        {
            builder.Log(LogTypes.Info, $"Implementing field interceptors: {field.Key.DeclaringType.Name.PadRight(40, ' ')} {field.Key.Name}");

            if (!field.Key.Modifiers.HasFlag(Modifiers.Private))
            {
                builder.Log(LogTypes.Error, field.Key.OriginType, $"The current version of the field interceptor only intercepts private fields. Field '{field.Key.Name}' in type '{field.Key.OriginType.Name}'");
                continue;
            }

            var type = field.Key.OriginType;
            var usage = field.Key.FindUsages().ToArray();
            var property = type.CreateProperty(field.Key);

            property.CustomAttributes.AddCompilerGeneratedAttribute();
            property.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
            property.CustomAttributes.AddNonSerializedAttribute();

            foreach (var attribute in field)
                attribute.Attribute.MoveTo(property);

            foreach (var item in usage)
                if (item.Field.IsStatic || !item.IsBeforeBaseCall)
                    item.Replace(property);
        }
    }
}