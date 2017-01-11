# ExtensionsReflection.GetPropertyNonPublicValue(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and NonPublic

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static T GetPropertyNonPublicValue<T>(
	this Object obj,
	string propertyName
)

```


#### Parameters
&nbsp;<dl><dt>obj</dt><dd>Type: System.Object<br />The Object to retrieve the value from</dd><dt>propertyName</dt><dd>Type: System.String<br />The string containing the name of the property to get.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The property's return value type</dd></dl>

#### Return Value
Type: *T*<br />The property value of the specified object.

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Object. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*obj* is null</td></tr><tr><td>NullReferenceException</td><td>The property defined by *propertyName* was not found</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />