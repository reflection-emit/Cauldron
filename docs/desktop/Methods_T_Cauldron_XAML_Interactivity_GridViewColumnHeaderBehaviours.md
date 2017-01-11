# GridViewColumnHeaderBehaviours Methods
 _**\[This is preliminary documentation and is subject to change.\]**_

The <a href="T_Cauldron_XAML_Interactivity_GridViewColumnHeaderBehaviours">GridViewColumnHeaderBehaviours</a> type exposes the following members.


## Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>AddHandler(RoutedEvent, Delegate)</td><td>
Adds a&nbsp;routed event handler for a specified routed event, adding the handler to the handler collection on the current element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>AddHandler(RoutedEvent, Delegate, Boolean)</td><td>
Adds a&nbsp;routed event handler for a specified routed event, adding the handler to the handler collection on the current element. Specify *handledEventsToo* as true to have the provided handler be invoked for routed event that had already been marked as handled by another element along the event route.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>AddLogicalChild</td><td>
Adds the provided object to the logical tree of this element.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>AddToEventRoute</td><td>
Adds handlers to the specified EventRoute for the current UIElement event handler collection.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>AddVisualChild</td><td>
Defines the parent-child relationship between two visuals.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ApplyAnimationClock(DependencyProperty, AnimationClock)</td><td>
Applies an animation to a specified&nbsp;dependency property on this element. Any existing animations are stopped and replaced with the new animation.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ApplyAnimationClock(DependencyProperty, AnimationClock, HandoffBehavior)</td><td>
Applies an animation to a specified&nbsp;dependency property on this element, with the ability to specify what happens if the property already has a running animation.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ApplyTemplate</td><td>
Builds the current template's visual tree if necessary, and returns a value that indicates whether the visual tree was rebuilt by this call.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Arrange</td><td>
Positions child elements and determines a size for a UIElement. Parent elements call this method from their ArrangeCore(Rect) implementation (or a WPF framework-level equivalent) to form a recursive layout update. This method constitutes the second pass of a layout update.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>ArrangeCore</td><td>
Implements ArrangeCore(Rect) (defined as virtual in UIElement) and seals the implementation.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>ArrangeOverride</td><td>
When overridden in a derived class, positions child elements and determines a size for a FrameworkElement derived class.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BeginAnimation(DependencyProperty, AnimationTimeline)</td><td>
Starts an animation for a specified animated property on this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BeginAnimation(DependencyProperty, AnimationTimeline, HandoffBehavior)</td><td>
Starts a specific animation for a specified animated property on this element, with the option of specifying what happens if the property already has a running animation.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BeginInit</td><td>
Starts the initialization process for this element.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BeginStoryboard(Storyboard)</td><td>
Begins the sequence of actions that are contained in the provided storyboard.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BeginStoryboard(Storyboard, HandoffBehavior)</td><td>
Begins the sequence of actions contained in the provided storyboard, with options specified for what should happen if the property is already animated.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BeginStoryboard(Storyboard, HandoffBehavior, Boolean)</td><td>
Begins the sequence of actions contained in the provided storyboard, with specified state for control of the animation after it is started.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BringIntoView()</td><td>
Attempts to bring this element into view, within any scrollable regions it is contained within.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>BringIntoView(Rect)</td><td>
Attempts to bring the provided region size of this element into view, within any scrollable regions it is contained within.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CaptureMouse</td><td>
Attempts to force capture of the mouse to this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CaptureStylus</td><td>
Attempts to force capture of the stylus to this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CaptureTouch</td><td>
Attempts to force capture of a touch to this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CheckAccess</td><td>
Determines whether the calling thread has access to this DispatcherObject.
 (Inherited from DispatcherObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ClearValue(DependencyProperty)</td><td>
Clears the local value of a property. The property to be cleared is specified by a DependencyProperty identifier.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ClearValue(DependencyPropertyKey)</td><td>
Clears the local value of a read-only property. The property to be cleared is specified by a DependencyPropertyKey.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>CoerceValue</td><td>
Coerces the value of the specified dependency property. This is accomplished by invoking any CoerceValueCallback function specified in property metadata for the dependency property as it exists on the calling DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>EndInit</td><td>
Indicates that the initialization process for the element is complete.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Equals</td><td>
Determines whether a provided DependencyObject is equivalent to the current DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>Finalize</td><td>
Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>FindCommonVisualAncestor</td><td>
Returns the common ancestor of two visual objects.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>FindName</td><td>
Finds an element that has the provided identifier name.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>FindResource</td><td>
Searches for a resource with the specified key, and throws an exception if the requested resource is not found.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Focus</td><td>
Attempts to set focus to this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetAnimationBaseValue</td><td>
Returns the base property value for the specified property on this element, disregarding any possible animated value from a running or stopped animation.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetBindingExpression</td><td>
Returns the BindingExpression that represents the binding on the specified property.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetHashCode</td><td>
Gets a hash code for this DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>GetLayoutClip</td><td>
Returns a geometry for a clipping mask. The mask applies if the layout system attempts to arrange an element that is larger than the available display space.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetLocalValueEnumerator</td><td>
Creates a specialized enumerator for determining which dependency properties have locally set values on this DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>GetTemplateChild</td><td>
Returns the named element in the visual tree of an instantiated ControlTemplate.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetType</td><td>
Gets the Type of the current instance.
 (Inherited from Object.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>GetUIParentCore</td><td>
Returns an alternative logical parent for this element if there is no visual parent.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>GetValue</td><td>
Returns the current effective value of a dependency property on this instance of a DependencyObject.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>GetVisualChild</td><td>
Overrides GetVisualChild(Int32), and returns a child at the specified index from a collection of child elements.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>HitTestCore(PointHitTestParameters)</td><td>
Implements HitTestCore(PointHitTestParameters) to supply base element hit testing behavior (returning HitTestResult).
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>HitTestCore(GeometryHitTestParameters)</td><td>
Implements HitTestCore(GeometryHitTestParameters) to supply base element hit testing behavior (returning GeometryHitTestResult).
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>InputHitTest</td><td>
Returns the input element within the current element that is at the specified coordinates, relative to the current element's origin.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>InvalidateArrange</td><td>
Invalidates the arrange state (layout) for the element. After the invalidation, the element will have its layout updated, which will occur asynchronously unless subsequently forced by UpdateLayout().
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>InvalidateMeasure</td><td>
Invalidates the measurement state (layout) for the element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>InvalidateProperty</td><td>
Re-evaluates the effective value for the specified dependency property
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>InvalidateVisual</td><td>
Invalidates the rendering of the element, and forces a complete new layout pass. OnRender(DrawingContext) is called after the layout cycle is completed.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>IsAncestorOf</td><td>
Determines whether the visual object is an ancestor of the descendant visual object.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>IsDescendantOf</td><td>
Determines whether the visual object is a descendant of the ancestor visual object.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>Measure</td><td>
Updates the DesiredSize of a UIElement. Parent elements call this method from their own MeasureCore(Size) implementations to form a recursive layout update. Calling this method constitutes the first pass (the "Measure" pass) of a layout update.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MeasureCore</td><td>
Implements basic measure-pass layout system behavior for FrameworkElement.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MeasureOverride</td><td>
When overridden in a derived class, measures the size in layout required for child elements and determines a size for the FrameworkElement-derived class.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>MemberwiseClone</td><td>
Creates a shallow copy of the current Object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>MoveFocus</td><td>
Moves the keyboard focus away from this element and to another element in a provided traversal direction.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnAccessKey</td><td>
Provides class handling for when an access key that is meaningful for this element is invoked.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>OnApplyTemplate</td><td>
When overridden in a derived class, is invoked whenever application code or internal processes call ApplyTemplate().
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_Interactivity_GridViewColumnHeaderBehaviours_OnAttach">OnAttach</a></td><td>
Occures when the behavior is attached to the object
 (Overrides <a href="M_Cauldron_XAML_Interactivity_Behaviour_1_OnAttach">Behaviour(T).OnAttach()</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnChildDesiredSizeChanged</td><td>
Supports layout behavior when a child element is resized.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnContextMenuClosing</td><td>
Invoked whenever an unhandled ContextMenuClosing routed event reaches this class in its route. Implement this method to add class handling for this event.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnContextMenuOpening</td><td>
Invoked whenever an unhandled ContextMenuOpening routed event reaches this class in its route. Implement this method to add class handling for this event.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_Interactivity_Behaviour_1_OnCopy">OnCopy</a></td><td>
Occures after shallow copying the behavior
 (Inherited from <a href="T_Cauldron_XAML_Interactivity_Behaviour_1">Behaviour(T)</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnCreateAutomationPeer</td><td>
Returns class-specific AutomationPeer implementations for the Windows Presentation Foundation (WPF) infrastructure.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_Interactivity_GridViewColumnHeaderBehaviours_OnDataContextChanged">OnDataContextChanged</a></td><td>
Occures if the DataContext of <a href="P_Cauldron_XAML_Interactivity_Behaviour_1_AssociatedObject">AssociatedObject</a> has changed
 (Overrides <a href="M_Cauldron_XAML_Interactivity_Behaviour_1_OnDataContextChanged">Behaviour(T).OnDataContextChanged()</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_Interactivity_Behaviour_1_OnDataContextPropertyChanged">OnDataContextPropertyChanged</a></td><td>
Occures if the DataContext has invoked the [!:INotifyPropertyChanged.PropertyChanged] event
 (Inherited from <a href="T_Cauldron_XAML_Interactivity_Behaviour_1">Behaviour(T)</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td><a href="M_Cauldron_XAML_Interactivity_GridViewColumnHeaderBehaviours_OnDetach">OnDetach</a></td><td>
Occures when the behaviour is detached from the object
 (Overrides <a href="M_Cauldron_XAML_Interactivity_Behaviour_1_OnDetach">Behaviour(T).OnDetach()</a>.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnDragEnter</td><td>
Invoked when an unhandled DragEnter&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnDragLeave</td><td>
Invoked when an unhandled DragLeave&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnDragOver</td><td>
Invoked when an unhandled DragOver&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnDrop</td><td>
Invoked when an unhandled DragEnter&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnGiveFeedback</td><td>
Invoked when an unhandled GiveFeedback&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnGotFocus</td><td>
Invoked whenever an unhandled GotFocus event reaches this element in its route.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnGotKeyboardFocus</td><td>
Invoked when an unhandled GotKeyboardFocus&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnGotMouseCapture</td><td>
Invoked when an unhandled GotMouseCapture&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnGotStylusCapture</td><td>
Invoked when an unhandled GotStylusCapture&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnGotTouchCapture</td><td>
Provides class handling for the GotTouchCapture routed event that occurs when a touch is captured to this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnInitialized</td><td>
Raises the Initialized event. This method is invoked whenever IsInitialized is set to true internally.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsKeyboardFocusedChanged</td><td>
Invoked when an unhandled IsKeyboardFocusedChanged event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsKeyboardFocusWithinChanged</td><td>
Invoked just before the IsKeyboardFocusWithinChanged event is raised by this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsMouseCapturedChanged</td><td>
Invoked when an unhandled IsMouseCapturedChanged event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsMouseCaptureWithinChanged</td><td>
Invoked when an unhandled IsMouseCaptureWithinChanged event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsMouseDirectlyOverChanged</td><td>
Invoked when an unhandled IsMouseDirectlyOverChanged event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsStylusCapturedChanged</td><td>
Invoked when an unhandled IsStylusCapturedChanged event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsStylusCaptureWithinChanged</td><td>
Invoked when an unhandled IsStylusCaptureWithinChanged event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnIsStylusDirectlyOverChanged</td><td>
Invoked when an unhandled IsStylusDirectlyOverChanged event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnKeyDown</td><td>
Invoked when an unhandled KeyDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnKeyUp</td><td>
Invoked when an unhandled KeyUp&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnLostFocus</td><td>
Raises the LostFocus&nbsp;routed event by using the event data that is provided.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnLostKeyboardFocus</td><td>
Invoked when an unhandled LostKeyboardFocus&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnLostMouseCapture</td><td>
Invoked when an unhandled LostMouseCapture&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnLostStylusCapture</td><td>
Invoked when an unhandled LostStylusCapture&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnLostTouchCapture</td><td>
Provides class handling for the LostTouchCapture routed event that occurs when this element loses a touch capture.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnManipulationBoundaryFeedback</td><td>
Called when the ManipulationBoundaryFeedback event occurs.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnManipulationCompleted</td><td>
Called when the ManipulationCompleted event occurs.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnManipulationDelta</td><td>
Called when the ManipulationDelta event occurs.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnManipulationInertiaStarting</td><td>
Called when the ManipulationInertiaStarting event occurs.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnManipulationStarted</td><td>
Called when the ManipulationStarted event occurs.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnManipulationStarting</td><td>
Provides class handling for the ManipulationStarting routed event that occurs when the manipulation processor is first created.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseDown</td><td>
Invoked when an unhandled MouseDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseEnter</td><td>
Invoked when an unhandled MouseEnter&nbsp;attached event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseLeave</td><td>
Invoked when an unhandled MouseLeave&nbsp;attached event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseLeftButtonDown</td><td>
Invoked when an unhandled MouseLeftButtonDown&nbsp;routed event is raised on this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseLeftButtonUp</td><td>
Invoked when an unhandled MouseLeftButtonUp&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseMove</td><td>
Invoked when an unhandled MouseMove&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseRightButtonDown</td><td>
Invoked when an unhandled MouseRightButtonDown&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseRightButtonUp</td><td>
Invoked when an unhandled MouseRightButtonUp&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseUp</td><td>
Invoked when an unhandled MouseUp&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnMouseWheel</td><td>
Invoked when an unhandled MouseWheel&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewDragEnter</td><td>
Invoked when an unhandled PreviewDragEnter&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewDragLeave</td><td>
Invoked when an unhandled PreviewDragLeave&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewDragOver</td><td>
Invoked when an unhandled PreviewDragOver&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewDrop</td><td>
Invoked when an unhandled PreviewDrop&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewGiveFeedback</td><td>
Invoked when an unhandled PreviewGiveFeedback&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewGotKeyboardFocus</td><td>
Invoked when an unhandled PreviewGotKeyboardFocus&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewKeyDown</td><td>
Invoked when an unhandled PreviewKeyDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewKeyUp</td><td>
Invoked when an unhandled PreviewKeyUp&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewLostKeyboardFocus</td><td>
Invoked when an unhandled PreviewKeyDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseDown</td><td>
Invoked when an unhandled PreviewMouseDown attached&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseLeftButtonDown</td><td>
Invoked when an unhandled PreviewMouseLeftButtonDown&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseLeftButtonUp</td><td>
Invoked when an unhandled PreviewMouseLeftButtonUp&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseMove</td><td>
Invoked when an unhandled PreviewMouseMove&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseRightButtonDown</td><td>
Invoked when an unhandled PreviewMouseRightButtonDown&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseRightButtonUp</td><td>
Invoked when an unhandled PreviewMouseRightButtonUp&nbsp;routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseUp</td><td>
Invoked when an unhandled PreviewMouseUp&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewMouseWheel</td><td>
Invoked when an unhandled PreviewMouseWheel&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewQueryContinueDrag</td><td>
Invoked when an unhandled PreviewQueryContinueDrag&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusButtonDown</td><td>
Invoked when an unhandled PreviewStylusButtonDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusButtonUp</td><td>
Invoked when an unhandled PreviewStylusButtonUp&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusDown</td><td>
Invoked when an unhandled PreviewStylusDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusInAirMove</td><td>
Invoked when an unhandled PreviewStylusInAirMove&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusInRange</td><td>
Invoked when an unhandled PreviewStylusInRange&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusMove</td><td>
Invoked when an unhandled PreviewStylusMove&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusOutOfRange</td><td>
Invoked when an unhandled PreviewStylusOutOfRange&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusSystemGesture</td><td>
Invoked when an unhandled PreviewStylusSystemGesture&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewStylusUp</td><td>
Invoked when an unhandled PreviewStylusUp&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewTextInput</td><td>
Invoked when an unhandled PreviewTextInput&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewTouchDown</td><td>
Provides class handling for the PreviewTouchDown routed event that occurs when a touch presses this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewTouchMove</td><td>
Provides class handling for the PreviewTouchMove routed event that occurs when a touch moves while inside this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPreviewTouchUp</td><td>
Provides class handling for the PreviewTouchUp routed event that occurs when a touch is released inside this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnPropertyChanged</td><td>
Invoked whenever the effective value of any dependency property on this FrameworkElement has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides OnPropertyChanged(DependencyPropertyChangedEventArgs).
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnQueryContinueDrag</td><td>
Invoked when an unhandled QueryContinueDrag&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnQueryCursor</td><td>
Invoked when an unhandled QueryCursor&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnRender</td><td>
When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnRenderSizeChanged</td><td>
Raises the SizeChanged event, using the specified information as part of the eventual event data.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStyleChanged</td><td>
Invoked when the style in use on this element changes, which will invalidate the layout.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusButtonDown</td><td>
Invoked when an unhandled StylusButtonDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusButtonUp</td><td>
Invoked when an unhandled StylusButtonUp&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusDown</td><td>
Invoked when an unhandled StylusDown&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusEnter</td><td>
Invoked when an unhandled StylusEnter&nbsp;attached event is raised by this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusInAirMove</td><td>
Invoked when an unhandled StylusInAirMove&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusInRange</td><td>
Invoked when an unhandled StylusInRange&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusLeave</td><td>
Invoked when an unhandled StylusLeave&nbsp;attached event is raised by this element. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusMove</td><td>
Invoked when an unhandled StylusMove&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusOutOfRange</td><td>
Invoked when an unhandled StylusOutOfRange&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusSystemGesture</td><td>
Invoked when an unhandled StylusSystemGesture&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnStylusUp</td><td>
Invoked when an unhandled StylusUp&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnTextInput</td><td>
Invoked when an unhandled TextInput&nbsp;attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnToolTipClosing</td><td>
Invoked whenever an unhandled ToolTipClosing routed event reaches this class in its route. Implement this method to add class handling for this event.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnToolTipOpening</td><td>
Invoked whenever the ToolTipOpening routed event reaches this class in its route. Implement this method to add class handling for this event.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnTouchDown</td><td>
Provides class handling for the TouchDown routed event that occurs when a touch presses inside this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnTouchEnter</td><td>
Provides class handling for the TouchEnter routed event that occurs when a touch moves from outside to inside the bounds of this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnTouchLeave</td><td>
Provides class handling for the TouchLeave routed event that occurs when a touch moves from inside to outside the bounds of this UIElement.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnTouchMove</td><td>
Provides class handling for the TouchMove routed event that occurs when a touch moves while inside this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnTouchUp</td><td>
Provides class handling for the TouchUp routed event that occurs when a touch is released inside this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnVisualChildrenChanged</td><td>
Called when the VisualCollection of the visual object is modified.
 (Inherited from Visual.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>OnVisualParentChanged</td><td>
Invoked when the parent of this element in the visual tree is changed. Overrides OnVisualParentChanged(DependencyObject).
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>ParentLayoutInvalidated</td><td>
Supports incremental layout implementations in specialized subclasses of FrameworkElement. ParentLayoutInvalidated(UIElement) is invoked when a child element has invalidated a property that is marked in metadata as affecting the parent's measure or arrange passes during layout.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>PointFromScreen</td><td>
Converts a Point in screen coordinates into a Point that represents the current coordinate system of the Visual.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>PointToScreen</td><td>
Converts a Point that represents the current coordinate system of the Visual into a Point in screen coordinates.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>PredictFocus</td><td>
Determines the next element that would receive focus relative to this element for a provided focus movement direction, but does not actually move the focus.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>RaiseEvent</td><td>
Raises a specific routed event. The RoutedEvent to be raised is identified within the RoutedEventArgs instance that is provided (as the RoutedEvent property of that event data).
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ReadLocalValue</td><td>
Returns the local value of a dependency property, if it exists.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>RegisterName</td><td>
Provides an accessor that simplifies access to the NameScope registration method.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ReleaseAllTouchCaptures</td><td>
Releases all captured touch devices from this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ReleaseMouseCapture</td><td>
Releases the mouse capture, if this element held the capture.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ReleaseStylusCapture</td><td>
Releases the stylus device capture, if this element held the capture.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ReleaseTouchCapture</td><td>
Attempts to release the specified touch device from this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>RemoveHandler</td><td>
Removes the specified routed event handler from this element.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>RemoveLogicalChild</td><td>
Removes the provided object from this element's logical tree. FrameworkElement updates the affected logical tree parent pointers to keep in sync with this deletion.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>RemoveVisualChild</td><td>
Removes the parent-child relationship between two visuals.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetBinding(DependencyProperty, BindingBase)</td><td>
Attaches a binding to this element, based on the provided binding object.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetBinding(DependencyProperty, String)</td><td>
Attaches a binding to this element, based on the provided source property name as a path qualification to the data source.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetCurrentValue</td><td>
Sets the value of a dependency property without changing its value source.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetResourceReference</td><td>
Searches for a resource with the specified name and sets up a resource reference to it for the specified property.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetValue(DependencyProperty, Object)</td><td>
Sets the local value of a dependency property, specified by its dependency property identifier.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>SetValue(DependencyPropertyKey, Object)</td><td>
Sets the local value of a read-only dependency property, specified by the DependencyPropertyKey identifier of the dependency property.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ShouldSerializeCommandBindings</td><td>
Returns whether serialization processes should serialize the contents of the CommandBindings property on instances of this class.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ShouldSerializeInputBindings</td><td>
Returns whether serialization processes should serialize the contents of the InputBindings property on instances of this class.
 (Inherited from UIElement.)</td></tr><tr><td>![Protected method](media/protmethod.gif "Protected method")</td><td>ShouldSerializeProperty</td><td>
Returns a value that indicates whether serialization processes should serialize the value for the provided dependency property.
 (Inherited from DependencyObject.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ShouldSerializeResources</td><td>
Returns whether serialization processes should serialize the contents of the Resources property.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ShouldSerializeStyle</td><td>
Returns whether serialization processes should serialize the contents of the Style property.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ShouldSerializeTriggers</td><td>
Returns whether serialization processes should serialize the contents of the Triggers property.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>ToString</td><td>
Returns a string that represents the current object.
 (Inherited from Object.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TransformToAncestor(Visual)</td><td>
Returns a transform that can be used to transform coordinates from the Visual to the specified Visual ancestor of the visual object.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TransformToAncestor(Visual3D)</td><td>
Returns a transform that can be used to transform coordinates from the Visual to the specified Visual3D ancestor of the visual object.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TransformToDescendant</td><td>
Returns a transform that can be used to transform coordinates from the Visual to the specified visual object descendant.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TransformToVisual</td><td>
Returns a transform that can be used to transform coordinates from the Visual to the specified visual object.
 (Inherited from Visual.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TranslatePoint</td><td>
Translates a point relative to this element to coordinates that are relative to the specified element.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>TryFindResource</td><td>
Searches for a resource with the specified key, and returns that resource if found.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>UnregisterName</td><td>
Simplifies access to the NameScope de-registration method.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>UpdateDefaultStyle</td><td>
Reapplies the default style to the current FrameworkElement.
 (Inherited from FrameworkElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>UpdateLayout</td><td>
Ensures that all visual child elements of this element are properly updated for layout.
 (Inherited from UIElement.)</td></tr><tr><td>![Public method](media/pubmethod.gif "Public method")</td><td>VerifyAccess</td><td>
Enforces that the calling thread has access to this DispatcherObject.
 (Inherited from DispatcherObject.)</td></tr></table>&nbsp;
<a href="#gridviewcolumnheaderbehaviours-methods">Back to Top</a>

## Extension Methods
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_As__1">As(T)</a></td><td>
Performs a cast between compatible reference types. If a convertion is not possible then null is returned.
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_AsBitmapImage">AsBitmapImage</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension_CreateObject__1">CreateObject(T)</a></td><td> (Defined by <a href="T_Cauldron_Dynamic_AnonymousTypeWithInterfaceExtension">AnonymousTypeWithInterfaceExtension</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualChildByName">FindVisualChildByName</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualChildren">FindVisualChildren(Type)</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualChildren__1">FindVisualChildren(T)()</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualParent">FindVisualParent(Type)</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualParent__1">FindVisualParent(T)()</a></td><td>Overloaded.   (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_FindVisualRootElement">FindVisualRootElement</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_GetDependencyProperties">GetDependencyProperties</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_GetInheritanceContext">GetInheritanceContext</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsReflection_GetPropertyNonPublicValue__1">GetPropertyNonPublicValue(T)</a></td><td>
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
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Activator_ExtensionsCloning_MapTo__1">MapTo(T)</a></td><td> (Defined by <a href="T_Cauldron_Activator_ExtensionsCloning">ExtensionsCloning</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_SaveVisualAsPngAsync">SaveVisualAsPngAsync</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_XAML_Extensions_SetBinding">SetBinding</a></td><td> (Defined by <a href="T_Cauldron_XAML_Extensions">Extensions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_ExtensionsConvertions_ToLong_1">ToLong</a></td><td>
Tries to convert a Object to an Int64
 (Defined by <a href="T_Cauldron_Core_Extensions_ExtensionsConvertions">ExtensionsConvertions</a>.)</td></tr><tr><td>![Public Extension Method](media/pubextension.gif "Public Extension Method")</td><td><a href="M_Cauldron_Core_Extensions_Extensions_TryDispose">TryDispose</a></td><td>
Tries to performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 

 This will dispose an object if it implements the IDisposable interface.
 (Defined by <a href="T_Cauldron_Core_Extensions_Extensions">Extensions</a>.)</td></tr></table>&nbsp;
<a href="#gridviewcolumnheaderbehaviours-methods">Back to Top</a>

## Explicit Interface Implementations
&nbsp;<table><tr><th></th><th>Name</th><th>Description</th></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_XAML_Interactivity_Behaviour_1_Cauldron_XAML_Interactivity_IBehaviour_Attach">IBehaviour.Attach</a></td><td> (Inherited from <a href="T_Cauldron_XAML_Interactivity_Behaviour_1">Behaviour(T)</a>.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_XAML_Interactivity_Behaviour_1_Cauldron_XAML_Interactivity_IBehaviour_Copy">IBehaviour.Copy</a></td><td> (Inherited from <a href="T_Cauldron_XAML_Interactivity_Behaviour_1">Behaviour(T)</a>.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_XAML_Interactivity_Behaviour_1_Cauldron_XAML_Interactivity_IBehaviour_DataContextChanged">IBehaviour.DataContextChanged</a></td><td> (Inherited from <a href="T_Cauldron_XAML_Interactivity_Behaviour_1">Behaviour(T)</a>.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_XAML_Interactivity_Behaviour_1_Cauldron_XAML_Interactivity_IBehaviour_DataContextPropertyChanged">IBehaviour.DataContextPropertyChanged</a></td><td> (Inherited from <a href="T_Cauldron_XAML_Interactivity_Behaviour_1">Behaviour(T)</a>.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td><a href="M_Cauldron_XAML_Interactivity_Behaviour_1_Cauldron_XAML_Interactivity_IBehaviour_Detach">IBehaviour.Detach</a></td><td> (Inherited from <a href="T_Cauldron_XAML_Interactivity_Behaviour_1">Behaviour(T)</a>.)</td></tr><tr><td>![Explicit interface implementation](media/pubinterface.gif "Explicit interface implementation")![Private method](media/privmethod.gif "Private method")</td><td>IQueryAmbient.IsAmbientPropertyAvailable</td><td>
For a description of this member, see the IsAmbientPropertyAvailable(String) method.
 (Inherited from FrameworkElement.)</td></tr></table>&nbsp;
<a href="#gridviewcolumnheaderbehaviours-methods">Back to Top</a>

## See Also


#### Reference
<a href="T_Cauldron_XAML_Interactivity_GridViewColumnHeaderBehaviours">GridViewColumnHeaderBehaviours Class</a><br /><a href="N_Cauldron_XAML_Interactivity">Cauldron.XAML.Interactivity Namespace</a><br />