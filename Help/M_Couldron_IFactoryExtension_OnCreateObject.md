# IFactoryExtension.OnCreateObject Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Occures when an object is created

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
void OnCreateObject(
	Object context,
	Type objectType,
	TypeInfo objectTypeInfo
)
```


#### Parameters
&nbsp;<dl><dt>context</dt><dd>Type: System.Object<br />The object instance</dd><dt>objectType</dt><dd>Type: System.Type<br />The Type of the object created</dd><dt>objectTypeInfo</dt><dd>Type: System.Reflection.TypeInfo<br />The TypeInfo of the object instance</dd></dl>

## See Also


#### Reference
<a href="T_Couldron_IFactoryExtension">IFactoryExtension Interface</a><br /><a href="N_Couldron">Couldron Namespace</a><br />