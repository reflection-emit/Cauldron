# ValidatableViewModelBase Methods
 _**\[This is preliminary documentation and is subject to change.\]**_

The <a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase</a> type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_DisposableViewModelBase_Dispose">Dispose()</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_DisposableViewModelBase_Dispose_1">Dispose(Boolean)</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_DisposableViewModelBase_Finalize">Finalize</a></td><td>
Destructors are used to destruct instances of classes.
 (Inherited from <a href="T_Couldron_ViewModels_DisposableViewModelBase">DisposableViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_GetErrors">GetErrors</a></td><td>
Gets the validation errors for a specified property or for the entire entity.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_OnAfterRaiseNotifyPropertyChanged">OnAfterRaiseNotifyPropertyChanged</a></td><td>
Occures after the event <a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a> has been invoked
 (Overrides <a href="M_Couldron_ViewModels_ViewModelBase_OnAfterRaiseNotifyPropertyChanged">ViewModelBase.OnAfterRaiseNotifyPropertyChanged(String)</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ViewModelBase_OnBeforeRaiseNotifyPropertyChanged">OnBeforeRaiseNotifyPropertyChanged</a></td><td>
Occured before the <a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a> event is invoked.
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_OnDispose">OnDispose</a></td><td>
Occures after Dispose() has been invoked
 (Overrides <a href="M_Couldron_ViewModels_DisposableViewModelBase_OnDispose">DisposableViewModelBase.OnDispose(Boolean)</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ViewModelBase_OnPropertyChanged">OnPropertyChanged</a></td><td>
Invokes the <a href="E_Couldron_ViewModels_ViewModelBase_PropertyChanged">PropertyChanged</a> event
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_OnValidation">OnValidation</a></td><td>
Occures on validation</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ViewModels_ViewModelBase_RaiseNotifyBehaviourInvoke">RaiseNotifyBehaviourInvoke</a></td><td>
Invokes the <a href="E_Couldron_ViewModels_ViewModelBase_BehaviourInvoke">BehaviourInvoke</a> event
 (Inherited from <a href="T_Couldron_ViewModels_ViewModelBase">ViewModelBase</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_Validate">Validate()</a></td><td>
Starts a validation on all properties</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_Validate_2">Validate(String)</a></td><td>
Starts a validation on all properties</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ViewModels_ValidatableViewModelBase_Validate_1">Validate(PropertyInfo, String)</a></td><td>
Starts a validation on all properties</td></tr></table>&nbsp;
<a href="#validatableviewmodelbase-methods">Back to Top</a>

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
<a href="#validatableviewmodelbase-methods">Back to Top</a>

## See Also


#### Reference
<a href="T_Couldron_ViewModels_ValidatableViewModelBase">ValidatableViewModelBase Class</a><br /><a href="N_Couldron_ViewModels">Couldron.ViewModels Namespace</a><br />