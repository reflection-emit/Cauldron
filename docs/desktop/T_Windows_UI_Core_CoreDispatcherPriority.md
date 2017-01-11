# CoreDispatcherPriority Enumeration
 _**\[This is preliminary documentation and is subject to change.\]**_

Describes the priorities at which operations can be invoked

**Namespace:**&nbsp;<a href="N_Windows_UI_Core">Windows.UI.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public enum CoreDispatcherPriority
```


## Members
&nbsp;<table><tr><th></th><th>Member name</th><th>Value</th><th>Description</th></tr><tr><td /><td target="F:Windows.UI.Core.CoreDispatcherPriority.Low">**Low**</td><td>-1</td><td>Low priority. Delegates are processed when the window's main thread is idle and there is no input pending in the queue.</td></tr><tr><td /><td target="F:Windows.UI.Core.CoreDispatcherPriority.Normal">**Normal**</td><td>0</td><td>Normal priority. Delegates are processed in the order they are scheduled.</td></tr><tr><td /><td target="F:Windows.UI.Core.CoreDispatcherPriority.High">**High**</td><td>1</td><td>High priority. Delegates are invoked immediately for all synchronous requests. Asynchronous requests are queued and processed before any other request type.</td></tr></table>

## See Also


#### Reference
<a href="N_Windows_UI_Core">Windows.UI.Core Namespace</a><br />