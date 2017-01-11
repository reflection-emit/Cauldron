# MessageManager Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Manages the messaging system


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Cauldron.Core.MessageManager<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static class MessageManager
```

The MessageManager type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MessageManager_Send">Send</a></td><td>
Sends a message to all message subscribers</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MessageManager_Subscribe__1">Subscribe(T)</a></td><td>
Subscribes to a message. If the subscriber implements the <a href="T_Cauldron_Core_IDisposableObject">IDisposableObject</a> interface, the MessageManager will automatically add the <a href="M_Cauldron_Core_MessageManager_Unsubscribe_1">Unsubscribe(Object)</a> method to the dispose event</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MessageManager_Unsubscribe">Unsubscribe()</a></td><td>
Unsubscribs all subscriptions</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MessageManager_Unsubscribe_1">Unsubscribe(Object)</a></td><td>
Unsubscribs all subscriptions from the defined subscriber</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_MessageManager_Unsubscribe__1">Unsubscribe(T)(Object)</a></td><td>
Unsubscribs all subscriptions from the defined subscriber for the given message type</td></tr></table>&nbsp;
<a href="#messagemanager-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />