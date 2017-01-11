# IValidatableViewModel Interface
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "T:Cauldron.XAML.Validation.ViewModels.IValidatableViewModel"\]

**Namespace:**&nbsp;<a href="N_Cauldron_XAML_Validation_ViewModels">Cauldron.XAML.Validation.ViewModels</a><br />**Assembly:**&nbsp;Cauldron.XAML.Validation (in Cauldron.XAML.Validation.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public interface IValidatableViewModel : IViewModel, 
	INotifyPropertyChanged, INotifyBehaviourInvocation, INotifyDataErrorInfo, IDisposableObject, IDisposable
```

The IValidatableViewModel type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ViewModels_IViewModel_Dispatcher">Dispatcher</a></td><td> (Inherited from <a href="T_Cauldron_XAML_ViewModels_IViewModel">IViewModel</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>HasErrors</td><td>
Gets a value that indicates whether the entity has validation errors.
 (Inherited from INotifyDataErrorInfo.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ViewModels_IViewModel_Id">Id</a></td><td> (Inherited from <a href="T_Cauldron_XAML_ViewModels_IViewModel">IViewModel</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Core_IDisposableObject_IsDisposed">IsDisposed</a></td><td>
Gets a value indicating if the object has been disposed or not
 (Inherited from <a href="T_Cauldron_Core_IDisposableObject">IDisposableObject</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ViewModels_IViewModel_IsLoading">IsLoading</a></td><td> (Inherited from <a href="T_Cauldron_XAML_ViewModels_IViewModel">IViewModel</a>.)</td></tr></table>&nbsp;
<a href="#ivalidatableviewmodel-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Dispose</td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 (Inherited from IDisposable.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetErrors</td><td>
Gets the validation errors for a specified property or for the entire entity.
 (Inherited from INotifyDataErrorInfo.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_ViewModels_IViewModel_OnException">OnException</a></td><td> (Inherited from <a href="T_Cauldron_XAML_ViewModels_IViewModel">IViewModel</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_ViewModels_IViewModel_RaiseNotifyBehaviourInvoke">RaiseNotifyBehaviourInvoke</a></td><td> (Inherited from <a href="T_Cauldron_XAML_ViewModels_IViewModel">IViewModel</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_ViewModels_IViewModel_RaisePropertyChanged">RaisePropertyChanged</a></td><td> (Inherited from <a href="T_Cauldron_XAML_ViewModels_IViewModel">IViewModel</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Validation_ViewModels_IValidatableViewModel_Validate">Validate()</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Validation_ViewModels_IValidatableViewModel_Validate_2">Validate(String)</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Validation_ViewModels_IValidatableViewModel_Validate_1">Validate(PropertyInfo, String)</a></td><td /></tr></table>&nbsp;
<a href="#ivalidatableviewmodel-interface">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Cauldron_XAML_INotifyBehaviourInvocation_BehaviourInvoke">BehaviourInvoke</a></td><td> (Inherited from <a href="T_Cauldron_XAML_INotifyBehaviourInvocation">INotifyBehaviourInvocation</a>.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Cauldron_Core_IDisposableObject_Disposed">Disposed</a></td><td>
Occures if the object has been disposed
 (Inherited from <a href="T_Cauldron_Core_IDisposableObject">IDisposableObject</a>.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>ErrorsChanged</td><td>
Occurs when the validation errors have changed for a property or for the entire entity.
 (Inherited from INotifyDataErrorInfo.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>PropertyChanged</td><td>
Occurs when a property value changes.
 (Inherited from INotifyPropertyChanged.)</td></tr></table>&nbsp;
<a href="#ivalidatableviewmodel-interface">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_Run">Run</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_RunAsync">RunAsync</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr></table>&nbsp;
<a href="#ivalidatableviewmodel-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_XAML_Validation_ViewModels">Cauldron.XAML.Validation.ViewModels Namespace</a><br />