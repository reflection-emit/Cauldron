# RelayCommand Constructor (Action, Func(Boolean))
 _**\[This is preliminary documentation and is subject to change.\]**_

Initializes a new instance of <a href="T_Couldron_RelayCommand">RelayCommand</a> class

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public RelayCommand(
	Action action,
	Func<bool> canexecute
)
```


#### Parameters
&nbsp;<dl><dt>action</dt><dd>Type: System.Action<br />The action that is invoked on command execution</dd><dt>canexecute</dt><dd>Type: System.Func(Boolean)<br />A delegate that indicates if the command can be executed or not. Should return true if the command can be executed.</dd></dl>

## See Also


#### Reference
<a href="T_Couldron_RelayCommand">RelayCommand Class</a><br /><a href="Overload_Couldron_RelayCommand__ctor">RelayCommand Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />