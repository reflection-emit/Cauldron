using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cauldron.Interception.Fody
{
    public sealed partial class ModuleWeaver : WeaverBase
    {
        private int counter = 0;

        private bool IsActivatorReferenced => this.Builder.TypeExists("Cauldron.Activator.Factory") && this.Builder.TypeExists("Cauldron.Activator.FactoryObjectCreatedEventArgs");

        private bool IsXAML => (this.Builder.IsReferenced("Cauldron.XAML") || this.Builder.IsReferenced("System.Xaml") || this.Builder.IsReferenced("Windows.UI.Xaml")) &&
                (this.Builder.TypeExists("Windows.UI.Xaml.Data.IValueConverter") || this.Builder.TypeExists("System.Windows.Data.IValueConverter") /* Fixes #39 */);

        public override void OnExecute()
        {
            if (!(this.Builder.IsReferenced("Cauldron.Interception") || this.Builder.TypeExists("Cauldron.Interception.IMethodInterceptor")))
            {
                this.Log(LogTypes.Warning, arg: $"The assembly 'Cauldron.Interception' is not referenced or used in '{this.Builder.Name}'. Weaving will not continue.");
                return;
            }

            var versionAttribute = typeof(ModuleWeaver)
                .Assembly
                .GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), true)
                .FirstOrDefault() as System.Reflection.AssemblyFileVersionAttribute;
            this.Log($"Cauldron Interception v" + versionAttribute.Version);

            var propertyInterceptingAttributes = this.Builder.FindAttributesByInterfaces(
                "Cauldron.Interception.IPropertyInterceptor",
                "Cauldron.Interception.IPropertyGetterInterceptor",
                "Cauldron.Interception.IPropertySetterInterceptor");

            var methodInterceptionAttributes = this.Builder.FindAttributesByInterfaces(
                "Cauldron.Interception.IMethodInterceptor");

            var simpleMethodInterceptionAttributes = this.Builder.FindAttributesByInterfaces(
                "Cauldron.Interception.ISimpleMethodInterceptor");

            var constructorInterceptionAttributes = this.Builder.FindAttributesByInterfaces(__IConstructorInterceptor.Type.Fullname);

            this.AddAssemblyWideAttributes(this.Builder);
            this.ImplementAnonymousTypeInterface(this.Builder);
            this.ImplementTimedCache(this.Builder);
            // this.ImplementMethodCache(this.Builder);
            this.ImplementBamlInitializer(this.Builder);
            this.ImplementTypeWidePropertyInterception(this.Builder, propertyInterceptingAttributes);
            this.ImplementTypeWideMethodInterception(this.Builder, methodInterceptionAttributes);
            this.ImplementTypeWideMethodInterception(this.Builder, simpleMethodInterceptionAttributes);
            // These should be done last, because they replace methods
            this.InterceptConstructors(this.Builder, constructorInterceptionAttributes);
            this.InterceptFields(this.Builder, propertyInterceptingAttributes);
            this.InterceptMethods(this.Builder, methodInterceptionAttributes);
            this.InterceptSimpleMethods(this.Builder, simpleMethodInterceptionAttributes);
            this.InterceptProperties(this.Builder, propertyInterceptingAttributes);

            this.CreateFactoryCache(this.Builder);

            if (this.Builder.TypeExists("Cauldron.XAML.ApplicationBase") && this.Builder.IsReferenced("PropertyChanged"))
                this.ImplementPropertyChangedEvent(this.Builder);

            if (this.Builder.TypeExists("Cauldron.XAML.Validation.ValidatorGroup"))
                this.AddValidatorInits(this.Builder);

            // Checks
            this.CheckAsyncMethodsNomenclature(this.Builder);
        }

        private void AddAttributeToXAMLResources(Builder builder)
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

                // Add a component contructor attribute to all .ctors
                foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                    ctor.CustomAttributes.Add(componentConstructorAttribute);
            }

            foreach (var item in viewModels)
            {
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
                    continue;

                if (item.CustomAttributes.HasAttribute(componentAttribute))
                    continue;

                item.CustomAttributes.Add(componentAttribute, item.Fullname);
                // Add a component contructor attribute to all .ctors
                foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                    ctor.CustomAttributes.Add(componentConstructorAttribute);
            }

            foreach (var item in valueConverters)
            {
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
                    continue;

                if (item.CustomAttributes.HasAttribute(componentAttribute))
                    continue;

                item.CustomAttributes.Add(componentAttribute, valueConverterInterface.Fullname);
                // Add a component contructor attribute to all .ctors
                foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                    ctor.CustomAttributes.Add(componentConstructorAttribute);
            }

            if (multiBindingConverters != null)
                foreach (var item in multiBindingConverters)
                {
                    if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
                        continue;

                    if (item.CustomAttributes.HasAttribute(componentAttribute))
                        continue;

                    item.CustomAttributes.Add(componentAttribute, multiBindingValueConverterInterface.Fullname);
                    // Add a component contructor attribute to all .ctors
                    foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                        ctor.CustomAttributes.Add(componentConstructorAttribute);
                }

            foreach (var item in resourceDictionaries)
            {
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters || item.IsNestedPrivate)
                    continue;

                if (item.CustomAttributes.HasAttribute(componentAttribute))
                    continue;

                item.CustomAttributes.Add(componentAttribute, resourceDictionaryBaseClass);
                // Add a component contructor attribute to all .ctors
                foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                    ctor.CustomAttributes.Add(componentConstructorAttribute);
            }
        }

        private void AddComponentAttribute(Builder builder, IEnumerable<BuilderType> builderTypes, Func<BuilderType, string> contractNameDelegate = null)
        {
            var componentConstructorAttribute = builder.GetType("Cauldron.Activator.ComponentConstructorAttribute");
            var componentAttribute = builder.GetType("Cauldron.Activator.ComponentAttribute").New(x => new
            {
                Type = x,
                ContractName = x.GetMethod("get_ContractName"),
                Policy = x.GetMethod("get_Policy"),
                Priority = x.GetMethod("get_Priority")
            });

            foreach (var item in builderTypes)
            {
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
                    continue;

                if (item.CustomAttributes.HasAttribute(componentAttribute.Type))
                    continue;

                var contractName = (contractNameDelegate == null ? item.Fullname : contractNameDelegate(item)).Replace('/', '+');
                item.CustomAttributes.Add(componentAttribute.Type, contractName);

                // Add a component contructor attribute to all .ctors
                foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                    ctor.CustomAttributes.Add(componentConstructorAttribute);
            }
        }

        private void AddEntranceAssemblyHACK(Builder builder)
        {
            if (builder.TypeExists("Cauldron.Core.ILoadedAssemblies"))
            {
                var module = builder.GetType("<Module>", SearchContext.Module);
                var cauldron = builder.GetType("<Cauldron>", SearchContext.Module);
                var assembly = builder.GetType("System.Reflection.Assembly").New(x => new { Type = x, Load = x.GetMethod("Load", 1) });

                // UWP has to actually use any type from the Assembly, so that it is not thrown out
                // while compiling to Nativ Code

                this.Log(builder.Name);

                if (builder.Name != "Cauldron.dll" && builder.TypeExists("Cauldron.Core.Reflection.AssembliesCORE"))
                {
                    module.CreateStaticConstructor().NewCode().Context(x =>
                    {
                        var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").New(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1) });
                        var typeInfo = builder.GetType("System.Reflection.TypeInfo").New(y => new { Type = y, Assembly = y.GetMethod("get_Assembly") });
                        var assemblies = builder.GetType("Cauldron.Core.Reflection.AssembliesCORE").New(y => new { Type = y, EntryAssembly = y.GetMethod("set_EntryAssembly", 1) });
                        x.Call(assemblies.EntryAssembly, x.NewCode().Callvirt(x.NewCode().Call(introspectionExtensions.GetTypeInfo, module), typeInfo.Assembly));
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

                    var loadedAssembliesInterface = builder.GetType("Cauldron.Core.ILoadedAssemblies").New(x => new { Type = x, ReferencedAssemblies = x.GetMethod("ReferencedAssemblies") });
                    cauldron.AddInterface(loadedAssembliesInterface.Type);

                    CreateAssemblyListingArray(builder,
                        cauldron.CreateMethod(Modifiers.Overrrides | Modifiers.Public, builder.MakeArray(assembly.Type), "ReferencedAssemblies"),
                        assembly.Type, builder.ReferencedAssemblies.Concat(referencedAssembliesFromOtherAssemblies).Distinct());
                }
            }
        }

        private void AddValidatorInits(Builder builder)
        {
            var attributes = builder.FindAttributesByBaseClass("Cauldron.XAML.Validation.ValidatorAttributeBase");
            var propertiesWithAttributes = builder.FindPropertiesByAttributes(attributes)
                .Where(x => !x.Property.IsStatic)
                .GroupBy(x => x.Property)
                .Select(x => new
                {
                    Key = x.Key,
                    Validators = x.ToArray()
                });

            foreach (var item in propertiesWithAttributes)
            {
                var addValidatorGroup = item.Key.OriginType.GetMethod("AddValidatorGroup", false, typeof(string));
                if (addValidatorGroup == null)
                    continue;
                var addValidatorAttribute = item.Key.OriginType.GetMethod("AddValidator", false, typeof(string).FullName, "Cauldron.XAML.Validation.ValidatorAttributeBase");

                this.Log($"Adding initializer for validators ({item.Validators.Length}) of property '{item.Key.Name}' in type '{item.Key.OriginType.Fullname}'");

                foreach (var ctors in item.Key.OriginType.GetRelevantConstructors())
                {
                    ctors.NewCode().Context(x =>
                    {
                        x.Call(Crumb.This, addValidatorGroup, item.Key.Name);

                        for (int i = 0; i < item.Validators.Length; i++)
                            x.Call(Crumb.This, addValidatorAttribute, item.Key.Name, x.NewCode().NewObj(item.Validators[i]));
                    })
                    .Insert(InsertionPosition.Beginning);
                }
            }
        }

        private void CreateAssemblyListingArray(Builder builder, Method method, BuilderType assemblyType, IEnumerable<AssemblyDefinition> assembliesToList)
        {
            var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").New(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1) });
            var typeInfo = builder.GetType("System.Reflection.TypeInfo").New(y => new { Type = y, Assembly = y.GetMethod("get_Assembly") });

            method.NewCode().Context(x =>
            {
                var returnValue = x.GetReturnVariable();
                var referencedTypes = this.FilterAssemblyList(assembliesToList);

                if (referencedTypes.Length > 0)
                {
                    x.Newarr(assemblyType, referencedTypes.Length).StoreLocal(returnValue);

                    for (int i = 0; i < referencedTypes.Length; i++)
                    {
                        x.Load(returnValue);
                        x.StoreElement(assemblyType, x.NewCode().Callvirt(x.NewCode().Call(introspectionExtensions.GetTypeInfo, referencedTypes[i].ToBuilderType(this.Builder).Import()), typeInfo.Assembly), i);
                    }
                }

                x.Return();
            }).Replace();
        }

        private void CreateAssemblyLoadDummy(Builder builder, Method method, BuilderType assemblyType, AssemblyDefinition[] assembliesToList)
        {
            var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").New(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1) });
            var typeInfo = builder.GetType("System.Reflection.TypeInfo").New(y => new { Type = y, Assembly = y.GetMethod("get_Assembly") });

            method.NewCode().Context(x =>
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
                        .ToBuilderType(this.Builder)?
                        .Import();

                    if (type == null)
                        continue;

                    x.Callvirt(x.NewCode().Call(introspectionExtensions.GetTypeInfo, type), typeInfo.Assembly).Pop();
                }
            }).Insert(InsertionPosition.Beginning);
        }

        private Method CreateAssigningMethod(BuilderType anonSource, BuilderType anonTarget, BuilderType anonTargetInterface, Method method)
        {
            var name = $"<{counter++}>f__Anon_Assign";
            var assignMethod = method.OriginType.CreateMethod(Modifiers.PrivateStatic, anonTarget, name, anonSource);
            assignMethod.NewCode()
                .Context(x =>
                {
                    var resultVar = x.GetReturnVariable();
                    x.Assign(resultVar).Set(x.NewCode().NewObj(anonTarget.ParameterlessContructor));

                    foreach (var property in anonSource.Properties)
                    {
                        try
                        {
                            var targetProperty = anonTarget.GetProperty(property.Name);
                            if (property.ReturnType.Fullname != targetProperty.ReturnType.Fullname)
                            {
                                this.Log(LogTypes.Error, property, $"The property '{property.Name}' does not have the expected return type. Is: {property.ReturnType.Fullname} Expected: {targetProperty.ReturnType.Fullname}");
                                continue;
                            }
                            x.Load(resultVar).Callvirt(targetProperty.Setter, x.NewCode().Load(Crumb.GetParameter(0)).Callvirt(property.Getter));
                        }
                        catch (MethodNotFoundException)
                        {
                            this.Log(LogTypes.Warning, anonTarget, $"The property '{property.Name}' does not exist in '{anonTarget.Name}'");
                        }
                    }

                    x.Load(resultVar).Return();
                })
                .Replace();

            assignMethod.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

            return assignMethod;
        }

        private void ExecuteModuleAddition(Builder builder)
        {
            var @string = builder.GetType(typeof(string));
            var arrayType = @string.MakeArray();

            // First find a type without namespace and with a static method called OnLoad
            var onLoadMethods = builder.GetTypes(SearchContext.Module)
                .Where(x => x.Namespace == null || x.Namespace == "")
                .Select(x => x.GetMethod("OnLoad", false, arrayType))
                .Where(x => x != null);

            if (onLoadMethods.Count() > 1)
            {
                this.Log(LogTypes.Error, onLoadMethods.FirstOrDefault(), "There is more than one 'static OnLoad(string[])' in the program.");
                return;
            }

            var onLoadMethod = onLoadMethods.FirstOrDefault();
            if (onLoadMethod == null)
                return;

            var module = builder.GetType("<Module>", SearchContext.Module);

            module.CreateStaticConstructor().NewCode().Context(x =>
            {
                var indexer = 0;
                var array = x.CreateVariable(arrayType);
                x.Newarr(@string, builder.UnusedReference.Length + builder.ReferencedAssemblies.Length).StoreLocal(array);

                for (int i = 0; i < builder.UnusedReference.Length; i++)
                {
                    x.Load(array);
                    x.StoreElement(@string, builder.UnusedReference[i].Filename, indexer++);
                }

                for (int i = 0; i < builder.ReferencedAssemblies.Length; i++)
                {
                    x.Load(array);
                    x.StoreElement(@string, builder.ReferencedAssemblies[i].Name.Name, indexer++);
                }

                x.Call(onLoadMethod, array);
            }).Insert(InsertionPosition.End);
        }

        private TypeDefinition[] FilterAssemblyList(IEnumerable<AssemblyDefinition> assemblies) =>
            assemblies
            .Where(x => x != null && x.FullName != null && !x.FullName.StartsWith("Microsoft.VisualStudio.TestTools.UnitTesting") && !x.FullName.StartsWith("System."))
            .Select(x => x.MainModule.Types
                    .FirstOrDefault(y => y.IsPublic && !y.IsGenericParameter && !y.HasCustomAttributes && !y.ContainsGenericParameter && !y.FullName.Contains('`') && y.FullName.Contains('.'))
            )
            .Where(x => x != null)
            .ToArray();

        private void ImplementAnonymousTypeInterface(Builder builder)
        {
            var cauldronCoreExtension = builder.GetType("Cauldron.Interception.Extensions");
            var createTypeMethod = cauldronCoreExtension.GetMethod("CreateType", 1).FindUsages().ToArray();
            var createdTypes = new Dictionary<string, BuilderType>();

            if (!createTypeMethod.Any())
                return;

            using (new StopwatchLog(this, "anonymous type to interface"))
            {
                foreach (var item in createTypeMethod)
                {
                    this.Log(LogTypes.Info, $"Implementing anonymous to interface {item}");
                    var interfaceToImplement = item.GetGenericArgument(0);

                    if (interfaceToImplement == null || !interfaceToImplement.IsInterface)
                    {
                        this.Log(LogTypes.Error, interfaceToImplement, $"{interfaceToImplement.Fullname} is not an interface.");
                        continue;
                    }

                    try
                    {
                        var type = item.GetPreviousInstructionObjectType();

                        if (type.Fullname.GetHashCode() == "System.Object".GetHashCode() && type.Fullname == "System.Object")
                        {
                            type = item.GetLastNewObjectType();

                            if (type.Fullname.GetHashCode() == "System.Object".GetHashCode() && type.Fullname == "System.Object")
                            {
                                this.Log(LogTypes.Error, item.HostMethod, $"Error in CreateObject in method '{item.HostMethod}'. Unable to detect anonymous type.");
                                continue;
                            }
                        }

                        if (createdTypes.ContainsKey(interfaceToImplement.Fullname))
                        {
                            item.Replace(CreateAssigningMethod(type, createdTypes[interfaceToImplement.Fullname], interfaceToImplement, item.HostMethod));
                            continue;
                        }

                        var anonymousTypeName = $"<>f__{interfaceToImplement.Name}_Cauldron_AnonymousType{counter++}";
                        this.Log($"- Creating new type: {type.Namespace}.{anonymousTypeName}");

                        var newType = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable, anonymousTypeName);
                        newType.AddInterface(interfaceToImplement);

                        // Implement the methods
                        foreach (var method in interfaceToImplement.Methods.Where(x => !x.IsPropertyGetterSetter))
                            newType.CreateMethod(Modifiers.Public | Modifiers.Overrrides, method.ReturnType, method.Name, method.Parameters)
                                .NewCode()
                                .ThrowNew(typeof(NotImplementedException), $"The method '{method.Name}' in type '{newType.Name}' is not implemented.")
                                .Replace();
                        // Implement the properties
                        foreach (var property in interfaceToImplement.Properties)
                            newType.CreateProperty(Modifiers.Public | Modifiers.Overrrides, property.ReturnType, property.Name);

                        // Create ctor
                        newType.CreateConstructor()
                            .NewCode()
                            .Context(x =>
                            {
                                x.Load(Crumb.This).Call(builder.GetType(typeof(object)).Import().ParameterlessContructor.Import());
                            })
                            .Return()
                            .Replace();

                        newType.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

                        createdTypes.Add(interfaceToImplement.Fullname, newType);

                        item.Replace(CreateAssigningMethod(type, newType, interfaceToImplement, item.HostMethod));
                    }
                    catch (Exception e)
                    {
                        this.Log(e, item.ToHostMethodInstructionsString());

                        throw;
                    }
                }
            }
        }

        private void InterceptFields(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            using (new StopwatchLog(this, "field"))
            {
                var fields = builder.FindFieldsByAttributes(attributes).GroupBy(x => x.Field).ToArray();

                foreach (var field in fields)
                {
                    this.Log($"Implementing interceptors in fields {field.Key}");

                    if (!field.Key.Modifiers.HasFlag(Modifiers.Private))
                    {
                        this.Log(LogTypes.Error, field.Key.OriginType, $"The current version of the field interceptor only intercepts private fields. Field '{field.Key.Name}' in type '{field.Key.OriginType.Name}'");
                        continue;
                    }

                    var type = field.Key.OriginType;
                    var usage = field.Key.FindUsages().ToArray();
                    var property = type.CreateProperty(field.Key);

                    property.CustomAttributes.AddCompilerGeneratedAttribute();
                    property.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);
                    property.CustomAttributes.AddNonSerializedAttribute();

                    foreach (var attribute in field)
                        attribute.Attribute.MoveTo(property);

                    foreach (var item in usage)
                        if (item.Field.IsStatic || !item.IsBeforeBaseCall)
                            item.Replace(property);
                }
            }
        }
    }
}