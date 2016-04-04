# Extensions.Move(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Moves the specified item to a new location in the collection

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void Move<T>(
	this ObservableCollection<T> source,
	T entry,
	int relativeIndex
)

```


#### Parameters
&nbsp;<dl><dt>source</dt><dd>Type: System.Collections.ObjectModel.ObservableCollection(*T*)<br />The source collection that contains the item</dd><dt>entry</dt><dd>Type: *T*<br />The item to move</dd><dt>relativeIndex</dt><dd>Type: System.Int32<br />The new position of the item relativ to its current position.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The Type of item contained in the collection</dd></dl>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type ObservableCollection(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />