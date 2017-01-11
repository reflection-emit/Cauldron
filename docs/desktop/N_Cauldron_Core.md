# Cauldron.Core Namespace
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "N:Cauldron.Core"\]


## Classes
&nbsp;<table><tr><th></th><th>Class</th><th>Description</th></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_ApplicationInfo">ApplicationInfo</a></td><td>
Provides methods to retrieve information about the application</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_Assemblies">Assemblies</a></td><td>
Contains methods and properties that helps to manage and gather Assembly information</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_Assemblies_AssemblyAddedEventArgs">Assemblies.AssemblyAddedEventArgs</a></td><td>
Contains data of the <a href="E_Cauldron_Core_Assemblies_LoadedAssemblyChanged">LoadedAssemblyChanged</a> event.</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_AssemblyAndResourceNameInfo">AssemblyAndResourceNameInfo</a></td><td>
Represents a resource info of an embedded resource with its corresponding <a href="P_Cauldron_Core_AssemblyAndResourceNameInfo_Assembly">Assembly</a></td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_AsyncHelper">AsyncHelper</a></td><td>
Provides methods for Asyncronous operations</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_ByteSizeFormatter">ByteSizeFormatter</a></td><td>
Formats a numeric value to a human readable size</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_ComparerUtils">ComparerUtils</a></td><td>
Provides methods for comparing objects</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_CreateInstanceIsAnInterfaceException">CreateInstanceIsAnInterfaceException</a></td><td>
Represents a exception that occures while creating an instance using an interface</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_DateTimeUtils">DateTimeUtils</a></td><td>
Provides useful methods regarding DateTime</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_DispatcherEx">DispatcherEx</a></td><td>
Wrapper class for handling CoreDispatcher (WinRT) and DispatcherObject (Windows Desktop)</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_DisplayNameAttribute">DisplayNameAttribute</a></td><td>
Specifies additional name for an Enum</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_DisposableBase">DisposableBase</a></td><td>
Represents a base class of a disposable object</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_DynamicEqualityComparer_1">DynamicEqualityComparer(T)</a></td><td>
Defines methods to support the comparison of objects for equality</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_ExecutionTimer">ExecutionTimer</a></td><td>
Provides a simple performace measurement of a code block 
```
using(var perf = new ExecutionTimer())
{
}
```</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_Mathc">Mathc</a></td><td>
Provides static methods for common mathematical functions.</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_MessageManager">MessageManager</a></td><td>
Manages the messaging system</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_MessagingArgs">MessagingArgs</a></td><td>
Provides message data</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_MiscUtils">MiscUtils</a></td><td>
Provides static methods</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_MonitorInfo">MonitorInfo</a></td><td>
Provides properties and methods for getting information about the monitor</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_Randomizer">Randomizer</a></td><td>
Provides a randomizer that is cryptographicly secure</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_SystemInfo">SystemInfo</a></td><td>
Provides methods for retrieving system information</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_UrlProtocol">UrlProtocol</a></td><td>
Provides methods for handling the registration of an Application to a URI Scheme and helper methods for handling the uri protocol</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_WebAuthenticationBrokerCallbackEventArgs">WebAuthenticationBrokerCallbackEventArgs</a></td><td>
Contains data for the the <a href="F_Cauldron_Core_WebAuthenticationBrokerWrapper_OnCallBack">OnCallBack</a></td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_WebAuthenticationBrokerWrapper">WebAuthenticationBrokerWrapper</a></td><td>
Provides a wrapper for the UWP WebAuthenticationBroker and the Desktop Authentication handling</td></tr><tr><td>![Public class](media/pubclass.gif "Public class")</td><td><a href="T_Cauldron_Core_Win32Api">Win32Api</a></td><td>
Provides methods and properties for common functions in the Windows API</td></tr></table>

## Interfaces
&nbsp;<table><tr><th></th><th>Interface</th><th>Description</th></tr><tr><td>![Public interface](media/pubinterface.gif "Public interface")</td><td><a href="T_Cauldron_Core_IDisposableObject">IDisposableObject</a></td><td>
Provides a mechanism for releasing unmanaged resources.</td></tr></table>

## Enumerations
&nbsp;<table><tr><th></th><th>Enumeration</th><th>Description</th></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Cauldron_Core_CreationCollisionOption">CreationCollisionOption</a></td><td>
Specifies what to do if a file or folder with the specified name already exists in the current folder when you create a new file or folder.</td></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Cauldron_Core_Encodings">Encodings</a></td><td>
All known encodings</td></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Cauldron_Core_HashAlgorithms">HashAlgorithms</a></td><td>
Describes the hash algorithms</td></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Cauldron_Core_LogonType">LogonType</a></td><td>
The type of logon operation to perform</td></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Cauldron_Core_NameCollisionOption">NameCollisionOption</a></td><td>
Specifies what to do if a file or folder with the specified name already exists in the current folder when you copy, move, or rename a file or folder.</td></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Cauldron_Core_ViewOrientation">ViewOrientation</a></td><td>
Defines the set of display orientation modes</td></tr><tr><td>![Public enumeration](media/pubenumeration.gif "Public enumeration")</td><td><a href="T_Cauldron_Core_WindowsMessages">WindowsMessages</a></td><td>
Windows Messages. 

 Defined in winuser.h from Windows SDK v6.1 

 Documentation pulled from MSDN.</td></tr></table>&nbsp;
