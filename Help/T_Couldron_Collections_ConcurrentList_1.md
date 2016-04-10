# ConcurrentList(*T*) Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Represents a thread-safe list of items that can be accessed by multiple threads concurrently.


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;Couldron.Collections.ConcurrentList(T)<br />
**Namespace:**&nbsp;<a href="N_Couldron_Collections">Couldron.Collections</a><br />**Assembly:**&nbsp;Couldron (in Couldron.dll) Version: 1.0.0.0 (1.0.0.0)

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
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1__ctor">ConcurrentList(T)()</a></td><td>
Initializes a new instance of the ConcurrentList(T) class that is empty and has the default initial capacity.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1__ctor_1">ConcurrentList(T)(IEnumerable(T))</a></td><td>
Initializes a new instance of the ConcurrentList(T) class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_Count">Count</a></td><td>
Gets the number of elements contained in the ConcurrentList(T)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_IsFixedSize">IsFixedSize</a></td><td>
Gets a value indicating whether the ConcurrentList(T) has a fixed size.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_IsReadOnly">IsReadOnly</a></td><td>
Gets a value indicating whether the ConcurrentList(T) is read-only.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_IsSynchronized">IsSynchronized</a></td><td></td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_Item">Item</a></td><td>
Gets or sets the element at the specified index.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_SyncRoot">SyncRoot</a></td><td>
Gets a value indicating whether access to the ConcurrentList(T) is synchronized (thread safe).</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Add">Add</a></td><td>
Adds an object to the end of the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_AddRange">AddRange</a></td><td>
dds the elements of the specified collection to the end of the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Clear">Clear</a></td><td>
Removes all elements from the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Contains">Contains(Func(T, Boolean))</a></td><td>
Determines whether an element is in the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Contains_1">Contains(Object)</a></td><td>
Determines whether an element is in the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Contains_2">Contains(T)</a></td><td>
Determines whether an element is in the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_CopyTo">CopyTo(Array, Int32)</a></td><td>
Copies a range of elements from the ConcurrentList(T) to a compatible one-dimensional array, starting at the specified index of the target array.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_CopyTo_1">CopyTo(T[], Int32)</a></td><td>
Copies a range of elements from the ConcurrentList(T) to a compatible one-dimensional array, starting at the specified index of the target array.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td> (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_GetEnumerator">GetEnumerator</a></td><td>
Returns an enumerator that iterates through the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_IndexOf">IndexOf(Object)</a></td><td>
Searches for the specified object and returns the zero-based index of the first occurrence within the entire ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_IndexOf_1">IndexOf(T)</a></td><td>
Searches for the specified object and returns the zero-based index of the first occurrence within the entire ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Insert">Insert(Int32, Object)</a></td><td>
Inserts an element into the ConcurrentList(T) at the specified index</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Insert_1">Insert(Int32, T)</a></td><td>
Inserts an element into the ConcurrentList(T) at the specified index</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td> (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Remove">Remove(Func(T, Boolean))</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_Remove_1">Remove(T)</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_RemoveAll">RemoveAll</a></td><td>
Removes all the elements that match the conditions defined by the specified predicate.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_RemoveAt">RemoveAt</a></td><td>
Removes the element at the specified index of the ConcurrentList(T)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_ToArray">ToArray</a></td><td>
Copies the elements of the ConcurrentList(T) to a new array.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_ToList">ToList</a></td><td>
Creates a List(T) from an ConcurrentList(T).</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td> (Inherited from Object.)</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_Any">Any()</a></td><td>Overloaded.  
Determines whether a sequence contains any elements.
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_Any_1">Any(Func(Object, Boolean))</a></td><td>Overloaded.  
Determines whether any element of a sequence satisfies a condition.
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_CastTo__1">CastTo(T)</a></td><td>
Performs certain types of conversions between compatible reference types or nullable types 

 Returns null if convertion was not successfull
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_Count">Count</a></td><td>
Gets the number of elements contained in the IEnumerable
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_DisposeAll">DisposeAll</a></td><td>
Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface. 

 If the object is a FrameworkElement it will try to find known diposable attached properties. 

 It will also dispose the the DataContext content.
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_ElementAt">ElementAt</a></td><td>
Returns the element at the defined index
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_FirstElement">FirstElement</a></td><td>
Returns the first element of a sequence.
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_Remove">Remove</a></td><td>
Removes the first occurrence of a specific object from the IEnumerable
 (Defined by <a href="T_Couldron_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Couldron_Extensions_ToArray__1">ToArray(T)</a></td><td>
Converts a IEnumerable to an array
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
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_System_Collections_IList_Add">IList.Add</a></td><td>
Adds an object to the end of the ConcurrentList(T)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_System_Collections_IEnumerable_GetEnumerator">IEnumerable.GetEnumerator</a></td><td>
Returns an enumerator that iterates through the ConcurrentList(T)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_System_Collections_Generic_IList{T}_Item">IList(T).Item</a></td><td>
Gets or sets the element at the specified index.</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td><a href="P_Couldron_Collections_ConcurrentList_1_System_Collections_IList_Item">IList.Item</a></td><td>
Gets or sets the element at the specified index.</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_System_Collections_Generic_ICollection{T}_Remove">ICollection(T).Remove</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Couldron_Collections_ConcurrentList_1_System_Collections_IList_Remove">IList.Remove</a></td><td>
Removes the first occurrence of a specific object from the ConcurrentList(T)</td></tr></table>&nbsp;
<a href="#concurrentlist(*t*)-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Couldron_Collections">Couldron.Collections Namespace</a><br />