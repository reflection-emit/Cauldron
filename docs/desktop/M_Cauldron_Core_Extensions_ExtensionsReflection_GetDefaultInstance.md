# ExtensionsReflection.GetDefaultInstance Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Retrieves the default value for a given Type. 

 http://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime By Mark Jones

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Object GetDefaultInstance(
	this Type type
)
```


#### Parameters
&nbsp;<dl><dt>type</dt><dd>Type: System.Type<br />The Type for which to get the default value</dd></dl>

#### Return Value
Type: Object<br />The default value for *type*

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Type. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Remarks
If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null. If a value type is supplied which is not publicly visible or which contains generic parameters, this method will fail with an exception.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />