# Win32Api.ActivateWindow Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Brings the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool ActivateWindow(
	IntPtr hwnd
)
```


#### Parameters
&nbsp;<dl><dt>hwnd</dt><dd>Type: System.IntPtr<br />A handle to the window that should be activated and brought to the foreground.</dd></dl>

#### Return Value
Type: Boolean<br />Returns true if successful; otherwise false

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>*hwnd* is Zero</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Win32Api">Win32Api Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />