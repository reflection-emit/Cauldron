# Utils.GetStringFromModule Method (UInt32)
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets a string from user32.dll text resources.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static string GetStringFromModule(
	uint index
)
```


#### Parameters
&nbsp;<dl><dt>index</dt><dd>Type: System.UInt32<br />The id of the text resource.</dd></dl>

#### Return Value
Type: String<br />The text resource string from user32.dll defined by *index*. Returns null if not found

## See Also


#### Reference
<a href="T_Couldron_Utils">Utils Class</a><br /><a href="Overload_Couldron_Utils_GetStringFromModule">GetStringFromModule Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />