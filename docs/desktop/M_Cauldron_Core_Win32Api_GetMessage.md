# Win32Api.GetMessage Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Retrieves the string sent by <a href="M_Cauldron_Core_Win32Api_SendMessage_1">SendMessage(IntPtr, String)</a>.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string GetMessage(
	int message,
	IntPtr lParam
)
```


#### Parameters
&nbsp;<dl><dt>message</dt><dd>Type: System.Int32<br />The message id of the message</dd><dt>lParam</dt><dd>Type: System.IntPtr<br />The lParameter from the message</dd></dl>

#### Return Value
Type: String<br />Returns null if *message* is not the correct message id

## See Also


#### Reference
<a href="T_Cauldron_Core_Win32Api">Win32Api Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />