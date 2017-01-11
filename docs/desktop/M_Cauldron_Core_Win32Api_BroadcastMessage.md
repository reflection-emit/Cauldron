# Win32Api.BroadcastMessage Method (UInt32)
 _**\[This is preliminary documentation and is subject to change.\]**_

The message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void BroadcastMessage(
	uint registeredWindowMessage
)
```


#### Parameters
&nbsp;<dl><dt>registeredWindowMessage</dt><dd>Type: System.UInt32<br />The registered window message. use <a href="M_Cauldron_Core_Win32Api_RegisterWindowMessage">RegisterWindowMessage(String)</a> to register a message.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>Invalid registered window message</td></tr><tr><td>Win32Exception</td><td>Win32 error occures</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Win32Api">Win32Api Class</a><br /><a href="Overload_Cauldron_Core_Win32Api_BroadcastMessage">BroadcastMessage Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />