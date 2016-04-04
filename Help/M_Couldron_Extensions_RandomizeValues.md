# Extensions.RandomizeValues Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Replaces the values of data in memory with random values. The GC handle will be freed.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static void RandomizeValues(
	this GCHandle target,
	int targetLength
)
```


#### Parameters
&nbsp;<dl><dt>target</dt><dd>Type: System.Runtime.InteropServices.GCHandle<br /></dd><dt>targetLength</dt><dd>Type: System.Int32<br /></dd></dl>

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type GCHandle. When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## Remarks
Will only work on Pinned

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />