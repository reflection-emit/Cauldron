# MonitorInfo.WmGetMinMaxInfo Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void WmGetMinMaxInfo(
	IntPtr windowHandle,
	IntPtr lParam
)
```


#### Parameters
&nbsp;<dl><dt>windowHandle</dt><dd>Type: System.IntPtr<br />The handle of the window displayed by the monitor</dd><dt>lParam</dt><dd>Type: System.IntPtr<br />Additional message-specific information</dd></dl>

## See Also


#### Reference
<a href="T_Cauldron_Core_MonitorInfo">MonitorInfo Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />