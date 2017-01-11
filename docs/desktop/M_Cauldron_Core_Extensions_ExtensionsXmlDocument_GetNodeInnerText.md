# ExtensionsXmlDocument.GetNodeInnerText Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets the concatenated values of the node and all its child nodes.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static string GetNodeInnerText(
	this XmlDocument target,
	string nodePath
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Xml.XmlDocument<br />The xml document</dd><dt>nodePath</dt><dd>Type: System.String<br />A node path separated by /</dd></dl>

#### Return Value
Type: String<br />The concatenated values of the node and all its child nodes.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type XmlDocument. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsXmlDocument">ExtensionsXmlDocument Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />