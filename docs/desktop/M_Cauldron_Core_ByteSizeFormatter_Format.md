# ByteSizeFormatter.Format Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public string Format(
	string format,
	Object arg,
	IFormatProvider formatProvider
)
```


#### Parameters
&nbsp;<dl><dt>format</dt><dd>Type: System.String<br />A format string containing formatting specifications.</dd><dt>arg</dt><dd>Type: System.Object<br />An object to format.</dd><dt>formatProvider</dt><dd>Type: System.IFormatProvider<br />An object that supplies format information about the current instance.</dd></dl>

#### Return Value
Type: String<br />The string representation of the value of arg, formatted as specified by format and formatProvider.

#### Implements
ICustomFormatter.Format(String, Object, IFormatProvider)<br />

## See Also


#### Reference
<a href="T_Cauldron_Core_ByteSizeFormatter">ByteSizeFormatter Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />