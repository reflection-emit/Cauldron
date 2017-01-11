# MonitorInfo.GetMonitorBounds Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets the bounds of the monitor that contains the defined window

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Nullable<Rect> GetMonitorBounds(
	IntPtr windowHandle
)
```


#### Parameters
&nbsp;<dl><dt>windowHandle</dt><dd>Type: System.IntPtr<br />The handle of the window displayed by the monitor</dd></dl>

#### Return Value
Type: Nullable(Rect)<br />Returns the bounds of the monitor. Returns null if window is not on any monitor.

## See Also


#### Reference
<a href="T_Cauldron_Core_MonitorInfo">MonitorInfo Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />