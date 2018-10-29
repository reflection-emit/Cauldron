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
        public void RegisterChild_IsChanged_OnChild_Test()
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

        [TestMethod]
        public void RegisterChild_IsChanged_OnChild_Then_Parent_Test()
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

            parent.IsChanged = false;

            Assert.IsFalse(parent.Children[0].IsChanged);
        }

        [TestMethod]
        public void RegisterChild_IsLoading_OnChild_Test()
        {
            var parent = new ChildRegisterTestViewModel();
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());

            parent.Children[0].IsLoading = true;

            Assert.IsTrue(parent.IsLoading);
        }

        [TestMethod]
        public void RegisterChild_IsLoading_OnChild_Then_Parent_Test()
        {
            var parent = new ChildRegisterTestViewModel();
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());

            parent.Children[0].IsLoading = true;

            Assert.IsTrue(parent.IsLoading);

            parent.IsLoading = false;

            Assert.IsTrue(parent.Children[0].IsLoading);
        }

        public void RegisterChild_RemoveChild_Should_Not_Trigger_Parent()
        {
            var parent = new ChildRegisterTestViewModel();
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());
            parent.Children.Add(new ChildRegisterChildTestViewmodel());

            var child = parent.Children[0];

            child.IsChanged = true;

            Assert.IsTrue(parent.IsChanged);

            parent.IsChanged = false;

            Assert.IsFalse(child.IsChanged);

            parent.Children.Remove(child);

            child.IsChanged = true;

            Assert.IsFalse(parent.IsChanged);
        }
    }

    #region Resources

    public class ChildRegisterChildTestViewmodel : ViewModelBase, IChangeAwareViewModel
    {
        public event EventHandler<PropertyIsChangedEventArgs> Changed;

        public event EventHandler IsChangedChanged;

        public bool IsChanged { get; set; }
        public string TestProperty { get; set; }

        public void RaisePropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    public class ChildRegisterTestViewModel : ViewModelBase, IChangeAwareViewModel
    {
        public event EventHandler<PropertyIsChangedEventArgs> Changed;

        public event EventHandler IsChangedChanged;

        [RegisterChildren(propagatesIsChange: true, propagatesIsLoading: true)]
        public ObservableCollection<ChildRegisterChildTestViewmodel> Children { get; private set; } = new ObservableCollection<ChildRegisterChildTestViewmodel>();

        public bool IsChanged { get; set; }

        public void RaisePropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    #endregion Resources
}