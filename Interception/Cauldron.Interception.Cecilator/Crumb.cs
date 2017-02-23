using Mono.Cecil;

namespace Cauldron.Interception.Cecilator
{
    internal enum CrumbTypes
    {
        Exception,
        This,
        Parameters
    }

    public class Crumb
    {
        internal Crumb()
        {
        }

        internal CrumbTypes CrumbType { get; set; }
        internal TypeReference ExceptionType { get; set; }
        internal string Name { get; set; }
    }
}