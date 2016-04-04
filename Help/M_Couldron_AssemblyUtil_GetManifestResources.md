# AssemblyUtil.GetManifestResources Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns all information about how the given resource has been persisted.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static List<AssemblyAndResourceNameInfo> GetManifestResources(
	string resourceInfoName
)
```


#### Parameters
&nbsp;<dl><dt>resourceInfoName</dt><dd>Type: System.String<br />The end of the fully qualified name of the embedded resource</dd></dl>

#### Return Value
Type: List(<a href="T_Couldron_AssemblyAndResourceNameInfo">AssemblyAndResourceNameInfo</a>)<br />An List(T) object that is populated with information about the resource's topology

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*resourceInfoName* is null.</td></tr><tr><td>ArgumentException</td><td>The *resourceInfoName* parameter is an empty string ("").</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_AssemblyUtil">AssemblyUtil Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />