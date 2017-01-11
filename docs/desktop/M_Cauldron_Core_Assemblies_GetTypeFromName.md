# Assemblies.GetTypeFromName Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Tries to find/identify a Type by its name

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Type GetTypeFromName(
	string typeName
)
```


#### Parameters
&nbsp;<dl><dt>typeName</dt><dd>Type: System.String<br />The name of the Type</dd></dl>

#### Return Value
Type: Type<br />The Type that is defined by the parameter *typeName*

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>The *typeName* parameter is null</td></tr><tr><td>ArgumentException</td><td>The *typeName* parameter is an empty string</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Assemblies">Assemblies Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />