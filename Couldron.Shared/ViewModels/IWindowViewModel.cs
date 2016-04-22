namespace Cauldron.ViewModels
{
    public interface IWindowViewModel : ICanClose
    {
        void SizeChanged(double width, double height);
    }
}