# Factory.IsSingleton Method (Type)
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns a value that indicates if the contract is a singleton or not

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static Nullable<bool> IsSingleton(
	Type contractType
)
```


#### Parameters
&nbsp;<dl><dt>contractType</dt><dd>Type: System.Type<br />The type that the contract name derives from</dd></dl>

#### Return Value
Type: Nullable(Boolean)<br />Returns null if the *contractType* does not exist

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>The parameter *contractType* is null</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Factory">Factory Class</a><br /><a href="Overload_Couldron_Factory_IsSingleton">IsSingleton Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />