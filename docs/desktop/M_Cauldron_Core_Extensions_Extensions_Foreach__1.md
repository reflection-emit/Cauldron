# Extensions.Foreach(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Performs the specified action on each element of the IEnumerable(T)

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<T> Foreach<T>(
	this IEnumerable<T> collection,
	Action<T> action
)

```


#### Parameters
&nbsp;<dl><dt>collection</dt><dd>Type: System.Collections.Generic.IEnumerable(*T*)<br />The collection to perform the action on</dd><dt>action</dt><dd>Type: System.Action(*T*)<br />The Action(T) delegate to perform on each element of the IEnumerable(T).</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type that is contained in the IEnumerable(T)</dd></dl>

#### Return Value
Type: IEnumerable(*T*)<br />Returns *collection*

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IEnumerable(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*action* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />