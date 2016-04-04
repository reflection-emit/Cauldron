# Utils.WmGetMinMaxInfo Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void WmGetMinMaxInfo(
	Window window,
	IntPtr lParam
)
```


#### Parameters
&nbsp;<dl><dt>window</dt><dd>Type: System.Windows.Window<br />The window to get the monitor info from</dd><dt>lParam</dt><dd>Type: System.IntPtr<br />Additional message-specific information</dd></dl>

## See Also


#### Reference
<a href="T_Couldron_Utils">Utils Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />