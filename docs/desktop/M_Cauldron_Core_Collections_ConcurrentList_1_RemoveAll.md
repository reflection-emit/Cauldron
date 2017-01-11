# ConcurrentList(*T*).RemoveAll Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Removes all the elements that match the conditions defined by the specified predicate.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public void RemoveAll(
	Predicate<T> match
)
```


#### Parameters
&nbsp;<dl><dt>match</dt><dd>Type: System.Predicate(<a href="T_Cauldron_Core_Collections_ConcurrentList_1">*T*</a>)<br />The Predicate(T) delegate that defines the conditions of the elements to remove.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>match is null.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />