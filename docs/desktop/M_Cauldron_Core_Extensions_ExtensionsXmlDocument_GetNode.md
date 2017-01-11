# ExtensionsXmlDocument.GetNode Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets a node defined by *nodePath*

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static XmlNode GetNode(
	this XmlNode target,
	string nodePath,
	string attributeName,
	string attributeValue
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Xml.XmlNode<br />The starting node</dd><dt>nodePath</dt><dd>Type: System.String<br />A node path separated by /</dd><dt>attributeName</dt><dd>Type: System.String<br />The name of the attribute to retrieve</dd><dt>attributeValue</dt><dd>Type: System.String<br />The value that the attribute *attributeName* should have</dd></dl>

#### Return Value
Type: XmlNode<br />Returns the node if successfull; otherwise null

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type XmlNode. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsXmlDocument">ExtensionsXmlDocument Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />