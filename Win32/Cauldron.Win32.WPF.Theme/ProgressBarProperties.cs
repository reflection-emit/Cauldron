using System.Windows;
using System.Windows.Controls;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides attached properties for the <see cref="ProgressBar"/>
    /// </summary>
    public static class ProgressBarProperties
    {
        #region Dependency Attached Property Diameter

        /// <summary>
        /// Identifies the Diameter dependency property
        /// </summary>
        internal static readonly DependencyProperty DiameterProperty = DependencyProperty.RegisterAttached("Diameter", typeof(double), typeof(ProgressBarProperties), new PropertyMetadata(8.0));

        /// <summary>
        /// Gets the value of Diameter
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        internal static double GetDiameter(DependencyObject obj) => (double)obj.GetValue(DiameterProperty);

        /// <summary>
        /// Sets the value of the Diameter attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        internal static void SetDiameter(DependencyObject obj, double value) => obj.SetValue(DiameterProperty, value);

        #endregion Dependency Attached Property Diameter

        #region Dependency Attached Property Distance

        /// <summary>
        /// Identifies the Distance dependency property
        /// </summary>
        public static readonly DependencyProperty DistanceProperty = DependencyProperty.RegisterAttached("Distance", typeof(double), typeof(ProgressBarProperties), new PropertyMetadata(1.1));

        /// <summary>
        /// Gets the value of Distance
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetDistance(DependencyObject obj)
        {
            return (double)obj.GetValue(DistanceProperty);
        }

        /// <summary>
        /// Sets the value of the Distance attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetDistance(DependencyObject obj, double value)
        {
            obj.SetValue(DistanceProperty, value);
        }

        #endregion Dependency Attached Property Distance

        #region Dependency Attached Property EllipseAnimationWellPosition

        /// <summary>
        /// Identifies the EllipseAnimationWellPosition dependency property
        /// </summary>
        internal static readonly DependencyProperty EllipseAnimationWellPositionProperty = DependencyProperty.RegisterAttached("EllipseAnimationWellPosition", typeof(double), typeof(ProgressBarProperties), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets the value of EllipseAnimationWellPosition
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        internal static double GetEllipseAnimationWellPosition(DependencyObject obj)
        {
            return (double)obj.GetValue(EllipseAnimationWellPositionProperty);
        }

        /// <summary>
        /// Sets the value of the EllipseAnimationWellPosition attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        internal static void SetEllipseAnimationWellPosition(DependencyObject obj, double value)
        {
            obj.SetValue(EllipseAnimationWellPositionProperty, value);
        }

        #endregion Dependency Attached Property EllipseAnimationWellPosition

        #region Dependency Attached Property EllipseAnimationEndPosition

        /// <summary>
        /// Identifies the EllipseAnimationEndPosition dependency property
        /// </summary>
        internal static readonly DependencyProperty EllipseAnimationEndPositionProperty = DependencyProperty.RegisterAttached("EllipseAnimationEndPosition", typeof(double), typeof(ProgressBarProperties), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets the value of EllipseAnimationEndPosition
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        internal static double GetEllipseAnimationEndPosition(DependencyObject obj)
        {
            return (double)obj.GetValue(EllipseAnimationEndPositionProperty);
        }

        /// <summary>
        /// Sets the value of the EllipseAnimationEndPosition attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        internal static void SetEllipseAnimationEndPosition(DependencyObject obj, double value)
        {
            obj.SetValue(EllipseAnimationEndPositionProperty, value);
        }

        #endregion Dependency Attached Property EllipseAnimationEndPosition

        #region Dependency Attached Property Value

        /// <summary>
        /// Identifies the Value dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(double), typeof(ProgressBarProperties), new PropertyMetadata(1.0));

        /// <summary>
        /// Gets the value of Value
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <returns>The value of the attached property</returns>
        public static double GetValue(DependencyObject obj)
        {
            return (double)obj.GetValue(ValueProperty);
        }

        /// <summary>
        /// Sets the value of the Value attached property
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject" /> with the attached property</param>
        /// <param name="value">The new value to set</param>
        public static void SetValue(DependencyObject obj, double value)
        {
            obj.SetValue(ValueProperty, value);
        }

        #endregion Dependency Attached Property Value
    }
}