![Cauldron Logo](https://raw.githubusercontent.com/Capgemini/Cauldron/master/cauldron2.png)

# Cauldron C# Toolkit
## Documentation
### Wiki
https://github.com/Capgemini/Cauldron/wiki
### .NET 4.6.1
https://Capgemini.github.io/Cauldron/win32/
### .NET Standard 2.0
https://Capgemini.github.io/Cauldron/netstandard/
### UWP
https://Capgemini.github.io/Cauldron/uwp/

## Required Visual Studio Extensions (For Contributors)
- [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB/releases)
- [CodeMaid](http://www.codemaid.net/)

## Nuget Packages
Assembly | Description   | NuGet
-------- | ------------- | ----------------
**Cauldron.Activator** | The activator is a simple and very fast dependency injection framework. It is based on attributes and does not require any configuration files for configuration. It also supports using static methods as component constructor. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Activator.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Activator/)
**Cauldron.Consoles** | Cauldron.Consoles is a Cauldron.Core based parameter parser which supports grouping of parameters in execution groups. It is also supports localization and has a nice parameter table :) | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Consoles.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Consoles/)
**Cauldron.Core** | Cauldron Core is the core toolkit assembly that the Cauldron Toolkit builds upon | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core/)
**Cauldron.Core.Collections** | Contains usefull collections and collection extensions. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Collections.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Collections/)
**Cauldron.Core.Comparing** | Provides usefull extensions and methods regarding comparing | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Comparing.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Comparing/)
**Cauldron.Core.Diagnostics** | Provides a set of methods that help debug code or output errors in compiled dlls. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Diagnostics.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Diagnostics/)
**Cauldron.Core.Disposable** | Implementation of IDisposable | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Disposable.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Disposable/)
**Cauldron.Core.Extensions** | Provides usefull extension methods | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Extensions.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Extensions/)
**Cauldron.Core.Extensions.Compression** | Provides System.IO.Compression.GZipStream extension methods for System.IO.Stream, System.IO.FileInfo, byte array and System.String | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Extensions.Compression.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Extensions.Compression/)
**Cauldron.Core.Extensions.Convertions** | Provides extension methods regarding common convertions. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Extensions.Convertions.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Extensions.Convertions/)
**Cauldron.Core.Extensions.IO** | Provides extensions methods for FileInfo, DirectoryInfo and other IO operations | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Extensions.IO.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Extensions.IO/)
**Cauldron.Core.Formatters** | Contains the following formatter implementations:<br/>      - ByteSizeFormatter<br/>      - MetricUnitFormatter<br/> | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Formatters.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Formatters/)
**Cauldron.Core.Interceptors** | A collection of interceptor implementations. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Interceptors.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Interceptors/)
**Cauldron.Core.MathExtensions** | Provides additional functions that are missing from System.Math | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.MathExtensions.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.MathExtensions/)
**Cauldron.Core.Net** | A collection of usefull methods regarding networks | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Net.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Net/)
**Cauldron.Core.Randomizer** | Provides a randomizer that is cryptographically secure | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Randomizer.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Randomizer/)
**Cauldron.Core.Reflection** | This package adds:<br/>      - Usefull extension methods to the existing System.Reflection.<br/>      - A expression base activator which is a lot faster than System.Activator.<br/>      - Enum DisplayName attribute<br/>      - addtional meta-data to an enum<br/>      - Assemblies static class<br/>      - A "wrapper" for Assembly but applied on all loaded Assemblies<br/> | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Reflection.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Reflection/)
**Cauldron.Core.Threading** | Provides useful helpers regarding threading | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Threading.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Threading/)
**Cauldron.Core.Yaml** | A YAML deserializer | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Core.Yaml.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Core.Yaml/)
**Cauldron.Cryptography** | Contains typical implementations for AES, RSA and RSA-AES encryptions. It also contains extensions that helps working with SecureString. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Cryptography.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Cryptography/)
**Cauldron.Interception.Fody** | Fody add-in that provides method, property, field and constructor interception. Supports Net45, NetStandard2.0 and UWP. | [![NuGet](https://img.shields.io/nuget/v/Cauldron.Interception.Fody.svg)](https://www.nuget.org/packages/Cauldron.Interception.Fody/)
**Cauldron.Localization** | A simple localization implementation that can work with different sources. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Localization.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Localization/)
**Cauldron.Win32.Extension.Impersonation** | Provides an extension for PrincipalContext to impersonate a user. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.Extension.Impersonation.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.Extension.Impersonation/)
**Cauldron.Win32.Extension.IO** | Provides extension methods with Windows specific functionalities like GetShortPath and IsNetworkPath | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.Extension.IO.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.Extension.IO/)
**Cauldron.Win32.Interceptors** | A collection of interceptor implementations. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.Interceptors.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.Interceptors/)
**Cauldron.Win32.MonitorInfo** | Provides properties and methods for getting information about the monitor | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.MonitorInfo.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.MonitorInfo/)
**Cauldron.Win32.Net** | Provides usefull implementations regarding network. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.Net.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.Net/)
**Cauldron.Win32.Threading** | A wrapper for the System.Windows.Threading.Dispatcher unify the usage for UWP and Win32. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.Threading.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.Threading/)
**Cauldron.Win32.UserInformation** | Consolidates methods for getting user information (profile picture, email address, home directory ect.) to a single class. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.UserInformation.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.UserInformation/)
**Cauldron.Win32.WindowsService** | Creating Windows Services is now a cake walk. This implementation can also handle localized service name and description. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.WindowsService.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.WindowsService/)
**Cauldron.Win32.WPF** | A simple MVVM framework that heavily uses IL-weaving based on Fody. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.WPF.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.WPF/)
**Cauldron.Win32.WPF.Interactivity** | Behaviours and Action for Cauldron.XAML | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.WPF.Interactivity.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.WPF.Interactivity/)
**Cauldron.Win32.WPF.ParameterPassing** | Handles passing of parameters to running instance(s) of an application. | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.WPF.ParameterPassing.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.WPF.ParameterPassing/)
**Cauldron.Win32.WPF.Validation** | Validation Framework for Cauldron.Win32.WPF | [![NuGet](https://img.shields.io/nuget/v/Capgemini.Cauldron.Win32.WPF.Validation.svg)](https://www.nuget.org/packages/Capgemini.Cauldron.Win32.WPF.Validation/)

## Release Notes
### __2.0.22__
#### Added
- __Cauldron.Interception.Fody:__ _Real support for Net45 added - The weaver itself still requires an installed Net461._
### __2.0.21__
#### Added
- __Capgemini.Cauldron.Core.Extensions:__ _PadOrCut extension method added. It allows to pad chars or cut the string to a specific length._
- __Capgemini.Cauldron.Core.Extensions.IO:__ _FileInfo extension - Rename added_
- __Capgemini.Cauldron.Win32.UserInformation:__ _WTSClientName added to User and CurrentUser. WTSClientName returns the Windows Terminal Session client name._
### __2.0.20__
#### Added
- __Cauldron.Interception.Fody:__ _Constructor interceptor added._
### __2.0.19__
#### Bugfix
- __Capgemini.Cauldron.Win32.WPF:__ _Several issues with the dispatcher fixed._
- __Capgemini.Cauldron.Win32.WPF:__ _Navigator was not able to find the correct view in some cases._
- __Capgemini.Cauldron.Win32.WPF:__ _ObjectToVisibilityConverter logic was inverted._
- __Capgemini.Cauldron.Win32.WPF:__ _Bugs in MessageDialog that caused the strings not to be localized correctly fixed_
- __Capgemini.Cauldron.Win32.WPF:__ _Fixes an issue where the SplashScreen can be removed from MainView and causes the application to exit_
- __Cauldron.Interception.Fody:__ _A bug was in "cleaning" fixed that caused local variables to be removed although they were still in use._
- __Cauldron.Interception.Fody:__ _Fixed a bug that caused the weaver to weav types that does not exist and then throws exceptions._
- __Cauldron.Interception.Fody:__ _Sort of fixed the issue that jumping to errors on async methods did not work._
- __Capgemini.Cauldron.Win32.WPF.Interactivity:__ _The "Localized" attached property will not continue loading if in design mode to avoid those weird errors in the WPF editor_
- __Capgemini.Cauldron.Core.Reflection:__ _A bug in Assemblies that causes the Custom assembly resolution to be invoked before all assemblies are added to the collection fixed._
- __Capgemini.Cauldron.Localization:__ _Added a Contains method to Locale._
- __Capgemini.Cauldron.Core.Threading:__ _BUG!!!! ... Dispatcher was using BeginInvoke instead of InvokeAsync_
- __Capgemini.Cauldron.Activator:__ _CreateFirst always returned null in some cases... This is fixed._
#### Change
- __Capgemini.Cauldron.Win32.WPF:__ _XAML resources loading order can now be changed by adding a dash and a numeric suffix to the file name. e.g. Style-01.xaml._
- __Capgemini.Cauldron.Core.Reflection:__ _Now Costura compatible._
#### Added
- __Cauldron.Interception.Fody:__ _Allowing non auto-properties to be intercepted._
- __Capgemini.Cauldron.Win32.WPF.Interactivity:__ _New property added to 'Localized': 'LocalizedText'_
### __2.0.18__
#### Bugfix
- __Cauldron.Interception.Fody:__ _If PropertyChanged.Fody weaved before Cauldron then Cauldron has removed the implementation for the particular property. This is now fixed._
- __Cauldron.Interception.Fody:__ _Fixed a bug that caused try-catches to be malformed if the modified method was empty._
- __Cauldron.Interception.Fody:__ _Fixed the weaver for the IChangeAwareViewModel interface._
#### Added
- __Cauldron.Interception.Fody:__ _IPropertyInitializer added. This interface forces an interceptor to be loaded on type init instead of the first call._
### __2.0.16__
#### Added
- __Capgemini.Cauldron.Win32.WPF:__ _The ViewAttribute now accepts a string as view name/type._
- __Capgemini.Cauldron.Win32.WPF:__ _OnIsLoadingChanged added to ViewModelBase._
- __Capgemini.Cauldron.Win32.WPF:__ _Message unsubsribe added to dispose of ViewModelBase._
- __Capgemini.Cauldron.Win32.WPF:__ _RegisterChildren attribute added._
- __Cauldron.Interception.Fody:__ _Sequence points added._
- __Capgemini.Cauldron.Core.Extensions.IO:__ _GetUniqueDirectoryName and GetUniqueFilename added._
#### Change
- __Capgemini.Cauldron.Win32.WPF:__ _Renamed IMessageDialog.ShowException to IMessageDialog.ShowExceptionAsync_
#### Bugfix
- __Cauldron.Interception.Fody:__ _Bug from yesterday regarding ComponentAttribute still occured on nested private classes. Now fixed._
### __2.0.13__
#### Added
- __Capgemini.Cauldron.Win32.WPF:__ _Several information from ApplicationInfo added as static resources._
- __Capgemini.Cauldron.Win32.WPF:__ _IViewAware interface added._
- __Cauldron.Interception.Fody:__ _Weaves an initializer for xaml resources to be able to auto-load them._
- __Capgemini.Cauldron.Activator:__ _CreateFirst method added to the Factory. This will auto-pick the implementation with the highest priority if multiple implementations are available._
#### Bugfix
- __Capgemini.Cauldron.Win32.WPF:__ _Now picks the correct Window implementaion._
- __Capgemini.Cauldron.Win32.WPF:__ _Bug regarding auto-selection of views in the navigator fixed._
- __Capgemini.Cauldron.Win32.WPF:__ _Bug in CauldronTemplateSelector regarding view orientation fixed._
- __Capgemini.Cauldron.Win32.WPF:__ _Fixed a bug that caused the application to crash if there are no IMultiValueConverter in the application._
- __Cauldron.Interception.Fody:__ _The weaving of the auto-ComponentAttribute attributed types caused an error in some cases if the type is not public._
- __Cauldron.Interception.Fody:__ _Interceptors that uses ISyncRoot interface was not correctly weaved. This caused the object initialization to be weaved in the .cctor even though it is not static._
- __Cauldron.Interception.Fody:__ _Useless method cache removed from weaver._
#### Change
- __Cauldron.Interception.Fody:__ _TimedCacheAttribute namespace change also reflected to weaver._
- __Capgemini.Cauldron.Win32.Interceptors:__ _TimedCacheAttribute namespace changed to Cauldron.Core.Interceptors._

### Old Release Notes
### 2.0.0 betas
- NetCore and Desktop libraries combined to NetStandard 2.0
- Cauldron.XAML renamed to Cauldron.Win32.WPF
- UWP libraries are combined together to a single library
- Win32 specific libraries has now the following nomenclature: Cauldron.Win32.*
- DispatcherEx is now injectable. It uses the new UnitTest aware Factory resolver to inject either the dummy dispatcher or the real deal. 
- ComparerUtils renamed to Comparer
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
