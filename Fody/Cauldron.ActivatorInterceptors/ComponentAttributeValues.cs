using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;

internal class ComponentAttributeValues
{
    public ComponentAttributeValues(AttributedType attributedType)
    {
        if (attributedType.Attribute.Properties.ContainsKey("InvokeOnObjectCreationEvent")) this.InvokeOnObjectCreationEvent = (bool)attributedType.Attribute.Properties["InvokeOnObjectCreationEvent"].Value;
        foreach (var item in attributedType.Attribute.ConstructorArguments)
        {
            switch (item.Type.FullName)
            {
                case "System.String":
                    this.ContractName = item.Value as string;
                    break;

                case "System.Type":
                    this.ContractType = (item.Value as TypeReference)?.ToBuilderType() ?? item.Value as BuilderType ?? Builder.Current.Import(item.Value as Type)?.ToBuilderType();
                    break;

                case "System.UInt32":
                    this.Priority = (uint)item.Value;
                    break;

                case "Cauldron.Activator.FactoryCreationPolicy":
                    this.Policy = (int)item.Value;
                    break;
            }
        }
    }

    public string ContractName { get; }
    public BuilderType ContractType { get; }
    public bool InvokeOnObjectCreationEvent { get; }
    public int Policy { get; }
    public uint Priority { get; }
}