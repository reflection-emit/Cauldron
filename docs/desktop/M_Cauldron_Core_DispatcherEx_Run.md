# DispatcherEx.Run Method (Action)
 _**\[This is preliminary documentation and is subject to change.\]**_

Executes the specified delegate synchronously with the specified arguments, with priority <a href="T_Windows_UI_Core_CoreDispatcherPriority">Normal</a>, on the thread that the Dispatcher was created on.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public void Run(
	Action action
)
```


#### Parameters
&nbsp;<dl><dt>action</dt><dd>Type: System.Action<br />The delegate to a method, which is pushed onto the Dispatcher event queue.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*action* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_DispatcherEx">DispatcherEx Class</a><br /><a href="Overload_Cauldron_Core_DispatcherEx_Run">Run Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />