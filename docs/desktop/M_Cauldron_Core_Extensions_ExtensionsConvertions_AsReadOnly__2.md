# ExtensionsConvertions.AsReadOnly(*TKey*, *TValue*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Converts a IDictionary(TKey, TValue) to a ReadOnlyDictionary(TKey, TValue).

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
	this IDictionary<TKey, TValue> dictionary
)

```


#### Parameters
&nbsp;<dl><dt>dictionary</dt><dd>Type: System.Collections.Generic.IDictionary(*TKey*, *TValue*)<br />The dictionary to wrap.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>TKey</dt><dd>The type of keys in the dictionary.</dd><dt>TValue</dt><dd>The type of values in the dictionary.</dd></dl>

#### Return Value
Type: ReadOnlyDictionary(*TKey*, *TValue*)<br />A new instance of ReadOnlyDictionary(TKey, TValue)

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type IDictionary(*TKey*, *TValue*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*dictionary* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />