# ISerializer.SerializeAsync Method (Object, DirectoryInfo, String)
 _**\[This is preliminary documentation and is subject to change.\]**_

Serializes an object.

**Namespace:**&nbsp;<a href="N_Cauldron_Potions">Cauldron.Potions</a><br />**Assembly:**&nbsp;Cauldron.Potions (in Cauldron.Potions.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
Task SerializeAsync(
	Object context,
	DirectoryInfo folder,
	string name
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: System.Object<br />The object to serialize</dd><dt>folder</dt><dd>Type: System.IO.DirectoryInfo<br />The directory where the file resides</dd><dt>name</dt><dd>Type: System.String<br />The name of the file</dd></dl>

#### Return Value
Type: Task<br />\[Missing <returns> documentation for "M:Cauldron.Potions.ISerializer.SerializeAsync(System.Object,System.IO.DirectoryInfo,System.String)"\]

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentNullException</td><td>*context* is null</td></tr><tr><td>NotSupportedException</td><td>*context* is a value type</td></tr></table>

## See Also


#### Reference
<a href="T_Cauldron_Potions_ISerializer">ISerializer Interface</a><br /><a href="Overload_Cauldron_Potions_ISerializer_SerializeAsync">SerializeAsync Overload</a><br /><a href="N_Cauldron_Potions">Cauldron.Potions Namespace</a><br />