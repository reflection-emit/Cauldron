# ExtensionsConvertions.As(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Performs a cast between compatible reference types. If a convertion is not possible then null is returned.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static T As<T>(
	this Object target
)
where T : class

```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Object<br />The object to convert</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The Type to convert to</dd></dl>

#### Return Value
Type: *T*<br />The object casted to *T*

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Object. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />