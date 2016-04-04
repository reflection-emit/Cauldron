# Extensions.CastTo(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Performs certain types of conversions between compatible reference types or nullable types 

 Returns null if convertion was not successfull

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static T CastTo<T>(
	this Object target
)
where T : class

```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Object<br />The object to convert</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type to convert the *target* to</dd></dl>

#### Return Value
Type: *T*<br />The converted object

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type Object. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />