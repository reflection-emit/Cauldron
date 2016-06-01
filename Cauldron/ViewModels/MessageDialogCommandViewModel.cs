using System.Windows.Input;

namespace Cauldron.ViewModels
{
    internal sealed class MessageDialogCommandViewModel : ViewModelBase
    {
        private string _text;

        /// <summary>
        /// Gets the Button command
        /// </summary>
        public ICommand ButtonCommand { get; set; }

        public string Text
        {
            get { return this._text; }
            set
            {
                if (this._text == value)
                    return;

                this._text = value;
                this.RaisePropertyChanged();
            }
        }
    }
}