# ExtensionsFrameworkElement.FindLogicalChildren(*T*) Method (DependencyObject)
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns all logical childs and sub child (recursively) of the element that matches the given type

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<FrameworkElement> FindLogicalChildren<T>(
	this DependencyObject element
)

```


#### Parameters
&nbsp;<dl><dt>element</dt><dd>Type: System.Windows.DependencyObject<br />The parent element</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The typ of child to search for</dd></dl>

#### Return Value
Type: IEnumerable(FrameworkElement)<br />A collection of FrameworkElement

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type DependencyObject. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_ExtensionsFrameworkElement">ExtensionsFrameworkElement Class</a><br /><a href="Overload_Couldron_ExtensionsFrameworkElement_FindLogicalChildren">FindLogicalChildren Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />