# YamlConvert.DeserializeObject(*T*) Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Deserializes the YAML to the specified .NET type.

**Namespace:**&nbsp;<a href="N_Cauldron_Core_Yaml">Cauldron.Core.Yaml</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public static T[] DeserializeObject<T>(
	string value
)
where T : class, new()

```


#### Parameters
&nbsp;<dl><dt>value</dt><dd>Type: System.String<br />The YAML to deserialize.</dd></dl>

#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of the object to deserialize to.</dd></dl>

#### Return Value
Type: *T*[]<br />The deserialized object from the YAML string.

## See Also


#### Reference
<a href="T_Cauldron_Core_Yaml_YamlConvert">YamlConvert Class</a><br /><a href="N_Cauldron_Core_Yaml">Cauldron.Core.Yaml Namespace</a><br />