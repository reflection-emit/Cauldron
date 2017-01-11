# ConcurrentKeyedCollectionEx(*TKey*, *TItem*) Class
 _**\[This is preliminary documentation and is subject to change.\]**_

Provides an implementation of the ConcurrentKeyedCollectionEx(TKey, TItem) that can define the key on construction


## Inheritance Hierarchy
System.Object<br />&nbsp;&nbsp;System.Collections.ObjectModel.Collection(*TItem*)<br />&nbsp;&nbsp;&nbsp;&nbsp;System.Collections.ObjectModel.KeyedCollection(*TKey*, *TItem*)<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">Cauldron.Core.Collections.ConcurrentKeyedCollection</a>(*TKey*, *TItem*)<br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Cauldron.Core.Collections.ConcurrentKeyedCollectionEx(TKey, TItem)<br />
**Namespace:**&nbsp;<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections</a><br />**Assembly:**&nbsp;Cauldron.Core (in Cauldron.Core.dll) Version: 1.0.0.5 (1.0.0.5)

## Syntax

**C#**<br />
``` C#
public sealed class ConcurrentKeyedCollectionEx<TKey, TItem> : ConcurrentKeyedCollection<TKey, TItem>

```


#### Type Parameters
&nbsp;<dl><dt>TKey</dt><dd>The type of keys in the collection.</dd><dt>TItem</dt><dd>The type of items in the collection.</dd></dl>&nbsp;
The ConcurrentKeyedCollectionEx(TKey, TItem) type exposes the following members.


## Constructors
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2__ctor">ConcurrentKeyedCollectionEx(TKey, TItem)(Func(TItem, TKey))</a></td><td>
Initializes a new instance of the ConcurrentKeyedCollectionEx(TKey, TItem) class that uses the default equality comparer.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2__ctor_1">ConcurrentKeyedCollectionEx(TKey, TItem)(Func(TItem, TKey), IEqualityComparer(TKey))</a></td><td>
Initializes a new instance of the ConcurrentKeyedCollectionEx(TKey, TItem) class that uses the specified equality comparer and creates a lookup dictionary when the specified threshold is exceeded.</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2__ctor_2">ConcurrentKeyedCollectionEx(TKey, TItem)(Func(TItem, TKey), IEqualityComparer(TKey), Int32)</a></td><td>
Initializes a new instance of the ConcurrentKeyedCollectionEx(TKey, TItem) class that uses the specified equality comparer and creates a lookup dictionary when the specified threshold is exceeded.</td></tr></table>&nbsp;
<a href="#concurrentkeyedcollectionex(*tkey*,-*titem*)-class">Back to Top</a>

## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Comparer</td><td>
Gets the generic equality comparer that is used to determine equality of keys in the collection.
 (Inherited from KeyedCollection(*TKey*, *TItem*).)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Count</td><td>
Gets the number of elements actually contained in the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Protected property](media/protproperty.gif "Protected property")</td><td>Dictionary</td><td>
Gets the lookup dictionary of the KeyedCollection(TKey, TItem).
 (Inherited from KeyedCollection(*TKey*, *TItem*).)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Item(TKey)</td><td>
Gets the element with the specified key.
 (Inherited from KeyedCollection(*TKey*, *TItem*).)</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td>Item(Int32)</td><td>
Gets or sets the element at the specified index.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Protected property](media/protproperty.gif "Protected property")</td><td>Items</td><td>
Gets a IList(T) wrapper around the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr></table>&nbsp;
<a href="#concurrentkeyedcollectionex(*tkey*,-*titem*)-class">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Add</td><td>
Adds an object to the end of the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollection_2_AddRange">AddRange</a></td><td>
Adds the elements of the specified collection to the end of the <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>
 (Inherited from <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>ChangeItemKey</td><td>
Changes the key associated with the specified element in the lookup dictionary.
 (Inherited from KeyedCollection(*TKey*, *TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Clear</td><td>
Removes all elements from the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollection_2_ClearItems">ClearItems</a></td><td>
Removes all elements from the <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>
 (Inherited from <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Contains(TKey)</td><td>
Determines whether the collection contains an element with the specified key.
 (Inherited from KeyedCollection(*TKey*, *TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Contains(T)</td><td>
Determines whether an element is in the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CopyTo</td><td>
Copies the entire Collection(T) to a compatible one-dimensional Array, starting at the specified index of the target array.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td>
Determines whether the specified object is equal to the current object.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetEnumerator</td><td>
Returns an enumerator that iterates through the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td>
Serves as the default hash function.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollectionEx_2_GetKeyForItem">GetKeyForItem</a></td><td>
When implemented in a derived class, extracts the key from the specified element.
 (Overrides KeyedCollection(TKey, TItem).GetKeyForItem(TItem).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td>
Gets the Type of the current instance.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>IndexOf</td><td>
Searches for the specified object and returns the zero-based index of the first occurrence within the entire Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Insert</td><td>
Inserts an element into the Collection(T) at the specified index.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollection_2_InsertItem">InsertItem</a></td><td>
Inserts an element into the <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a> at the specified index.
 (Inherited from <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td>
Creates a shallow copy of the current Object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Remove(TKey)</td><td>
Removes the element with the specified key from the KeyedCollection(TKey, TItem).
 (Inherited from KeyedCollection(*TKey*, *TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Remove(T)</td><td>
Removes the first occurrence of a specific object from the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>RemoveAt</td><td>
Removes the element at the specified index of the Collection(T).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollection_2_RemoveItem">RemoveItem</a></td><td>
Removes the element at the specified index of the <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>
 (Inherited from <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_Core_Collections_ConcurrentKeyedCollection_2_SetItem">SetItem</a></td><td>
Replaces the item at the specified index with the specified item.
 (Inherited from <a href="T_Cauldron_Core_Collections_ConcurrentKeyedCollection_2">ConcurrentKeyedCollection(TKey, TItem)</a>.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td>
Returns a string that represents the current object.
 (Inherited from Object.)</td></tr></table>&nbsp;
<a href="#concurrentkeyedcollectionex(*tkey*,-*titem*)-class">Back to Top</a>

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
<a href="#concurrentkeyedcollectionex(*tkey*,-*titem*)-class">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IList.Add</td><td>
Adds an item to the IList.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IList.Contains</td><td>
Determines whether the IList contains a specific value.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>ICollection.CopyTo</td><td>
Copies the elements of the ICollection to an Array, starting at a particular Array index.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IEnumerable.GetEnumerator</td><td>
Returns an enumerator that iterates through a collection.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IList.IndexOf</td><td>
Determines the index of a specific item in the IList.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IList.Insert</td><td>
Inserts an item into the IList at the specified index.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td>IList.IsFixedSize</td><td>
Gets a value indicating whether the IList has a fixed size.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td>ICollection(T).IsReadOnly</td><td>
Gets a value indicating whether the ICollection(T) is read-only.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td>IList.IsReadOnly</td><td>
Gets a value indicating whether the IList is read-only.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td>ICollection.IsSynchronized</td><td>
Gets a value indicating whether access to the ICollection is synchronized (thread safe).
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td>IList.Item</td><td>
Gets or sets the element at the specified index.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IList.Remove</td><td>
Removes the first occurrence of a specific object from the IList.
 (Inherited from Collection(*TItem*).)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private property](media/privproperty.gif "Private property")</td><td>ICollection.SyncRoot</td><td>
Gets an object that can be used to synchronize access to the ICollection.
 (Inherited from Collection(*TItem*).)</td></tr></table>&nbsp;
<a href="#concurrentkeyedcollectionex(*tkey*,-*titem*)-class">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_Core_Collections">Cauldron.Core.Collections Namespace</a><br />