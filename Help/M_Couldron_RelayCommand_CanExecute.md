# RelayCommand.CanExecute Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Defines the method that determines whether the command can execute in its current state.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public bool CanExecute(
	Object parameter
)
```


#### Parameters
&nbsp;<dl><dt>parameter</dt><dd>Type: System.Object<br />Data used by the command. If the command does not require data to be passed, this object can be set to null.</dd></dl>

#### Return Value
Type: Boolean<br />true if this command can be executed; otherwise, false.

#### Implements
ICommand.CanExecute(Object)<br />

## See Also


#### Reference
<a href="T_Couldron_RelayCommand">RelayCommand Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />