# ComparerUtils.UnEquals(*T*, *TValue*) Method (*T*, *T*, Func(*T*, *TValue*))
 _**\[This is preliminary documentation and is subject to change.\]**_

Determines whether *a* is unequal to *b*

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static bool UnEquals<T, TValue>(
	T a,
	T b,
	Func<T, TValue> selector
)

```


#### Parameters
&nbsp;<dl><dt>a</dt><dd>Type: *T*<br />The first object to compare</dd><dt>b</dt><dd>Type: *T*<br />The second object to compare</dd><dt>selector</dt><dd>Type: System.Func(*T*, *TValue*)<br />The value selector which will be used for the compare</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the objects to be compared</dd><dt>TValue</dt><dd>The values of the object used to compare them (e.g. Hash)</dd></dl>

#### Return Value
Type: Boolean<br />true if *a* is unequal to *b*; otherwise, false.

## See Also


#### Reference
<a href="T_Cauldron_Core_ComparerUtils">ComparerUtils Class</a><br /><a href="Overload_Cauldron_Core_ComparerUtils_UnEquals">UnEquals Overload</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />