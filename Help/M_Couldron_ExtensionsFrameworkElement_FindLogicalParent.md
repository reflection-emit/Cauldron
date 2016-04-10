# ExtensionsFrameworkElement.FindLogicalParent Method (DependencyObject, Type)
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns the parent object with the specified type of the specified object by processing the logical tree.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static DependencyObject FindLogicalParent(
	this DependencyObject element,
	Type dependencyObjectType
)
```


#### Parameters
&nbsp;<dl><dt>element</dt><dd>Type: System.Windows.DependencyObject<br />The object to find the parent object for. This is expected to be either a FrameworkElement or a FrameworkContentElement.</dd><dt>dependencyObjectType</dt><dd>Type: System.Type<br />The type of the parent to find</dd></dl>

#### Return Value
Type: DependencyObject<br />The requested parent object.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type DependencyObject. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_ExtensionsFrameworkElement">ExtensionsFrameworkElement Class</a><br /><a href="Overload_Couldron_ExtensionsFrameworkElement_FindLogicalParent">FindLogicalParent Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />