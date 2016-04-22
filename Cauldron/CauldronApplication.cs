using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Cauldron
{
    /// <summary>
    /// Provides a base class for a Cauldron toolkit application
    /// </summary>
    public abstract class CauldronApplication : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CauldronApplication"/>
        /// </summary>
        public CauldronApplication()
        {
            this.Resources.Add(nameof(ThemeAccentColor), Colors.SteelBlue);
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.OnConstruction();

            // Add the custom template selector to the resources
            this.Resources.Add(typeof(CauldronTemplateSelector).Name, new CauldronTemplateSelector());

            // Add all Value converters to the dictionary
            foreach (var valueConverter in AssemblyUtil.ExportedTypes.Where(x => !x.ContainsGenericParameters && x.ImplementsInterface<IValueConverter>()))
                this.Resources.Add(valueConverter.Name, Activator.CreateInstance(valueConverter));

            // find all resourcedictionaries and add them to the existing resources
            foreach (var resourceDictionary in AssemblyUtil.ExportedTypes
                    .Where(x => x.IsSubclassOf(typeof(ResourceDictionary)))
                    .OrderByDescending(x => x.Assembly.FullName.StartsWith("Cauldron.")))
                this.Resources.MergedDictionaries.Add(Activator.CreateInstance(resourceDictionary) as ResourceDictionary);
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