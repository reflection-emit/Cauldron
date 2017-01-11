# UnknownParameterException Class
 _**\[This is preliminary documentation and is subject to change.\]**_

\[Missing <summary> documentation for "T:Cauldron.Consoles.UnknownParameterException"\]


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;System.Exception<br />&nbsp;&nbsp;&nbsp;&nbsp;Cauldron.Consoles.UnknownParameterException<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Consoles">Cauldron.Consoles</a><br />**Assembly:**&nbsp;Cauldron.Consoles (in Cauldron.Consoles.dll) Version: 1.0.0.2 (1.0.0.2)

## Syntax

**C#**<br />
``` C#
public sealed class UnknownParameterException : Exception
```

The UnknownParameterException type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Consoles_UnknownParameterException__ctor">UnknownParameterException</a></td><td>
Initializes a new instance of the UnknownParameterException class</td></tr></table>&nbsp;
<a href="#unknownparameterexception-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Data</td><td>
Gets a collection of key/value pairs that provide additional user-defined information about the exception.
 (Inherited from Exception.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>HelpLink</td><td>
Gets or sets a link to the help file associated with this exception.
 (Inherited from Exception.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>HResult</td><td>
Gets or sets HRESULT, a coded numerical value that is assigned to a specific exception.
 (Inherited from Exception.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>InnerException</td><td>
Gets the Exception instance that caused the current exception.
 (Inherited from Exception.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Message</td><td>
Gets a message that describes the current exception.
 (Inherited from Exception.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Consoles_UnknownParameterException_Parameter">Parameter</a></td><td /></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Source</td><td>
Gets or sets the name of the application or the object that causes the error.
 (Inherited from Exception.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>StackTrace</td><td>
Gets a string representation of the immediate frames on the call stack.
 (Inherited from Exception.)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>TargetSite</td><td>
Gets the method that throws the current exception.
 (Inherited from Exception.)</td></tr></table>&nbsp;
<a href="#unknownparameterexception-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetBaseException</td><td>
When overridden in a derived class, returns the Exception that is the root cause of one or more subsequent exceptions.
 (Inherited from Exception.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td>
Serves as the default hash function.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetObjectData</td><td>
When overridden in a derived class, sets the SerializationInfo with information about the exception.
 (Inherited from Exception.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td>
Gets the runtime type of the current instance.
 (Inherited from Exception.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td>
Creates a shallow copy of the current Object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td>
Creates and returns a string representation of the current exception.
 (Inherited from Exception.)</td></tr></table>&nbsp;
<a href="#unknownparameterexception-class">Back to Top</a>

## Events
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Protected event](media/protevent.gif "Protected event")</td><td>SerializeObjectState</td><td>
Occurs when an exception is serialized to create an exception state object that contains serialized data about the exception.
 (Inherited from Exception.)</td></tr></table>&nbsp;
<a href="#unknownparameterexception-class">Back to Top</a>

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
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_GetStackTrace">GetStackTrace</a></td><td>
Gets the stacktrace of the exception and the inner exceptions recursively
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_IsDerivedFrom__1">IsDerivedFrom(T)</a></td><td>
Checks if an object is not compatible (does not derive) with a given type.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Activator_ExtensionsCloning_MapTo__1">MapTo(T)</a></td><td> (Defined by <a href="T_Cauldron_Activator_ExtensionsCloning">ExtensionsCloning</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_ToLong_1">ToLong</a></td><td>
Tries to convert a Object to an Int64
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_TryDispose">TryDispose</a></td><td>
Tries to performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr></table>&nbsp;
<a href="#unknownparameterexception-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Consoles">Cauldron.Consoles Namespace</a><br />