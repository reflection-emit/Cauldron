# Win32Api.SendMessage Method (IntPtr, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Sends the specified message string to a window. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void SendMessage(
	IntPtr hwnd,
	string message
)
```


#### Parameters
&nbsp;<dl><dt>hwnd</dt><dd>Type: System.IntPtr<br />A handle to the window whose window procedure will receive the message.</dd><dt>message</dt><dd>Type: System.String<br />The message to be sent to the window</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>Win32Exception</td><td>Win32 error occures</td></tr><tr><td>ArgumentException</td><td>HWND is Zero</td></tr><tr><td>ArgumentNullException</td><td>*message* is null</td></tr><tr><td>ArgumentException</td><td>*message* is empty</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Win32Api">Win32Api Class</a><br /><a href="Overload_Cauldron_Core_Win32Api_SendMessage">SendMessage Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />