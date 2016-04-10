# Mathc.Substract Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Substracts *b* from *a*

 If *a* and *b* are null then null is returned. If *a* is null then 0 is returned. If *b* is null then *a* is returned; 

 Tries to cast primitiv Type and use the - operator. If the Type is unknown then reflection is used to determin the operator.

**Namespace:**&nbsp;<a href="N_Couldron_Core">Couldron.Core</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static Object Substract(
	Object a,
	Object b
)
```


#### Parameters
&nbsp;<dl><dt>a</dt><dd>Type: System.Object<br />The minuend</dd><dt>b</dt><dd>Type: System.Object<br />The substrahend</dd></dl>

#### Return Value
Type: Object<br />Returns the difference of the substraction

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>Operator cannot be applied</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Core_Mathc">Mathc Class</a><br /><a href="N_Couldron_Core">Couldron.Core Namespace</a><br />