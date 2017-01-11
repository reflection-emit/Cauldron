# MessageManager.Send Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Sends a message to all message subscribers

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<Object> Send(
	MessagingArgs args
)
```


#### Parameters
&nbsp;<dl><dt>args</dt><dd>Type: <a href="T_Cauldron_Core_MessagingArgs">Cauldron.Core.MessagingArgs</a><br />The argument of the message</dd></dl>

#### Return Value
Type: IEnumerable(Object)<br />A collection of listeners that subscribed to this message

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*args* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_MessageManager">MessageManager Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />