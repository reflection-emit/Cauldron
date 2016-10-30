using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cauldron.XAML.Interactivity
{
    /// <summary>
    /// Decorates an Element with an up-arrow and down-arrow
    /// </summary>
    internal class SortAdorner : Adorner
    {
        private static Geometry ascGeometry = Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");
        private static Geometry descGeometry = Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        #region Dependency Property Foreground

        /// <summary>
        /// Identifies the <see cref="Foreground" /> dependency property
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(SortAdorner), new PropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the <see cref="Foreground" /> Property
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)this.GetValue(ForegroundProperty); }
            set { this.SetValue(ForegroundProperty, value); }
        }

        #endregion Dependency Property Foreground

        public SortAdorner(UIElement element, ListSortDirection direction) : base(element)
        {
            this.Direction = direction;
        }

        public ListSortDirection Direction { get; private set; }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transform = new TranslateTransform(AdornedElement.RenderSize.Width - 15, (AdornedElement.RenderSize.Height - 5) / 2);
            drawingContext.PushTransform(transform);
            drawingContext.DrawGeometry(this.Foreground, null, this.Direction == ListSortDirection.Descending ? descGeometry : ascGeometry);
            drawingContext.Pop();
        }
    }
}