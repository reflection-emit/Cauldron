using System.Reflection;

namespace Cauldron.Reflection
{
    internal class ActivatorKey
    {
        public ObjectActivator activator;
        public ParameterInfo[] parameterInfos;
    }
}