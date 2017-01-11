# Extensions.IsDerivedFrom(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Checks if an object is not compatible (does not derive) with a given type.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool IsDerivedFrom<T>(
	this Object target
)

```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Object<br />The object to be tested</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type</dd></dl>

#### Return Value
Type: Boolean<br />Returns true if the object cannot be casted to *T*, otherwise false.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Object. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_Extensions">Extensions Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />