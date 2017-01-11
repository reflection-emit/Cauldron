# ConcurrentList(*T*) Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Represents a thread-safe list of items that can be accessed by multiple threads concurrently.


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Cauldron.Core.Collections.ConcurrentList(T)<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public class ConcurrentList<T> : IEnumerable<T>, 
	IEnumerable, IList<T>, ICollection<T>, IList, ICollection, 
	IReadOnlyList<T>, IReadOnlyCollection<T>

```


#### Type Parameters
&nbsp;<dl><dt>T</dt><dd>The type of elements in the list.</dd></dl>&nbsp;
The ConcurrentList(T) type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1__ctor">ConcurrentList(T)()</a></td><td>
Initializes a new instance of the ConcurrentList(T) class that is empty and has the default initial capacity.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1__ctor_1">ConcurrentList(T)(IEnumerable(T))</a></td><td>
Initializes a new instance of the ConcurrentList(T) class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_Count">Count</a></td><td>
Gets the number of elements contained in the ConcurrentList(T)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_IsFixedSize">IsFixedSize</a></td><td>
Gets a value indicating whether the ConcurrentList(T) has a fixed size.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_IsReadOnly">IsReadOnly</a></td><td>
Gets a value indicating whether the ConcurrentList(T) is read-only.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_IsSynchronized">IsSynchronized</a></td><td></td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_Item">Item</a></td><td>
Gets or sets the element at the specified index.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_SyncRoot">SyncRoot</a></td><td>
Gets a value indicating whether access to the ConcurrentList(T) is synchronized (thread safe).</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Add">Add</a></td><td>
Adds an object to the end of the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_AddRange">AddRange</a></td><td>
Adds the elements of the specified collection to the end of the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Clear">Clear</a></td><td>
Removes all elements from the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Contains">Contains(Func(T, Boolean))</a></td><td>
Determines whether an element is in the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Contains_1">Contains(Object)</a></td><td>
Determines whether an element is in the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Contains_2">Contains(T)</a></td><td>
Determines whether an element is in the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_CopyTo">CopyTo(Array, Int32)</a></td><td>
Copies a range of elements from the ConcurrentList(T) to a compatible one-dimensional array, starting at the specified index of the target array.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_CopyTo_1">CopyTo(T[], Int32)</a></td><td>
Copies a range of elements from the ConcurrentList(T) to a compatible one-dimensional array, starting at the specified index of the target array.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_GetEnumerator">GetEnumerator</a></td><td>
Returns an enumerator that iterates through the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td>
Serves as the default hash function.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td>
Gets the Type of the current instance.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_IndexOf">IndexOf(Object)</a></td><td>
Searches for the specified object and returns the zero-based index of the first occurrence within the entire ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_IndexOf_1">IndexOf(T)</a></td><td>
Searches for the specified object and returns the zero-based index of the first occurrence within the entire ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Insert">Insert(Int32, Object)</a></td><td>
Inserts an element into the ConcurrentList(T) at the specified index</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Insert_1">Insert(Int32, T)</a></td><td>
Inserts an element into the ConcurrentList(T) at the specified index</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td>
Creates a shallow copy of the current Object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Remove">Remove(Func(T, Boolean))</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_Remove_1">Remove(T)</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_RemoveAll">RemoveAll</a></td><td>
Removes all the elements that match the conditions defined by the specified predicate.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_RemoveAt">RemoveAt</a></td><td>
Removes the element at the specified index of the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_ToArray">ToArray</a></td><td>
Copies the elements of the ConcurrentList(T) to a new array.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_ToList">ToList</a></td><td>
Creates a List(T) from an ConcurrentList(T).</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td>
Returns a string that represents the current object.
 (Inherited from Object.)</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_Any_">Any_()</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_Any__1">Any_(Func(Object, Boolean))</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_As__1">As(T)</a></td><td>
Performs a cast between compatible reference types. If a convertion is not possible then null is returned.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_Count_">Count_</a></td><td> (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension_CreateObject__1">CreateObject(T)</a></td><td> (Defined by <a href="T_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension">AnonymousTypeWithInterfaceExtension</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_Distinct__1">Distinct(T)</a></td><td>
Returns distinct elements from a sequence by using a selector to compare values.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_ElementAt_">ElementAt_</a></td><td> (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_Except_">Except_</a></td><td> (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_FirstElement_">FirstElement_</a></td><td> (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_Foreach__1">Foreach(T)</a></td><td>
Performs the specified action on each element of the IEnumerable(T)
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyNonPublicValue__1">GetPropertyNonPublicValue(T)</a></td><td>
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
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_Join__1">Join(T)</a></td><td>
Concatenates the members of a collection, using the specified *separator* between each member.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Activator_ExtensionsCloning_MapTo__1">MapTo(T)</a></td><td> (Defined by <a href="T_Cauldron_Activator_ExtensionsCloning">ExtensionsCloning</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_SequenceEqual_">SequenceEqual_</a></td><td> (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_Swap__1">Swap(T)</a></td><td>
Swaps two elements in a collection
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_ToArray">ToArray</a></td><td>
Converts a IEnumerable to an array
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_ToArray___1">ToArray_(T)</a></td><td> (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_IEnumerableExtensions_Extensions_ToList___1">ToList_(T)</a></td><td> (Defined by <a href="T_Cauldron_IEnumerableExtensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_ToLong_1">ToLong</a></td><td>
Tries to convert a Object to an Int64
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_TryDispose">TryDispose</a></td><td>
Tries to performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_System_Collections_IList_Add">IList.Add</a></td><td>
Adds an object to the end of the ConcurrentList(T)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_System_Collections_IEnumerable_GetEnumerator">IEnumerable.GetEnumerator</a></td><td>
Returns an enumerator that iterates through the ConcurrentList(T)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_System_Collections_Generic_IList{T}_Item">IList(T).Item</a></td><td>
Gets or sets the element at the specified index.</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td><a href="P_Cauldron_Core_Collections_ConcurrentList_1_System_Collections_IList_Item">IList.Item</a></td><td>
Gets or sets the element at the specified index.</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_System_Collections_Generic_ICollection{T}_Remove">ICollection(T).Remove</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentList_1_System_Collections_IList_Remove">IList.Remove</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />