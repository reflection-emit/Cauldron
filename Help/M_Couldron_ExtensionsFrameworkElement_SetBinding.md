# ExtensionsFrameworkElement.SetBinding Method (DependencyObject, DependencyProperty, Object, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Attaches a binding to this element, based on the provided source property name as a path qualification to the data source.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void SetBinding(
	this DependencyObject dependencyObject,
	DependencyProperty dp,
	Object source,
	string propertyPath
)
```


#### Parameters
&nbsp;<dl><dt>dependencyObject</dt><dd>Type: System.Windows.DependencyObject<br />The DependencyObject that is extended</dd><dt>dp</dt><dd>Type: System.Windows.DependencyProperty<br />Identifies the destination property where the binding should be established</dd><dt>source</dt><dd>Type: System.Object<br />The object to use as the binding source.</dd><dt>propertyPath</dt><dd>Type: System.String<br />The path to the binding source property.</dd></dl>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type DependencyObject. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_ExtensionsFrameworkElement">ExtensionsFrameworkElement Class</a><br /><a href="Overload_Couldron_ExtensionsFrameworkElement_SetBinding">SetBinding Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />