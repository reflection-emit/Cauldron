# ExtensionsFrameworkElement.Content(*T*, *TContent*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void Content<T, TContent>(
	this T element,
	Action<TContent> action
)
where T : ContentPresenter
where TContent : FrameworkElement

```


#### Parameters
&nbsp;<dl><dt>element</dt><dd>Type: *T*<br /></dd><dt>action</dt><dd>Type: System.Action(*TContent*)<br /></dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd /><dt>TContent</dt><dd /></dl>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type . When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_ExtensionsFrameworkElement">ExtensionsFrameworkElement Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />