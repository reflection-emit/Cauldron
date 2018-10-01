using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interceptors.TimedCacheAttribute")]
    public sealed class __TimedCacheAttribute : HelperTypeBase<__TimedCacheAttribute>
    {
        [HelperTypeMethod("CreateKey", 2)]
        public Method CreateKey { get; private set; }

        [HelperTypeMethod("GetCache", 1)]
        public Method GetCache { get; private set; }

        [HelperTypeMethod("HasCache", 1)]
        public Method HasCache { get; private set; }

        [HelperTypeMethod("SetCache", 2)]
        public Method SetCache { get; private set; }
    }
}