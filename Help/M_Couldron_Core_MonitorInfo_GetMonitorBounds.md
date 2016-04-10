# MonitorInfo.GetMonitorBounds Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets the bounds of the monitor that contains the defined window

**Namespace:**&nbsp;<a href="N_Couldron_Core">Couldron.Core</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static Nullable<Rect> GetMonitorBounds(
	Window window
)
```


#### Parameters
&nbsp;<dl><dt>window</dt><dd>Type: System.Windows.Window<br />The window displayed by the monitor</dd></dl>

#### Return Value
Type: Nullable(Rect)<br />Returns the bounds of the monitor. Returns null if window is not on any monitor.

## See Also


#### Reference
<a href="T_Couldron_Core_MonitorInfo">MonitorInfo Class</a><br /><a href="N_Couldron_Core">Couldron.Core Namespace</a><br />