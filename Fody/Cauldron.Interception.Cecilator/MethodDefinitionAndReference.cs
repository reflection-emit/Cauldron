using Mono.Cecil;

namespace Cauldron.Interception.Cecilator
{
    public struct MethodDefinitionAndReference
    {
        public MethodDefinition definition;
        public MethodReference reference;
    }
}