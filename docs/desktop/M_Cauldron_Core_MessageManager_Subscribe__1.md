# MessageManager.Subscribe(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Subscribes to a message. If the subscriber implements the <a href="T_Cauldron_Core_IDisposableObject">IDisposableObject</a> interface, the <a href="T_Cauldron_Core_MessageManager">MessageManager</a> will automatically add the <a href="M_Cauldron_Core_MessageManager_Unsubscribe_1">Unsubscribe(Object)</a> method to the dispose event

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static void Subscribe<T>(
	Object subscriber,
	Action<T> subscriptionHandler
)
where T : MessagingArgs

```


#### Parameters
&nbsp;<dl><dt>subscriber</dt><dd>Type: System.Object<br />The object that subscribes to a message</dd><dt>subscriptionHandler</dt><dd>Type: System.Action(*T*)<br />The handler that will be invoked on message recieve</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of message to subscribe to</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*subscriber* is null</td></tr><tr><td>ArgumentNullException</td><td>*subscriptionHandler* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_MessageManager">MessageManager Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />