# Extensions.RunSync(*TResult*) Method (Task(*TResult*))
 _**\[This is preliminary documentation and is subject to change.\]**_

Runs the Task synchronously on the default TaskScheduler.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static TResult RunSync<TResult>(
	this Task<TResult> task
)

```


#### Parameters
&nbsp;<dl><dt>task</dt><dd>Type: System.Threading.Tasks.Task(*TResult*)<br />The task instance</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TResult</dt><dd>The type of the result produced by this Task</dd></dl>

#### Return Value
Type: *TResult*<br />The value returned by the function

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Task(*TResult*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="Overload_Cauldron_Core_Extensions_Extensions_RunSync">RunSync Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />