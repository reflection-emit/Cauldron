# Serializer.DeserializeAsync(*T*) Method (String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Deserializes an object.

**Namespace:**&nbsp;<a href="N_Cauldron_Potions">Cauldron.Potions</a><br />**Assembly:**&nbsp;Cauldron.Potions (in Cauldron.Potions.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public Task<T> DeserializeAsync<T>(
	string name
)
where T : class

```


#### Parameters
&nbsp;<dl><dt>name</dt><dd>Type: System.String<br />The name of the file</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The object type to deserialize</dd></dl>

#### Return Value
Type: Task(*T*)<br />An instance of the deserialized object

#### Implements
<a href="M_Cauldron_Potions_ISerializer_DeserializeAsync__1_1">ISerializer.DeserializeAsync(T)(String)</a><br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>NotSupportedException</td><td>*T* is a value type</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Potions_Serializer">Serializer Class</a><br /><a href="Overload_Cauldron_Potions_Serializer_DeserializeAsync">DeserializeAsync Overload</a><br /><a href="N_Cauldron_Potions">Cauldron.Potions Namespace</a><br />