# Extensions.Join(*T*) Method (IEnumerable(*T*), String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Concatenates the members of a collection, using the specified *separator* between each member.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string Join<T>(
	this IEnumerable<T> source,
	string separator
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.Collections.Generic.IEnumerable(*T*)<br />A collection that contains the objects to concatenate.</dd><dt>separator</dt><dd>Type: System.String<br />The string to use as a *separator*. *separator* is included in the returned string only if values has more than one element.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>\[Missing <typeparam name="T"/> documentation for "M:Cauldron.Core.Extensions.Extensions.Join``1(System.Collections.Generic.IEnumerable{``0},System.String)"\]</dd></dl>

#### Return Value
Type: String<br />A string that consists of the members of values delimited by the *separator* string. If values has no members, the method returns Empty.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IEnumerable(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*source* is null.</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="Overload_Cauldron_Core_Extensions_Extensions_Join">Join Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />