# ValidatableContainerChangeAwareViewModelBase Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Represents the Base class of a ViewModel that can have registered child viewmodels


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;<a href="T_Couldron_ViewModels_ViewModelBase">Couldron.ViewModels.ViewModelBase</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ViewModels_DisposableViewModelBase">Couldron.ViewModels.DisposableViewModelBase</a><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ViewModels_ValidatableViewModelBase">Couldron.ViewModels.ValidatableViewModelBase</a><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">Couldron.ViewModels.ValidatableChangeAwareViewModelBase</a><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Couldron.ViewModels.ValidatableContainerChangeAwareViewModelBase<br />
**Namespace:**&nbsp;<a href="N_Couldron_ViewModels">Couldron.ViewModels</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public abstract class ValidatableContainerChangeAwareViewModelBase : ValidatableChangeAwareViewModelBase, 
	IContainerViewModel, IViewModel, INotifyPropertyChanged, INotifyBehaviourInvokation
```

The ValidatableContainerChangeAwareViewModelBase type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase__ctor">ValidatableContainerChangeAwareViewModelBase()</a></td><td>
Initializes a new instance of ValidatableContainerChangeAwareViewModelBase</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase__ctor_1">ValidatableContainerChangeAwareViewModelBase(Guid)</a></td><td>
Initializes a new instance of ValidatableContainerChangeAwareViewModelBase</td></tr></table>&nbsp;
<a href="#validatablecontainerchangeawareviewmodelbase-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_ViewModelBase_Dispatcher">Dispatcher</a></td><td>
Gets the <a href="P_Couldron_ViewModels_ViewModelBase_Dispatcher">Dispatcher</a> this <a href="T_Couldron_Core_CouldronDispatcher">CouldronDispatcher</a> is associated with.
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_ValidatableViewModelBase_Errors">Errors</a></td><td>
Gets or sets the error info strings
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_ValidatableViewModelBase_HasErrors">HasErrors</a></td><td>
Gets a value that indicates if the ViewModel has errors after validation
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_ViewModelBase_Id">Id</a></td><td>
Gets the unique Id of the view model
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_ValidatableChangeAwareViewModelBase_IsChanged">IsChanged</a></td><td>
Gets or sets a value that indicates if any property value has been changed
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">ValidatableChangeAwareViewModelBase</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_DisposableViewModelBase_IsDisposed">IsDisposed</a></td><td>
Gets a value indicating if the object has been disposed or not
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_ValidatableChangeAwareViewModelBase_IsLoading">IsLoading</a></td><td>
Gets or sets a value that indicates if the viewmodel is loading
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">ValidatableChangeAwareViewModelBase</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_ViewModels_ValidatableChangeAwareViewModelBase_Parent">Parent</a></td><td>
Gets or sets the parent viewmodel
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">ValidatableChangeAwareViewModelBase</a>.)</td></tr></table>&nbsp;
<a href="#validatablecontainerchangeawareviewmodelbase-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_DisposableViewModelBase_Dispose">Dispose()</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_DisposableViewModelBase_Dispose_1">Dispose(Boolean)</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_DisposableViewModelBase_Finalize">Finalize</a></td><td>
Destructors are used to destruct instances of classes.
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_GetErrors">GetErrors</a></td><td>
Gets the validation errors for a specified property or for the entire entity.
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase_GetRegistered">GetRegistered(Guid)</a></td><td>
Returns a registered child viewmodel</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase_GetRegistered__1">GetRegistered(T)()</a></td><td>
Returns a registered Child ViewModel</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableChangeAwareViewModelBase_OnAfterRaiseNotifyPropertyChanged">OnAfterRaiseNotifyPropertyChanged</a></td><td>
Occures after the event PropertyChanged has been invoked
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">ValidatableChangeAwareViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ViewModelBase_OnBeforeRaiseNotifyPropertyChanged">OnBeforeRaiseNotifyPropertyChanged</a></td><td>
Occured before the <a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a> event is invoked.
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase_OnDispose">OnDispose</a></td><td>
Occures after Dispose() has been invoked
 (Overrides <a href="M_Couldron_ViewModels_ValidatableViewModelBase_OnDispose">ValidatableViewModelBase.OnDispose(Boolean)</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableChangeAwareViewModelBase_OnIsChangedChanged">OnIsChangedChanged</a></td><td>
Occures if the Change Property has changed its value
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">ValidatableChangeAwareViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableChangeAwareViewModelBase_OnIsLoadingChanged">OnIsLoadingChanged</a></td><td>
Occures if the Loading Property has changed its value
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">ValidatableChangeAwareViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ViewModelBase_OnPropertyChanged">OnPropertyChanged</a></td><td>
Invokes the <a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a> event
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_OnValidation">OnValidation</a></td><td>
Occures on validation
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ViewModelBase_RaiseNotifyBehaviourInvoke">RaiseNotifyBehaviourInvoke</a></td><td>
Invokes the <a href="E_Couldron_ViewModels_ViewModelBase_BehaviourInvoke">BehaviourInvoke</a> event
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase_Register">Register</a></td><td>
Registers a child model to the current ViewModel</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase_UnRegister_1">UnRegister(Guid)</a></td><td>
Unregisters a registered viewmodel. This will also dispose the viewmodel.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableContainerChangeAwareViewModelBase_UnRegister">UnRegister(IViewModel)</a></td><td>
Unregisters a registered viewmodel. This will also dispose the viewmodel.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_Validate">Validate()</a></td><td>
Starts a validation on all properties
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_Validate_2">Validate(String)</a></td><td>
Starts a validation on all properties
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_Validate_1">Validate(PropertyInfo, String)</a></td><td>
Starts a validation on all properties
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr></table>&nbsp;
<a href="#validatablecontainerchangeawareviewmodelbase-class">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Couldron_ViewModels_ViewModelBase_BehaviourInvoke">BehaviourInvoke</a></td><td>
Occures if a behaviour should be invoked
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Couldron_ViewModels_ValidatableChangeAwareViewModelBase_Changed">Changed</a></td><td>
Occures when a value has changed
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableChangeAwareViewModelBase">ValidatableChangeAwareViewModelBase</a>.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Couldron_ViewModels_DisposableViewModelBase_Disposed">Disposed</a></td><td>
Occures if the object has been disposed
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Couldron_ViewModels_ValidatableViewModelBase_ErrorsChanged">ErrorsChanged</a></td><td>
Occures if the count of the errors has changed
 (Inherited from <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a>.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a></td><td>
Occurs when a property value changes.
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr></table>&nbsp;
<a href="#validatablecontainerchangeawareviewmodelbase-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_CastTo__1">CastTo(T)</a></td><td>
Performs certain types of conversions between compatible reference types or nullable types 

 Returns null if convertion was not successfull
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_DisposeAll">DisposeAll</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface. 

 If the object is a FrameworkElement it will try to find known diposable attached properties. 

 It will also dispose the the DataContext content.
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToBool">ToBool</a></td><td>
Tries to convert an Object to a Boolean
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToDouble">ToDouble</a></td><td>
Tries to convert a Object to an Double
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToInteger">ToInteger</a></td><td>
Tries to convert a Object to an Int32
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToString2">ToString2</a></td><td>
Returns a string that represents the current object. 

 If the object is null a Empty will be returned
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr></table>&nbsp;
<a href="#validatablecontainerchangeawareviewmodelbase-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />