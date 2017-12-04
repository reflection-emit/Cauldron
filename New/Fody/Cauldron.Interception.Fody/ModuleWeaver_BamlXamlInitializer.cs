using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System;
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
            if (builder.BamlResourceNames == null || !builder.BamlResourceNames.Any() || !builder.TypeExists("System.Windows.Application"))
                return;

            var application = new __Application(builder);
            var extensions = new __Extensions(builder);
            var resourceDictionary = new __ResourceDictionary(builder);

            this.LogInfo($"Implementing XAML initializer for baml resources.");

            // First we have to find every InitializeComponent method so that we can remove bamls that are already initialized.
            var allInitializeComponentMethods = builder.FindMethodsByName(SearchContext.Module, "InitializeComponent", 0).Where(x => !x.IsAbstract);
            var ldStrs = new ConcurrentBag<string>();

            Parallel.ForEach(allInitializeComponentMethods, methods =>
            {
                foreach (var str in methods.GetLoadStrings())
                    ldStrs.Add(str);
            });

            var xamlList = builder.BamlResourceNames.Select(x => x.Replace(".baml", ".xaml"));
            var xamlWithInitializers = ldStrs.Select(x => x.Substring(x.IndexOf("component/") + "component/".Length)).ToArray();
            var xamlThatRequiredInitializers = xamlList.Where(x => !xamlWithInitializers.Contains(x));

            foreach (var item in xamlThatRequiredInitializers)
            {
                builder.GetType("System.UriKind").Import();
                var type = builder.CreateType("XamlGeneratedNamespace", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, "generated_" + Path.GetFileNameWithoutExtension(item));
                type.CreateConstructor().NewCode().Context(x =>
                {
                    x.Load(x.This).Call(builder.GetType(typeof(object)).Import().ParameterlessContructor.Import());
                    x.Call(application.LoadComponent, x.This, x.NewCode().Call(extensions.RelativeUri, $"/{Path.GetFileNameWithoutExtension(this.Builder.Name)};component/{item}"));
                })
                .Return()
                .Replace();

                type.ParameterlessContructor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                type.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                type.CustomAttributes.AddCompilerGeneratedAttribute();

                type.CustomAttributes.Add(builder.GetType("Cauldron.Activator.ComponentAttribute"), resourceDictionary.Type.Name);
            }
        }
    }
}