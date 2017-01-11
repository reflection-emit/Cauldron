# ByteSizeFormatter.GetFormat Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns an object that provides formatting services for the specified type.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public Object GetFormat(
	Type formatType
)
```


#### Parameters
&nbsp;<dl><dt>formatType</dt><dd>Type: System.Type<br />An object that specifies the type of format object to return.</dd></dl>

#### Return Value
Type: Object<br />An instance of the object specified by formatType, if the IFormatProvider implementation can supply that type of object; otherwise, null.

#### Implements
IFormatProvider.GetFormat(Type)<br />

## See Also


#### Reference
<a href="T_Cauldron_Core_ByteSizeFormatter">ByteSizeFormatter Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />