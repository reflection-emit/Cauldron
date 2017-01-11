# DispatcherEx.Run Method (CoreDispatcherPriority, Action)
 _**\[This is preliminary documentation and is subject to change.\]**_

Executes the specified delegate synchronously with the specified arguments, at the specified priority, on the thread that the Dispatcher was created on.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public void Run(
	CoreDispatcherPriority priority,
	Action action
)
```


#### Parameters
&nbsp;<dl><dt>priority</dt><dd>Type: <a href="T_Windows_UI_Core_CoreDispatcherPriority">Windows.UI.Core.CoreDispatcherPriority</a><br />The priority, relative to the other pending operations in the Dispatcher event queue, the specified method is invoked.</dd><dt>action</dt><dd>Type: System.Action<br />The delegate to a method, which is pushed onto the Dispatcher event queue.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*action* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_DispatcherEx">DispatcherEx Class</a><br /><a href="Overload_Cauldron_Core_DispatcherEx_Run">Run Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />