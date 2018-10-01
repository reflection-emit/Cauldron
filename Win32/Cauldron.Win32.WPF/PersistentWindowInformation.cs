using System;
using System.IO;
using System.Windows;
using Windows.Storage;

namespace Cauldron.XAML
{
    internal sealed class PersistentWindowInformation
    {
        public double Height { get; set; }

        public double Left { get; set; }

        public int State { get; set; }

        public double Top { get; set; }

        public double Width { get; set; }

        public static void Load(Window window, Type viewModelType)
        {
            PersistentWindowProperties.SetHeight(window, MathEx.Clamp(window.ActualHeight, window.MinHeight, window.MaxHeight));
            PersistentWindowProperties.SetWidth(window, MathEx.Clamp(window.ActualWidth, window.MinWidth, window.MaxWidth));

            var filename = Path.Combine(ApplicationData.Current.LocalFolder.FullName, viewModelType.FullName.GetHash() + "_Navigator");

            if (!File.Exists(filename))
                return;

            var dictionary = new KeyRawValueDictionary();
            dictionary.Deserialize(File.ReadAllBytes(filename));

            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = dictionary["Left"].ToDouble();
            window.Top = dictionary["Top"].ToDouble();
            window.Height = Math.Max(dictionary["Height"].ToDouble(), 1);
            window.Width = Math.Max(dictionary["Width"].ToDouble(), 1);
            window.WindowState = (WindowState)dictionary["WindowState"].ToInteger();
        }

        public static void Save(Window window, Type viewModelType)
        {
            var filename = Path.Combine(ApplicationData.Current.LocalFolder.FullName, viewModelType.FullName.GetHash() + "_Navigator");

            var dictionary = new KeyRawValueDictionary
            {
                { "Width", MathEx.Clamp(PersistentWindowProperties.GetWidth(window), window.MinWidth, window.MaxWidth) },
                { "Height", MathEx.Clamp(PersistentWindowProperties.GetHeight(window), window.MinHeight, window.MaxHeight) },
                { "Top", window.Top },
                { "Left", window.Left },
                { "WindowState", (int)window.WindowState }
            };

            File.WriteAllBytes(filename, dictionary.Serialize());
        }
    }
}