# ICanClose Interface
 _**\[This is preliminary documentation and is subject to change.\]**_

**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public interface ICanClose : IViewModel, 
	INotifyPropertyChanged, INotifyBehaviourInvokation
```

The ICanClose type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_IViewModel_Dispatcher">Dispatcher</a></td><td>
Gets the <a href="P_Couldron_ViewModels_IViewModel_Dispatcher">Dispatcher</a> this <a href="T_Couldron_Core_CouldronDispatcher">CouldronDispatcher</a> is associated with.
 (Inherited from <a href="T_Couldron_ViewModels_IViewModel">IViewModel</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_IViewModel_Id">Id</a></td><td>
Gets the unique Id of the view model
 (Inherited from <a href="T_Couldron_ViewModels_IViewModel">IViewModel</a>.)</td></tr></table>&nbsp;
<a href="#icanclose-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ICanClose_Activated">Activated</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ICanClose_CanClose">CanClose</a></td><td>
Occures if a control is about to close. If returns false, the closing will be cancelled.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ICanClose_Deactivated">Deactivated</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_IViewModel_OnPropertyChanged">OnPropertyChanged</a></td><td>
Invokes the PropertyChanged event
 (Inherited from <a href="T_Couldron_ViewModels_IViewModel">IViewModel</a>.)</td></tr></table>&nbsp;
<a href="#icanclose-interface">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Couldron_INotifyBehaviourInvokation_BehaviourInvoke">BehaviourInvoke</a></td><td>
Occures if a behaviour should be invoked
 (Inherited from <a href="T_Couldron_INotifyBehaviourInvokation">INotifyBehaviourInvokation</a>.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>PropertyChanged</td><td> (Inherited from INotifyPropertyChanged.)</td></tr></table>&nbsp;
<a href="#icanclose-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />