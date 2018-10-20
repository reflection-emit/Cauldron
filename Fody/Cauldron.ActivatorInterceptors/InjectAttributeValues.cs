using Cauldron.Interception.Cecilator;
using Mono.Cecil;

internal class InjectAttributeValues
{
    public InjectAttributeValues(BuilderCustomAttribute builderCustomAttribute)
    {
        if (builderCustomAttribute.ConstructorArguments != null && builderCustomAttribute.ConstructorArguments.Length > 0)
            this.Arguments = builderCustomAttribute.ConstructorArguments[0].Value as CustomAttributeArgument[];

        if (builderCustomAttribute.Properties.ContainsKey("ContractType")) this.ContractType = (builderCustomAttribute.Properties["ContractType"].Value as TypeReference)?.ToBuilderType();
        if (builderCustomAttribute.Properties.ContainsKey("ContractName")) this.ContractName = builderCustomAttribute.Properties["ContractName"].Value as string;
        if (builderCustomAttribute.Properties.ContainsKey("InjectFirst")) this.InjectFirst = (bool)builderCustomAttribute.Properties["InjectFirst"].Value;
        if (builderCustomAttribute.Properties.ContainsKey("IsOrdered")) this.IsOrdered = (bool)builderCustomAttribute.Properties["IsOrdered"].Value;
        if (builderCustomAttribute.Properties.ContainsKey("MakeThreadSafe")) this.MakeThreadSafe = (bool)builderCustomAttribute.Properties["MakeThreadSafe"].Value;
        if (builderCustomAttribute.Properties.ContainsKey("ForceDontCreateMany")) this.ForceDontCreateMany = (bool)builderCustomAttribute.Properties["ForceDontCreateMany"].Value;
        if (builderCustomAttribute.Properties.ContainsKey("NoPreloading")) this.NoPreloading = (bool)builderCustomAttribute.Properties["NoPreloading"].Value;
    }

    public CustomAttributeArgument[] Arguments { get; }
    public string ContractName { get; }
    public BuilderType ContractType { get; }
    public bool ForceDontCreateMany { get; }
    public bool InjectFirst { get; }
    public bool IsOrdered { get; }
    public bool MakeThreadSafe { get; }
    public bool NoPreloading { get; }
}