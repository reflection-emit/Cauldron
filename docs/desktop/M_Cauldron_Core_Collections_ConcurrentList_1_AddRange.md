# ConcurrentList(*T*).AddRange Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Adds the elements of the specified collection to the end of the <a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T)</a>

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public void AddRange(
	IEnumerable<T> collection
)
```


#### Parameters
&nbsp;<dl><dt>collection</dt><dd>Type: System.Collections.Generic.IEnumerable(<a href="T_Cauldron_Core_Collections_ConcurrentList_1">*T*</a>)<br />The collection whose elements should be added to the end of the <a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T)</a>. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</dd></dl>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>collection is null.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Collections_ConcurrentList_1">ConcurrentList(T) Class</a><br /><a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />