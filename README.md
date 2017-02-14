![Cauldron Logo](https://raw.githubusercontent.com/reflection-emit/Cauldron/master/cauldron2.png)

# Cauldron C# Toolkit
Assembly | Description
-------- | -------------
**Cauldron.Interception.Fody** | Fody add-in that provides method, property and field interception. It also provides weavers for Cauldron.Core and Cauldron.Activator.
**Cauldron.Core** | Cauldron Core is the core toolkit assembly that the Cauldron Toolkit builds upon.
**Cauldron.Activator** | The activator is a simple and extensible dependency injection framework. It is based on attributes and does not require any configuration files for configuration. It also supports using static methods as component constructor.
**Cauldron.Consoles** | Cauldron.Consoles is a Cauldron.Core based parameter parser which supports grouping of parameters in execution groups. It is also supports localization and has a nice parameter table.
**Cauldron.Cryptography** | Contains typical implementations for AES, RSA and RSA-AES encryptions. It also contains extensions that helps working with SecureString.
**Cauldron.Localization** | A simple localization implementation that can work with different sources.
**Cauldron.XAML** | A Simple MVVM framework based on Cauldron.Core.
**Cauldron.XAML.Interactivity** | Behaviours and Action for Cauldron.XAML. (incomplete)
**Cauldron.XAML.Validation** | Validation Framework for Cauldron.XAML.

## Documentation
### Wiki
https://github.com/reflection-emit/Cauldron/wiki
### .NET Desktop
https://reflection-emit.github.io/Cauldron/desktop/
### UWP
https://reflection-emit.github.io/Cauldron/uwp/
### Eve Online API for .NET 4.5.2 and UWP
https://reflection-emit.github.io/Cauldron/eveonline/
## Required Visual Studio Extensions
- [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB/releases)
- [CodeMaid](http://www.codemaid.net/)

## Release Notes
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
