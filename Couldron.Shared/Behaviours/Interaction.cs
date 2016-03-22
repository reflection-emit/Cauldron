using Couldron.Collections;
using System;
using System.Linq;
using System.Windows;

namespace Couldron.Behaviours
{
    /// <summary>
    /// Defines a <see cref="BehaviourCollection"/> attached property
    /// </summary>
    public static class Interaction
    {
        /// <summary>
        /// Gets or sets the <see cref="BehaviourCollection"/> associated with a specified object.
        /// </summary>
        public static readonly DependencyProperty BehavioursProperty = DependencyProperty.RegisterAttached("BehavioursInternal",
            typeof(BehaviourCollection), typeof(Interaction), new PropertyMetadata(null));

        /// <summary>
        /// Adds a new behaviour to the <see cref="BehaviourCollection"/>
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> to which to add to</param>
        /// <param name="behaviour">The behaviour to add</param>
        public static void AddBehaviour(DependencyObject dependencyObject, IBehaviour behaviour)
        {
            GetBehaviours(dependencyObject).Add(behaviour);
        }

        /// <summary>
        /// Get a behaviours in the <see cref="BehaviourCollection"/> associated with the specified object
        /// </summary>
        /// <typeparam name="T">The type of the behaviour</typeparam>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> from which to retrieve the <see cref="BehaviourCollection"/>.</param>
        /// <returns>An array of behaviours</returns>
        public static T[] GetBehaviour<T>(DependencyObject dependencyObject) where T : IBehaviour
        {
            var behaviourCollection = Interaction.GetBehaviours(dependencyObject);

            if (behaviourCollection == null)
                return null;

            return behaviourCollection.Where(x => x.GetType() == typeof(T)).ToArray<T>();
        }

        /// <summary>
        /// Gets the <see cref="BehaviourCollection"/> associated with a specified object.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> from which to retrieve the <see cref="BehaviourCollection"/>.</param>
        /// <returns>A <see cref="BehaviourCollection"/> containing the behaviors associated with the specified object.</returns>
        public static BehaviourCollection GetBehaviours(DependencyObject dependencyObject)
        {
            var list = dependencyObject.GetValue(Interaction.BehavioursProperty) as BehaviourCollection;

            if (list == null)
            {
                list = new BehaviourCollection();
                list.owner = dependencyObject;

                dependencyObject.SetValue(Interaction.BehavioursProperty, list);
            }

            return list;
        }

        /// <summary>
        /// Sets the <see cref="BehaviourCollection"/> associated with a specified object.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> on which to set the <see cref="BehaviourCollection"/>.</param>
        /// <param name="value">The <see cref="BehaviourCollection"/> associated with the object.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dependencyObject"/> is null</exception>
        public static void SetBehaviours(DependencyObject dependencyObject, BehaviourCollection value)
        {
            if (dependencyObject == null)
                throw new ArgumentNullException(nameof(dependencyObject));

            dependencyObject.SetValue(Interaction.BehavioursProperty, value);
        }
    }
}