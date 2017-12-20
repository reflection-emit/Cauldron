using Cauldron.Interception.Cecilator;
using Cauldron.Interception.Fody.HelperTypes;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

            var constructorInterceptionHelperClass = new __IConstructorInterceptor(this.Builder);
            var constructorInterceptionAttributes = this.Builder.FindAttributesByInterfaces(constructorInterceptionHelperClass.Type.Fullname);

            this.ImplementAnonymousTypeInterface(this.Builder);
            this.ImplementTimedCache(this.Builder);
            // this.ImplementMethodCache(this.Builder);
            this.ImplementBamlInitializer(this.Builder);
            this.ImplementTypeWidePropertyInterception(this.Builder, propertyInterceptingAttributes);
            this.ImplementTypeWideMethodInterception(this.Builder, methodInterceptionAttributes);
            // These should be done last, because they replace methods
            this.InterceptConstructors(this.Builder, constructorInterceptionAttributes);
            this.InterceptFields(this.Builder, propertyInterceptingAttributes);
            this.InterceptMethods(this.Builder, methodInterceptionAttributes);
            this.InterceptProperties(this.Builder, propertyInterceptingAttributes);

            this.CreateFactoryCache(this.Builder);

            if (this.Builder.TypeExists("Cauldron.XAML.ApplicationBase") && this.Builder.IsReferenced("PropertyChanged"))
                this.ImplementPropertyChangedEvent(this.Builder);

            if (this.Builder.TypeExists("Cauldron.XAML.Validation.ValidatorGroup"))
                this.AddValidatorInits(this.Builder);

            // Checks
            this.CheckAsyncMthodsNomenclature(this.Builder);
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
                var addValidatorGroup = item.Key.DeclaringType.GetMethod("AddValidatorGroup", false, typeof(string));
                if (addValidatorGroup == null)
                    continue;
                var addValidatorAttribute = item.Key.DeclaringType.GetMethod("AddValidator", false, typeof(string).FullName, "Cauldron.XAML.Validation.ValidatorAttributeBase");

                this.Log($"Adding initializer for validators ({item.Validators.Length}) of property '{item.Key.Name}' in type '{item.Key.DeclaringType.Fullname}'");

                foreach (var ctors in item.Key.DeclaringType.GetRelevantConstructors())
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
            var factory = new __Factory(builder);

            // Before we start let us find all factoryextensions and add a component attribute to them
            var factoryResolverInterface = builder.GetType("Cauldron.Activator.IFactoryResolver");
            this.AddComponentAttribute(builder, builder.FindTypesByInterface(factoryResolverInterface), x => factoryResolverInterface.Fullname);
            // Also the same to all types that inherits from Factory<>
            var factoryGeneric = builder.GetType("Cauldron.Activator.Factory`1");
            this.AddComponentAttribute(builder, builder.FindTypesByBaseClass(factoryGeneric), type =>
               {
                   var factoryBase = type.BaseClasses.FirstOrDefault(x => x.Fullname.StartsWith("Cauldron.Activator.Factory"));
                   if (factoryBase == null)
                       return type.Fullname;

                   return factoryBase.GetGenericArgument(0).Fullname;
               });

            int counter = 0;
            var arrayAvatar = builder.GetType("System.Array").New(x => new
            {
                Length = x.GetMethod("get_Length")
            });
            var extensionAvatar = builder.GetType("Cauldron.ExtensionsReflection").New(x => new
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
                this.Log("Hardcoding component factory .ctor: " + component.Type.Fullname);

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
                       x.Load(Crumb.This).Call(builder.GetType(typeof(object)).Import().ParameterlessContructor.Import());
                       x.Assign(componentAttributeField).NewObj(component);
                   })
                   .Return()
                   .Replace();

                // Implement the methods
                componentType.CreateMethod(Modifiers.Public | Modifiers.Overrrides, createInstanceInterfaceMethod.ReturnType, createInstanceInterfaceMethod.Name, createInstanceInterfaceMethod.Parameters)
                    .NewCode()
                    .Context(x =>
                    {
                        var localVariable = x.GetReturnVariable();
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
                                this.Log("- " + ctors[index].Fullname);

                                var ctor = ctors[index];
                                // add a EditorBrowsable attribute
                                ctor.CustomAttributes.AddEditorBrowsableAttribute(EditorBrowsableState.Never);
                                var ctorParameters = ctor.Parameters;

                                if (ctorParameters.Length > 0)
                                {
                                    // In this case we have to find a parameterless constructor first
                                    if (component.Type.ParameterlessContructor != null && !parameterlessCtorAlreadyHandled && component.Type.ParameterlessContructor.Modifiers.HasFlag(Modifiers.Public))
                                    {
                                        x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.NewObj(component.Type.ParameterlessContructor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                        x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(component.Type.ParameterlessContructor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                        parameterlessCtorAlreadyHandled = true;
                                    }

                                    var code = x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(ctorParameters.Length);

                                    for (int i = 0; i < ctorParameters.Length; i++)
                                        code.And.Load(Crumb.GetParameter(0).UnPacked(i)).Is(ctorParameters[i]);

                                    if (ctor.Name == ".ctor")
                                        code.Then(y => y.NewObj(ctor, Crumb.GetParameter(0).UnPacked()).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                    else
                                        code.Then(y => y.Call(ctor, Crumb.GetParameter(0).UnPacked()).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                }
                                else
                                {
                                    if (ctor.Name == ".ctor")
                                    {
                                        x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.NewObj(ctor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                        x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(ctor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                    }
                                    else
                                    {
                                        x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.Call(ctor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                        x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.Call(ctor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
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
                                this.Log(LogTypes.Error, component.Type, $"The component '{component.Type.Fullname}' has no ComponentConstructor attribute or the constructor is not public");
                            else if (component.Type.ParameterlessContructor.Modifiers.HasFlag(Modifiers.Public))
                            {
                                x.Load(Crumb.GetParameter(0)).IsNull().Then(y => y.NewObj(component.Type.ParameterlessContructor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());
                                x.Load(Crumb.GetParameter(0)).Call(arrayAvatar.Length).EqualTo(0).Then(y => y.NewObj(component.Type.ParameterlessContructor).StoreLocal(localVariable).Call(factory.OnObjectCreation, localVariable, Crumb.This).Return());

                                this.Log($"The component '{component.Type.Fullname}' has no ComponentConstructor attribute. A parameterless ctor was found and will be used.");
                            }
                        }
                    })
                    .Context(x => x.Call(extensionAvatar.CreateInstance, component.Type, Crumb.GetParameter(0)).StoreLocal(x.GetReturnVariable()).Call(factory.OnObjectCreation, x.GetReturnVariable(), Crumb.This).Return())
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

            this.Log("Adding component IFactoryCache Interface");
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
            this.Log($"Creating Cauldron Cache");

            var cauldron = builder.CreateType("", TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, "<Cauldron>");
            cauldron.CreateConstructor().NewCode()
                .Context(x => x.Call(Crumb.This, builder.GetType(typeof(object)).Import().ParameterlessContructor.Import()))
                .Return()
                .Replace();

            if (this.IsActivatorReferenced && this.IsXAML)
                AddAttributeToXAMLResources(builder);

            if (builder.TypeExists("Cauldron.Activator.Factory"))
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
            var stopwatch = Stopwatch.StartNew();

            var cauldronCoreExtension = builder.GetType("Cauldron.Interception.Extensions");
            var createTypeMethod = cauldronCoreExtension.GetMethod("CreateType", 1).FindUsages().ToArray();
            var createdTypes = new Dictionary<string, BuilderType>();

            if (!createTypeMethod.Any())
                return;

            foreach (var item in createTypeMethod)
            {
                this.Log($"Implementing anonymous to interface {item}");
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
            stopwatch.Stop();
            this.Log($"Implementing anonymous type to interface took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }

        private void InterceptFields(Builder builder, IEnumerable<BuilderType> attributes)
        {
            if (!attributes.Any())
                return;

            var stopwatch = Stopwatch.StartNew();
            var fields = builder.FindFieldsByAttributes(attributes).GroupBy(x => x.Field).ToArray();

            foreach (var field in fields)
            {
                this.Log($"Implementing interceptors in fields {field.Key}");

                if (!field.Key.Modifiers.HasFlag(Modifiers.Private))
                {
                    this.Log(LogTypes.Error, field.Key.DeclaringType, $"The current version of the field interceptor only intercepts private fields. Field '{field.Key.Name}' in type '{field.Key.DeclaringType.Name}'");
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
            this.Log($"Implementing field interceptors took {stopwatch.Elapsed.TotalMilliseconds}ms");
        }
    }
}