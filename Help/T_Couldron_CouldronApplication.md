# CouldronApplication Class
 _**\[This is preliminary documentation and is subject to change.\]**_


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;System.Windows.Threading.DispatcherObject<br />&nbsp;&nbsp;&nbsp;&nbsp;System.Windows.Application<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Couldron.CouldronApplication<br />
**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public abstract class CouldronApplication : Application
```

The CouldronApplication type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_CouldronApplication__ctor">CouldronApplication</a></td><td>
Initializes a new instance of the CouldronApplication</td></tr></table>&nbsp;
<a href="#couldronapplication-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Dispatcher</td><td> (Inherited from DispatcherObject.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>MainWindow</td><td> (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Properties</td><td> (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Resources</td><td> (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>ShutdownMode</td><td> (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>StartupUri</td><td> (Inherited from Application.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_CouldronApplication_ThemeAccentColor">ThemeAccentColor</a></td><td>
Gets or sets the Couldron theme accent color 

 There is no garantee that the used theme supports the accent color</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Windows</td><td> (Inherited from Application.)</td></tr></table>&nbsp;
<a href="#couldronapplication-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CheckAccess</td><td> (Inherited from DispatcherObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>FindResource</td><td> (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnActivated</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_CouldronApplication_OnConstruction">OnConstruction</a></td><td>
Occures on initialization of CouldronApplication</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnDeactivated</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnExit</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnFragmentNavigation</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnLoadCompleted</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigated</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigating</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigationFailed</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigationProgress</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnNavigationStopped</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnSessionEnding</td><td> (Inherited from Application.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStartup</td><td> (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Run()</td><td> (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Run(Window)</td><td> (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Shutdown()</td><td> (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Shutdown(Int32)</td><td> (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TryFindResource</td><td> (Inherited from Application.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>VerifyAccess</td><td> (Inherited from DispatcherObject.)</td></tr></table>&nbsp;
<a href="#couldronapplication-class">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Activated</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Deactivated</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>DispatcherUnhandledException</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Exit</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>FragmentNavigation</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>LoadCompleted</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Navigated</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Navigating</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationFailed</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationProgress</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>NavigationStopped</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>SessionEnding</td><td> (Inherited from Application.)</td></tr><tr><td>![Public event](media/pubevent.gif "Public event")</td><td>Startup</td><td> (Inherited from Application.)</td></tr></table>&nbsp;
<a href="#couldronapplication-class">Back to Top</a>

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
<a href="#couldronapplication-class">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IQueryAmbient.IsAmbientPropertyAvailable</td><td> (Inherited from Application.)</td></tr></table>&nbsp;
<a href="#couldronapplication-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron">Couldron Namespace</a><br />