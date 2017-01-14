using Cauldron.Core;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Cauldron.XAML
{
    [DataContract(IsReference = false, Name = "PersistentWindowInformation", Namespace = "Cauldron.XAML.Navigation")]
    internal sealed class PersistentWindowInformation
    {
        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Width { get; set; }

        public static async Task Load(Window window, Type viewModelType)
        {
            PersistentWindowProperties.SetHeight(window, Mathc.Clamp(window.ActualHeight, window.MinHeight, window.MaxHeight));
            PersistentWindowProperties.SetWidth(window, Mathc.Clamp(window.ActualWidth, window.MinWidth, window.MaxWidth));

            var obj = await Serializer.DeserializeAsync<PersistentWindowInformation>("Navigator");

            if (obj == null)
                return;

            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = obj.Left;
            window.Top = obj.Top;
            window.Height = obj.Height;
            window.Width = obj.Width;
            window.WindowState = (WindowState)obj.State;
        }

        public static async Task Save(Window window, Type viewModelType)
        {
            var obj = new PersistentWindowInformation();
            obj.Width = Mathc.Clamp(PersistentWindowProperties.GetWidth(window), window.MinWidth, window.MaxWidth);
            obj.Height = Mathc.Clamp(PersistentWindowProperties.GetHeight(window), window.MinHeight, window.MaxHeight);
            obj.Top = window.Top;
            obj.Left = window.Left;
            obj.State = (int)window.WindowState;

            await Serializer.SerializeAsync(obj, "Navigator");
        }
    }
}