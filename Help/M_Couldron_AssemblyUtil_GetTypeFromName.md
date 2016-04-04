# AssemblyUtil.GetTypeFromName Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Tries to find/identify a Type by its name

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

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
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>The *typeName* parameter is null</td></tr><tr><td>ArgumentException</td><td>The *typeName* parameter is an empty string</td></tr><tr><td>FileNotFoundException</td><td>*typeName* was not found.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_AssemblyUtil">AssemblyUtil Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />