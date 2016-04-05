using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Couldron
{
    /// <summary>
    /// Provides a base class for a Couldron toolkit application
    /// </summary>
    public abstract class CouldronApplication : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldronApplication"/>
        /// </summary>
        public CouldronApplication()
        {
            this.Resources.Add(nameof(ThemeAccentColor), Colors.SteelBlue);
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.OnConstruction();

            // Add the custom template selector to the resources
            this.Resources.Add(typeof(CouldronTemplateSelector).Name, new CouldronTemplateSelector());

            // Add all Value converters to the dictionary
            foreach (var valueConverter in AssemblyUtil.ExportedTypes.Where(x => !x.ContainsGenericParameters && x.ImplementsInterface<IValueConverter>()))
                this.Resources.Add(valueConverter.Name, Activator.CreateInstance(valueConverter));

            // find all resourcedictionaries and add them to the existing resources
            foreach (var resourceDictionary in AssemblyUtil.ExportedTypes
                    .Where(x => x.IsSubclassOf(typeof(ResourceDictionary)))
                    .OrderByDescending(x => x.Assembly.FullName.StartsWith("Couldron.")))
                this.Resources.MergedDictionaries.Add(Activator.CreateInstance(resourceDictionary) as ResourceDictionary);
        }

        /// <summary>
        /// Gets or sets the Couldron theme accent color
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
        /// Occures on initialization of <see cref="CouldronApplication"/>
        /// </summary>
        protected virtual void OnConstruction()
        {
        }
    }
}