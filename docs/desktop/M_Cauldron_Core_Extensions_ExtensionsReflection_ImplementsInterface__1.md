# ExtensionsReflection.ImplementsInterface(*T*) Method (TypeInfo)
 _**\[This is preliminary documentation and is subject to change.\]**_

Checks if the type has implemented the defined interface

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool ImplementsInterface<T>(
	this TypeInfo typeInfo
)

```


#### Parameters
&nbsp;<dl><dt>typeInfo</dt><dd>Type: System.Reflection.TypeInfo<br />The type that may implements the interface *T*</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of interface to look for</dd></dl>

#### Return Value
Type: Boolean<br />True if the *typeInfo* has implemented the interface *T*

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type TypeInfo. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>The type *T* is not an interface</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="Overload_Cauldron_Core_Extensions_ExtensionsReflection_ImplementsInterface">ImplementsInterface Overload</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />