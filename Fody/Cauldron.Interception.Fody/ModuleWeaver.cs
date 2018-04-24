using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System.Collections.Generic;
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
            this.AddAssemblyWideAttributes(this.Builder);
            this.ExecuteModuleAddition(this.Builder);
            this.ExecuteInterceptionScripts(this.Builder);
            this.AddEntranceAssemblyHACK(this.Builder);
        }

        private void AddEntranceAssemblyHACK(Builder builder)
        {
            if (builder.TypeExists("Cauldron.Core.ILoadedAssemblies"))
            {
                var module = builder.GetType("<Module>", SearchContext.Module);
                var cauldron = builder.GetType("<Cauldron>", SearchContext.Module);
                var assembly = builder.GetType("System.Reflection.Assembly").With(x => new { Type = x, Load = x.GetMethod("Load", 1) });

                // UWP has to actually use any type from the Assembly, so that it is not thrown out
                // while compiling to Nativ Code

                this.Log(builder.Name);

                if (builder.Name != "Cauldron.dll" && builder.TypeExists("Cauldron.Core.Reflection.AssembliesCORE"))
                {
                    module.CreateStaticConstructor().NewCoder().Context(context =>
                    {
                        var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1) });
                        var typeInfo = builder.GetType("System.Reflection.TypeInfo").With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly") });
                        var assemblies = builder.GetType("Cauldron.Core.Reflection.AssembliesCORE").With(y => new { Type = y, EntryAssembly = y.GetMethod("set_EntryAssembly", 1) });
                        return context.Call(assemblies.EntryAssembly, x =>
                            x.Call(introspectionExtensions.GetTypeInfo, module)
                                .Call(typeInfo.Assembly)).End;
                    })
                    .Insert(InsertionPosition.End);

                    module.StaticConstructor.CustomAttributes.Add(typeof(MethodImplAttribute), MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization);
                }

                // Add a new interface to <Cauldron> type
                if (builder.TypeExists("Cauldron.Core.ILoadedAssemblies"))
                {
                    var referencedAssembliesFromOtherAssemblies = builder
                        .FindTypesByInterface(SearchContext.AllReferencedModules, "Cauldron.Core.ILoadedAssemblies")
                        .Select(x => x.GetMethod("ReferencedAssemblies"))
                        .SelectMany(x => x.GetTokens())
                        .Where(x => x != null)
                        .Select(x => x.Module.Assembly);

                    var loadedAssembliesInterface = builder.GetType("Cauldron.Core.ILoadedAssemblies").With(x => new { Type = x, ReferencedAssemblies = x.GetMethod("ReferencedAssemblies") });
                    cauldron.AddInterface(loadedAssembliesInterface.Type);

                    CreateAssemblyListingArray(builder,
                        cauldron.CreateMethod(Modifiers.Overrides | Modifiers.Public, builder.MakeArray(assembly.Type), "ReferencedAssemblies"),
                        assembly.Type, builder.ReferencedAssemblies.Concat(referencedAssembliesFromOtherAssemblies).Distinct());
                }
            }
        }

        private void CreateAssemblyListingArray(Builder builder, Method method, BuilderType assemblyType, IEnumerable<AssemblyDefinition> assembliesToList)
        {
            var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1) });
            var typeInfo = builder.GetType("System.Reflection.TypeInfo").With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly") });

            method.NewCoder().Context(context =>
            {
                var returnValue = context.GetOrCreateReturnVariable();
                var referencedTypes = this.FilterAssemblyList(assembliesToList);

                if (referencedTypes.Length > 0)
                {
                    context.SetValue(returnValue, x => x.Newarr(assemblyType, referencedTypes.Length));

                    for (int i = 0; i < referencedTypes.Length; i++)
                    {
                        context.Load(returnValue)
                            .Call(introspectionExtensions.GetTypeInfo, referencedTypes[i].ToBuilderType().Import())
                            .Call(typeInfo.Assembly)
                            .StoreElement(assemblyType, i);
                    }
                }

                return context.Return();
            }).Replace();
        }

        private void CreateAssemblyLoadDummy(Builder builder, Method method, BuilderType assemblyType, AssemblyDefinition[] assembliesToList)
        {
            var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1) });
            var typeInfo = builder.GetType("System.Reflection.TypeInfo").With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly") });

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

        private void ExecuteModuleAddition(Builder builder)
        {
            using (new StopwatchLog(this, "ModuleLoad"))
            {
                var @string = builder.GetType(typeof(string));
                var arrayType = @string.MakeArray();

                // First find a type without namespace and with a static method called OnLoad
                var onLoadMethods = builder.FindMethodsByName(SearchContext.Module_NoGenerated, "ModuleLoad", 1)
                    .Where(x => x.IsStatic && x.ReturnType == "System.Void" && x.Parameters[0] == arrayType)
                    .Where(x => x != null);

                if (!onLoadMethods.Any())
                    return;

                if (onLoadMethods.Count() > 1)
                {
                    this.Log(LogTypes.Error, onLoadMethods.FirstOrDefault(), "There is more than one 'static ModuleLoad(string[])' in the program.");
                    return;
                }

                var onLoadMethod = onLoadMethods.First();
                var module = builder.GetType("<Module>", SearchContext.Module);

                module.CreateStaticConstructor().NewCoder().Context(context =>
                {
                    var indexer = 0;
                    var array = context.AssociatedMethod.GetOrCreateVariable(arrayType);
                    context.SetValue(array, x => x.Newarr(@string, builder.UnusedReference.Length + builder.ReferencedAssemblies.Length));

                    for (int i = 0; i < builder.UnusedReference.Length; i++)
                    {
                        var item = builder.UnusedReference[i]?.Filename;
                        if (string.IsNullOrEmpty(item))
                            continue;

                        context
                            .Load(array)
                            .StoreElement(item, indexer++);
                    }

                    for (int i = 0; i < builder.ReferencedAssemblies.Length; i++)
                    {
                        var item = builder.ReferencedAssemblies[i]?.Name?.Name;
                        if (string.IsNullOrEmpty(item))
                            continue;

                        context.Load(array)
                            .StoreElement(item, indexer++);
                    }

                    return context.Call(onLoadMethod, array).End;
                }).Insert(InsertionPosition.End);
            }
        }

        private TypeDefinition[] FilterAssemblyList(IEnumerable<AssemblyDefinition> assemblies) =>
            assemblies
            .Where(x => x != null && x.FullName != null && !x.FullName.StartsWith("Microsoft.VisualStudio.TestTools.UnitTesting") && !x.FullName.StartsWith("System."))
            .Select(x => x.MainModule.Types
                    .FirstOrDefault(y => y.IsPublic && !y.IsGenericParameter && !y.HasCustomAttributes && !y.ContainsGenericParameter && !y.FullName.Contains('`') && y.FullName.Contains('.'))
            )
            .Where(x => x != null)
            .ToArray();
    }
}