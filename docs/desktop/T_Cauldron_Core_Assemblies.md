# Assemblies Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Contains methods and properties that helps to manage and gather Assembly information


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Cauldron.Core.Assemblies<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static class Assemblies
```

The Assemblies type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_Assemblies_AssemblyAndResourceNamesInfo">AssemblyAndResourceNamesInfo</a></td><td>
Gets a collection of <a href="T_Cauldron_Core_AssemblyAndResourceNameInfo">AssemblyAndResourceNameInfo</a> that contains all fully qualified filename of embedded resources and thier corresponding Assembly</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_Assemblies_Classes">Classes</a></td><td>
Gets a collection of classes loaded to the Assemblies</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_Assemblies_ExportedTypes">ExportedTypes</a></td><td>
Gets a collection of exported types found in the loaded Assembly</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_Assemblies_Interfaces">Interfaces</a></td><td>
Gets a colleciton of Interfaces found in the loaded Assembly</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_Assemblies_IsDebugging">IsDebugging</a></td><td>
Gets a value that determines if the GetEntryAssembly() or GetCallingAssembly() is in debug mode</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_Assemblies_Known">Known</a></td><td>
Gets a collection of Assembly that is loaded to the Assemblies</td></tr></table>&nbsp;
<a href="#assemblies-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_GetFirstAssemblyWithResourceName">GetFirstAssemblyWithResourceName</a></td><td>
Returns the first found Assembly that contains an embedded resource with the given resource name</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_GetManifestResource_1">GetManifestResource(String)</a></td><td>
Loads the specified manifest resource from this assembly.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_GetManifestResource">GetManifestResource(Func(AssemblyAndResourceNameInfo, Boolean))</a></td><td>
Loads the specified manifest resource from this assembly.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_GetManifestResourceInfo">GetManifestResourceInfo</a></td><td>
Returns information about how the given resource has been persisted.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_GetManifestResources">GetManifestResources</a></td><td>
Returns all information about how the given resource has been persisted.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_GetTypeFromName">GetTypeFromName</a></td><td>
Tries to find/identify a Type by its name</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_GetTypesImplementsInterface__1">GetTypesImplementsInterface(T)</a></td><td>
Returns all Types that implements the interface *T*</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_LoadAssembly_1">LoadAssembly(FileInfo)</a></td><td>
Loads the contents of an assembly file on the specified path.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Assemblies_LoadAssembly">LoadAssembly(DirectoryInfo, String)</a></td><td>
Loads the contents of all assemblies that matches the specified filter</td></tr></table>&nbsp;
<a href="#assemblies-class">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")![Static member](media/static.gif "Static member")</td><td><a href="E_Cauldron_Core_Assemblies_LoadedAssemblyChanged">LoadedAssemblyChanged</a></td><td>
Occures if the _assemblies has changed</td></tr></table>&nbsp;
<a href="#assemblies-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />