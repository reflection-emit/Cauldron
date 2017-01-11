# ExtensionsReflection.GetMethod Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns all the methods of the current Type, using the specified binding constraints including those of the base classes.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static MethodInfo GetMethod(
	this Type type,
	string methodName,
	Type[] parameterTypes,
	BindingFlags bindingFlags
)
```


#### Parameters
&nbsp;<dl><dt>type</dt><dd>Type: System.Type<br />The Type that contains the method</dd><dt>methodName</dt><dd>Type: System.String<br />The name of the method to get</dd><dt>parameterTypes</dt><dd>Type: System.Type[]<br />The type of parameters the method should have</dd><dt>bindingFlags</dt><dd>Type: System.Reflection.BindingFlags<br />A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</dd></dl>

#### Return Value
Type: MethodInfo<br />A MethodInfo object representing the method defined for the current Type that match the specified binding constraint.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Type. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />