using Cauldron.Interception.Cecilator;

namespace Cauldron.Interception.Fody.HelperTypes
{
    public sealed class __ResourceDictionary : HelperTypeBase
    {
        public __ResourceDictionary(Builder builder) : base(builder, "Windows.UI.Xaml.ResourceDictionary", "System.Windows.ResourceDictionary")
        {
            this.MergedDictionaries = this.type.GetMethod("get_MergedDictionaries", 0);
            this.Ctor = this.type.GetMethod(".ctor", 0);
            this.SetSource = this.type.GetMethod("set_Source", 1);
        }

        public Method Ctor { get; private set; }
        public Method MergedDictionaries { get; private set; }
        public Method SetSource { get; private set; }
    }
}