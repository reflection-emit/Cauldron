# AssemblyUtil Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Contains utilities that helps to manage and gather Assembly information

Contains utilities that helps to manage and gather Assembly information


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Couldron.AssemblyUtil<br />
**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static class AssemblyUtil
```

The AssemblyUtil type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Couldron_AssemblyUtil_Assemblies">Assemblies</a></td><td>
Gets a collection of Assembly that is loaded to the AssemblyUtil</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Couldron_AssemblyUtil_AssemblyAndResourceNamesInfo">AssemblyAndResourceNamesInfo</a></td><td>
Gets a collection of <a href="T_Couldron_AssemblyAndResourceNameInfo">AssemblyAndResourceNameInfo</a> that contains all fully qualified filename of embedded resources and thier corresponding Assembly</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Couldron_AssemblyUtil_Classes">Classes</a></td><td>
Gets a collection of classes loaded to the AssemblyUtil</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Couldron_AssemblyUtil_ExportedTypes">ExportedTypes</a></td><td>
Gets a collection of exported types found in the loaded Assembly</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Couldron_AssemblyUtil_Interfaces">Interfaces</a></td><td>
Gets a colleciton of Interfaces found in the loaded Assembly</td></tr></table>&nbsp;
<a href="#assemblyutil-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_AssemblyUtil_GetFirstAssemblyWithResourceName">GetFirstAssemblyWithResourceName</a></td><td>
Returns the first found Assembly that contains an embedded resource with the given resource name</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_AssemblyUtil_GetManifestResourceInfo">GetManifestResourceInfo</a></td><td>
Returns information about how the given resource has been persisted.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_AssemblyUtil_GetManifestResources">GetManifestResources</a></td><td>
Returns all information about how the given resource has been persisted.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_AssemblyUtil_GetManifestResourceStream">GetManifestResourceStream</a></td><td>
Loads the specified manifest resource from this assembly.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_AssemblyUtil_GetTypeFromName">GetTypeFromName</a></td><td>
Tries to find/identify a Type by its name</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_AssemblyUtil_GetTypesImplementsInterface__1">GetTypesImplementsInterface(T)</a></td><td>
Returns all Types that implements the interface *T*</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Couldron_AssemblyUtil_LoadAssembly">LoadAssembly</a></td><td>
Loads the contents of an assembly file on the specified path.</td></tr></table>&nbsp;
<a href="#assemblyutil-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron">Couldron Namespace</a><br />