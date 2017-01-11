# Assemblies.GetTypesImplementsInterface(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns all Types that implements the interface *T*

**Namespace:**&nbsp;<a href="N_Cauldron_Core">Cauldron.Core</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static IEnumerable<TypeInfo> GetTypesImplementsInterface<T>()

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The interface Type</dd></dl>

#### Return Value
Type: IEnumerable(TypeInfo)<br />A colletion of Type that implements the interface *T* otherwise null

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>*T* is not an interface</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Core_Assemblies">Assemblies Class</a><br /><a href="N_Cauldron_Core">Cauldron.Core Namespace</a><br />