# ConcurrentList(*T*).RemoveAll Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Removes all the elements that match the conditions defined by the specified predicate.

**Namespace:**&nbsp;<a href="N_Couldron_Collections">Couldron.Collections</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public void RemoveAll(
	Predicate<T> match
)
```


#### Parameters
&nbsp;<dl><dt>match</dt><dd>Type: System.Predicate(<a href="T_Couldron_Collections_ConcurrentList_1">*T*</a>)<br />The Predicate(T) delegate that defines the conditions of the elements to remove.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>match is null.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="N_Couldron_Collections">Couldron.Collections Namespace</a><br />