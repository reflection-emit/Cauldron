using Cauldron.XAML.Validation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides a display for a control's header and isMandatory symbol. The TemplateParent requires
    /// the attached property <see cref="ValidationProperties.IsMandatoryProperty"/>
    /// </summary>
    public class HeaderDisplay : Control
    {
        #region Dependency Property Header

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(HeaderDisplay), new PropertyMetadata(null, OnHeaderPropertyChanged));

        /// <summary>
        /// Gets or sets the <see cref="Header"/> Property
        /// </summary>
        public string Header
        {
            get { return this.GetValue(HeaderProperty) as string; }
            set { this.SetValue(HeaderProperty, value); }
        }

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as HeaderDisplay).Visibility = string.IsNullOrEmpty(args.NewValue as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion Dependency Property Header

        #region Dependency Property IsMandatory

        /// <summary>
        /// Identifies the <see cref="IsMandatory"/> dependency property
        /// </summary>
        public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(nameof(IsMandatory), typeof(Visibility), typeof(HeaderDisplay), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets the <see cref="IsMandatory"/> Property
        /// </summary>
        public Visibility IsMandatory
        {
            get { return (Visibility)this.GetValue(IsMandatoryProperty); }
            set { this.SetValue(IsMandatoryProperty, value); }
        }

        #endregion Dependency Property IsMandatory

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderDisplay"/>
        /// </summary>
        public HeaderDisplay()
        {
            this.Visibility = Visibility.Collapsed;
            this.Background = Brushes.Transparent;
            this.IsTabStop = false;
        }

        /// <exclude/>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (this.TemplatedParent == null)
                return;

            this.SetBinding(IsMandatoryProperty, new Binding
            {
                Source = this.TemplatedParent,
                Path = new PropertyPath(ValidationProperties.IsMandatoryProperty),
                Converter = new ValueConverters.BooleanToVisibilityConverter(),
                ConverterParameter = false
            });
        }
    }
}