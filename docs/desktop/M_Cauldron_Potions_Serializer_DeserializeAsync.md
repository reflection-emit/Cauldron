# Serializer.DeserializeAsync Method (Type, DirectoryInfo, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Deserializes an object.

**Namespace:**&nbsp;<a href="N_Cauldron_Potions">Cauldron.Potions</a><br />**Assembly:**&nbsp;Cauldron.Potions (in Cauldron.Potions.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public Task<Object> DeserializeAsync(
	Type type,
	DirectoryInfo folder,
	string name
)
```


#### Parameters
&nbsp;<dl><dt>type</dt><dd>Type: System.Type<br />The object type to deserialize</dd><dt>folder</dt><dd>Type: System.IO.DirectoryInfo<br />The directory where the file resides</dd><dt>name</dt><dd>Type: System.String<br />The name of the file</dd></dl>

#### Return Value
Type: Task(Object)<br />An instance of the deserialized object

#### Implements
<a href="M_Cauldron_Potions_ISerializer_DeserializeAsync">ISerializer.DeserializeAsync(Type, DirectoryInfo, String)</a><br />

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*type* is null</td></tr><tr><td>NotSupportedException</td><td>*type* is a value type</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Potions_Serializer">Serializer Class</a><br /><a href="Overload_Cauldron_Potions_Serializer_DeserializeAsync">DeserializeAsync Overload</a><br /><a href="N_Cauldron_Potions">Cauldron.Potions Namespace</a><br />