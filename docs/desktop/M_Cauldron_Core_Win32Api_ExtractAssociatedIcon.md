# Win32Api.ExtractAssociatedIcon Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Extracts an icon from a exe, dll or ico file.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static byte[] ExtractAssociatedIcon(
	string filename,
	int iconIndex = 0
)
```


#### Parameters
&nbsp;<dl><dt>filename</dt><dd>Type: System.String<br />The filename of the resource</dd><dt>iconIndex (Optional)</dt><dd>Type: System.Int32<br />THe index of the icon</dd></dl>

#### Return Value
Type: Byte[]<br />The icon represented by a byte array

## See Also


#### Reference
<a href="T_Cauldron_Core_Win32Api">Win32Api Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />