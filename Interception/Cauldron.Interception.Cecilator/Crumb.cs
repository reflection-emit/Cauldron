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

        internal int? Index { get; set; }

        internal string Name { get; set; }

        public Crumb this[int index]
        {
            get
            {
                return new Crumb
                {
                    CrumbType = CrumbTypes.Parameters,
                    Index = index
                };
            }
        }
    }
}