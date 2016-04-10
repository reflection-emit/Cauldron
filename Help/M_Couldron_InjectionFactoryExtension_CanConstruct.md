# InjectionFactoryExtension.CanConstruct Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Returns true if a Type can be constructed with this <a href="T_Couldron_IFactoryExtension">IFactoryExtension</a> implementation

**Namespace:**&nbsp;<a href="N_Couldron">Couldron</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public bool CanConstruct(
	Type objectType,
	TypeInfo objectTypeInfo
)
```


#### Parameters
&nbsp;<dl><dt>objectType</dt><dd>Type: System.Type<br />The Type of the object created</dd><dt>objectTypeInfo</dt><dd>Type: System.Reflection.TypeInfo<br />The TypeInfo of the object instance</dd></dl>

#### Return Value
Type: Boolean<br />True if can be constructed

#### Implements
<a href="M_Couldron_IFactoryExtension_CanConstruct">IFactoryExtension.CanConstruct(Type, TypeInfo)</a><br />

## See Also


#### Reference
<a href="T_Couldron_InjectionFactoryExtension">InjectionFactoryExtension Class</a><br /><a href="N_Couldron">Couldron Namespace</a><br />