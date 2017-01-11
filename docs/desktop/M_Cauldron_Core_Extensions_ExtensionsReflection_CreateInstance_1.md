# ExtensionsReflection.CreateInstance Method (Type, Object[])
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates an instance of the specified type using the constructor that best matches the specified parameters.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static Object CreateInstance(
	this Type type,
	params Object[] args
)
```


#### Parameters
&nbsp;<dl><dt>type</dt><dd>Type: System.Type<br />The type of object to create.</dd><dt>args</dt><dd>Type: System.Object[]<br />An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If args is an empty array or null, the constructor that takes no parameters (the default constructor) is invoked.</dd></dl>

#### Return Value
Type: Object<br />A reference to the newly created object.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Type. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*type* is null</td></tr><tr><td><a href="T_Cauldron_Core_CreateInstanceIsAnInterfaceException">CreateInstanceIsAnInterfaceException</a></td><td>*type* is an interface</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="Overload_Cauldron_Core_Extensions_ExtensionsReflection_CreateInstance">CreateInstance Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />