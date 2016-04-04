# Utils.Equals Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Determines whether the specified object is equal to the current object. 

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all primitiv types with the == operator and as last resort uses Equals(Object, Object) to determine equality

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static bool Equals(
	Object a,
	Object b
)
```


#### Parameters
&nbsp;<dl><dt>a</dt><dd>Type: System.Object<br />The first object to compare</dd><dt>b</dt><dd>Type: System.Object<br />The second object to compare</dd></dl>

#### Return Value
Type: Boolean<br />true if the specified object is equal to the current object; otherwise, false.

## See Also


#### Reference
<a href="T_Couldron_Utils">Utils Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />