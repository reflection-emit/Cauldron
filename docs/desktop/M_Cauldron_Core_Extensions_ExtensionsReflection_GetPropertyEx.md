# ExtensionsReflection.GetPropertyEx Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Gets a specific property of the current Type. 

 This method will try to find the exact property if an AmbiguousMatchException occures

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static PropertyInfo GetPropertyEx(
	this Type type,
	string propertyName,
	BindingFlags bindingFlags = BindingFlags.Default|BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public
)
```


#### Parameters
&nbsp;<dl><dt>type</dt><dd>Type: System.Type<br />The Type that contains the property</dd><dt>propertyName</dt><dd>Type: System.String<br />The string containing the name of the property to get</dd><dt>bindingFlags (Optional)</dt><dd>Type: System.Reflection.BindingFlags<br />A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</dd></dl>

#### Return Value
Type: PropertyInfo<br />An object representing the public property with the specified name, if found; otherwise, null.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Type. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*type* is null</td></tr><tr><td>ArgumentNullException</td><td>*propertyName* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />