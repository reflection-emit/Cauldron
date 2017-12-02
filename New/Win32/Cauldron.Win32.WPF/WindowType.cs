using Cauldron.Activator;
using System;
using System.Windows;

namespace Cauldron.XAML
{
    internal class WindowType
    {
        public bool IsFactoryType { get; set; }
        public Type Type { get; set; }

        public Window CreateWindow()
        {
            if (this.IsFactoryType)
                return Factory.Create<Window>();

            return this.Type.CreateInstance() as Window;
        }
    }
}