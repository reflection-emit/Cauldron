# IFactoryExtension.Construct Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Creates an Object described by *objectType* or *objectTypeInfo*.

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
Object Construct(
	Type objectType,
	TypeInfo objectTypeInfo
)
```


#### Parameters
&nbsp;<dl><dt>objectType</dt><dd>Type: System.Type<br />The Type of the object created</dd><dt>objectTypeInfo</dt><dd>Type: System.Reflection.TypeInfo<br />The TypeInfo of the object instance</dd></dl>

#### Return Value
Type: Object<br />The new instance of *objectType* or *objectTypeInfo*

## See Also


#### Reference
<a href="T_Couldron_IFactoryExtension">IFactoryExtension Interface</a><br /><a href="N_Couldron">Couldron Namespace</a><br />