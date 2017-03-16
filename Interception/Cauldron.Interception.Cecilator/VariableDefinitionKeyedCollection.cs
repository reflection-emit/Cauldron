using Mono.Cecil.Cil;
using System.Collections.ObjectModel;

namespace Cauldron.Interception.Cecilator
{
    internal class VariableDefinitionKeyedCollection : KeyedCollection<string, VariableDefinition>
    {
        protected override string GetKeyForItem(VariableDefinition item) => item.Name;
    }
}