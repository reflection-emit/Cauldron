# ExtensionsReflection Methods
 _**\[This is preliminary documentation and is subject to change.\]**_

The <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a> type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_AreReferenceAssignable">AreReferenceAssignable</a></td><td>
Returns a value that indicates whether the specified type can be assigned to the current type.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_CreateInstance">CreateInstance(ConstructorInfo, Object[])</a></td><td>
Creates an instance of the specified type using the constructor</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_CreateInstance_1">CreateInstance(Type, Object[])</a></td><td>
Creates an instance of the specified type using the constructor that best matches the specified parameters.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetChildrenType">GetChildrenType</a></td><td>
Returns the type of T in an IEnumerable(T) implementation</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetCustomAttribute__2">GetCustomAttribute(TAttib, TEnum)</a></td><td>
Gets the attribute of an enum member</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetDefaultInstance">GetDefaultInstance</a></td><td>
Retrieves the default value for a given Type. 

 http://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime By Mark Jones</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetDictionaryKeyValueTypes">GetDictionaryKeyValueTypes</a></td><td>
Returns the type of TKey and TValue in an IDictionary(TKey, TValue) implementation</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetDisplayName__1">GetDisplayName(TEnum)</a></td><td>
Returns the name value of the <a href="T_Cauldron_Core_DisplayNameAttribute">DisplayNameAttribute</a> of the enum member</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetFieldsEx">GetFieldsEx</a></td><td>
Returns all the fields of the current Type, using the specified binding constraints including those of the base classes.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetMethod">GetMethod</a></td><td>
Returns all the methods of the current Type, using the specified binding constraints including those of the base classes.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetMethodEx">GetMethodEx</a></td><td>
Searches for the specified public method whose parameters match the specified argument types.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetMethodsEx">GetMethodsEx</a></td><td>
Returns all the methods of the current Type, using the specified binding constraints including those of the base classes.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertiesEx">GetPropertiesEx</a></td><td>
Returns all the properties of the current Type, using the specified binding constraints including those of the base classes.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyEx">GetPropertyEx</a></td><td>
Gets a specific property of the current Type. 

 This method will try to find the exact property if an AmbiguousMatchException occures</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyFromPath">GetPropertyFromPath</a></td><td>
Tries to find a property defined by a path</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyNonPublicValue__1">GetPropertyNonPublicValue(T)</a></td><td>
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and NonPublic</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue">GetPropertyValue(Object, String, BindingFlags)</a></td><td>
Searches for the specified property, using the specified binding constraints and returns its value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1">GetPropertyValue(T)(Object, String)</a></td><td>
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and Public</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1_1">GetPropertyValue(T)(Object, String, BindingFlags)</a></td><td>
Searches for the specified property, using the specified binding constraints and returns its value.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_ImplementsInterface">ImplementsInterface(TypeInfo, Type)</a></td><td>
Checks if the type has implemented the defined interface</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_ImplementsInterface_1">ImplementsInterface(Type, Type)</a></td><td>
Checks if the type has implemented the defined interface</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_ImplementsInterface__1">ImplementsInterface(T)(TypeInfo)</a></td><td>
Checks if the type has implemented the defined interface</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_ImplementsInterface__1_1">ImplementsInterface(T)(Type)</a></td><td>
Checks if the type has implemented the defined interface</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_IsCollectionOrList">IsCollectionOrList</a></td><td>
Checks if the type implements the ICollection or IList interface</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_IsNullable">IsNullable</a></td><td>
Gets a value indicating whether the current type is a Nullable(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_MatchesArgumentTypes">MatchesArgumentTypes</a></td><td>
Returns true if the argument types defined by *argumentTypes* matches with the argument types of *method*</td></tr></table>&nbsp;
<a href="#extensionsreflection-methods">Back to Top</a>

## See Also


#### Reference
<a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection Class</a><br /><a href="N_Cauldron_Core_Extensions">Cauldron.Core.Extensions Namespace</a><br />