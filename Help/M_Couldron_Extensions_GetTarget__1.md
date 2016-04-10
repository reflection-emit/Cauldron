# Extensions.GetTarget(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Retrieves the target object referenced by the current WeakReference(T) object 

 Returns null if the target is not available

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static T GetTarget<T>(
	this WeakReference<T> weakReference
)
where T : class

```


#### Parameters
&nbsp;<dl><dt>weakReference</dt><dd>Type: System.WeakReference(*T*)<br />The current WeakReference(T) object</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the object referenced.</dd></dl>

#### Return Value
Type: *T*<br />Contains the target object, if it is available; otherwise null

#### Usage Note
In Visual Basic and C#, you can call this method as an instance method on any object of type WeakReference(*T*). When you use instance method syntax to call this method, omit the first parameter. For more information, see <a href="http://msdn.microsoft.com/en-us/library/bb384936.aspx">Extension Methods (Visual Basic)</a> or <a href="http://msdn.microsoft.com/en-us/library/bb383977.aspx">Extension Methods (C# Programming Guide)</a>.

## See Also


#### Reference
<a href="T_Couldron_Extensions">Extensions Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />