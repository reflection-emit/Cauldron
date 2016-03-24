using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Couldron
{
    public abstract class CouldronApplication : Application
    {
        public CouldronApplication()
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            this.OnConstruction();

            // Add the custom template selector to the resources
            this.Resources.Add(typeof(CouldronTemplateSelector).Name, new CouldronTemplateSelector());

            // Add all Value converters to the dictionary
            foreach (var valueConverter in AssemblyUtil.DefinedTypes.Where(x => x.ImplementsInterface<IValueConverter>()))
                this.Resources.Add(valueConverter.Name, Activator.CreateInstance(valueConverter));

            // find all resourcedictionaries and add them to the existing resources
            foreach (var resourceDictionary in AssemblyUtil.DefinedTypes.Where(x => x.IsSubclassOf(typeof(ResourceDictionary))))
                this.Resources.MergedDictionaries.Add(Activator.CreateInstance(resourceDictionary) as ResourceDictionary);
        }

        protected virtual void OnConstruction()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }
}