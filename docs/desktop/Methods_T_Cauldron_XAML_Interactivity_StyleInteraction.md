# StyleInteraction Methods
 _**\[This is preliminary documentation and is subject to change.\]**_

The <a href="T_Cauldron_XAML_Interactivity_StyleInteraction">StyleInteraction</a> type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CheckAccess</td><td>
Determines whether the calling thread has access to this DispatcherObject.
 (Inherited from DispatcherObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ClearValue(DependencyProperty)</td><td>
Clears the local value of a property. The property to be cleared is specified by a DependencyProperty identifier.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ClearValue(DependencyPropertyKey)</td><td>
Clears the local value of a read-only property. The property to be cleared is specified by a DependencyPropertyKey.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CoerceValue</td><td>
Coerces the value of the specified dependency property. This is accomplished by invoking any CoerceValueCallback function specified in property metadata for the dependency property as it exists on the calling DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td>
Determines whether a provided DependencyObject is equivalent to the current DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td>
Gets a hash code for this DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetLocalValueEnumerator</td><td>
Creates a specialized enumerator for determining which dependency properties have locally set values on this DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_XAML_Interactivity_StyleInteraction_GetTemplate">GetTemplate</a></td><td>
Gets the <a href="T_Cauldron_XAML_Interactivity_InteractionTemplate">InteractionTemplate</a> associated with a specified object.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td>
Gets the Type of the current instance.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetValue</td><td>
Returns the current effective value of a dependency property on this instance of a DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>InvalidateProperty</td><td>
Re-evaluates the effective value for the specified dependency property
 (Inherited from DependencyObject.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td>
Creates a shallow copy of the current Object.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPropertyChanged</td><td>
Invoked whenever the effective value of any dependency property on this DependencyObject has been updated. The specific dependency property that changed is reported in the event data.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ReadLocalValue</td><td>
Returns the local value of a dependency property, if it exists.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetCurrentValue</td><td>
Sets the value of a dependency property without changing its value source.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")![Static member](media/static.gif "Static member")</td><td><a href="M_Cauldron_XAML_Interactivity_StyleInteraction_SetTemplate">SetTemplate</a></td><td>
Sets the <a href="T_Cauldron_XAML_Interactivity_InteractionTemplate">InteractionTemplate</a> associated with a specified object.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetValue(DependencyProperty, Object)</td><td>
Sets the local value of a dependency property, specified by its dependency property identifier.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetValue(DependencyPropertyKey, Object)</td><td>
Sets the local value of a read-only dependency property, specified by the DependencyPropertyKey identifier of the dependency property.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>ShouldSerializeProperty</td><td>
Returns a value that indicates whether serialization processes should serialize the value for the provided dependency property.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td>
Returns a string that represents the current object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>VerifyAccess</td><td>
Enforces that the calling thread has access to this DispatcherObject.
 (Inherited from DispatcherObject.)</td></tr></table>&nbsp;
<a href="#styleinteraction-methods">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_As__1">As(T)</a></td><td>
Performs a cast between compatible reference types. If a convertion is not possible then null is returned.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension_CreateObject__1">CreateObject(T)</a></td><td> (Defined by <a href="T_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension">AnonymousTypeWithInterfaceExtension</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualChildByName">FindVisualChildByName</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualChildren">FindVisualChildren(Type)</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualChildren__1">FindVisualChildren(T)()</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualParent">FindVisualParent(Type)</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualParent__1">FindVisualParent(T)()</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualRootElement">FindVisualRootElement</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_GetDependencyProperties">GetDependencyProperties</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_GetInheritanceContext">GetInheritanceContext</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyNonPublicValue__1">GetPropertyNonPublicValue(T)</a></td><td>
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and NonPublic
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue">GetPropertyValue(String, BindingFlags)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1">GetPropertyValue(T)(String)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value. 

 Default BindingFlags are Instance and Public
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyValue__1_1">GetPropertyValue(T)(String, BindingFlags)</a></td><td>Overloaded.  
Searches for the specified property, using the specified binding constraints and returns its value.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsReflection">ExtensionsReflection</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_GetVisualChildren">GetVisualChildren</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_GetVisualParent">GetVisualParent</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_IsDerivedFrom__1">IsDerivedFrom(T)</a></td><td>
Checks if an object is not compatible (does not derive) with a given type.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Activator_ExtensionsCloning_MapTo__1">MapTo(T)</a></td><td> (Defined by <a href="T_Cauldron_Activator_ExtensionsCloning">ExtensionsCloning</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_ToLong_1">ToLong</a></td><td>
Tries to convert a Object to an Int64
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_TryDispose">TryDispose</a></td><td>
Tries to performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr></table>&nbsp;
<a href="#styleinteraction-methods">Back to Top</a>

## See Also


#### Reference
<a href="T_Cauldron_XAML_Interactivity_StyleInteraction">StyleInteraction Class</a><br /><a href="N_Cauldron_XAML_Interactivity">Cauldron.XAML.Interactivity Namespace</a><br />