![Cauldron Logo](https://raw.githubusercontent.com/Capgemini/Cauldron/master/cauldron2.png)

# Cauldron C# Toolkit
Assembly | Description   | NuGet
-------- | ------------- | ----------------
**Cauldron.Interception.Fody** | Fody add-in that provides method, property and field interception. It also provides weavers for Cauldron.Core and Cauldron.Activator._<br/>Available for .Net452, UWP and NetStandard1.6_ | [![NuGet](https://img.shields.io/nuget/v/Cauldron.Interception.Fody.svg)](https://www.nuget.org/packages/Cauldron.Interception.Fody/)
**Cauldron.Core** | Cauldron Core is the core toolkit assembly that the Cauldron Toolkit builds upon._<br/>Available for .Net452, UWP and NetStandard1.6_ | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core/)
**Cauldron.Activator** | The activator is a simple and very fast dependency injection. It is based on attributes and does not require any configuration files for configuration. It also supports using static methods as component constructor._<br/>Available for .Net452, UWP and NetStandard1.6_ | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Activator.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Activator/)
**Cauldron.Consoles** | Cauldron.Consoles is a Cauldron.Core based parameter parser which supports grouping of parameters in execution groups. It also supports localization and has a nice parameter table. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Consoles.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Consoles/)
**Cauldron.Cryptography** | Contains typical implementations for AES, RSA and RSA-AES encryptions. It also contains extensions that helps working with SecureString. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Cryptography.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Cryptography/)
**Cauldron.Localization** | A simple localization implementation that can work with different sources. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Localization.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Localization/)
**Cauldron.XAML** | A simple MVVM framework based on Cauldron.Core. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.XAML.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.XAML/)
**Cauldron.XAML.Interactivity** | Behaviours and Action for Cauldron.XAML. (incomplete) | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.XAML.Interactivity.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.XAML.Interactivity/)
**Cauldron.XAML.Validation** | Validation Framework for Cauldron.XAML. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.XAML.Validation.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.XAML.Validation/)

## Documentation
### Wiki
https://github.com/Capgemini/Cauldron/wiki
### .NET Desktop
https://Capgemini.github.io/Cauldron/desktop/
### UWP
https://Capgemini.github.io/Cauldron/uwp/
### Eve Online API for .NET 4.5.2 and UWP
https://Capgemini.github.io/Cauldron/eveonline/
## Required Visual Studio Extensions (For Contributors)
- [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB/releases)
- [CodeMaid](http://www.codemaid.net/)

## Release Notes
### 1.2.8
- Breaking changes: Reorganisation of the XAML libraries
- Skinning added (Skins from first version of Cauldron re-added)
- The App class (which inherits from ApplicationBase) is now recognized as Splashscreen if it has assigned a view. The OnPreload method is only called if a view is assigned if the navigation-mode is not SinglePage.
- Bug fixes in Interception.Fody
  - Properties as constructors now works as expected
  - ComponentConstructor attributes are now no longer removed after weaving
- Locale class can now be injected without causing an error during Assembly verification
- Singleton<> base class component construtor attribute removed, because it is redundant and causes errors.
- NetCore versions of Cauldron.Interception, Cauldron.Core and Cauldron.Activator are now part of the NuGet package again.
- Password scoring now works a bit better
- Inline text for TextBlock fixed
- XAML.Validation is now awaitable
- Module OnLoad "interceptor" added
- Bug fixes - see Incidents
### 1.2.7 (1.2.0 to 1.2.6 betas)
- NetCore Dlls droped from package due to issues with Fody - This will be back as soon as NetCore and Fody works a lot better
- Types with the component attribute get a hard-coded CreateInstance method. The Factory will use this method to create an instance of the type. This should give the factory an instancing performance almost on par with the __new__ keyword.
- Types that inherits or implements the following classes or interfaces are considered as components and will also recieve a CreateInstance method: ResourceDictionary, IValueConverter, INotifyPropertyChanged, FrameworkElement
- ComponentConstructor Attribute
  - Now also accepts static Properties as Component constructor
  - New Property added: Priority; This is used by the Factory to order the result of CreateMany. 0 is lowest; uint.Max is highest.
- Breaking changes in Assemblies class. Some methods and properties were removed without replacement.
- Method and Property interceptor can now be used to intercept all methods and properties in a class. Excluded method can be attributed by the DoNotIntercept attribute.
- CreateObject<> renamed to CreateType, which fits more to what it does.
- Locale class redesigned for more performance
- Localization source implementations now requires a component attribute
- Basic implementation base classes added for ILocalizationSource
- XML serialization/deserialization in Serializer class replaced by much faster JSON.NET
- Breaking changes in Factory - The performance of the Factory was boosted with the following drawbacks
  - IFactoryInitializeComponent interface removed - Factory does not support this anymore
  - IFactoryExtension removed - The Factory is not extensible anymore :( ... To Resolve ambigousity the IFactoryResolver can be use instead.
  - This is the current performance of the factory <br/>![Factory performance](https://raw.githubusercontent.com/Capgemini/Cauldron/feature/images/factory-performance.PNG)
- Upgraded to the newest version of Fody
- Minor bug fixes
### 1.1.4
- Inject attribute default paramaters are now "params"
- Assemblies class now throws a better error message if an Assembly cannot be loaded
- Better error message in Inject attribute
- Errors in Cauldron.Interception.Fody copy method fixed [1](https://github.com/Capgemini/Cauldron/commit/8206509d7956cc0e47c38c12249c4b68e29cb162) [2](https://github.com/Capgemini/Cauldron/commit/a1fb4a03c43689388c22e58dd2555c4a79c9170c)
- TimedCache attribute key generation fixed.
bBlock now works- Bug fix for anonymous type to interface convertion
### 1.1.1
- Several minor bug fixes
- TimedCacheAttribute now supports async methods
- Unused variables are now removed from the method's local varible list
### 1.1.0
- Cauldron.Interception is now using Cecilator
- MethodOf, FieldOf, ChildOf removed
- New Interceptor added: TimedCacheAttribute - Caches Methods using MemoryCache
- Several Bug fixes - See issues section
### 1.0.8
- CreateObject moved to Cauldron.Core
- Performance boost to CreateInstance
- IEquatable<> interface added to User class
- Minor bugs fixed
- Bug fixed that caused Cauldron.Interception.dll to be referenced with copy local set to false.

### 1.0.7
- Bug fix in Cauldron.Interception.Fody regarding nested classes and generic classes and methods
- References of the Nuget packages updated

### 1.0.6
- Inject attribute from Cauldron.Injection moved to Cauldron.Activator
  - InjectAttribute is now based on Cauldron.Interception
- Cauldron property interceptors setters can deal with IEnumerables if target property implements the IEnumerable<> interface
- Experimental ChildTypeOf method added.
- Cauldron.Activator has now an extension that can create types from interfaces.
  - CreateObject extension removed from Cauldron.Dynamic
- Cauldron.Injection removed
- Fody add-in weaver bugs fixed
- Nuget packages fixed

### 1.0.5
- Reference to Fody
- Method, fields and property interceptor added
  - Try Catch Finally implementation
  - Method, property and field interceptors with SemaphoreSlim implementation
  - methodof and fieldof implementations in Cauldron.Core.Reflection
- Cauldron.IEnumerableExtensions removed

### 1.0.4
- .NET Standard 1.6 added to NuGet package
- Missing resources in UWP packages added

### 1.0.3
- Behaviour of As<> Extension changed. It will use implicit and explicit operators if casting did not work.
- string Replace(string,char[],char) extension method added.
- Examples added to the following methods
  - ExtensionsDirectoryServices.Impersonate
  - ConsoleUtils.WriteTable
  - AsyncHelper.NullGuard
- Extensions.IsDerivedFrom<T> removed
- Extension.LowerFirstCharacter optimized
- Cauldron.XAML.Interactivity.TextBoxHeader removed
- Reference to Cauldron.UWP.XAML.Potions in Cauldron.UWP.XAML removed
- Several minor bug fixes

### 1.0.2
- ByteSizeFormatter moved to Cauldron.Core.Formatters
- MetricUnitFormatter added -> key is metric -> .ToStringEx("metric") or "{0:metric}"
- ByteSizeFormatter key changed from B to byte -> .ToStringEx("byte") or "{0:byte}"
- ToStringEx extension method added
- Java property file reader / writer added
- NavigationFrame now always retrieve the View in the following order (on UWP and Desktop)
  - Defined in ViewAttribute
    - Variants such as Mobile, Desktop, Xbox, Iot, Landscape, Portrait
  - DataTemplate
    - Variants such as Mobile, Desktop, Xbox, Iot, Landscape, Portrait
  - Type (View Type) By Name
- New method added in ApplicationBase to be able to load additional assemblies before initializing XAML / WPF
- Several minor fixes