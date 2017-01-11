# ApplicationBase Class
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "T:Cauldron.XAML.ApplicationBase"\]


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;System.Windows.Threading.DispatcherObject<br />&nbsp;&nbsp;&nbsp;&nbsp;System.Windows.Application<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Cauldron.XAML.ApplicationBase<br />
**Namespace:**&nbsp;<a href="N_Cauldron_XAML">Cauldron.XAML</a><br />**Assembly:**&nbsp;Cauldron.XAML (in Cauldron.XAML.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public abstract class ApplicationBase : Application, 
	IViewModel, INotifyPropertyChanged, INotifyBehaviourInvocation
```

The ApplicationBase type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_ApplicationBase__ctor">ApplicationBase</a></td><td>
Initializes a new instance of the ApplicationBase class</td></tr></table>&nbsp;
<a href="#applicationbase-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_ApplicationSplash">ApplicationSplash</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_Dispatcher">Dispatcher</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_Id">Id</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_IsLoading">IsLoading</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_IsSingleInstance">IsSingleInstance</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_IsSinglePage">IsSinglePage</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>MainWindow</td><td>
Gets or sets the main window of the application.
 (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_MessageDialog">MessageDialog</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_Navigator">Navigator</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Properties</td><td>
Gets a collection of application-scope properties.
 (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Resources</td><td>
Gets or sets a collection of application-scope resources, such as styles and brushes.
 (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_ShouldBringToFront">ShouldBringToFront</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>ShutdownMode</td><td>
Gets or sets the condition that causes the Shutdown() method to be called.
 (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>StartupUri</td><td>
Gets or sets a UI that is automatically shown when an application starts.
 (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_ApplicationBase_UrlProtocolNames">UrlProtocolNames</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Windows</td><td>
Gets the instantiated windows in an application.
 (Inherited from Application.)</td></tr></table>&nbsp;
<a href="#applicationbase-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_AfterRaiseNotifyPropertyChanged">AfterRaiseNotifyPropertyChanged</a></td><td /></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_BeforeRaiseNotifyPropertyChanged">BeforeRaiseNotifyPropertyChanged</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CheckAccess</td><td>
Determines whether the calling thread has access to this DispatcherObject.
 (Inherited from DispatcherObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>FindResource</td><td>
Searches for a user interface (UI) resource, such as a Style or Brush, with the specified key, and throws an exception if the requested resource is not found (see XAML Resources).
 (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td>
Serves as the default hash function.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td>
Gets the Type of the current instance.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td>
Creates a shallow copy of the current Object.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnActivated(EventArgs)</td><td>
Raises the Activated event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_OnActivated">OnActivated(String[])</a></td><td /></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_OnActivationProtocol">OnActivationProtocol</a></td><td /></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_OnConstruction">OnConstruction</a></td><td /></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnDeactivated</td><td>
Raises the Deactivated event.
 (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_OnException">OnException</a></td><td /></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnExit</td><td>
Raises the Exit event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnFragmentNavigation</td><td>
Raises the FragmentNavigation event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnLoadCompleted</td><td>
Raises the LoadCompleted event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigated</td><td>
Raises the Navigated event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigating</td><td>
Raises the Navigating event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigationFailed</td><td>
Raises the NavigationFailed event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigationProgress</td><td>
Raises the NavigationProgress event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigationStopped</td><td>
Raises the NavigationStopped event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_OnPreload">OnPreload</a></td><td /></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnSessionEnding</td><td>
Raises the SessionEnding event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStartup(StartupEventArgs)</td><td>
Raises the Startup event.
 (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_OnStartup">OnStartup(LaunchActivatedEventArgs)</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_RaiseNotifyBehaviourInvoke">RaiseNotifyBehaviourInvoke</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_ApplicationBase_RaisePropertyChanged">RaisePropertyChanged</a></td><td /></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Run()</td><td>
Starts a Windows Presentation Foundation (WPF) application.
 (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Run(Window)</td><td>
Starts a Windows Presentation Foundation (WPF) application and opens the specified window.
 (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Shutdown()</td><td>
Shuts down an application.
 (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Shutdown(Int32)</td><td>
Shuts down an application that returns the specified exit code to the operating system.
 (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td>
Returns a string that represents the current object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TryFindResource</td><td>
Searches for the specified resource.
 (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>VerifyAccess</td><td>
Enforces that the calling thread has access to this DispatcherObject.
 (Inherited from DispatcherObject.)</td></tr></table>&nbsp;
<a href="#applicationbase-class">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Activated</td><td>
Occurs when an application becomes the foreground application.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Cauldron_XAML_ApplicationBase_BehaviourInvoke">BehaviourInvoke</a></td><td /></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Deactivated</td><td>
Occurs when an application stops being the foreground application.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>DispatcherUnhandledException</td><td>
Occurs when an exception is thrown by an application but not handled.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Exit</td><td>
Occurs just before an application shuts down, and cannot be canceled.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>FragmentNavigation</td><td>
Occurs when a navigator in the application begins navigation to a content fragment, Navigation occurs immediately if the desired fragment is in the current content, or after the source XAML content has been loaded if the desired fragment is in different content.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>LoadCompleted</td><td>
Occurs when content that was navigated to by a navigator in the application has been loaded, parsed, and has begun rendering.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Navigated</td><td>
Occurs when the content that is being navigated to by a navigator in the application has been found, although it may not have completed loading.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Navigating</td><td>
Occurs when a new navigation is requested by a navigator in the application.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationFailed</td><td>
Occurs when an error occurs while a navigator in the application is navigating to the requested content.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationProgress</td><td>
Occurs periodically during a download that is being managed by a navigator in the application to provide navigation progress information.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationStopped</td><td>
Occurs when the StopLoading method of a navigator in the application is called, or when a new navigation is requested by a navigator while a current navigation is in progress.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td><a href="E_Cauldron_XAML_ApplicationBase_PropertyChanged">PropertyChanged</a></td><td /></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>SessionEnding</td><td>
Occurs when the user ends the Windows session by logging off or shutting down the operating system.
 (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Startup</td><td>
Occurs when the Run() method of the Application object is called.
 (Inherited from Application.)</td></tr></table>&nbsp;
<a href="#applicationbase-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_As__1">As(T)</a></td><td>
Performs a cast between compatible reference types. If a convertion is not possible then null is returned.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension_CreateObject__1">CreateObject(T)</a></td><td> (Defined by <a href="T_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension">AnonymousTypeWithInterfaceExtension</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyNonPublicValue__1">GetPropertyNonPublicValue(T)</a></td><td>
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and NonPublic
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue">GetPropertyValue(String, BindingFlags)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1">GetPropertyValue(T)(String)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and Public
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1_1">GetPropertyValue(T)(String, BindingFlags)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_IsDerivedFrom__1">IsDerivedFrom(T)</a></td><td>
Checks if an object is not compatible (does not derive) with a given type.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Activator_ExtensionsCloning_MapTo__1">MapTo(T)</a></td><td> (Defined by <a href="T_Cauldron_Activator_ExtensionsCloning">ExtensionsCloning</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_Run">Run</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_RunAsync">RunAsync</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_ToLong_1">ToLong</a></td><td>
Tries to convert a Object to an Int64
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_TryDispose">TryDispose</a></td><td>
Tries to performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr></table>&nbsp;
<a href="#applicationbase-class">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IQueryAmbient.IsAmbientPropertyAvailable</td><td>
Queries for whether a specified ambient property is available in the current scope.
 (Inherited from Application.)</td></tr></table>&nbsp;
<a href="#applicationbase-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_XAML">Cauldron.XAML Namespace</a><br />