using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver
    {
        public void ImplementBamlInitializer(Builder builder)
        {
            this.Log($"Checking for xaml/baml resources without initializers.");

            var xamlList = builder.ResourceNames?.Where(x => x.EndsWith(".baml")).Select(x => x.Replace(".baml", ".xaml")).ToArray();

            if (!this.IsActivatorReferenced || !this.IsXAML || xamlList == null || xamlList.Length == 0 || !builder.TypeExists("System.Windows.Application"))
                return;

            var application = new __Application(builder);
            var extensions = new __Extensions(builder);
            var resourceDictionary = new __ResourceDictionary(builder);
            var collection = new __ICollection_1(builder);
            var uri = new __Uri(builder);

            this.Log($"Implementing XAML initializer for baml resources.");

            // First we have to find every InitializeComponent method so that we can remove bamls that are already initialized.
            var allInitializeComponentMethods = builder.FindMethodsByName(SearchContext.Module, "InitializeComponent", 0).Where(x => !x.IsAbstract);
            var ldStrs = new ConcurrentBag<string>();

            Parallel.ForEach(allInitializeComponentMethods, methods =>
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

                        if (uint.TryParse(x.Substring(dashPosition, pointPosition > dashPosition ? pointPosition - dashPosition : x.Length - dashPosition), out uint result))
                            index = result;
                    }

                    return new { Index = index, Item = x };
                })
                .OrderBy(x => x.Index)
                .ThenBy(x => x.Item)
                .ToArray();

            var resourceDictionaryMergerClass = builder.CreateType("XamlGeneratedNamespace", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, "<>_generated_resourceDictionary_Loader");
            resourceDictionaryMergerClass.CustomAttributes.Add(builder.GetType("Cauldron.Activator.ComponentAttribute"), resourceDictionary.Type.Fullname);
            resourceDictionaryMergerClass.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
            resourceDictionaryMergerClass.CustomAttributes.AddCompilerGeneratedAttribute();

            //var method = resourceDictionaryMergerClass.CreateMethod(Modifiers.Private, "AddToDictionary", typeof(string));
            resourceDictionaryMergerClass.CreateConstructor().NewCode().Context(x =>
            {
                x.Load(Crumb.This).Call(builder.GetType(typeof(object)).Import().ParameterlessContructor.Import());

                var resourceDick = x.CreateVariable(collection.Type.MakeGeneric(resourceDictionary.Type));
                x.Call(
                        x.NewCode().Call(x.NewCode().Call(application.Current) /* Instance */, application.Resources) /* Instance */, resourceDictionary.MergedDictionaries)
                                    .StoreLocal(resourceDick);

                var resourceDictionaryInstance = x.CreateVariable(resourceDictionary.Type);

                foreach (var item in xamlThatRequiredInitializers)
                {
                    this.Log($"- Adding XAML '{item.Item}' with index '{item.Index}' to the Application's MergeDictionary");
                    x.NewObj(resourceDictionary.Ctor).StoreLocal(resourceDictionaryInstance);
                    x.Call(resourceDictionaryInstance, resourceDictionary.SetSource,
                         x.NewCode().NewObj(uri.Ctor, $"pack://application:,,,/{Path.GetFileNameWithoutExtension(this.Builder.Name)};component/{item.Item}")); // TODO -Need modification for UWP)
                    x.Call(resourceDick, collection.Add.MakeGeneric(resourceDictionary.Type), resourceDictionaryInstance);
                }
            })
            .Return()
            .Replace();

            // Let us look for ResourceDictionaries without a InitializeComponent in their ctor
            // TODO

            resourceDictionaryMergerClass.ParameterlessContructor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
            resourceDictionaryMergerClass.ParameterlessContructor.CustomAttributes.Add(builder.GetType("Cauldron.Activator.ComponentConstructorAttribute"));
        }
    }
}