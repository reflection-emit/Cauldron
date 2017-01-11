# Assemblies.GetFirstAssemblyWithResourceName Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns the first found Assembly that contains an embedded resource with the given resource name

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Assembly GetFirstAssemblyWithResourceName(
	string resourceInfoName
)
```


#### Parameters
&nbsp;<dl><dt>resourceInfoName</dt><dd>Type: System.String<br />The end of the fully qualified name of the embedded resource</dd></dl>

#### Return Value
Type: Assembly<br />The first found Assembly otherwise returns null

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*resourceInfoName* is null</td></tr><tr><td>ArgumentException</td><td>The *resourceInfoName* parameter is an empty string ("").</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Assemblies">Assemblies Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />