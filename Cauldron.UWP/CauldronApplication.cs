using System;
using System.Linq;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Cauldron
{
    public abstract class CauldronApplication : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CauldronApplication"/>
        /// </summary>
        public CauldronApplication()
        {
            this.Resources.Add(nameof(ThemeAccentColor), Colors.SteelBlue);
            this.OnConstruction();

            // Add the custom template selector to the resources
            this.Resources.Add(typeof(CauldronTemplateSelector).Name, new CauldronTemplateSelector());

            // Add all Value converters to the dictionary
            foreach (var valueConverter in AssemblyUtil.ExportedTypes.Where(x => x.ImplementsInterface<IValueConverter>()))
                this.Resources.Add(valueConverter.Name, Activator.CreateInstance(valueConverter.AsType()));

            // find all resourcedictionaries and add them to the existing resources
            foreach (var resourceDictionary in AssemblyUtil.ExportedTypes
                    .Where(x => x.IsSubclassOf(typeof(ResourceDictionary)))
                    .OrderByDescending(x => x.Assembly.FullName.StartsWith("Cauldron.")))
                this.Resources.MergedDictionaries.Add(Activator.CreateInstance(resourceDictionary.AsType()) as ResourceDictionary);
        }

        /// <summary>
        /// Gets or sets the Cauldron theme accent color
        /// <para/>
        /// There is no garantee that the used theme supports the accent color
        /// </summary>
        public Color ThemeAccentColor
        {
            get
            {
                return (Color)this.Resources[nameof(ThemeAccentColor)];
            }
            set
            {
                this.Resources[nameof(ThemeAccentColor)] = value;
            }
        }

        /// <summary>
        /// Occures on initialization of <see cref="CauldronApplication"/>
        /// </summary>
        protected virtual void OnConstruction()
        {
        }
    }
}