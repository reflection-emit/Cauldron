# LessThanAttribute.OnValidate Method 
 _**\[This is preliminary documentation and is subject to change.\]**_

Invokes the validation on the specified context with the specified parameters

**Namespace:**&nbsp;<a href="N_Couldron_Validation">Couldron.Validation</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
protected override bool OnValidate(
	PropertyInfo sender,
	IValidatableViewModel context,
	PropertyInfo propertyInfo,
	Object value
)
```


#### Parameters
&nbsp;<dl><dt>sender</dt><dd>Type: System.Reflection.PropertyInfo<br />The property that invoked the validation</dd><dt>context</dt><dd>Type: <a href="T_Couldron_ViewModels_IValidatableViewModel">Couldron.ViewModels.IValidatableViewModel</a><br />The Viewmodel context that has to be validated</dd><dt>propertyInfo</dt><dd>Type: System.Reflection.PropertyInfo<br />The PropertyInfo of the validated property</dd><dt>value</dt><dd>Type: System.Object<br />The value of the property</dd></dl>

#### Return Value
Type: Boolean<br />Has to return true on validation error otherwise false

## See Also


#### Reference
<a href="T_Couldron_Validation_LessThanAttribute">LessThanAttribute Class</a><br /><a href="N_Couldron_Validation">Couldron.Validation Namespace</a><br />