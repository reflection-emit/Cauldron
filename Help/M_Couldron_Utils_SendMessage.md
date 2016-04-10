# Utils.SendMessage Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void SendMessage(
	Window window,
	WindowsMessages windowMessage,
	IntPtr wParam,
	IntPtr lParam
)
```


#### Parameters
&nbsp;<dl><dt>window</dt><dd>Type: System.Windows.Window<br />The window whose window procedure will receive the message.</dd><dt>windowMessage</dt><dd>Type: <a href="T_Couldron_WindowsMessages">Couldron.WindowsMessages</a><br />The message to be sent.</dd><dt>wParam</dt><dd>Type: System.IntPtr<br />Additional message-specific information.</dd><dt>lParam</dt><dd>Type: System.IntPtr<br />Additional message-specific information.</dd></dl>

## See Also


#### Reference
<a href="T_Couldron_Utils">Utils Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />