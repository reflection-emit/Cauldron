# ISerializer.DeserializeAsync(*T*) Method (DirectoryInfo, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Deserializes an object.

**Namespace:**&nbsp;<a href="N_Cauldron_Potions">Cauldron.Potions</a><br />**Assembly:**&nbsp;Cauldron.Potions (in Cauldron.Potions.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
Task<T> DeserializeAsync<T>(
	DirectoryInfo folder,
	string name
)
where T : class

```


#### Parameters
&nbsp;<dl><dt>folder</dt><dd>Type: System.IO.DirectoryInfo<br />The directory where the file resides</dd><dt>name</dt><dd>Type: System.String<br />The name of the file</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>\[Missing <typeparam name="T"/> documentation for "M:Cauldron.Potions.ISerializer.DeserializeAsync``1(System.IO.DirectoryInfo,System.String)"\]</dd></dl>

#### Return Value
Type: Task(*T*)<br />An instance of the deserialized object

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*name* is null</td></tr><tr><td>NotSupportedException</td><td>*name* is a value type</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Potions_ISerializer">ISerializer Interface</a><br /><a href="Overload_Cauldron_Potions_ISerializer_DeserializeAsync">DeserializeAsync Overload</a><br /><a href="N_Cauldron_Potions">Cauldron.Potions Namespace</a><br />