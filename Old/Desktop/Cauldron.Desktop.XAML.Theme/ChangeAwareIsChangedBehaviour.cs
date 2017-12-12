using Cauldron.XAML.Interactivity;
using Cauldron.XAML.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cauldron.XAML.Theme
{
    internal class ChangeAwareIsChangedBehaviour : Behaviour<TextBlock>
    {
        protected override void OnAttach()
        {
            if (this.AssociatedObject.DataContext is IChangeAwareViewModel)
                this.AssociatedObject.SetBinding(TextBlock.VisibilityProperty, new Binding
                {
                    Source = this.AssociatedObject.DataContext,
                    Path = new PropertyPath(nameof(IChangeAwareViewModel.IsChanged)),
                    Mode = BindingMode.OneWay,
                    Converter = new Cauldron.XAML.ValueConverters.BooleanToVisibilityConverter()
                });
            else
                this.AssociatedObject.Visibility = Visibility.Collapsed;
        }

        protected override void OnDetach()
        {
        }
    }
}