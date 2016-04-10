# IRelayCommand Interface
 _**\[This is preliminary documentation and is subject to change.\]**_

Defines a command

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public interface IRelayCommand : ICommand, 
	INotifyPropertyChanged
```

The IRelayCommand type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CanExecute</td><td> (Inherited from ICommand.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Execute</td><td> (Inherited from ICommand.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_IRelayCommand_RefreshCanExecute">RefreshCanExecute</a></td><td>
Triggers the CanExecuteChanged event and forces the control to refresh the execution state</td></tr></table>&nbsp;
<a href="#irelaycommand-interface">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>CanExecuteChanged</td><td> (Inherited from ICommand.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>PropertyChanged</td><td> (Inherited from INotifyPropertyChanged.)</td></tr></table>&nbsp;
<a href="#irelaycommand-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron">Couldron Namespace</a><br />