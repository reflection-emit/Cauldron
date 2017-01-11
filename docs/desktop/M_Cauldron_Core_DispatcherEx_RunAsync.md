# DispatcherEx.RunAsync Method (Action)
 _**\[This is preliminary documentation and is subject to change.\]**_

Executes the specified delegate asynchronously with the specified arguments, with priority <a href="T_Windows_UI_Core_CoreDispatcherPriority">Normal</a>, on the thread that the Dispatcher was created on.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public Task RunAsync(
	Action action
)
```


#### Parameters
&nbsp;<dl><dt>action</dt><dd>Type: System.Action<br />The delegate to a method, which is pushed onto the Dispatcher event queue.</dd></dl>

#### Return Value
Type: Task<br />Returns a awaitable task

## See Also


#### Reference
<a href="T_Cauldron_Core_DispatcherEx">DispatcherEx Class</a><br /><a href="Overload_Cauldron_Core_DispatcherEx_RunAsync">RunAsync Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />