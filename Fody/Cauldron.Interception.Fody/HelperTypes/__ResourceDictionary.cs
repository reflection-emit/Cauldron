using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    [HelperTypeName("Windows.UI.Xaml.ResourceDictionary", "System.Windows.ResourceDictionary")]
    public sealed class __ResourceDictionary : HelperTypeBase<__ResourceDictionary>
    {
        [HelperTypeMethod(".ctor")]
        public Method Ctor { get; private set; }

        [HelperTypeMethod("get_MergedDictionaries")]
        public Method MergedDictionaries { get; private set; }

        [HelperTypeMethod("set_Source", 1)]
        public Method SetSource { get; private set; }
    }
}