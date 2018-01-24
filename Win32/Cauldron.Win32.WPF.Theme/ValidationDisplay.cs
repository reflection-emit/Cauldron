using Cauldron.XAML.Validation;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Cauldron.XAML.Theme
{
    /// <summary>
    /// Provides a display for validation errors. The TemplateParent requires the attached properties
    /// <see cref="ValidationProperties.ErrorsProperty"/> and <see cref="ValidationProperties.HasErrorsProperty"/>
    /// </summary>
    public class ValidationDisplay : Control
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValidationDisplay"/>
        /// </summary>
        public ValidationDisplay()
        {
            this.Background = Brushes.Transparent;
            this.IsTabStop = false;
        }

        /// <exclude/>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (this.TemplatedParent == null)
                return;

            this.SetBinding(Control.ToolTipProperty, new Binding
            {
                Source = this.TemplatedParent,
                Path = new PropertyPath(ValidationProperties.ErrorsProperty),
                Converter = new InlineTextValueConverter()
            });

            this.SetBinding(Control.VisibilityProperty, new Binding
            {
                Source = this.TemplatedParent,
                Path = new PropertyPath(ValidationProperties.HasErrorsProperty),
                Converter = new BooleanToVisibilityConverter()
            });
        }
    }
}