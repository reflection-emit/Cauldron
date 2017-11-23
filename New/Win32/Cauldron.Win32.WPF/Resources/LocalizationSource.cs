using Cauldron.Activator;
using Cauldron.Localization;
using System.ComponentModel;

namespace Cauldron.XAML.Resources
{
    /// <summary>
    /// An implementation of <see cref="ILocalizationSource"/> for Cauldron.XAML
    /// </summary>
    [Component(typeof(ILocalizationSource), FactoryCreationPolicy.Singleton)]
    public sealed class LocalizationSource : YamlLocalizationSourceBase<LocalizationKeyValue>
    {
        /// <exclude/>
        [ComponentConstructor]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LocalizationSource() : base("7B5BE4E7E11D87C4E23B68589848BB2A.yaml")
        {
        }
    }
}