# IBehaviour Interface
 _**\[This is preliminary documentation and is subject to change.\]**_

Represents a behaviour

**Namespace:**&nbsp;<a href="N_Cauldron_XAML_Interactivity">Cauldron.XAML.Interactivity</a><br />**Assembly:**&nbsp;Cauldron.XAML.Interactivity (in Cauldron.XAML.Interactivity.dll) Version: 1.0.0.0 (1.0.0.0)

## Syntax

**C#**<br />
``` C#
public interface IBehaviour
```

The IBehaviour type exposes the following members.


## Properties
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_Interactivity_IBehaviour_AssociatedObject">AssociatedObject</a></td><td>
Gets the [!:DependencyObject] to which the behavior is attached.</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_Interactivity_IBehaviour_IsAssignedFromTemplate">IsAssignedFromTemplate</a></td><td>
Gets a value that indicates the behaviour was assigned from a template</td></tr><tr><td>![Public property](media/pubproperty.gif "Public property")</td><td><a href="P_Cauldron_XAML_Interactivity_IBehaviour_Name">Name</a></td><td>
Gets or sets a name of the behaviour</td></tr></table>&nbsp;
<a href="#ibehaviour-interface">Back to Top</a>

## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Interactivity_IBehaviour_Attach">Attach</a></td><td>
Attaches a behaviour to a [!:DependencyObject]</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Interactivity_IBehaviour_Copy">Copy</a></td><td>
Creates a shallow copy of the instance</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Interactivity_IBehaviour_DataContextChanged">DataContextChanged</a></td><td>
Occures if the data context of the <a href="P_Cauldron_XAML_Interactivity_IBehaviour_AssociatedObject">AssociatedObject</a> has changed. This is only valid if <a href="P_Cauldron_XAML_Interactivity_IBehaviour_AssociatedObject">AssociatedObject</a> is a [!:FrameworkElement].</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Interactivity_IBehaviour_DataContextPropertyChanged">DataContextPropertyChanged</a></td><td>
Occurs when a property value changes of the <a href="P_Cauldron_XAML_Interactivity_IBehaviour_AssociatedObject">AssociatedObject</a>.DataContext. This is only valid if <a href="P_Cauldron_XAML_Interactivity_IBehaviour_AssociatedObject">AssociatedObject</a> is a [!:FrameworkElement].</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td><a href="M_Cauldron_XAML_Interactivity_IBehaviour_Detach">Detach</a></td><td>
Detaches a behviour from a [!:DependencyObject]</td></tr></table>&nbsp;
<a href="#ibehaviour-interface">Back to Top</a>

## See Also


#### Reference
<a href="N_Cauldron_XAML_Interactivity">Cauldron.XAML.Interactivity Namespace</a><br />