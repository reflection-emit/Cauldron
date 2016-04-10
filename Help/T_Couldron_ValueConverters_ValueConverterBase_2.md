# ValueConverterBase(*TInput*, *TOutput*) Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Provides a base class for Binding value converters.


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Couldron.ValueConverters.ValueConverterBase(TInput, TOutput)<br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="#inheritance-hierarchy" />
**Namespace:**&nbsp;<a href="N_Couldron_ValueConverters">Couldron.ValueConverters</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public abstract class ValueConverterBase<TInput, TOutput> : IValueConverter

```


#### Type Parameters
&nbsp;<dl><dt>TInput</dt><dd>The Type of the value produced by the binding source</dd><dt>TOutput</dt><dd>The Type of the value produced by the binding target</dd></dl>&nbsp;
The ValueConverterBase(TInput, TOutput) type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ValueConverters_ValueConverterBase_2__ctor">ValueConverterBase(TInput, TOutput)</a></td><td /></tr></table>&nbsp;
<a href="#valueconverterbase(*tinput*,-*toutput*)-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ValueConverters_ValueConverterBase_2_Convert">Convert</a></td><td>
Converts a value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_ValueConverters_ValueConverterBase_2_ConvertBack">ConvertBack</a></td><td>
Converts a value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ValueConverters_ValueConverterBase_2_OnConvert">OnConvert</a></td><td>
Converts a value</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_ValueConverters_ValueConverterBase_2_OnConvertBack">OnConvertBack</a></td><td>
Converts a value</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td> (Inherited from Object.)</td></tr></table>&nbsp;
<a href="#valueconverterbase(*tinput*,-*toutput*)-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_CastTo__1">CastTo(T)</a></td><td>
Performs certain types of conversions between compatible reference types or nullable types 

 Returns null if convertion was not successfull
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_DisposeAll">DisposeAll</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface. 

 If the object is a FrameworkElement it will try to find known diposable attached properties. 

 It will also dispose the the DataContext content.
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToBool">ToBool</a></td><td>
Tries to convert an Object to a Boolean
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToDouble">ToDouble</a></td><td>
Tries to convert a Object to an Double
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToInteger">ToInteger</a></td><td>
Tries to convert a Object to an Int32
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_ExtensionConvertions_ToString2">ToString2</a></td><td>
Returns a string that represents the current object. 

 If the object is null a Empty will be returned
 (Defined by <a href="T_Couldron_ExtensionConvertions">ExtensionConvertions</a>.)</td></tr></table>&nbsp;
<a href="#valueconverterbase(*tinput*,-*toutput*)-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron_ValueConverters">Couldron.ValueConverters Namespace</a><br />

## Inheritance HierarchySystem.Object<br />&nbsp;&nbsp;Couldron.ValueConverters.ValueConverterBase(TInput, TOutput)<br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_BooleanInvertConverter">Couldron.ValueConverters.BooleanInvertConverter</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_BooleanToVisibilityConverter">Couldron.ValueConverters.BooleanToVisibilityConverter</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_CollectionHasElementsToBoolConverter">Couldron.ValueConverters.CollectionHasElementsToBoolConverter</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_KeyToLocalizedStringConverter">Couldron.ValueConverters.KeyToLocalizedStringConverter</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_NullOrEmptyTovisibilityConverter">Couldron.ValueConverters.NullOrEmptyTovisibilityConverter</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_ObjectToBooleanConverter">Couldron.ValueConverters.ObjectToBooleanConverter</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_ObjectToVisibilityConverter">Couldron.ValueConverters.ObjectToVisibilityConverter</a><br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_ValueConverters_StringFormatConverter">Couldron.ValueConverters.StringFormatConverter</a><br />