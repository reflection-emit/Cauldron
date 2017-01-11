# Win32Api.RegisterWindowMessage Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used when sending or posting messages.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static uint RegisterWindowMessage(
	string message
)
```


#### Parameters
&nbsp;<dl><dt>message</dt><dd>Type: System.String<br />The message to be registered.</dd></dl>

#### Return Value
Type: UInt32<br />Returns a message identifier in the range 0xC000 through 0xFFFF

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>Win32Exception</td><td>Win32 error occures</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Win32Api">Win32Api Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />