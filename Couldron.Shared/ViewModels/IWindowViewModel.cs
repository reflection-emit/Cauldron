using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Couldron.ViewModels
{
    public interface IWindowViewModel : IViewModel
    {
        bool CanClose();

        void GotFocus();

        void LostFocus();

        void SizeChanged(double width, double height);
    }
}