# MonitorInfo Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Provides properties and methods for getting information about the monitor


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Cauldron.Core.MonitorInfo<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static class MonitorInfo
```

The MonitorInfo type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_MonitorInfo_MonitorCount">MonitorCount</a></td><td>
Gets the count of the monitor connected to the device</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_MonitorInfo_PrimaryMonitorBounds">PrimaryMonitorBounds</a></td><td>
Gets the bounds of the primary monitor</td></tr></table>&nbsp;
<a href="#monitorinfo-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MonitorInfo_GetCurrentOrientation">GetCurrentOrientation</a></td><td>
Returns the orientation of the current view</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MonitorInfo_GetMonitorBounds">GetMonitorBounds</a></td><td>
Gets the bounds of the monitor that contains the defined window</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MonitorInfo_WindowIsInAnyMonitor">WindowIsInAnyMonitor</a></td><td>
Determines if the window is shown in any of the monitors.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MonitorInfo_WmGetMinMaxInfo">WmGetMinMaxInfo</a></td><td>
Sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.</td></tr></table>&nbsp;
<a href="#monitorinfo-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />