- # Cauldron C# Toolkit

## Documentation
### .NET Desktop
https://reflection-emit.github.io/Cauldron/desktop/
### UWP
https://reflection-emit.github.io/Cauldron/uwp/
### Eve Online API for .NET 4.5.2 and UWP
https://reflection-emit.github.io/Cauldron/eveonline/
## Required Visual Studio Extensions
- [NuGet Package Project](https://marketplace.visualstudio.com/items?itemName=NuProjTeam.NuGetPackageProject)
- [Sandcastle Help File Builder](https://github.com/EWSoftware/SHFB/releases)
- [CodeMaid](http://www.codemaid.net/)

## Release Notes
### Assembly Version 1.0.0.9 - NuGet 1.0.2
- ByteSizeFormatter moved to Cauldron.Core.Formatters
- MetricUnitFormatter added -> key is metric -> .ToStringEx("metric") or "{0:metric}"
- ByteSizeFormatter key changed from B to byte -> .ToStringEx("byte") or "{0:byte}"
- ToStringEx extension method added
- Java property file reader / writer added
