using Cauldron.Interception.Cecilator;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cauldron.Interception.Fody
{
    public sealed class ModuleWeaver : WeaverBase
    {
        private int counter = 0;

        public override void OnExecute()
        {
            if (!this.Builder.IsReferenced("Cauldron.Interception"))
            {
                this.LogWarning($"The assembly 'Cauldron.Interception' is not referenced or used in '{this.Builder.Name}'. Weaving will not continue.");
                return;
            }

            var propertyInterceptingAttributes = this.Builder.FindAttributesByInterfaces(
                "Cauldron.Interception.IPropertyInterceptor",
                "Cauldron.Interception.ILockablePropertyGetterInterceptor",
                "Cauldron.Interception.ILockablePropertySetterInterceptor",
                "Cauldron.Interception.IPropertyGetterInterceptor",
                "Cauldron.Interception.IPropertySetterInterceptor");

            var methodInterceptionAttributes = this.Builder.FindAttributesByInterfaces(
                "Cauldron.Interception.ILockableMethodInterceptor",
                "Cauldron.Interception.IMethodInterceptor");

            this.ImplementAnonymousTypeInterface(this.Builder);
            this.ImplementTimedCache(this.Builder);
            this.ImplementMethodCache(this.Builder);
            this.ImplementTypeWidePropertyInterception(this.Builder, propertyInterceptingAttributes);
            this.ImplementTypeWideMethodInterception(this.Builder, methodInterceptionAttributes);
            // These should be done last, because they replace methods
            this.InterceptFields(this.Builder, propertyInterceptingAttributes);
            this.InterceptMethods(this.Builder, methodInterceptionAttributes);
            this.InterceptProperties(this.Builder, propertyInterceptingAttributes);

            this.CreateFactoryCache(this.Builder);

            if (this.Builder.IsReferenced("Cauldron.XAML") && this.Builder.IsReferenced("PropertyChanged"))
                this.ImplementPropertyChangedEvent(this.Builder);

            if (this.Builder.IsReferenced("Cauldron.XAML.Validation"))
                this.AddValidatorInits(this.Builder);
        }

        private void AddAttributeToXAMLResources(Builder builder)
        {
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
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
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
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
                    continue;

                if (item.CustomAttributes.HasAttribute(componentAttribute))
                    continue;

                item.CustomAttributes.Add(componentAttribute, valueConverterInterface.Fullname);
                // Add a component contructor attribute to all .ctors
                foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                    ctor.CustomAttributes.Add(componentConstructorAttribute);
            }

            foreach (var item in resourceDictionaries)
            {
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
                    continue;

                if (item.CustomAttributes.HasAttribute(componentAttribute))
                    continue;

                item.CustomAttributes.Add(componentAttribute, resourceDictionaryBaseClass);
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
                // while compiling to Nativ Code CreateAssemblyLoadDummy(builder,
                // module.CreateStaticConstructor(), assembly.Type, builder.UnusedReference.Select(x
                // => x.AssemblyDefinition).ToArray());

                module.CreateStaticConstructor().NewCode().Context(x =>
                {
                    if (builder.TypeExists("Cauldron.Core.Assemblies"))
                    {
                        var introspectionExtensions = builder.GetType("System.Reflection.IntrospectionExtensions").New(y => new { Type = y, GetTypeInfo = y.GetMethod("GetTypeInfo", 1) });
                        var typeInfo = builder.GetType("System.Reflection.TypeInfo").New(y => new { Type = y, Assembly = y.GetMethod("get_Assembly") });
                        var assemblies = builder.GetType("Cauldron.Core.AssembliesCORE").New(y => new { Type = y, EntryAssembly = y.GetMethod("set_EntryAssembly", 1) });
                        x.Call(assemblies.EntryAssembly, x.NewCode().Callvirt(x.NewCode().Call(introspectionExtensions.GetTypeInfo, module), typeInfo.Assembly));
                    }
                })
                .Insert(InsertionPosition.End);

                module.StaticConstructor.CustomAttributes.Add(typeof(MethodImplAttribute), MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization);

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
                var addValidatorGroup = item.Key.DeclaringType.GetMethod("AddValidatorGroup", false, typeof(string));
                if (addValidatorGroup == null)
                    continue;
                var addValidatorAttribute = item.Key.DeclaringType.GetMethod("AddValidator", false, typeof(string).FullName, "Cauldron.XAML.Validation.ValidatorAttributeBase");

                this.LogInfo($"Adding initializer for validators ({item.Validators.Length}) of property '{item.Key.Name}' in type '{item.Key.DeclaringType.Fullname}'");

                foreach (var ctors in item.Key.DeclaringType.GetRelevantConstructors())
                {
                    ctors.NewCode().Context(x =>
                    {
                        x.Call(x.This, addValidatorGroup, item.Key.Name);

                        for (int i = 0; i < item.Validators.Length; i++)
                            x.Call(x.This, addValidatorAttribute, item.Key.Name, x.NewCode().NewObj(item.Validators[i]));
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
                    if (assembliesToList[i] == null || assembliesToList[i].FullName == null || assembliesToList[i].FullName.StartsWith("Microsoft.VisualStudio.TestPlatform"))
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
            var assignMethod = method.DeclaringType.CreateMethod(Modifiers.PrivateStatic, anonTarget, name, anonSource);
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
                                this.LogError($"The property '{property.Name}' in '{method.Name}' in type '{method.DeclaringType.Name}' does not have the expected return type. Is: {property.ReturnType.Fullname} Expected: {targetProperty.ReturnType.Fullname}");
                                continue;
                            }
                            x.Load(resultVar).Callvirt(targetProperty.Setter, x.NewCode().Load(x.GetParameter(0)).Callvirt(property.Getter));
                        }
                        catch (MethodNotFoundException)
                        {
                            this.LogError($"The property '{property.Name}' does not exist in '{anonTarget.Name}'");
                        }
                    }

                    x.Load(resultVar).Return();
                })
                .Replace();

            assignMethod.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

            return assignMethod;
        }

        private void CreateComponentCache(Builder builder, BuilderType cauldron)
        {
            var componentAttribute = builder.GetType("Cauldron.Activator.ComponentAttribute").New(x => new
            {
                Type = x,
                ContractName = x.GetMethod("get_ContractName"),
                Policy = x.GetMethod("get_Policy"),
                Priority = x.GetMethod("get_Priority")
            });
            var componentConstructorAttribute = builder.GetType("Cauldron.Activator.ComponentConstructorAttribute");

            // Before we start let us find all factoryextensions and add a component attribute to them
            var factoryResolverInterface = builder.GetType("Cauldron.Activator.IFactoryResolver");
            var factoryResolvers = builder.FindTypesByInterface(factoryResolverInterface);

            foreach (var item in factoryResolvers)
            {
                if (item.IsAbstract || item.IsInterface || item.HasUnresolvedGenericParameters)
                    continue;

                if (item.CustomAttributes.HasAttribute(componentAttribute.Type))
                    continue;

                item.CustomAttributes.Add(componentAttribute.Type, factoryResolverInterface.Fullname);
                // Add a component contructor attribute to all .ctors
                foreach (var ctor in item.Methods.Where(x => x.Name == ".ctor"))
                    ctor.CustomAttributes.Add(componentConstructorAttribute);
            }

            int counter = 0;
            var arrayAvatar = builder.GetType("System.Array").New(x => new
            {
                Length = x.GetMethod("get_Length")
            });
            var extensionAvatar = builder.GetType("Cauldron.Core.Extensions.ExtensionsReflection").New(x => new
            {
                CreateInstance = x.GetMethod("CreateInstance", 2)
            });
            var factoryCacheInterface = builder.GetType("Cauldron.Activator.IFactoryCache");
            var factoryTypeInfoInterface = builder.GetType("Cauldron.Activator.IFactoryTypeInfo");
            var createInstanceInterfaceMethod = factoryTypeInfoInterface.GetMethod("CreateInstance", 1);

            // Get all Components
            var components = builder.FindTypesByAttribute(componentAttribute.Type);
            var componentTypes = new List<BuilderType>();

            // Create types with the components properties
            foreach (var component in components)
            {
                this.LogInfo("Hardcoding component factory .ctor: " + component.Type.Fullname);

                var componentType = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, $"<>f__IFactoryTypeInfo_{component.Type.Name}_{counter++}");
                var componentAttributeField = componentType.CreateField(Modifiers.Private, componentAttribute.Type, "componentAttribute");
                componentType.AddInterface(factoryTypeInfoInterface);
                componentTypes.Add(componentType);

                // Create ctor
                componentType
                   .CreateConstructor()
                   .NewCode()
                   .Context(x =>
                   {
                       x.Load(x.This).Call(builder.GetType(typeof(object)).ParameterlessContructor);
                       x.Assign(componentAttributeField).NewObj(component);
                   })
                   .Return()
                   .Replace();

                // Implement the methods
                componentType.CreateMethod(Modifiers.Public | Modifiers.Overrrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                    .NewCode()
                    .Context(x =>
                    {
                        // Find any method with a componentcontructor attribute
                        var ctors = component.Type.GetMethods(y =>
                                {
                                    if (y.Name == ".cctor")
                                        return true;

                                    if (!y.Resolve().IsPublic)
                                        return false;

                                    if (y.Name == ".ctor" && y.DeclaringType.FullName.GetHashCode() != component.Type.Fullname.GetHashCode() && y.DeclaringType.FullName != component.Type.Fullname)
                                        return false;

                                    if (y.Name.StartsWith("set_"))
                                        return false;

                                    return true;
                                })
                                .Where(y => y.CustomAttributes.HasAttribute(componentConstructorAttribute))
                                .Concat(
                                    component.Type.GetAllProperties().Where(y => y.CustomAttributes.HasAttribute(componentConstructorAttribute))
                                        .Select(y =>
                                        {
                                            y.CustomAttributes.Remove(componentConstructorAttribute);
                                            y.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

                                            return y.Getter;
                                        })
                                    )
                                .OrderBy(y => y.Parameters.Length)
                                .ToArray();

                        if (ctors.Length > 0)
                        {
                            bool parameterlessCtorAlreadyHandled = false;

                            for (int index = 0; index < ctors.Length; index++)
                            {
                                this.LogInfo(ctors[index].Fullname);

                                var ctor = ctors[index];
                                // add a EditorBrowsable attribute
                                ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                                var ctorParameters = ctor.Parameters;

                                if (ctorParameters.Length > 0)
                                {
                                    // In this case we have to find a parameterless constructor first
                                    if (component.Type.ParameterlessContructor != null && !parameterlessCtorAlreadyHandled && component.Type.ParameterlessContructor.Modifiers.HasFlag(Modifiers.Public))
                                    {
                                        x.Load(x.GetParameter(0)).IsNull().Then(y => y.NewObj(component.Type.ParameterlessContructor).Return());
                                        x.Load(x.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(component.Type.ParameterlessContructor).Return());
                                        parameterlessCtorAlreadyHandled = true;
                                    }

                                    var code = x.Load(x.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(ctorParameters.Length);

                                    for (int i = 0; i < ctorParameters.Length; i++)
                                        code.And.Load(x.GetParameter(0).UnPacked(i)).Is(ctorParameters[i]);

                                    if (ctor.Name == ".ctor")
                                        code.Then(y => y.NewObj(ctor, x.GetParameter(0).UnPacked()).Return());
                                    else
                                        code.Then(y => y.Call(ctor, x.GetParameter(0).UnPacked()).Return());
                                }
                                else
                                {
                                    if (ctor.Name == ".ctor")
                                    {
                                        x.Load(x.GetParameter(0)).IsNull().Then(y => y.NewObj(ctor).Return());
                                        x.Load(x.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(ctor).Return());
                                    }
                                    else
                                    {
                                        x.Load(x.GetParameter(0)).IsNull().Then(y => y.Call(ctor).Return());
                                        x.Load(x.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.Call(ctor).Return());
                                    }

                                    parameterlessCtorAlreadyHandled = true;
                                }
                            }
                        }
                        else
                        {
                            // In case we don't have constructor with ComponentConstructor Attribute,
                            // then we should look for a parameterless Ctor
                            if (component.Type.ParameterlessContructor == null)
                                this.LogError($"The component '{component.Type.Fullname}' has no ComponentConstructor attribute or the constructor is not public");
                            else if (component.Type.ParameterlessContructor.Modifiers.HasFlag(Modifiers.Public))
                            {
                                x.Load(x.GetParameter(0)).IsNull().Then(y => y.NewObj(component.Type.ParameterlessContructor).Return());
                                x.Load(x.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(component.Type.ParameterlessContructor).Return());

                                this.LogWarning($"The component '{component.Type.Fullname}' has no ComponentConstructor attribute. A parameterless ctor was found and will be used.");
                            }
                        }
                    })
                    .Context(x => x.Call(extensionAvatar.CreateInstance, component.Type, x.GetParameter(0)).Return())
                    .Return()
                    .Replace();

                // Implement the properties
                foreach (var property in factoryTypeInfoInterface.Properties)
                {
                    var propertyResult = componentType.CreateProperty(Modifiers.Public | Modifiers.Overrrides, property.ReturnType, property.Name, true);
                    propertyResult.BackingField.Remove();

                    switch (property.Name)
                    {
                        case "ContractName":
                            propertyResult.Getter.NewCode().Call(componentAttributeField, componentAttribute.ContractName).Return().Replace();
                            break;

                        case "CreationPolicy":
                            propertyResult.Getter.NewCode().Call(componentAttributeField, componentAttribute.Policy).Return().Replace();
                            break;

                        case "Priority":
                            propertyResult.Getter.NewCode().Call(componentAttributeField, componentAttribute.Priority).Return().Replace();
                            break;

                        case "Type":
                            propertyResult.Getter.NewCode().Load(component.Type).Return().Replace();
                            break;
                    }
                }

                componentType.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                // Also remove the component attribute
                component.Attribute.Remove();
            }

            this.LogInfo("Adding component IFactoryCache Interface");
            cauldron.AddInterface(factoryCacheInterface);
            var factoryCacheInterfaceAvatar = factoryCacheInterface.New(x => new
            {
                Components = x.GetMethod("GetComponents")
            });
            var ctorCoder = cauldron.ParameterlessContructor.NewCode();
            cauldron.CreateMethod(Modifiers.Public | Modifiers.Overrrides, factoryCacheInterfaceAvatar.Components.ReturnType, factoryCacheInterfaceAvatar.Components.Name)
                .NewCode()
                .Context(x =>
                {
                    var resultValue = x.GetReturnVariable();
                    x.Newarr(factoryTypeInfoInterface, componentTypes.Count).StoreLocal(resultValue);

                    for (int i = 0; i < componentTypes.Count; i++)
                    {
                        var field = cauldron.CreateField(Modifiers.Private, factoryTypeInfoInterface, "<FactoryType>f__" + i);
                        x.Load(resultValue);
                        x.StoreElement(factoryTypeInfoInterface, field, i);
                        // x.StoreElement(factoryTypeInfoInterface,
                        // x.NewCode().NewObj(componentTypes[i].ParameterlessContructor), i);
                        ctorCoder.Assign(field).NewObj(componentTypes[i].ParameterlessContructor);
                    }
                })
                .Return()
                .Replace();

            ctorCoder.Insert(InsertionPosition.End);
        }

        private void CreateFactoryCache(Builder builder)
        {
            this.LogInfo($"Creating Cauldron Cache");

            var cauldron = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<Cauldron>");
            cauldron.CreateConstructor().NewCode()
                .Context(x => x.Call(x.NewCode().This, builder.GetType(typeof(object)).ParameterlessContructor))
                .Return()
                .Replace();

            if (builder.IsReferenced("Cauldron.Activator") &&
                (builder.IsReferenced("Cauldron.XAML") || builder.IsReferenced("System.Xaml") || builder.IsReferenced("Windows.UI.Xaml")) &&
                (builder.TypeExists("Windows.UI.Xaml.Data.IValueConverter") || builder.TypeExists("System.Windows.Data.IValueConverter") /* Fixes #39 */))
                AddAttributeToXAMLResources(builder);

            if (builder.IsReferenced("Cauldron.Activator"))
                CreateComponentCache(builder, cauldron);

            this.ExecuteModuleAddition(builder);
            this.AddEntranceAssemblyHACK(builder);
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
                this.LogError("There is more than one 'static OnLoad(string[])' in the program.");
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
            .Where(x => x != null && x.FullName != null && !x.FullName.StartsWith("Microsoft.VisualStudio.TestPlatform") && !x.FullName.StartsWith("System."))
            .Select(x => x.MainModule.Types
                    .FirstOrDefault(y => y.IsPublic && !y.IsGenericParameter && !y.HasCustomAttributes && !y.ContainsGenericParameter && !y.FullName.Contains('`') && y.FullName.Contains('.'))
            )
            .Where(x => x != null)
            .ToArray();

        private void ImplementAnonymousTypeInterface(Builder builder)
        {
            var stopwatch = Stopwatch.StartNew();

            var cauldronCoreExtension = builder.GetType("Cauldron.Interception.Extensions");
            var createTypeMethod = cauldronCoreExtension.GetMethod("CreateType", 1).FindUsages().ToArray();
            var createdTypes = new Dictionary<string, BuilderType>();

            if (!createTypeMethod.Any())
                return;

            foreach (var item in createTypeMethod)
            {
                this.LogInfo($"Implementing anonymous to interface {item}");
                var interfaceToImplement = item.GetGenericArgument(0);

                if (interfaceToImplement == null || !interfaceToImplement.IsInterface)
                {
                    this.LogError($"{interfaceToImplement.Fullname} is not an interface.");
                    continue;
                }

                var type = item.GetPreviousInstructionObjectType();

                if (type.Fullname.GetHashCode() == "System.Object".GetHashCode() && type.Fullname == "System.Object")
                {
                    type = item.GetLastNewObjectType();

                    if (type.Fullname.GetHashCode() == "System.Object".GetHashCode() && type.Fullname == "System.Object")
                    {
                        this.LogError($"Error in CreateObject in method '{item.HostMethod}'. Unable to detect anonymous type.");
                        continue;
                    }
                }

                if (createdTypes.ContainsKey(interfaceToImplement.Fullname))
                {
                    item.Replace(CreateAssigningMethod(type, createdTypes[interfaceToImplement.Fullname], interfaceToImplement, item.HostMethod));
                    continue;
                }

                var anonymousTypeName = $"<>f__{interfaceToImplement.Name}_Cauldron_AnonymousType{counter++}";
                this.LogInfo($"- Creating new type: {type.Namespace}.{anonymousTypeName}");

                var newType = builder.CreateType(type.Namespace, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit | TypeAttributes.Serializable, anonymousTypeName);
                newType.AddInterface(interfaceToImplement);

                // Implement the methods
                foreach (var method in interfaceToImplement.Methods.Where(x => !x.Name.StartsWith("get_") && !x.Name.StartsWith("set_")))
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
                        x.Load(x.This).Call(builder.GetType(typeof(object)).ParameterlessContructor);
                    })
                    .Return()
                    .Replace();

                newType.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);

                createdTypes.Add(interfaceToImplement.Fullname, newType);

                item.Replace(CreateAssigningMethod(type, newType, interfaceToImplement, item.HostMethod));
            }
            stopwatch.Stop();
            this.LogInfo($"Implementing anonymous type to interface took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void ImplementMethodCache(Builder builder)
        {
            var methods = builder.FindMethodsByAttribute("Cauldron.Interception.CacheAttribute");

            if (!methods.Any())
                return;

            var task = builder.GetType("System.Threading.Tasks.Task`1").New(x => new
            {
                GetResult = x.GetMethod("get_Result"),
                FromResult = x.GetMethod("FromResult", 1)
            });

            foreach (var method in methods)
            {
                this.LogInfo($"Implementing Cache for method {method.Method.Name}");

                if (method.Method.ReturnType.Fullname == "System.Void")
                {
                    this.LogWarning("CacheAttribute does not support void return types");
                    continue;
                }

                var cacheField = $"<{method.Method.Name}>m__MethodCache";

                if (method.AsyncMethod == null && method.Method.ReturnType.Inherits(typeof(Task).FullName))
                    this.LogWarning($"- CacheAttribute for method {method.Method.Name} will not be implemented. Methods that returns 'Task' without async are not supported.");
                else if (method.AsyncMethod == null)
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var cache = method.Method.DeclaringType.CreateField(method.Method.Modifiers & ~Modifiers.Public | Modifiers.Private, method.Method.ReturnType, cacheField);
                            var returnVariable = x.GetReturnVariable();

                            x.Load(cache).IsNull().Then(y =>
                            {
                                // TODO - Dont create a new method
                                y.Assign(cache).Set(y.NewCode().OriginalBody()).Return();
                            })
                            .Load(cache).Return();
                        })
                        .Replace();
                else if (method.AsyncMethod != null)
                {
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var cache = method.Method.DeclaringType.CreateField(method.Method.Modifiers & ~Modifiers.Public | Modifiers.Private, taskReturnType, cacheField);

                            x.Load(cache).IsNotNull().Then(y =>
                            {
                                y.Call(task.FromResult.MakeGeneric(taskReturnType), cache).Return();
                            });
                        }).Insert(InsertionPosition.Beginning);

                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var cache = method.Method.DeclaringType.GetField(cacheField);
                            var returnVariable = x.GetReturnVariable();
                            x.Assign(cache).Set(x.NewCode().Call(returnVariable, task.GetResult.MakeGeneric(taskReturnType)));
                        }).Insert(InsertionPosition.End);
                }

                method.Attribute.Remove();
            }
        }

        private void ImplementPropertyChangedEvent(Builder builder)
        {
            var changeAwareViewModelInterface = builder.GetType("Cauldron.XAML.ViewModels.IChangeAwareViewModel")
                .New(x => new
                {
                    RaisePropertyChanged = x.GetMethod("RaisePropertyChanged", 3)
                });
            var eventHandler = builder.GetType("System.EventHandler`1")
                .New(x => new
                {
                    Invoke = x.GetMethod("Invoke", 2)
                });
            var propertyIsChangedEventArgs = builder.GetType("Cauldron.XAML.PropertyIsChangedEventArgs")
                .New(x => new
                {
                    Type = x,
                    Ctor = x.GetMethod(".ctor", 3)
                });

            // Get all viewmodels with implemented change aware interface
            var viewModels = builder.FindTypesByInterface("Cauldron.XAML.ViewModels.IChangeAwareViewModel")
                .OrderBy(x =>
                {
                    if (x.Implements("Cauldron.XAML.ViewModels.IChangeAwareViewModel", false))
                        return 0;

                    return 1;
                });

            foreach (var vm in viewModels)
            {
                if (vm.IsInterface)
                    continue;

                var changed = vm.GetField("Changed", false);
                Method method;

                if (changed != null)
                {
                    method = vm.CreateMethod(Modifiers.Protected, "<>RaisePropertyChangedEventRaise", typeof(string), typeof(object), typeof(object));
                    method.NewCode()
                            .Load(changed)
                            .IsNotNull()
                            .Then(x =>
                            {
                                x.Callvirt(eventHandler.Invoke.MakeGeneric(propertyIsChangedEventArgs.Type),
                                    x.NewCode().Load(changed), x.NewCode().NewObj(propertyIsChangedEventArgs.Ctor, x.NewCode().This, x.GetParameter(0), x.GetParameter(1), x.GetParameter(2)));
                            })
                            .Return()
                            .Replace();
                }
                else
                    method = vm.GetMethod("<>RaisePropertyChangedEventRaise", true, typeof(string), typeof(object), typeof(object));

                this.LogInfo($"Implementing RaisePropertyChanged Raise Event in '{vm.Fullname}'");
                var raisePropertyChanged = vm.GetMethod("RaisePropertyChanged", false, typeof(string), typeof(object), typeof(object));

                if (raisePropertyChanged == null)
                    continue;

                if (!raisePropertyChanged.IsAbstract && !raisePropertyChanged.HasMethodBaseCall())
                    raisePropertyChanged
                        .NewCode()
                        .Context(x => x.Call(x.NewCode().This, method, x.GetParameter(0), x.GetParameter(1), x.GetParameter(2)))
                        .Insert(InsertionPosition.Beginning);
            }
        }

        private void ImplementTimedCache(Builder builder)
        {
            var methods = builder.FindMethodsByAttribute("Cauldron.Interception.TimedCacheAttribute");

            if (!methods.Any())
                return;

            var timedCacheAttribute = builder.GetType("Cauldron.Interception.TimedCacheAttribute")
                .New(x => new
                {
                    CreateKey = x.GetMethod("CreateKey", 2),
                    HasCache = x.GetMethod("HasCache", 1),
                    SetCache = x.GetMethod("SetCache", 2),
                    GetCache = x.GetMethod("GetCache", 1)
                });

            var task = builder.GetType("System.Threading.Tasks.Task`1").New(x => new
            {
                GetResult = x.GetMethod("get_Result"),
                FromResult = x.GetMethod("FromResult", 1)
            });

            foreach (var method in methods)
            {
                this.LogInfo($"Implementing TimedCache in method {method.Method.Name}");

                if (method.Method.ReturnType.Fullname == "System.Void")
                {
                    this.LogWarning("TimedCacheAttribute does not support void return types");
                    continue;
                }

                var keyName = "<>timedcache_key";
                var timecacheVarName = "<>timedcache";

                if (method.AsyncMethod == null && method.Method.ReturnType.Inherits(typeof(Task).FullName))
                    this.LogWarning($"- TimedCacheAttribute for method {method.Method.Name} will not be implemented. Methods that returns 'Task' without async are not supported.");
                else if (method.AsyncMethod == null)
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var timedCache = x.CreateVariable(timecacheVarName, method.Attribute.Type);
                            var key = x.CreateVariable(keyName, timedCacheAttribute.CreateKey);
                            var returnVariable = x.GetReturnVariable();

                            x.Assign(timedCache).NewObj(method);

                            // Create a cache key
                            x.Call(timedCacheAttribute.CreateKey, method.Method.Fullname, x.GetParametersArray())
                                    .StoreLocal(key);

                            // check
                            x.Load(timedCache).Call(timedCacheAttribute.HasCache, key)
                                    .IsTrue().Then(y =>
                                    {
                                        y.Load(timedCache).Call(timedCacheAttribute.GetCache, key)
                                            .As(method.Method.ReturnType)
                                            .StoreLocal(returnVariable)
                                            .Return();
                                    });

                            // TODO - Dont create a new method
                            x.OriginalBody().StoreLocal(returnVariable);

                            // Set the cache
                            x.Load(timedCache).Call(timedCacheAttribute.SetCache, key, returnVariable);

                            x.Load(returnVariable).Return();
                        })
                        .Replace();
                else if (method.AsyncMethod != null)
                {
                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var timedCache = x.CreateVariable(timecacheVarName, method.Attribute.Type);
                            var cacheKey = x.CreateVariable(typeof(string));

                            x.Assign(cacheKey).Set(x.NewCode().Call(timedCacheAttribute.CreateKey, method.Method.Fullname, x.GetParametersArray()));
                            x.Assign(timedCache).NewObj(method);
                            x.Load(timedCache).Call(timedCacheAttribute.HasCache, cacheKey)
                                .IsTrue().Then(y =>
                                {
                                    y.Call(task.FromResult.MakeGeneric(taskReturnType), y.NewCode().Call(timedCache, timedCacheAttribute.GetCache, cacheKey).As(taskReturnType))
                                        .Return();
                                });
                        }).Insert(InsertionPosition.Beginning);

                    method.Method.NewCode()
                        .Context(x =>
                        {
                            var taskReturnType = method.Method.ReturnType.GetGenericArgument(0);
                            var returnVariable = x.GetReturnVariable();
                            x.LoadVariable(2).Call(timedCacheAttribute.SetCache, x.NewCode().LoadVariable(3), x.NewCode().Call(returnVariable, task.GetResult.MakeGeneric(taskReturnType)));
                        }).Insert(InsertionPosition.End);
                }

                method.Attribute.Remove();
            }
        }

        private void ImplementTypeWideMethodInterception(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            var doNotInterceptAttribute = builder.GetType("DoNotInterceptAttribute");
            var types = builder
                .FindTypesByAttributes(attributes)
                .GroupBy(x => x.Type)
                .Select(x => new
                {
                    Key = x.Key,
                    Item = x.ToArray()
                })
                .ToArray();

            foreach (var type in types)
            {
                this.LogInfo($"Implementing interceptors in type {type.Key.Fullname}");

                foreach (var method in type.Key.Methods)
                {
                    if (method.Name == ".ctor" || method.Name == ".cctor")
                        continue;

                    if (method.CustomAttributes.HasAttribute(doNotInterceptAttribute))
                    {
                        method.CustomAttributes.Remove(doNotInterceptAttribute);
                        continue;
                    }

                    for (int i = 0; i < type.Item.Length; i++)
                        method.CustomAttributes.Copy(type.Item[i].Attribute);
                }

                for (int i = 0; i < type.Item.Length; i++)
                    type.Item[i].Remove();
            }

            stopwatch.Stop();
            this.LogInfo($"Implementing class wide method interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void ImplementTypeWidePropertyInterception(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            var doNotInterceptAttribute = builder.GetType("DoNotInterceptAttribute");
            var types = builder
                .FindTypesByAttributes(attributes)
                .GroupBy(x => x.Type)
                .Select(x => new
                {
                    Key = x.Key,
                    Item = x.ToArray()
                })
                .ToArray();

            foreach (var type in types)
            {
                this.LogInfo($"Implementing interceptors in type {type.Key.Fullname}");

                foreach (var property in type.Key.Properties)
                {
                    if (!property.IsAutoProperty)
                        continue;

                    if (property.CustomAttributes.HasAttribute(doNotInterceptAttribute))
                    {
                        property.CustomAttributes.Remove(doNotInterceptAttribute);
                        continue;
                    }

                    for (int i = 0; i < type.Item.Length; i++)
                        property.CustomAttributes.Copy(type.Item[i].Attribute);
                }

                for (int i = 0; i < type.Item.Length; i++)
                    type.Item[i].Remove();
            }

            stopwatch.Stop();
            this.LogInfo($"Implementing class wide property interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void InterceptFields(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();
            var fields = builder.FindFieldsByAttributes(attributes).GroupBy(x => x.Field).ToArray();

            foreach (var field in fields)
            {
                this.LogInfo($"Implementing interceptors in fields {field.Key}");

                if (field.Key.Modifiers.HasFlag(Modifiers.Public))
                {
                    this.LogWarning($"The current version of the field interceptor only intercepts private fields. Field '{field.Key.Name}' in type '{field.Key.DeclaringType.Name}'");
                    continue;
                }

                var type = field.Key.DeclaringType;
                var usage = field.Key.FindUsages().ToArray();
                var property = type.CreateProperty(field.Key);

                property.CustomAttributes.AddCompilerGeneratedAttribute();
                property.CustomAttributes.AddDebuggerBrowsableAttribute(DebuggerBrowsableState.Never);

                foreach (var attribute in field)
                    attribute.Attribute.MoveTo(property);

                foreach (var item in usage)
                    if (item.Field.IsStatic || !item.IsBeforeBaseCall)
                        item.Replace(property);
            }

            stopwatch.Stop();
            this.LogInfo($"Implementing field interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void InterceptMethods(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            #region define Interfaces and the methods we want to invoke

            var iLockableMethodInterceptor = builder.GetType("Cauldron.Interception.ILockableMethodInterceptor")
                .New(x => new
                {
                    Lockable = true,
                    Type = x,
                    OnEnter = x.GetMethod("OnEnter", 5),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iMethodInterceptor = builder.GetType("Cauldron.Interception.IMethodInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnEnter = x.GetMethod("OnEnter", 4),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            #endregion define Interfaces and the methods we want to invoke

            var semaphoreSlim = builder.GetType(typeof(SemaphoreSlim)).New(x => new
            {
                Ctor = x.GetMethod(".ctor", true, typeof(int), typeof(int)),
                Release = x.GetMethod("Release"),
                CurrentCount = x.GetMethod("get_CurrentCount")
            });

            var asyncTaskMethodBuilder = builder.GetType("System.Runtime.CompilerServices.AsyncTaskMethodBuilder")
                .New(x => new
                {
                    GetTask = x.GetMethod("get_Task")
                });

            var asyncTaskMethodBuilderGeneric = builder.GetType("System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1")
                .New(x => new
                {
                    SetResult = x.GetMethod("SetResult", 1),
                    GetTask = x.GetMethod("get_Task")
                });

            var task = builder.GetType("System.Threading.Tasks.Task").New(x => new
            {
                GetException = x.GetMethod("get_Exception")
            });

            var methods = builder
                .FindMethodsByAttributes(attributes)
                .GroupBy(x => new { Method = x.Method, AsyncMethod = x.AsyncMethod })
                .Select(x => new
                {
                    Key = x.Key,
                    Item = x.Select(y => new
                    {
                        Interface = y.Attribute.Type.Implements(iLockableMethodInterceptor.Type.Fullname) ? iLockableMethodInterceptor : iMethodInterceptor,
                        Attribute = y
                    }).ToArray()
                }).ToArray();

            foreach (var method in methods)
            {
                this.LogInfo($"Implementing interceptors in method {method.Key}");

                var targetedMethod = method.Key.AsyncMethod == null ? method.Key.Method : method.Key.AsyncMethod;
                var attributedMethod = method.Key.Method;

                if (method.Key.AsyncMethod != null && !targetedMethod.DeclaringType.Fields.Any(x => x.Name == "<>4__this"))
                {
                    var thisField = targetedMethod.DeclaringType.CreateField(Modifiers.Public, attributedMethod.DeclaringType, "<>4__this");
                    var position = targetedMethod.AsyncMethodHelper.GetAsyncTaskMethodBuilderInitialization();
                    attributedMethod.NewCode()
                        .LoadVariable(0)
                        .Assign(thisField)
                        .Set(attributedMethod.NewCode().This)
                        .Insert(InsertionAction.After, position);
                }

                var typeInstance = method.Key.AsyncMethod == null ? (object)targetedMethod.NewCode().This : targetedMethod.DeclaringType.Fields.FirstOrDefault(x => x.Name == "<>4__this");

                var lockable = method.Item.Any(x => x.Interface.Lockable);
                var semaphoreFieldName = $"<{attributedMethod.Name}>lock_" + attributedMethod.Identification;

                if (lockable)
                    foreach (var ctor in targetedMethod.DeclaringType.GetRelevantConstructors())
                        ctor.NewCode()
                            .Assign(targetedMethod.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                            .NewObj(semaphoreSlim.Ctor, 1, 1)
                            .Insert(Cecilator.InsertionPosition.Beginning);

                targetedMethod
                .NewCode()
                    .Context(x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                        {
                            var item = method.Item[i];
                            x.Assign(x.CreateVariable("<>interceptor_" + i, item.Interface.Type)).NewObj(item.Attribute);
                            item.Attribute.Remove();
                        }
                    })
                    .Try(x =>
                    {
                        for (int i = 0; i < method.Item.Length; i++)
                        {
                            var item = method.Item[i];
                            if (item.Interface.Lockable)
                                x.LoadVariable("<>interceptor_" + i).Callvirt(item.Interface.OnEnter, x.NewCode().LoadField(semaphoreFieldName), attributedMethod.DeclaringType, typeInstance, attributedMethod, x.GetParametersArray());
                            else
                                x.LoadVariable("<>interceptor_" + i).Callvirt(item.Interface.OnEnter, attributedMethod.DeclaringType, typeInstance, attributedMethod, x.GetParametersArray());
                        }
                        x.OriginalBody();

                        // Special case for async methods
                        if (method.Key.AsyncMethod != null && method.Key.Method.ReturnType.Fullname == typeof(Task).FullName) // Task return
                        {
                            var exceptionVar = x.CreateVariable(builder.GetType("System.Exception"));

                            x.Assign(exceptionVar).Set(
                                x.NewCode().Call(method.Key.AsyncMethod.DeclaringType.GetField("<>t__builder"), asyncTaskMethodBuilder.GetTask)
                                .Call(task.GetException));

                            x.Load(exceptionVar).IsNotNull().Then(y =>
                            {
                                for (int i = 0; i < method.Item.Length; i++)
                                    y.LoadVariable("<>interceptor_" + i).Callvirt(method.Item[i].Interface.OnException, exceptionVar);
                            });
                        }
                        else if (method.Key.AsyncMethod != null) // Task<> return
                        {
                            var exceptionVar = x.CreateVariable(builder.GetType("System.Exception"));
                            var taskArgument = method.Key.Method.ReturnType.GetGenericArgument(0);

                            x.Assign(exceptionVar).Set(
                                x.NewCode().Call(method.Key.AsyncMethod.DeclaringType.GetField("<>t__builder"), asyncTaskMethodBuilderGeneric.GetTask.MakeGeneric(taskArgument))
                                .Call(task.GetException));

                            x.Load(exceptionVar).IsNotNull().Then(y =>
                            {
                                for (int i = 0; i < method.Item.Length; i++)
                                    y.LoadVariable("<>interceptor_" + i).Callvirt(method.Item[i].Interface.OnException, exceptionVar);
                            });
                        }
                    })
                    .Catch(typeof(Exception), x =>
                    {
                        if (method.Key.AsyncMethod == null)
                            for (int i = 0; i < method.Item.Length; i++)
                                x.LoadVariable("<>interceptor_" + i).Callvirt(method.Item[i].Interface.OnException, x.Exception);

                        x.Rethrow();
                    })
                    .Finally(x =>
                    {
                        if (lockable)
                            x.LoadField(semaphoreFieldName)
                            .Call(semaphoreSlim.CurrentCount)
                                .EqualTo(0)
                                    .Then(y => y.LoadField(semaphoreFieldName).Call(semaphoreSlim.Release).Pop());

                        for (int i = 0; i < method.Item.Length; i++)
                            x.LoadVariable("<>interceptor_" + i).Callvirt(method.Item[i].Interface.OnExit);
                    })
                    .EndTry()
                    .Return()
                .Replace();
            };

            stopwatch.Stop();
            this.LogInfo($"Implementing method interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void InterceptProperties(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();

            #region define Interfaces and the methods we want to invoke

            var iLockablePropertyGetterInterceptor = builder.GetType("Cauldron.Interception.ILockablePropertyGetterInterceptor")
                .New(x => new
                {
                    Lockable = true,
                    Type = x,
                    OnGet = x.GetMethod("OnGet", 3),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iLockablePropertySetterInterceptor = builder.GetType("Cauldron.Interception.ILockablePropertySetterInterceptor")
                .New(x => new
                {
                    Lockable = true,
                    Type = x,
                    OnSet = x.GetMethod("OnSet", 4),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iPropertyGetterInterceptor = builder.GetType("Cauldron.Interception.IPropertyGetterInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnGet = x.GetMethod("OnGet", 2),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            var iPropertySetterInterceptor = builder.GetType("Cauldron.Interception.IPropertySetterInterceptor")
                .New(x => new
                {
                    Lockable = false,
                    Type = x,
                    OnSet = x.GetMethod("OnSet", 3),
                    OnException = x.GetMethod("OnException", 1),
                    OnExit = x.GetMethod("OnExit")
                });

            #endregion define Interfaces and the methods we want to invoke

            var semaphoreSlim = builder.GetType(typeof(SemaphoreSlim)).New(x => new
            {
                Ctor = x.GetMethod(".ctor", true, typeof(int), typeof(int)),
                Release = x.GetMethod("Release"),
                CurrentCount = x.GetMethod("get_CurrentCount")
            });

            var propertyInterceptionInfo = builder.GetType("Cauldron.Interception.PropertyInterceptionInfo").New(x => new
            {
                Type = x,
                Ctor = x.GetMethod(".ctor", 7)
            });

            var properties = builder
                .FindPropertiesByAttributes(attributes)
                .GroupBy(x => x.Property)
                .Select(x => new
                {
                    Key = x.Key,
                    Item = x.Select(y => new
                    {
                        InterfaceSetter = y.Attribute.Type.Implements(iLockablePropertySetterInterceptor.Type.Fullname) ? iLockablePropertySetterInterceptor : y.Attribute.Type.Implements(iPropertySetterInterceptor.Type.Fullname) ? iPropertySetterInterceptor : null,
                        InterfaceGetter = y.Attribute.Type.Implements(iLockablePropertyGetterInterceptor.Type.Fullname) ? iLockablePropertyGetterInterceptor : y.Attribute.Type.Implements(iPropertyGetterInterceptor.Type.Fullname) ? iPropertyGetterInterceptor : null,
                        Attribute = y,
                        Property = y.Property
                    }).ToArray()
                })
                .Select(x => new
                {
                    Property = x.Key,
                    InterceptorInfos = x.Item,
                    HasGetterInterception = x.Item.Any(y => y.InterfaceGetter != null),
                    HasSetterInterception = x.Item.Any(y => y.InterfaceSetter != null),
                    RequiredLocking = x.Item.Any(y => (y.InterfaceGetter != null && y.InterfaceGetter.Lockable) || (y.InterfaceSetter != null && y.InterfaceSetter.Lockable))
                })
                .ToArray();

            foreach (var member in properties)
            {
                this.LogInfo($"Implementing interceptors in property {member.Property}");

                if (!member.Property.IsAutoProperty)
                {
                    this.LogWarning($"{member.Property.Name}: The current version of the property interceptor only supports auto-properties.");
                    continue;
                }

                var semaphoreFieldName = $"<{member.Property.Name}>lock_" + member.Property.Identification;

                if (member.RequiredLocking)
                {
                    if (member.Property.IsStatic)
                        (member.Property.DeclaringType.StaticConstructor ?? member.Property.DeclaringType.CreateStaticConstructor())
                            .NewCode()
                                .Assign(member.Property.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                                .NewObj(semaphoreSlim.Ctor, 1, 1)
                                .Insert(Cecilator.InsertionPosition.Beginning);
                    else
                        foreach (var ctor in member.Property.DeclaringType.GetRelevantConstructors().Where(x => !x.IsStatic))
                            ctor.NewCode()
                                .Assign(member.Property.CreateField(typeof(SemaphoreSlim), semaphoreFieldName))
                                .NewObj(semaphoreSlim.Ctor, 1, 1)
                                .Insert(Cecilator.InsertionPosition.Beginning);
                }

                var propertyField = member.Property.CreateField(propertyInterceptionInfo.Type, $"<{member.Property.Name}>p__propertyInfo");

                var actionObjectCtor = builder.Import(typeof(Action<object>).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
                var propertySetter = member.Property.DeclaringType.CreateMethod(member.Property.IsStatic ? Modifiers.PrivateStatic : Modifiers.Private, $"<{member.Property.Name}>m__setterMethod", builder.GetType(typeof(object)));

                #region Setter "Delegate"

                var setterCode = propertySetter.NewCode();
                if (!member.Property.BackingField.FieldType.IsGenericType)
                {
                    var tryDisposeMethod = builder.GetType("Cauldron.Interception.Extensions").GetMethod("TryDisposeInternal", 1);

                    if (member.Property.BackingField.FieldType.ParameterlessContructor != null && member.Property.BackingField.FieldType.ParameterlessContructor.IsPublic)
                        setterCode.Load(member.Property.BackingField).IsNull().Then(y =>
                            y.Assign(member.Property.BackingField).Set(propertySetter.NewCode()
                                .NewObj(member.Property.BackingField.FieldType.ParameterlessContructor)));

                    // Only this if the property implements idisposable
                    if (member.Property.BackingField.FieldType.Implements(typeof(IDisposable)))
                        setterCode.Call(tryDisposeMethod, member.Property.BackingField);

                    setterCode.Load(propertySetter.NewCode().GetParameter(0)).IsNull().Then(x =>
                    {
                        // Just clear if its clearable
                        if (member.Property.BackingField.FieldType.Implements(typeof(IList)))
                            x.Load(member.Property.BackingField).Callvirt(builder.GetType(typeof(IList)).GetMethod("Clear")).Return();
                        // Otherwise if the property is not a value type and nullable
                        else if (!member.Property.BackingField.FieldType.IsValueType || member.Property.BackingField.FieldType.IsNullable || member.Property.BackingField.FieldType.IsArray)
                            x.Assign(member.Property.BackingField).Set(null).Return();
                        else // otherwise... throw an exception
                            x.ThrowNew(typeof(NotSupportedException), "Value types does not accept null values.");
                    });

                    if (member.Property.BackingField.FieldType.IsArray)
                        setterCode.Load(propertySetter.NewCode().GetParameter(0)).Is(typeof(IEnumerable))
                            .Then(x => x.Assign(member.Property.BackingField).Set(propertySetter.NewCode().GetParameter(0)).Return())
                            .ThrowNew(typeof(NotSupportedException), "Value does not inherits from IEnumerable");
                    else if (member.Property.BackingField.FieldType.Implements(typeof(IList)) && member.Property.BackingField.FieldType.ParameterlessContructor != null)
                    {
                        var addRange = member.Property.BackingField.FieldType.GetMethod("AddRange", 1, false);
                        if (addRange == null)
                        {
                            var add = member.Property.BackingField.FieldType.GetMethod("Add", 1);
                            var array = setterCode.CreateVariable(member.Property.BackingField.FieldType.ChildType.MakeArray());
                            setterCode.Assign(array).Set(propertySetter.NewCode().GetParameter(0));
                            setterCode.For(array, (x, item) => x.Load(member.Property.BackingField).Callvirt(add, item));
                            if (!add.ReturnType.IsVoid)
                                setterCode.Pop();
                        }
                        else
                            setterCode.Load(member.Property.BackingField).Callvirt(addRange, propertySetter.NewCode().GetParameter(0));
                    }
                    else
                        setterCode.Assign(member.Property.BackingField).Set(propertySetter.NewCode().GetParameter(0));
                }
                else
                    setterCode.Assign(member.Property.BackingField).Set(propertySetter.NewCode().GetParameter(0));

                setterCode.Return().Replace();

                #endregion Setter "Delegate"

                #region Getter implementation

                if (member.HasGetterInterception)
                    member.Property.Getter
                        .NewCode()
                            .Context(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    x.Assign(x.CreateVariable("<>interceptor_" + i, item.InterfaceGetter.Type)).NewObj(item.Attribute);
                                    item.Attribute.Remove();
                                }

                                x.Load(propertyField).IsNull().Then(y =>
                                    y.Assign(propertyField)
                                        .NewObj(propertyInterceptionInfo.Ctor,
                                            member.Property.Getter,
                                            member.Property.Setter,
                                            member.Property.Name,
                                            member.Property.ReturnType,
                                            y.This,
                                            member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                            y.NewCode().NewObj(actionObjectCtor, propertySetter)));
                            })
                            .Try(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    if (item.InterfaceGetter.Lockable)
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceGetter.OnGet, x.NewCode().LoadField(semaphoreFieldName), propertyField, member.Property.BackingField);
                                    else
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceGetter.OnGet, propertyField, member.Property.BackingField);
                                }
                            })
                            .Catch(typeof(Exception), x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceGetter.OnException, x.Exception);

                                x.Rethrow();
                            })
                            .Finally(x =>
                            {
                                if (member.RequiredLocking)
                                    x.LoadField(semaphoreFieldName)
                                    .Call(semaphoreSlim.CurrentCount)
                                        .EqualTo(0)
                                            .Then(y => y.LoadField(semaphoreFieldName).Call(semaphoreSlim.Release).Pop());

                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceGetter.OnExit);
                            })
                            .EndTry()
                            .Load(member.Property.BackingField)
                            .Return()
                        .Replace();

                #endregion Getter implementation

                #region Setter implementation

                if (member.HasSetterInterception)
                    member.Property.Setter
                        .NewCode()
                            .Context(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    x.Assign(x.CreateVariable("<>interceptor_" + i, item.InterfaceSetter.Type)).NewObj(item.Attribute);
                                    item.Attribute.Remove();
                                }

                                x.Load(propertyField).IsNull().Then(y =>
                                    y.Assign(propertyField)
                                        .NewObj(propertyInterceptionInfo.Ctor,
                                            member.Property.Getter,
                                            member.Property.Setter,
                                            member.Property.Name,
                                            member.Property.ReturnType,
                                            y.This,
                                            member.Property.ReturnType.IsArray || member.Property.ReturnType.Implements(typeof(IEnumerable)) ? member.Property.ReturnType.ChildType : null,
                                            y.NewCode().NewObj(actionObjectCtor, propertySetter)));
                            })
                            .Try(x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                {
                                    var item = member.InterceptorInfos[i];
                                    if (item.InterfaceSetter.Lockable)
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceSetter.OnSet, x.NewCode().LoadField(semaphoreFieldName), propertyField, member.Property.BackingField, member.Property.Setter.NewCode().GetParameter(0));
                                    else
                                        x.LoadVariable("<>interceptor_" + i).Callvirt(item.InterfaceSetter.OnSet, propertyField, member.Property.BackingField, member.Property.Setter.NewCode().GetParameter(0));

                                    x.IsFalse().Then(y => y.Assign(member.Property.BackingField).Set(member.Property.Setter.NewCode().GetParameter(0)));
                                }
                            })
                            .Catch(typeof(Exception), x =>
                            {
                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceSetter.OnException, x.Exception);

                                x.Rethrow();
                            })
                            .Finally(x =>
                            {
                                if (member.RequiredLocking)
                                    x.LoadField(semaphoreFieldName)
                                    .Call(semaphoreSlim.CurrentCount)
                                        .EqualTo(0)
                                            .Then(y => y.LoadField(semaphoreFieldName).Call(semaphoreSlim.Release).Pop());

                                for (int i = 0; i < member.InterceptorInfos.Length; i++)
                                    x.LoadVariable("<>interceptor_" + i).Callvirt(member.InterceptorInfos[i].InterfaceSetter.OnExit);
                            })
                            .EndTry()
                            .Return()
                        .Replace();

                #endregion Setter implementation

                // Also remove the compilergenerated attribute
                member.Property.Getter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
                member.Property.Setter?.CustomAttributes.Remove(typeof(CompilerGeneratedAttribute));
            }

            stopwatch.Stop();
            this.LogInfo($"Implementing property interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }
    }
}