using System;
using System.Collections.Generic;

#if WINDOWS_UWP
using Windows.UI.Xaml;
#else

using System.Windows;

#endif

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Defines a container that enables to attach behaviours to a <see cref="Style"/>
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <Setter Property="behaviour:StyleInteraction.Template">
    ///     <Setter.Value>
    ///         <behaviour:InteractionTemplate>
    ///             <behaviour:NavigateToLinkDescribedByContent />
    ///          </behaviour:InteractionTemplate>
    ///     </Setter.Value>
    /// </Setter>
    /// ]]>
    /// </example>
    public sealed class StyleInteraction : DependencyObject
    {
        /// <summary>
        /// Gets or sets the <see cref="InteractionTemplate"/> associated with a specified object.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.RegisterAttached("Template", typeof(InteractionTemplate),
                typeof(StyleInteraction), new PropertyMetadata(default(InteractionTemplate), OnTemplateChanged));

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
        /// Gets the <see cref="InteractionTemplate"/> associated with a specified object.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> from which to retrieve the <see cref="InteractionTemplate"/>.</param>
        /// <returns>A <see cref="InteractionTemplate"/> containing the behaviors associated with the specified object.</returns>
        public static InteractionTemplate GetTemplate(DependencyObject dependencyObject) =>
            (InteractionTemplate)dependencyObject.GetValue(TemplateProperty);

        /// <summary>
        /// Sets the <see cref="InteractionTemplate"/> associated with a specified object.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> on which to set the <see cref="InteractionTemplate"/>.</param>
        /// <param name="value">The <see cref="InteractionTemplate"/> associated with the object.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dependencyObject"/> is null</exception>
        public static void SetTemplate(DependencyObject dependencyObject, InteractionTemplate value)
        {
            if (dependencyObject == null)
                throw new ArgumentNullException(nameof(dependencyObject));

            dependencyObject.SetValue(TemplateProperty, value);
        }

        private static void OnTemplateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var dt = args.NewValue as InteractionTemplate;

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