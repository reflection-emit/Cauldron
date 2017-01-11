# Win32Api Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Provides methods and properties for common functions in the Windows API


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Cauldron.Core.Win32Api<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static class Win32Api
```

The Win32Api type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")![Static member](media/static.gif "Static member")</td><td><a href="P_Cauldron_Core_Win32Api_IsCurrentUserAnAdministrator">IsCurrentUserAnAdministrator</a></td><td>
Tests whether the current user is an elevated administrator.</td></tr></table>&nbsp;
<a href="#win32api-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_ActivateWindow">ActivateWindow</a></td><td>
Brings the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_BroadcastMessage">BroadcastMessage(UInt32)</a></td><td>
The message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_BroadcastMessage_1">BroadcastMessage(UInt32, IntPtr)</a></td><td>
The message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_BroadcastMessage_2">BroadcastMessage(UInt32, IntPtr, IntPtr)</a></td><td>
The message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_ExtractAssociatedIcon">ExtractAssociatedIcon</a></td><td>
Extracts an icon from a exe, dll or ico file.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_GetMessage">GetMessage</a></td><td>
Retrieves the string sent by <a href="M_Cauldron_Core_Win32Api_SendMessage_1">SendMessage(IntPtr, String)</a>.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_GetMousePosition">GetMousePosition</a></td><td>
Returns the mouse position on screen</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_GetUserTilePath">GetUserTilePath</a></td><td>
Gets the path of the user's profile picture</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_RegisterWindowMessage">RegisterWindowMessage</a></td><td>
Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used when sending or posting messages.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_SendMessage_1">SendMessage(IntPtr, String)</a></td><td>
Sends the specified message string to a window. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_SendMessage">SendMessage(IntPtr, WindowsMessages, IntPtr, IntPtr)</a></td><td>
Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Win32Api_StartElevated">StartElevated</a></td><td>
Starts the EntryAssembly elevated.</td></tr></table>&nbsp;
<a href="#win32api-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />