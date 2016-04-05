using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Couldron.Attached
{
    /// <summary>
    /// Provides an attached property required for the validation
    /// </summary>
    public static partial class ValidationProperties
    {
        #region Dependency Attached Property IsEnabledByError

        /// <summary>
        /// Identifies the MyProperty dependency property
        /// <para/>
        /// Sets the <see cref="Control.IsEnabled"/> property to false if the associated source property has errors
        /// </summary>
        public static readonly DependencyProperty IsEnabledByErrorProperty = DependencyProperty.RegisterAttached("IsEnabledByError", typeof(string), typeof(ValidationProperties), new PropertyMetadata(null, OnMyPropertyChanged));

        /// <summary>
        /// Gets the value of IsEnabledByError
        /// <para/>
        /// Sets the <see cref="Control.IsEnabled"/> property to false if the associated source property has errors
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static string GetIsEnabledByError(DependencyObject obj)
        {
            return (string)obj.GetValue(IsEnabledByErrorProperty);
        }

        /// <summary>
        /// Sets the value of the IsEnabledByError attached property
        /// <para/>
        /// Sets the <see cref="Control.IsEnabled"/> property to false if the associated source property has errors
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetIsEnabledByError(DependencyObject obj, string value)
        {
            obj.SetValue(IsEnabledByErrorProperty, value);
        }

        private static void OnMyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var propertyName = args.NewValue as string;

            if (string.IsNullOrEmpty(propertyName))
                return;

            var control = dependencyObject as Control;

            if (control == null)
                return;

            control.DataContext.CastTo<INotifyDataErrorInfo>()
                .IsNotNull(x =>
                {
                    control.IsEnabled = !x.GetErrors(propertyName).Any();
                });
        }

        #endregion Dependency Attached Property IsEnabledByError
    }
}