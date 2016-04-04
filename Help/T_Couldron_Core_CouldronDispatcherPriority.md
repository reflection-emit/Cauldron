# CouldronDispatcherPriority Enumeration
 _**\[This is preliminary documentation and is subject to change.\]**_

Describes the priorities at which operations can be invoked

**Namespace:**&nbsp;<a href="N_Couldron_Core">Couldron.Core</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public enum CouldronDispatcherPriority
```


## Members
&nbsp;<table><tr><th></th><th>Member name</th><th>Value</th><th>Description</th></tr><tr><td /><td target="F:Couldron.Core.CouldronDispatcherPriority.Low">**Low**</td><td>-1</td><td>Low priority. Delegates are processed when the window's main thread is idle and there is no input pending in the queue.</td></tr><tr><td /><td target="F:Couldron.Core.CouldronDispatcherPriority.Normal">**Normal**</td><td>0</td><td>Normal priority. Delegates are processed in the order they are scheduled.</td></tr><tr><td /><td target="F:Couldron.Core.CouldronDispatcherPriority.High">**High**</td><td>1</td><td>High priority. Delegates are invoked immediately for all synchronous requests. Asynchronous requests are queued and processed before any other request type.</td></tr></table>

## See Also


#### Reference
<a href="N_Couldron_Core">Couldron.Core Namespace</a><br />