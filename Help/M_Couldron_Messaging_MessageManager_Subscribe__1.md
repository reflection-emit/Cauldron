# MessageManager.Subscribe(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Subscribes to a message

**Namespace:**&nbsp;<a href="N_Couldron_Messaging">Couldron.Messaging</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void Subscribe<T>(
	Object subscriber,
	Action<T> subscribtionHandler
)
where T : MessagingArgs

```


#### Parameters
&nbsp;<dl><dt>subscriber</dt><dd>Type: System.Object<br />The object that subscribes to a message</dd><dt>subscribtionHandler</dt><dd>Type: System.Action(*T*)<br />The handler that will be invoked on message recieve</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of message to subscribe to</dd></dl>

## See Also


#### Reference
<a href="T_Couldron_Messaging_MessageManager">MessageManager Class</a><br /><a href="N_Couldron_Messaging">Couldron.Messaging Namespace</a><br />