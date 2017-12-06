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

        public static Crumb This => new Crumb { CrumbType = CrumbTypes.This };

        internal CrumbTypes CrumbType { get; set; }

        internal TypeReference ExceptionType { get; set; }

        internal int? Index { get; set; }

        internal string Name { get; set; }

        internal bool UnPackArray { get; set; }

        internal int UnPackArrayIndex { get; set; }

        public static Crumb GetParameter(int index) => new Crumb
        {
            CrumbType = CrumbTypes.Parameters,
            Index = index
        };

        public Crumb UnPacked(int arrayIndex = 0) => new Crumb
        {
            UnPackArray = true,
            CrumbType = this.CrumbType,
            ExceptionType = this.ExceptionType,
            Index = this.Index,
            Name = this.Name,
            UnPackArrayIndex = arrayIndex
        };
    }
}