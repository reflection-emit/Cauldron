using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver : WeaverBase
    {
        protected override void OnExecute()
        {
            var versionAttribute = typeof(ModuleWeaver)
                .Assembly
                .GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), true)
                .FirstOrDefault() as System.Reflection.AssemblyFileVersionAttribute;

            this.Log($"Cauldron Interception v" + versionAttribute.Version);

            if (this.Builder.TypeExists("CauldronInterceptionHelper"))
                return;

            this.CreateCauldronEntry(this.Builder);
            this.AddAssemblyWideAttributes(this.Builder);
            this.ExecuteInterceptionScripts(this.Builder);
            this.AddEntranceAssemblyHACK(this.Builder);
            this.ExecuteModuleAddition(this.Builder);
        }

        private void AddEntranceAssemblyHACK(Builder builder)
        {
            var assembly = builder.GetType("System.Reflection.Assembly").Import().With(x => new { Type = x, Load = x.GetMethod("Load", 1).Import() });
            var cauldron = builder.GetType("CauldronInterceptionHelper", SearchContext.Module);
            var referencedAssembliesMethod = cauldron.CreateMethod(Modifiers.PublicStatic, builder.MakeArray(assembly.Type), "GetReferencedAssemblies");
            var voidMain = builder.GetMain();

            // Add the Entrance Assembly and referenced assemblies hack for UWP
            if (builder.IsUWP && voidMain != null)
            {
                if (builder.Name != "Cauldron.dll" && builder.TypeExists("Cauldron.Core.Reflection.AssembliesCore"))
                {
                    voidMain.NewCoder().Context(context =>
                    {
                        var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").Import().With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1).Import() });
                        var typeInfo = builder.GetType("System.Reflection.TypeInfo").Import().With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly").Import() });
                        var assemblies = builder.GetType("Cauldron.Core.Reflection.AssembliesCore").Import().With(y => new
                        {
                            Type = y,
                            SetEntryAssembly = y.GetMethod("SetEntryAssembly", 1).Import(),
                            SetReferenceAssemblies = y.GetMethod("SetReferenceAssemblies", 1).Import()
                        });

                        return context.Call(assemblies.SetEntryAssembly, x =>
                            x.Call(introspectionExtensions.GetTypeInfo, voidMain.DeclaringType)
                                .Call(typeInfo.Assembly))
                            .End
                            .Call(assemblies.SetReferenceAssemblies, x => x.Call(referencedAssembliesMethod))
                            .End;
                    })
                    .Insert(InsertionPosition.Beginning);
                }
            }

            IEnumerable<AssemblyDefinition> referencedAssemblies = builder.ReferencedAssemblies;

            if (this.Config.Attribute("ReferenceCopyLocal").With(x => x == null ? true : (bool)x))
                referencedAssemblies = referencedAssemblies.Concat(builder.ReferenceCopyLocal);

            CreateAssemblyListingArray(builder, referencedAssembliesMethod, assembly.Type, this.GetAllReferencedAssemblies(referencedAssemblies));
        }

        private void CreateAssemblyListingArray(Builder builder, Method method, BuilderType assemblyType, IEnumerable<AssemblyDefinition> assembliesToList)
        {
            var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").Import().With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1).Import() });
            var typeInfo = builder.GetType("System.Reflection.TypeInfo").Import().With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly").Import() });

            method.NewCoder().Context(context =>
            {
                var returnValue = context.GetOrCreateReturnVariable();
                var referencedTypes = this.FilterAssemblyList(assembliesToList.Distinct(new AssemblyDefinitionEqualityComparer())).ToArray();

                if (referencedTypes.Length > 0)
                {
                    context.SetValue(returnValue, x => x.Newarr(assemblyType, referencedTypes.Length));

                    for (int i = 0; i < referencedTypes.Length; i++)
                    {
                        context.Load(returnValue)
                            .StoreElement(context.NewCoder()
                                .Call(introspectionExtensions.GetTypeInfo, referencedTypes[i].ToBuilderType().Import())
                                .Call(typeInfo.Assembly), i);
                    }
                }

                return context.Return();
            }).Replace();
        }

        private void CreateAssemblyLoadDummy(Builder builder, Method method, BuilderType assemblyType, AssemblyDefinition[] assembliesToList)
        {
            var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").Import().With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1).Import() });
            var typeInfo = builder.GetType("System.Reflection.TypeInfo").Import().With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly").Import() });

            method.NewCoder().Context(context =>
            {
                for (int i = 0; i < assembliesToList.Length; i++)
                {
                    // We make an exeption on test platform
                    if (assembliesToList[i] == null ||
                        assembliesToList[i].FullName == null ||
                        assembliesToList[i].FullName.StartsWith("Microsoft.VisualStudio.TestTools.UnitTesting"))
                        continue;

                    var type = assembliesToList[i].Modules
                        .SelectMany(y => y.Types)
                        .FirstOrDefault(y => y.IsPublic && y.FullName.Contains('.') && !y.HasCustomAttributes && !y.IsGenericParameter && !y.ContainsGenericParameter && !y.FullName.Contains('`'))?
                        .ToBuilderType()?
                        .Import();

                    if (type == null)
                        continue;

                    context.Call(introspectionExtensions.GetTypeInfo, type)
                        .Call(typeInfo.Assembly)
                        .Pop();
                }

                return context;
            }).Insert(InsertionPosition.Beginning);
        }

        private void CreateCauldronEntry(Builder builder)
        {
            BuilderType cauldron = null;

            if (builder.TypeExists("CauldronInterceptionHelper", SearchContext.Module))
                cauldron = builder.GetType("CauldronInterceptionHelper", SearchContext.Module);
            else
            {
                cauldron = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "CauldronInterceptionHelper");
                cauldron.CreateConstructor();
                cauldron.CustomAttributes.AddCompilerGeneratedAttribute();
            }

            cauldron.CustomAttributes.AddDebuggerDisplayAttribute(cauldron.Assembly.Name.Name);
        }

        private void ExecuteModuleAddition(Builder builder)
        {
            using (new StopwatchLog(this, "ModuleLoad"))
            {
                var assembly = builder.GetType(typeof(System.Reflection.Assembly));
                var cauldron = builder.GetType("CauldronInterceptionHelper", SearchContext.Module);
                var arrayType = assembly.MakeArray();
                var referencedAssembliesMethod = cauldron.GetMethod("GetReferencedAssemblies");

                // First find a type without namespace and with a static method called ModuleLoad
                var onLoadMethods = builder.FindMethodsByName(SearchContext.Module_NoGenerated, "ModuleLoad", 1)
                    .Where(x => x.IsStatic && x.ReturnType == BuilderType.Void && x.Parameters[0] == arrayType)
                    .Where(x => x != null);

                if (!onLoadMethods.Any())
                    return;

                if (onLoadMethods.Count() > 1)
                {
                    this.Log(LogTypes.Error, onLoadMethods.FirstOrDefault(), "There is more than one 'static ModuleLoad(Assembly[])' in the program.");
                    return;
                }

                var onLoadMethod = onLoadMethods.First();
                var module = builder.GetType("<Module>", SearchContext.Module);

                module
                    .CreateStaticConstructor().NewCoder()
                    .Call(onLoadMethod, x => x.Call(referencedAssembliesMethod))
                    .End
                    .Insert(InsertionPosition.End);
            }
        }

        private IEnumerable<TypeDefinition> FilterAssemblyList(IEnumerable<AssemblyDefinition> assemblies)
        {
            foreach (var item in assemblies)
            {
                if (item == null)
                    continue;

                if (item.FullName == null)
                    continue;

                if (item.FullName.StartsWith("Microsoft."))
                    continue;

                if (item.FullName.StartsWith("System."))
                    continue;

                if (item.FullName.StartsWith("Windows."))
                    continue;

                if (item.FullName == "testhost")
                    continue;

                foreach (var type in item.MainModule.Types)
                {
                    if (!type.IsPublic)
                        continue;

                    if (type.IsGenericParameter)
                        continue;

                    if (type.ContainsGenericParameter)
                        continue;

                    if (type.IsEnum)
                        continue;

                    if (type.IsInterface)
                        continue;

                    if (type.FullName.IndexOf('.') < 0)
                        continue;

                    if (type.FullName.IndexOf('<') >= 0)
                        continue;

                    if (type.FullName.IndexOf('>') >= 0)
                        continue;

                    if (type.FullName.IndexOf('`') >= 0)
                        continue;

                    if (type.Namespace.StartsWith("System."))
                        continue;

                    if (type.Name == "mscorlib")
                        continue;

                    yield return type;
                    break;
                }
            }
        }
    }
}