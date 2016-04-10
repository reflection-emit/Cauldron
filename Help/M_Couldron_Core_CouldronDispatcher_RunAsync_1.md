# CouldronDispatcher.RunAsync Method (Action)
 _**\[This is preliminary documentation and is subject to change.\]**_

Executes the specified delegate asynchronously with the specified arguments, at the specified priority, on the thread that the System.Windows.Threading.Dispatcher was created on.

**Namespace:**&nbsp;<a href="N_Couldron_Core">Couldron.Core</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

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
<a href="T_Couldron_Core_CouldronDispatcher">CouldronDispatcher Class</a><br /><a href="Overload_Couldron_Core_CouldronDispatcher_RunAsync">RunAsync Overload</a><br /><a href="N_Couldron_Core">Couldron.Core Namespace</a><br />