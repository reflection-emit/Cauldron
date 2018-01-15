using Newtonsoft.Json;

namespace Cauldron.Interception.Fody
{
    public class AssemblyWideAttributeConfig
    {
        [JsonProperty("debug-only")]
        public bool DebugOnly { get; private set; } = false;

        [JsonProperty("decorator")]
        public DecoratorDecription[] Decorator { get; private set; } = new DecoratorDecription[0];

        [JsonProperty("is-active")]
        public bool IsActive { get; private set; } = true;
    }

    public class DecoratorDecription
    {
        [JsonProperty("decorate-class")]
        public bool DecorateClass { get; private set; } = true;

        [JsonProperty("decorate-method")]
        public bool DecorateMethod { get; private set; } = true;

        [JsonProperty("decorate-property")]
        public bool DecorateProperty { get; private set; } = true;

        [JsonProperty("parameters")]
        public DecoratorDecriptionParameter[] Parameters { get; private set; } = new DecoratorDecriptionParameter[0];

        [JsonProperty("target-class-filter")]
        public TargetClassFilter TargetClassFilter { get; private set; } = new TargetClassFilter();

        [JsonProperty("target-method-filter")]
        public TargetMethodFilter TargetMethodFilter { get; private set; } = new TargetMethodFilter();

        [JsonProperty("target-property-filter")]
        public TargetPropertyFilter TargetPropertyFilter { get; private set; } = new TargetPropertyFilter();

        [JsonProperty("type-name")]
        public string TypeName { get; private set; } = null;
    }

    public class DecoratorDecriptionParameter
    {
        [JsonProperty("type-name")]
        public string TypeName { get; private set; }

        [JsonProperty("value")]
        public string Value { get; private set; }
    }

    public abstract class FilterBase
    {
        [JsonProperty("instanced")]
        public bool Instanced { get; private set; } = true;

        [JsonProperty("internal")]
        public bool Internal { get; private set; } = true;

        [JsonProperty("name")]
        public string Name { get; private set; } = "\\w*";

        [JsonProperty("private")]
        public bool Private { get; private set; } = true;

        [JsonProperty("protected")]
        public bool Protected { get; private set; } = true;

        [JsonProperty("public")]
        public bool Public { get; private set; } = true;

        [JsonProperty("static")]
        public bool Static { get; private set; } = true;
    }

    public class TargetClassFilter : FilterBase
    {
        [JsonProperty("requires-interfaces")]
        public string[] ReguiresInterfaces { get; private set; } = new string[0];
    }

    public class TargetMethodFilter : FilterBase
    {
        [JsonProperty("parameter-match")]
        public bool ParameterMatch { get; private set; } = false;

        [JsonProperty("parameters")]
        public string[] Parameters { get; private set; } = new string[0];

        [JsonProperty("parameter-strict")]
        public bool ParameterStrict { get; private set; } = false;

        [JsonProperty("return-types-name")]
        public string[] ReturnTypeNames { get; private set; } = new string[0];
    }

    public class TargetPropertyFilter : FilterBase
    {
        [JsonProperty("return-types-name")]
        public string[] ReturnTypeNames { get; private set; } = new string[0];
    }
}