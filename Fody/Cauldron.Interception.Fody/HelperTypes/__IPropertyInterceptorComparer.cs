using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Cauldron.Interception.IPropertyInterceptorComparer")]
    public sealed class __IPropertyInterceptorComparer : HelperTypeBase<__IPropertyInterceptorComparer>
    {
        [HelperTypeMethod("get_AreEqual")]
        public Method GetAreEqual { get; private set; }

        [HelperTypeMethod("set_AreEqual", 1)]
        public Method SetAreEqual { get; private set; }
    }
}