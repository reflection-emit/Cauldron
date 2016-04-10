# Utils.GetStringFromModule Method (String, UInt32)
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets a string from the dll defined by *moduleName* text resources.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static string GetStringFromModule(
	string moduleName,
	uint index
)
```


#### Parameters
&nbsp;<dl><dt>moduleName</dt><dd>Type: System.String<br />The dll to retrieve the string resources from. e.g. user32.dll, shell32.dll</dd><dt>index</dt><dd>Type: System.UInt32<br />The id of the text resource.</dd></dl>

#### Return Value
Type: String<br />The text resource string from user32.dll defined by *index*. Returns null if not found

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>Parameter *moduleName* is empty</td></tr><tr><td>ArgumentNullException</td><td>Parameter *moduleName* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Utils">Utils Class</a><br /><a href="Overload_Couldron_Utils_GetStringFromModule">GetStringFromModule Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />