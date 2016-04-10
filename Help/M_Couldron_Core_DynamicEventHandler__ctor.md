# DynamicEventHandler Constructor 
 _**\[This is preliminary documentation and is subject to change.\]**_

Initializes a new instance of <a href="T_Couldron_Core_DynamicEventHandler">DynamicEventHandler</a>

**Namespace:**&nbsp;<a href="N_Couldron_Core">Couldron.Core</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public DynamicEventHandler(
	Object associatedObject,
	string eventName,
	Action<Object, Object> eventHandler
)
```


#### Parameters
&nbsp;<dl><dt>associatedObject</dt><dd>Type: System.Object<br />The objects that contains the event</dd><dt>eventName</dt><dd>Type: System.String<br />The name of the event</dd><dt>eventHandler</dt><dd>Type: System.Action(Object, Object)<br />A delegate that handles the event</dd></dl>

## See Also


#### Reference
<a href="T_Couldron_Core_DynamicEventHandler">DynamicEventHandler Class</a><br /><a href="N_Couldron_Core">Couldron.Core Namespace</a><br />