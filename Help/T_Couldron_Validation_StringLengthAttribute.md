# StringLengthAttribute Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Validates a property for the string length. 

 A String that is shorter or longer than the specified length will cause a validation error


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;System.Attribute<br />&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Couldron_Validation_ValidationBaseAttribute">Couldron.Validation.ValidationBaseAttribute</a><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Couldron.Validation.StringLengthAttribute<br />
**Namespace:**&nbsp;<a href="N_Couldron_Validation">Couldron.Validation</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
[AttributeUsageAttribute(AttributeTargets.Property, AllowMultiple = false, 
	Inherited = false)]
public sealed class StringLengthAttribute : ValidationBaseAttribute
```

The StringLengthAttribute type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Validation_StringLengthAttribute__ctor_1">StringLengthAttribute(Int32, String)</a></td><td>
Initializes a new instance of StringLengthAttribute</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Validation_StringLengthAttribute__ctor">StringLengthAttribute(Int32, Int32, String)</a></td><td>
Initializes a new instance of StringLengthAttribute</td></tr></table>&nbsp;
<a href="#stringlengthattribute-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_Validation_ValidationBaseAttribute_ErrorMessageKey">ErrorMessageKey</a></td><td>
Gets or sets the key of the localized error message string
 (Inherited from <a href="T_Couldron_Validation_ValidationBaseAttribute">ValidationBaseAttribute</a>.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>TypeId</td><td> (Inherited from Attribute.)</td></tr></table>&nbsp;
<a href="#stringlengthattribute-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td> (Inherited from Attribute.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td> (Inherited from Attribute.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>IsDefaultAttribute</td><td> (Inherited from Attribute.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Match</td><td> (Inherited from Attribute.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_Validation_StringLengthAttribute_OnValidate">OnValidate</a></td><td>
Invokes the validation on the specified context with the specified parameters
 (Overrides <a href="M_Couldron_Validation_ValidationBaseAttribute_OnValidate">ValidationBaseAttribute.OnValidate(PropertyInfo, IValidatableViewModel, PropertyInfo, Object)</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Couldron_Validation_ValidationBaseAttribute_ValidationMessage">ValidationMessage</a></td><td>
Occures on validation 

 Can be used to modify the validation error message.
 (Inherited from <a href="T_Couldron_Validation_ValidationBaseAttribute">ValidationBaseAttribute</a>.)</td></tr></table>&nbsp;
<a href="#stringlengthattribute-class">Back to Top</a>

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
<a href="#stringlengthattribute-class">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.GetIDsOfNames</td><td> (Inherited from Attribute.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.GetTypeInfo</td><td> (Inherited from Attribute.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.GetTypeInfoCount</td><td> (Inherited from Attribute.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.Invoke</td><td> (Inherited from Attribute.)</td></tr></table>&nbsp;
<a href="#stringlengthattribute-class">Back to Top</a>

## Exceptions
&nbsp;<table><tr><th>Exception</th><th>Condition</th></tr><tr><td>ArgumentException</td><td>If the property is applied to a type that is not a string</td></tr></table>

## See Also


#### Reference
<a href="N_Couldron_Validation">Couldron.Validation Namespace</a><br />