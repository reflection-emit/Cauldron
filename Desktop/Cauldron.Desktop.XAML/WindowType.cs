using Cauldron.Activator;
using Cauldron.Core.Extensions;
using System;
using System.Windows;

namespace Cauldron.XAML
{
    internal class WindowType
    {
        public bool IsCutomWindow { get { return this.IsFactoryType || this.Type == typeof(Window); } }
        public bool IsFactoryType { get; set; }
        public Type Type { get; set; }

        public Window CreateWindow()
        {
            if (this.IsFactoryType)
                return Factory.Create<Window>();

            if (!this.IsCutomWindow)
                return new Window();

            return this.Type.CreateInstance() as Window;
        }
    }
}