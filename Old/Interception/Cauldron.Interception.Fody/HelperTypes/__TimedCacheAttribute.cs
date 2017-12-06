using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __TimedCacheAttribute : HelperTypeBase
    {
        public const string TypeName = "Cauldron.Interception.TimedCacheAttribute";

        public __TimedCacheAttribute(Builder builder) : base(builder, TypeName)
        {
            this.CreateKey = this.type.GetMethod("CreateKey", 2);
            this.HasCache = this.type.GetMethod("HasCache", 1);
            this.SetCache = this.type.GetMethod("SetCache", 2);
            this.GetCache = this.type.GetMethod("GetCache", 1);
        }

        public Method CreateKey { get; private set; }
        public Method GetCache { get; private set; }
        public Method HasCache { get; private set; }
        public Method SetCache { get; private set; }
    }
}