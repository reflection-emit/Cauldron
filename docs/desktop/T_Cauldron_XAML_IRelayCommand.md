# IRelayCommand Interface
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "T:Cauldron.XAML.IRelayCommand"\]

**Namespace:**&nbsp;<a href="N_Cauldron_XAML">Cauldron.XAML</a><br />**Assembly:**&nbsp;Cauldron.XAML (in Cauldron.XAML.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public interface IRelayCommand : ICommand, 
	INotifyPropertyChanged
```

The IRelayCommand type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CanExecute</td><td>
Defines the method that determines whether the command can execute in its current state.
 (Inherited from ICommand.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Execute</td><td>
Defines the method to be called when the command is invoked.
 (Inherited from ICommand.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_IRelayCommand_RefreshCanExecute">RefreshCanExecute</a></td><td /></tr></table>&nbsp;
<a href="#irelaycommand-interface">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>CanExecuteChanged</td><td>
Occurs when changes occur that affect whether or not the command should execute.
 (Inherited from ICommand.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>PropertyChanged</td><td>
Occurs when a property value changes.
 (Inherited from INotifyPropertyChanged.)</td></tr></table>&nbsp;
<a href="#irelaycommand-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_XAML">Cauldron.XAML Namespace</a><br />