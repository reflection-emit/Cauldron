# Interaction.GetBehaviour(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Get a behaviours in the <a href="T_Couldron_Collections_BehaviourCollection">BehaviourCollection</a> associated with the specified object

**Namespace:**&nbsp;<a href="N_Couldron_Behaviours">Couldron.Behaviours</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static T[] GetBehaviour<T>(
	DependencyObject dependencyObject
)
where T : IBehaviour

```


#### Parameters
&nbsp;<dl><dt>dependencyObject</dt><dd>Type: System.Windows.DependencyObject<br />The DependencyObject from which to retrieve the <a href="T_Couldron_Collections_BehaviourCollection">BehaviourCollection</a>.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the behaviour</dd></dl>

#### Return Value
Type: *T*[]<br />An array of behaviours

## See Also


#### Reference
<a href="T_Couldron_Behaviours_Interaction">Interaction Class</a><br /><a href="N_Couldron_Behaviours">Couldron.Behaviours Namespace</a><br />