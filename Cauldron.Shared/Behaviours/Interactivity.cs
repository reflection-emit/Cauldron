using Cauldron.Collections;
using System;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml;

#else

using System.Windows;

#endif

namespace Cauldron.Behaviours
{
    /// <summary>
    /// Defines a container that enables to attach behaviours to a <see cref="Style"/>
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <Setter Property="behaviour:Interactivity.Template">
    ///     <Setter.Value>
    ///         <behaviour:InteractivityTemplate>
    ///             <behaviour:NavigateToLinkDescribedByContent />
    ///          </behaviour:InteractivityTemplate>
    ///     </Setter.Value>
    /// </Setter>
    /// ]]>
    /// </example>
    public class Interactivity : FrameworkElement
    {
        /// <summary>
        /// Gets or sets the <see cref="InteractivityTemplate"/> associated with a specified object.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached("Template", typeof(InteractivityTemplate),
                typeof(Interactivity), new PropertyMetadata(default(InteractivityTemplate), OnTemplateChanged));

        private List<IBehaviour> _behaviors;

        /// <summary>
        /// Gets a collection of behaviours
        /// </summary>
        public List<IBehaviour> Behaviours
        {
            get
            {
                if (_behaviors == null)
                    _behaviors = new List<IBehaviour>();

                return _behaviors;
            }
        }

        /// <summary>
        /// Gets the <see cref="InteractivityTemplate"/> associated with a specified object.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> from which to retrieve the <see cref="InteractivityTemplate"/>.</param>
        /// <returns>A <see cref="InteractivityTemplate"/> containing the behaviors associated with the specified object.</returns>
        public static InteractivityTemplate GetTemplate(DependencyObject dependencyObject)
        {
            return (InteractivityTemplate)dependencyObject.GetValue(TemplateProperty);
        }

        /// <summary>
        /// Sets the <see cref="InteractivityTemplate"/> associated with a specified object.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> on which to set the <see cref="InteractivityTemplate"/>.</param>
        /// <param name="value">The <see cref="InteractivityTemplate"/> associated with the object.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dependencyObject"/> is null</exception>
        public static void SetTemplate(DependencyObject dependencyObject, InteractivityTemplate value)
        {
            if (dependencyObject == null)
                throw new ArgumentNullException(nameof(dependencyObject));

            dependencyObject.SetValue(TemplateProperty, value);
        }

        private static void OnTemplateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            InteractivityTemplate dt = args.NewValue as InteractivityTemplate;

            if (dt == null)
                return;

            BehaviourCollection bc = Interaction.GetBehaviours(dependencyObject);

            // Remove all behaviours first that were assigned by this class
            bc.RemoveAllTemplateAssignedBehaviours();

            // Then let us create a shallow copy of all behaviours
            foreach (IBehaviour behavior in dt)
                bc.Add(behavior.Copy());
        }
    }
}