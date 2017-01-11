# AsyncHelper.NullGuard(*TResult*) Method (Task(*TResult*))
 _**\[This is preliminary documentation and is subject to change.\]**_

Insures that an awaited method always returns a Task.

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Task<TResult> NullGuard<TResult>(
	Task<TResult> task
)

```


#### Parameters
&nbsp;<dl><dt>task</dt><dd>Type: System.Threading.Tasks.Task(*TResult*)<br />The awaitable task</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TResult</dt><dd>The type of the result produced by Task(TResult).</dd></dl>

#### Return Value
Type: Task(*TResult*)<br />An awaitable task

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>NotSupportedException</td><td>*TResult* is a value type</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_AsyncHelper">AsyncHelper Class</a><br /><a href="Overload_Cauldron_Core_AsyncHelper_NullGuard">NullGuard Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />