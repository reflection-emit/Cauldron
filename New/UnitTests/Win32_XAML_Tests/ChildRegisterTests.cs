using Cauldron.XAML;
using Cauldron.XAML.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;

namespace Win32_XAML_Tests
{
    [TestClass]
    public class ChildRegisterTests
    {
        [TestMethod]
        public void RegisterChild_Test()
        {
            var parent = new ChildRegisterTestViewModel();
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());

            parent.Children[0].IsChanged = true;

            Assert.IsTrue(parent.IsChanged);
        }
    }

    #region Resources

    public class ChildRegisterChildTestViewmodel : ViewModelBase, IChangeAwareViewModel
    {
        public event EventHandler<PropertyIsChangedEventArgs> Changed;

        public bool IsChanged { get; set; }
        public string TestProperty { get; set; }

        public void RaisePropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    public class ChildRegisterTestViewModel : ViewModelBase, IChangeAwareViewModel
    {
        public event EventHandler<PropertyIsChangedEventArgs> Changed;

        [RegisterChildren(propagatesIsChange: true, propagatesIsLoading: true)]
        public ObservableCollection<ChildRegisterChildTestViewmodel> Children { get; private set; } = new ObservableCollection<ChildRegisterChildTestViewmodel>();

        public bool IsChanged { get; set; }

        public void RaisePropertyChanged(string propertyName, object before, object after)
        {
            // Auto implementation
        }
    }

    #endregion Resources
}