# ComparerUtils.GreaterThan Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Determines whether *a* is greater than *b*

 Checks reference equality first with ReferenceEquals(Object, Object). Then it checks all known types with the > operator, then with reflection on 'op_GreaterThan'

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool GreaterThan(
	Object a,
	Object b
)
```


#### Parameters
&nbsp;<dl><dt>a</dt><dd>Type: System.Object<br />The first object to compare</dd><dt>b</dt><dd>Type: System.Object<br />The second object to compare</dd></dl>

#### Return Value
Type: Boolean<br />true if *a* is greater than *b*; otherwise, false.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>Greater than operator cannot be applied</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_ComparerUtils">ComparerUtils Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />