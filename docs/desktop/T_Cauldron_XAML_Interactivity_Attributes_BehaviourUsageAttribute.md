# BehaviourUsageAttribute Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Specifies that a behaviour can only be applied once


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;System.Attribute<br />&nbsp;&nbsp;&nbsp;&nbsp;Cauldron.XAML.Interactivity.Attributes.BehaviourUsageAttribute<br />
**Namespace:**&nbsp;<a href="N_Cauldron_XAML_Interactivity_Attributes">Cauldron.XAML.Interactivity.Attributes</a><br />**Assembly:**&nbsp;Cauldron.XAML.Interactivity (in Cauldron.XAML.Interactivity.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
[AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class BehaviourUsageAttribute : Attribute
```

The BehaviourUsageAttribute type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Interactivity_Attributes_BehaviourUsageAttribute__ctor">BehaviourUsageAttribute()</a></td><td>
Initializes a new instance of BehaviourUsageAttribute

 The <a href="P_Cauldron_XAML_Interactivity_Attributes_BehaviourUsageAttribute_AllowMultiple">AllowMultiple</a> property is true</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Interactivity_Attributes_BehaviourUsageAttribute__ctor_1">BehaviourUsageAttribute(Boolean)</a></td><td>
Initializes a new instance of BehaviourUsageAttribute</td></tr></table>&nbsp;
<a href="#behaviourusageattribute-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_Interactivity_Attributes_BehaviourUsageAttribute_AllowMultiple">AllowMultiple</a></td><td>
Gets a value that indicates if a behaviour can be applied once or multiple times</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>TypeId</td><td>
When implemented in a derived class, gets a unique identifier for this Attribute.
 (Inherited from Attribute.)</td></tr></table>&nbsp;
<a href="#behaviourusageattribute-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td>
Returns a value that indicates whether this instance is equal to a specified object.
 (Inherited from Attribute.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td>
Returns the hash code for this instance.
 (Inherited from Attribute.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td>
Gets the Type of the current instance.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>IsDefaultAttribute</td><td>
When overridden in a derived class, indicates whether the value of this instance is the default value for the derived class.
 (Inherited from Attribute.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Match</td><td>
When overridden in a derived class, returns a value that indicates whether this instance equals a specified object.
 (Inherited from Attribute.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td>
Creates a shallow copy of the current Object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td>
Returns a string that represents the current object.
 (Inherited from Object.)</td></tr></table>&nbsp;
<a href="#behaviourusageattribute-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_As__1">As(T)</a></td><td>
Performs a cast between compatible reference types. If a convertion is not possible then null is returned.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension_CreateObject__1">CreateObject(T)</a></td><td> (Defined by <a href="T_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension">AnonymousTypeWithInterfaceExtension</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyNonPublicValue__1">GetPropertyNonPublicValue(T)</a></td><td>
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and NonPublic
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue">GetPropertyValue(String, BindingFlags)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1">GetPropertyValue(T)(String)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and Public
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1_1">GetPropertyValue(T)(String, BindingFlags)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_IsDerivedFrom__1">IsDerivedFrom(T)</a></td><td>
Checks if an object is not compatible (does not derive) with a given type.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Activator_ExtensionsCloning_MapTo__1">MapTo(T)</a></td><td> (Defined by <a href="T_Cauldron_Activator_ExtensionsCloning">ExtensionsCloning</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_ToLong_1">ToLong</a></td><td>
Tries to convert a Object to an Int64
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_TryDispose">TryDispose</a></td><td>
Tries to performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr></table>&nbsp;
<a href="#behaviourusageattribute-class">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.GetIDsOfNames</td><td>
Maps a set of names to a corresponding set of dispatch identifiers.
 (Inherited from Attribute.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.GetTypeInfo</td><td>
Retrieves the type information for an object, which can be used to get the type information for an interface.
 (Inherited from Attribute.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.GetTypeInfoCount</td><td>
Retrieves the number of type information interfaces that an object provides (either 0 or 1).
 (Inherited from Attribute.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>_Attribute.Invoke</td><td>
Provides access to properties and methods exposed by an object.
 (Inherited from Attribute.)</td></tr></table>&nbsp;
<a href="#behaviourusageattribute-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_XAML_Interactivity_Attributes">Cauldron.XAML.Interactivity.Attributes Namespace</a><br />