# ExtensionsReflection.GetFieldsEx Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns all the fields of the current Type, using the specified binding constraints including those of the base classes.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<FieldInfo> GetFieldsEx(
	this Type type,
	BindingFlags bindingFlags = BindingFlags.Default|BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public
)
```


#### Parameters
&nbsp;<dl><dt>type</dt><dd>Type: System.Type<br />The Type that contains the fields</dd><dt>bindingFlags (Optional)</dt><dd>Type: System.Reflection.BindingFlags<br />A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</dd></dl>

#### Return Value
Type: IEnumerable(FieldInfo)<br />A collection of FieldInfo objects representing all the fields defined for the current Type.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Type. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />