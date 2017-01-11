# Extensions.SliceColumn(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns the elements of the first dimension of a multidimensional array

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<T> SliceColumn<T>(
	this T[,] array,
	int column
)

```


#### Parameters
&nbsp;<dl><dt>array</dt><dd>Type: *T*[,]<br />The array to get the dimension from</dd><dt>column</dt><dd>Type: System.Int32<br />The second dimension of the array</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type that is contained in the array</dd></dl>

#### Return Value
Type: IEnumerable(*T*)<br />The second dimension of the array depending on the *column*

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />