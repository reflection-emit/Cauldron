using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Cecilator.Coders;
using Cauldron.Interception.Fody;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

public static class Weaver_WPF
{
    public const string Name = "WPF/XAML Interceptors";
    public const int Priority = 50;

    [Display("WPF/XAML Interceptor")]
    public static void Implement(Builder builder)
    {
        var multiBindingValueConverterInterface = builder.TypeExists("System.Windows.Data.IMultiValueConverter") ? builder.GetType("System.Windows.Data.IMultiValueConverter").Import() : null;
        var valueConverterInterface = builder.TypeExists("Windows.UI.Xaml.Data.IValueConverter") ? builder.GetType("Windows.UI.Xaml.Data.IValueConverter").Import() : builder.GetType("System.Windows.Data.IValueConverter").Import();
        var notifyPropertyChangedInterface = builder.GetType("System.ComponentModel.INotifyPropertyChanged").Import();
        var componentAttribute = builder.GetType("Cauldron.Activator.ComponentAttribute").Import();
        var componentConstructorAttribute = builder.GetType("Cauldron.Activator.ComponentConstructorAttribute").Import();
        var windowType = builder.TypeExists("System.Windows.Window") ? builder.GetType("System.Windows.Window") : null;

        var views = builder.FindTypesByBaseClass("FrameworkElement").Where(x => x.IsPublic);
        var viewModels = builder.FindTypesByInterface(notifyPropertyChangedInterface).Where(x => x.IsPublic);
        var valueConverters = builder.FindTypesByInterface(valueConverterInterface).Where(x => x.IsPublic);
        var resourceDictionaryBaseClass = builder.TypeExists("Windows.UI.Xaml.ResourceDictionary") ? "Windows.UI.Xaml.ResourceDictionary" : "System.Windows.ResourceDictionary";
        var resourceDictionaries = builder.FindTypesByBaseClass(resourceDictionaryBaseClass).Where(x => x.IsPublic);
        var multiBindingConverters = multiBindingValueConverterInterface == null ? null : builder.FindTypesByInterface(multiBindingValueConverterInterface).Where(x => x.IsPublic);

        builder.Log(LogTypes.Info, "Adding ComponentAttribute to Views");
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

        builder.Log(LogTypes.Info, "Adding ComponentAttribute to ViewModels");
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

        builder.Log(LogTypes.Info, "Adding ComponentAttribute to ValueConverters");
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

        builder.Log(LogTypes.Info, "Adding ComponentAttribute to MultiBindingConverters");
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

        builder.Log(LogTypes.Info, "Adding ComponentAttribute to ResourceDictionaries");
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

    [Display("XAML initializer for baml resources")]
    public static void ImplementBamlInit(Builder builder)
    {
        if (builder.ResourceNames == null)
            return;

        var xamlList = builder.ResourceNames.Where(x => x.EndsWith(".baml")).Select(x => x.Replace(".baml", ".xaml")).ToArray();

        builder.Log(LogTypes.Info, "Checking for xaml/baml resources without initializers.");

        var resourceDictionary = new __ResourceDictionary();
        builder.Log(LogTypes.Info, "Implementing XAML initializer for baml resources.");

        // First we have to find every InitializeComponent method so that we can remove bamls that are already initialized.
        var allInitializeComponentMethods = builder.FindMethodsByName(SearchContext.Module, "InitializeComponent", 0).Where(x => !x.IsAbstract);
        var ldStrs = new System.Collections.Concurrent.ConcurrentBag<string>();

        System.Threading.Tasks.Parallel.ForEach(allInitializeComponentMethods, methods =>
        {
            foreach (var str in methods.GetLoadStrings())
                ldStrs.Add(str);
        });

        var xamlWithInitializers = ldStrs.Select(x => x.Substring(x.IndexOf("component/") + "component/".Length)).ToArray();
        var xamlThatRequiredInitializers = xamlList
            .Where(x => !xamlWithInitializers.Contains(x))
            .Select(x =>
            {
                var index = uint.MaxValue;
                if (x.IndexOf('-') > 0)
                {
                    var dashPosition = x.LastIndexOf('-') + 1;
                    var pointPosition = x.IndexOf('.', dashPosition);

                    uint result;
                    if (uint.TryParse(x.Substring(dashPosition, pointPosition > dashPosition ? pointPosition - dashPosition : x.Length - dashPosition), out result))
                        index = result;
                }

                return new { Index = index, Item = x };
            })
            .OrderBy(x => x.Index)
            .ThenBy(x => x.Item)
            .ToArray();

        var resourceDictionaryMergerClass = builder.CreateType("XamlGeneratedNamespace", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, "<>_generated_resourceDictionary_Loader");
        resourceDictionaryMergerClass.CustomAttributes.Add(BuilderTypes2.ComponentAttribute.BuilderType, __ResourceDictionary.Type.Fullname);
        resourceDictionaryMergerClass.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
        resourceDictionaryMergerClass.CustomAttributes.AddCompilerGeneratedAttribute();

        //var method = resourceDictionaryMergerClass.CreateMethod(Modifiers.Private, "AddToDictionary", typeof(string));
        resourceDictionaryMergerClass.CreateConstructor().NewCoder().Context(context =>
        {
            var resourceDick = context.AssociatedMethod.GetOrCreateVariable(BuilderTypes.ICollection1.BuilderType.MakeGeneric(__ResourceDictionary.Type));
            context
                .SetValue(resourceDick, x =>
                    x.Call(BuilderTypes2.Application.GetMethod_get_Current())
                    .Call(BuilderTypes2.Application.GetMethod_get_Resources())
                    .Call(resourceDictionary.MergedDictionaries));

            var resourceDictionaryInstance = context.AssociatedMethod.GetOrCreateVariable(__ResourceDictionary.Type);

            foreach (var item in xamlThatRequiredInitializers)
            {
                builder.Log(LogTypes.Info, $"- Adding XAML '{item.Item}' with index '{item.Index}' to the Application's MergeDictionary");
                context.SetValue(resourceDictionaryInstance, x => x.NewObj(resourceDictionary.Ctor));
                context.Load(resourceDictionaryInstance)
                    .Call(resourceDictionary.SetSource,
                        x => x.NewObj(BuilderTypes.Uri.GetMethod_ctor(), $"pack://application:,,,/{Path.GetFileNameWithoutExtension(builder.Name)};component/{item.Item}"));
                context.Load(resourceDick)
                    .Call(BuilderTypes.ICollection1.GetMethod_Add().MakeGeneric(__ResourceDictionary.Type), resourceDictionaryInstance);
            }

            return context;
        })
        .Return()
        .Replace();

        // Let us look for ResourceDictionaries without a InitializeComponent in their ctor
        // TODO

        resourceDictionaryMergerClass.ParameterlessContructor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
        resourceDictionaryMergerClass.ParameterlessContructor.CustomAttributes.Add(BuilderTypes2.ComponentConstructorAttribute.BuilderType);
    }

    [Display("IChangeAwareViewModel implementer")]
    public static void ImplementPropertyChangedEvent(Builder builder)
    {
        var changeAwareInterface = new __IChangeAwareViewModel();
        var viewModelInterface = new __IViewModel();

        // Get all viewmodels with implemented change aware interface
        var viewModels = builder.FindTypesByInterface(__IChangeAwareViewModel.Type)
            .OrderBy(x =>
            {
                if (x.Implements(__IChangeAwareViewModel.Type, false))
                    return 0;

                return 1;
            });

        foreach (var vm in viewModels)
        {
            if (vm.IsInterface)
                continue;

            var method = vm.GetMethod("<>RaisePropertyChangedEventRaise", false, typeof(string), typeof(object), typeof(object));
            var getIsChangeChangedEvent = __IChangeAwareViewModel.GetIsChangedChanged(vm);
            var getIsChangeEvent = __IChangeAwareViewModel.GetChanged(vm);

            if (method == null && getIsChangeChangedEvent != null && getIsChangeEvent != null)
            {
                method = vm.CreateMethod(Modifiers.Protected, "<>RaisePropertyChangedEventRaise", typeof(string), typeof(object), typeof(object));
                method.NewCoder()
                    .If(x => x.Load(CodeBlocks.GetParameter(0)).Is("IsChanged"), then =>

                              then.If(z => z.Load(getIsChangeChangedEvent).IsNotNull(), thenInner =>
                                  thenInner.Load(getIsChangeChangedEvent).Call(BuilderTypes.EventHandler.GetMethod_Invoke(), CodeBlocks.This, thenInner.NewCoder().NewObj(BuilderTypes.EventArgs.GetMethod_ctor())))
                                    .Call(viewModelInterface.RaisePropertyChanged, CodeBlocks.GetParameter(0))
                                      .Return()
                         )
                    .If(x => x.Load(getIsChangeEvent).IsNotNull(), then =>

                             then.Call(viewModelInterface.RaisePropertyChanged, CodeBlocks.GetParameter(0))
                                .End
                                .Load(getIsChangeEvent).Call(BuilderTypes.EventHandler1.GetMethod_Invoke().MakeGeneric(changeAwareInterface.PropertyIsChangedEventArgs.ToBuilderType),
                                     x => CodeBlocks.This,
                                     x => x.NewObj(changeAwareInterface.PropertyIsChangedEventArgs.Ctor, CodeBlocks.GetParameter(0), CodeBlocks.GetParameter(1), CodeBlocks.GetParameter(2)))

                        )
                        .Return()
                        .Replace();
                method.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
            }

            if (method == null)
                continue;

            builder.Log(LogTypes.Info, $"Implementing RaisePropertyChanged Raise Event in '{vm.Fullname}'");
            var raisePropertyChanged = vm.GetMethod("RaisePropertyChanged", false, typeof(string), typeof(object), typeof(object));

            if (raisePropertyChanged == null)
                continue;

            if (!raisePropertyChanged.IsAbstract && !raisePropertyChanged.HasMethodBaseCall())
                raisePropertyChanged
                    .NewCoder()
                    .Call(method, CodeBlocks.GetParameter(0), CodeBlocks.GetParameter(1), CodeBlocks.GetParameter(2))
                    .End
                    .Insert(InsertionPosition.Beginning);

            // Repair IsChanged
            if (!vm.Implements(changeAwareInterface.ToBuilderType, false))
                continue;

            var isChangedSetter = vm.GetMethod("set_IsChanged", 1, false);
            if (isChangedSetter != null)
            {
                isChangedSetter.NewCoder()
                    .If(x => x.Call(viewModelInterface.IsLoading).Is(true),
                        then => then.Return())
                    .Insert(InsertionPosition.Beginning);
            }
        }
    }
}