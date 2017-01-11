# Win32Api.SendMessage Method (IntPtr, WindowsMessages, IntPtr, IntPtr)
 _**\[This is preliminary documentation and is subject to change.\]**_

Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void SendMessage(
	IntPtr hwnd,
	WindowsMessages windowMessage,
	IntPtr wParam,
	IntPtr lParam
)
```


#### Parameters
&nbsp;<dl><dt>hwnd</dt><dd>Type: System.IntPtr<br />The window handle of the sending window</dd><dt>windowMessage</dt><dd>Type: <a href="T_Cauldron_Core_WindowsMessages">Cauldron.Core.WindowsMessages</a><br />The message to be sent.</dd><dt>wParam</dt><dd>Type: System.IntPtr<br />Additional message-specific information.</dd><dt>lParam</dt><dd>Type: System.IntPtr<br />Additional message-specific information.</dd></dl>

## See Also


#### Reference
<a href="T_Cauldron_Core_Win32Api">Win32Api Class</a><br /><a href="Overload_Cauldron_Core_Win32Api_SendMessage">SendMessage Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />