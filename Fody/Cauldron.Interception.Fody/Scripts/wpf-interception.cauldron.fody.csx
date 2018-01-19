public const string Name = "WPF/XAML Interceptors";
public const int Priority = 50;

public static void Implement(Builder builder)
{
    var multiBindingValueConverterInterface = builder.TypeExists("System.Windows.Data.IMultiValueConverter") ? builder.GetType("System.Windows.Data.IMultiValueConverter") : null;
    var valueConverterInterface = builder.TypeExists("Windows.UI.Xaml.Data.IValueConverter") ? builder.GetType("Windows.UI.Xaml.Data.IValueConverter") : builder.GetType("System.Windows.Data.IValueConverter");
    var notifyPropertyChangedInterface = builder.GetType("System.ComponentModel.INotifyPropertyChanged");
    var componentAttribute = builder.GetType("Cauldron.Activator.ComponentAttribute");
    var componentConstructorAttribute = builder.GetType("Cauldron.Activator.ComponentConstructorAttribute");
    var windowType = builder.TypeExists("System.Windows.Window") ? builder.GetType("System.Windows.Window") : null;

    var views = builder.FindTypesByBaseClass("FrameworkElement").Where(x => x.IsPublic);
    var viewModels = builder.FindTypesByInterface(notifyPropertyChangedInterface).Where(x => x.IsPublic);
    var valueConverters = builder.FindTypesByInterface(valueConverterInterface).Where(x => x.IsPublic);
    var resourceDictionaryBaseClass = builder.TypeExists("Windows.UI.Xaml.ResourceDictionary") ? "Windows.UI.Xaml.ResourceDictionary" : "System.Windows.ResourceDictionary";
    var resourceDictionaries = builder.FindTypesByBaseClass(resourceDictionaryBaseClass).Where(x => x.IsPublic);
    var multiBindingConverters = multiBindingValueConverterInterface == null ? null : builder.FindTypesByInterface(multiBindingValueConverterInterface).Where(x => x.IsPublic);

    builder.Log(LogTypes.Info, "Adding ComponenAttribute to Views");
    foreach (var item in views)
    {
        if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
            continue;

        if (item.CustomAttributes.HasAttribute(componentAttribute))
            continue;

        // We have to make some exceptions here Everything that inherits from Window, should
        // have to contractname Window ... but only for desktop... because UWP does not have
        // custom windows
        if (windowType != null && item.IsSubclassOf(windowType))
            item.CustomAttributes.Add(componentAttribute, windowType.Fullname);
        else
            item.CustomAttributes.Add(componentAttribute, item.Fullname);

        builder.Log(LogTypes.Info, "- " + item.Name);

        // Add a component contructor attribute to all .ctors
        foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
            ctor.CustomAttributes.Add(componentConstructorAttribute);
    }

    builder.Log(LogTypes.Info, "Adding ComponenAttribute to ViewModels");
    foreach (var item in viewModels)
    {
        if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
            continue;

        if (item.CustomAttributes.HasAttribute(componentAttribute))
            continue;

        builder.Log(LogTypes.Info, "- " + item.Name);

        item.CustomAttributes.Add(componentAttribute, item.Fullname);
        // Add a component contructor attribute to all .ctors
        foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
            ctor.CustomAttributes.Add(componentConstructorAttribute);
    }

    builder.Log(LogTypes.Info, "Adding ComponenAttribute to ValueConverters");
    foreach (var item in valueConverters)
    {
        if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
            continue;

        if (item.CustomAttributes.HasAttribute(componentAttribute))
            continue;

        builder.Log(LogTypes.Info, "- " + item.Name);
        item.CustomAttributes.Add(componentAttribute, valueConverterInterface.Fullname);
        // Add a component contructor attribute to all .ctors
        foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
            ctor.CustomAttributes.Add(componentConstructorAttribute);
    }

    builder.Log(LogTypes.Info, "Adding ComponenAttribute to MultiBindingConverters");
    if (multiBindingConverters != null)
        foreach (var item in multiBindingConverters)
        {
            if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
                continue;

            if (item.CustomAttributes.HasAttribute(componentAttribute))
                continue;

            builder.Log(LogTypes.Info, "- " + item.Name);
            item.CustomAttributes.Add(componentAttribute, multiBindingValueConverterInterface.Fullname);
            // Add a component contructor attribute to all .ctors
            foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                ctor.CustomAttributes.Add(componentConstructorAttribute);
        }

    builder.Log(LogTypes.Info, "Adding ComponenAttribute to ResourceDictionaries");
    foreach (var item in resourceDictionaries)
    {
        if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
            continue;

        if (item.CustomAttributes.HasAttribute(componentAttribute))
            continue;

        builder.Log(LogTypes.Info, "- " + item.Name);
        item.CustomAttributes.Add(componentAttribute, resourceDictionaryBaseClass);
        // Add a component contructor attribute to all .ctors
        foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
            ctor.CustomAttributes.Add(componentConstructorAttribute);
    }
}