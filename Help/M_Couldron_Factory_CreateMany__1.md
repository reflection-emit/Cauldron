# Factory.CreateMany(*T*) Method (Object[])
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates instances of the specified type depending on the <a href="T_Couldron_FactoryAttribute">FactoryAttribute</a>

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<T> CreateMany<T>(
	params Object[] parameters
)

```


#### Parameters
&nbsp;<dl><dt>parameters</dt><dd>Type: System.Object[]<br />An array of arguments that match in number, order, and type the parameters of the constructor to invoke. If args is an empty array or null, the constructor that takes no parameters (the default constructor) is invoked.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The Type that contract name derives from</dd></dl>

#### Return Value
Type: IEnumerable(*T*)<br />A collection of the newly created objects.

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>KeyNotFoundException</td><td>The contract described by *T* was not found</td></tr><tr><td>Exception</td><td>Unknown <a href="T_Couldron_FactoryCreationPolicy">FactoryCreationPolicy</a></td></tr><tr><td>NotSupportedException</td><td>An Object with <a href="T_Couldron_FactoryCreationPolicy">Singleton</a> with an implemented IDisposable must also implement the <a href="T_Couldron_Core_IDisposableObject">IDisposableObject</a> interface.</td></tr></table>

## See Also


#### Reference
<a href="T_Couldron_Factory">Factory Class</a><br /><a href="Overload_Couldron_Factory_CreateMany">CreateMany Overload</a><br /><a href="N_Couldron">Couldron Namespace</a><br />