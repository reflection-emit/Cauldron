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
            this.LogInfo($"Checking for xaml/baml resources without initializers.");

            var xamlList = builder.ResourceNames?.Where(x => x.EndsWith(".baml")).Select(x => x.Replace(".baml", ".xaml")).ToArray();

            if (xamlList == null || xamlList.Length == 0 || !builder.TypeExists("System.Windows.Application"))
                return;

            var application = new __Application(builder);
            var extensions = new __Extensions(builder);
            var resourceDictionary = new __ResourceDictionary(builder);
            var collection = new __ICollection_1(builder);

            this.LogInfo($"Implementing XAML initializer for baml resources.");

            // First we have to find every InitializeComponent method so that we can remove bamls that are already initialized.
            var allInitializeComponentMethods = builder.FindMethodsByName(SearchContext.Module, "InitializeComponent", 0).Where(x => !x.IsAbstract);
            var ldStrs = new ConcurrentBag<string>();

            Parallel.ForEach(allInitializeComponentMethods, methods =>
            {
                foreach (var str in methods.GetLoadStrings())
                    ldStrs.Add(str);
            });

            var xamlWithInitializers = ldStrs.Select(x => x.Substring(x.IndexOf("component/") + "component/".Length)).ToArray();
            var xamlThatRequiredInitializers = xamlList.Where(x => !xamlWithInitializers.Contains(x));

            var resourceDictionaryMergerClass = builder.CreateType("XamlGeneratedNamespace", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, "<>_generated_resourceDictionary_Loader");
            resourceDictionaryMergerClass.CustomAttributes.Add(builder.GetType("Cauldron.Activator.ComponentAttribute"), resourceDictionary.Type.Fullname);
            resourceDictionaryMergerClass.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
            resourceDictionaryMergerClass.CustomAttributes.AddCompilerGeneratedAttribute();

            //var method = resourceDictionaryMergerClass.CreateMethod(Modifiers.Private, "AddToDictionary", typeof(string));
            resourceDictionaryMergerClass.CreateConstructor().NewCode().Context(x =>
            {
                x.Load(x.This).Call(builder.GetType(typeof(object)).Import().ParameterlessContructor.Import());

                var resourceDick = x.CreateVariable(collection.Type.MakeGeneric(resourceDictionary.Type));
                x.Call(
                        x.NewCode().Call(x.NewCode().Call(application.Current) /* Instance */, application.Resources) /* Instance */, resourceDictionary.MergedDictionaries)
                                    .StoreLocal(resourceDick);

                var resourceDictionaryInstance = x.CreateVariable(resourceDictionary.Type);

                foreach (var item in xamlThatRequiredInitializers)
                {
                    x.NewObj(resourceDictionary.Ctor).StoreLocal(resourceDictionaryInstance);
                    x.Call(resourceDick, collection.Add.MakeGeneric(resourceDictionary.Type), resourceDictionaryInstance);
                    x.Call(resourceDictionaryInstance, resourceDictionary.SetSource,
                        x.NewCode().Call(extensions.RelativeUri, $"/{Path.GetFileNameWithoutExtension(this.Builder.Name)};component/{item}")); // TODO -Need modification for UWP)
                }
            })
            .Return()
            .Replace();

            resourceDictionaryMergerClass.ParameterlessContructor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
            resourceDictionaryMergerClass.ParameterlessContructor.CustomAttributes.Add(builder.GetType("Cauldron.Activator.ComponentConstructorAttribute"));
        }
    }
}