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
            this.CreateCauldronEntry(this.Builder);
            this.AddAssemblyWideAttributes(this.Builder);
            this.ExecuteInterceptionScripts(this.Builder);
            this.AddEntranceAssemblyHACK(this.Builder);
            this.ExecuteModuleAddition(this.Builder);
        }

        private void AddEntranceAssemblyHACK(Builder builder)
        {
            var assembly = builder.GetType("System.Reflection.Assembly").Import().With(x => new { Type = x, Load = x.GetMethod("Load", 1).Import() });

            // Add the Entrance Assembly hack for UWP
            if (builder.IsUWP)
            {
                var voidMain = builder.FindMethodsByName(SearchContext.Module_NoGenerated, "Main", 1)
                    .FirstOrDefault(x => x.ReturnType == BuilderType.Void && x.Parameters[0].ChildType == BuilderType.String);

                if (builder.Name != "Cauldron.dll" && builder.TypeExists("Cauldron.Core.Reflection.AssembliesCORE"))
                {
                    voidMain.NewCoder().Context(context =>
                    {
                        var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").Import().With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1).Import() });
                        var typeInfo = builder.GetType("System.Reflection.TypeInfo").Import().With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly").Import() });
                        var assemblies = builder.GetType("Cauldron.Core.Reflection.AssembliesCORE").Import().With(y => new { Type = y, EntryAssembly = y.GetMethod("SetEntryAssembly", 1).Import() });
                        return context.Call(assemblies.EntryAssembly, x =>
                            x.Call(introspectionExtensions.GetTypeInfo, voidMain.DeclaringType)
                                .Call(typeInfo.Assembly)).End;
                    })
                    .Insert(InsertionPosition.Beginning);
                }
            }

            var cauldron = builder.GetType("<Cauldron>", SearchContext.Module);
            var referencedAssembliesMethod = cauldron.CreateMethod(Modifiers.InternalStatic, builder.MakeArray(assembly.Type), "ReferencedAssembliesInternal");

            // Add a new interface to <Cauldron> type
            if (builder.TypeExists("Cauldron.Core.ILoadedAssemblies"))
            {
                var loadedAssembliesInterface = builder.GetType("Cauldron.Core.ILoadedAssemblies").Import().With(x => new { Type = x, ReferencedAssemblies = x.GetMethod("ReferencedAssemblies").Import() });
                cauldron.AddInterface(loadedAssembliesInterface.Type);
                cauldron.CreateMethod(Modifiers.Overrides | Modifiers.Public, builder.MakeArray(assembly.Type), "ReferencedAssemblies")
                    .NewCoder()
                    .Call(referencedAssembliesMethod)
                    .Return()
                    .Replace();
            }

            CreateAssemblyListingArray(builder, referencedAssembliesMethod,
                assembly.Type, builder.ReferencedAssemblies.Concat(builder.ReferenceCopyLocal).Distinct());
        }

        private void CreateAssemblyListingArray(Builder builder, Method method, BuilderType assemblyType, IEnumerable<AssemblyDefinition> assembliesToList)
        {
            var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").Import().With(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1).Import() });
            var typeInfo = builder.GetType("System.Reflection.TypeInfo").Import().With(y => new { Type = y, Assembly = y.GetMethod("get_Assembly").Import() });

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

            if (builder.TypeExists("<Cauldron>", SearchContext.Module))
                cauldron = builder.GetType("<Cauldron>", SearchContext.Module);
            else
            {
                cauldron = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<Cauldron>");
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
                var cauldron = builder.GetType("<Cauldron>", SearchContext.Module);
                var arrayType = assembly.MakeArray();
                var referencedAssembliesMethod = cauldron.GetMethod("ReferencedAssembliesInternal");

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