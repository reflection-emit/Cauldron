using System;
using Cauldron.Activator;
using Cauldron.XAML.ViewModels;
using PropertyChanged;
using Cauldron.XAML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cauldron.Threading;
using System.Threading.Tasks;

namespace Cauldron.Test
{
    public class BirdViewModel : ViewModelBase
    {
        public double FlightHeight { get; set; }

        public string Name { get; set; }

        [AlsoNotifyFor(nameof(FlightHeight))]
        public double Speed { get; set; }
    }

    public abstract class IsChangedViewModel_Abstract : ViewModelBase, IChangeAwareViewModel
    {
        public event EventHandler<PropertyIsChangedEventArgs> Changed;

        public event EventHandler IsChangedChanged;

        public bool IsChanged { get; set; } /* This should be implemented by PropertyChanged.Fody */

        public string TestProperty { get; set; } /* This should be implemented by PropertyChanged.Fody */

        public abstract void RaisePropertyChanged(string propertyName, object before, object after);
    }

    public class IsChangedViewModel_OverrideVirtual : IsChangedViewModel_With_Virtual
    {
        public override void RaisePropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    public class IsChangedViewModel_OverrideVirtual_With_BaseCall : IsChangedViewModel_With_Virtual
    {
        public override void RaisePropertyChanged(string propertyName, object before, object after)
        {
            base.RaisePropertyChanged(propertyName, before, after);
        }
    }

    public class IsChangedViewModel_Simple : ViewModelBase, IChangeAwareViewModel
    {
        public event EventHandler<PropertyIsChangedEventArgs> Changed;

        public event EventHandler IsChangedChanged;

        public bool IsChanged { get; set; } /* This should be implemented by PropertyChanged.Fody */

        public string TestProperty { get; set; } /* This should be implemented by PropertyChanged.Fody */

        public void RaisePropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    public class IsChangedViewModel_SimpleOverride : IsChangedViewModel_Abstract
    {
        public override void RaisePropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    public class IsChangedViewModel_SimpleOverride_With_BaseClassCall : IsChangedViewModel_SimpleOverride
    {
        public override void RaisePropertyChanged(string propertyName, object before, object after)
        {
            base.RaisePropertyChanged(propertyName, before, after);
        }
    }

    public class IsChangedViewModel_With_Virtual : ViewModelBase, IChangeAwareViewModel
    {
        public event EventHandler<PropertyIsChangedEventArgs> Changed;

        public event EventHandler IsChangedChanged;

        public bool IsChanged { get; set; } /* This should be implemented by PropertyChanged.Fody */

        public string TestProperty { get; set; } /* This should be implemented by PropertyChanged.Fody */

        public virtual void RaisePropertyChanged(string propertyName, object before, object after)
        {
        }
    }

    [TestClass]
    public class ViewModelTest
    {
        [TestMethod]
        public void ChangeAwareViewModel_AbstractBase_SimpleClass_Test()
        {
            var triggered = false;
            var vm = new IsChangedViewModel_SimpleOverride();
            vm.Changed += (s, e) =>
            {
                triggered = true;
            };
            vm.TestProperty = "tt;";
            Assert.AreEqual(true, triggered);
        }

        [TestMethod]
        public void ChangeAwareViewModel_AbstractBase_SimpleOverride_With_BaseClassCall_Test()
        {
            var triggered = 0;
            var vm = new IsChangedViewModel_SimpleOverride_With_BaseClassCall();
            vm.Changed += (s, e) =>
            {
                triggered++;
            };
            vm.TestProperty = "tt;";

            // Should only be triggered once
            Assert.AreEqual(1, triggered);
        }

        [TestMethod]
        public void ChangeAwareViewModel_SimpleClass_Test()
        {
            var triggered = false;
            var vm = new IsChangedViewModel_Simple();
            vm.Changed += (s, e) =>
            {
                triggered = true;
            };
            vm.TestProperty = "tt;";

            Assert.AreEqual(true, triggered);
        }

        [TestMethod]
        public void ChangeAwareViewModel_Virtual_Override_Test()
        {
            var triggered = 0;
            var vm = new IsChangedViewModel_OverrideVirtual();
            vm.Changed += (s, e) =>
            {
                triggered++;
            };
            vm.TestProperty = "tt;";

            // Should only be triggered once
            Assert.AreEqual(1, triggered);
        }

        [TestMethod]
        public void ChangeAwareViewModel_Virtual_Override_With_BaseCall_Test()
        {
            var triggered = 0;
            var vm = new IsChangedViewModel_OverrideVirtual_With_BaseCall();
            vm.Changed += (s, e) =>
            {
                triggered++;
            };
            vm.TestProperty = "tt;";

            // Should only be triggered once
            Assert.AreEqual(1, triggered);
        }

        [TestMethod]
        public void ChangeAwareViewModel_Virtual_Test()
        {
            var triggered = 0;
            var vm = new IsChangedViewModel_With_Virtual();
            vm.Changed += (s, e) =>
            {
                triggered++;
            };
            vm.TestProperty = "tt;";

            // Should only be triggered once
            Assert.AreEqual(1, triggered);
        }

        [TestMethod]
        public void NotifyPropertyChanged_Multiple_PropertyChange_Fire_Test()
        {
            var vm = Factory.Create<BirdViewModel>();
            var isFlightChanged = false;
            var isSpeedChanged = false;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BirdViewModel.FlightHeight))
                    isFlightChanged = true;
                else if (e.PropertyName == nameof(BirdViewModel.Speed))
                    isSpeedChanged = true;
            };
            vm.Speed = 88.2;

            Assert.AreEqual(true, isFlightChanged);
            Assert.AreEqual(true, isSpeedChanged);
        }

        [TestMethod]
        public void NotifyPropertyChanged_Same_Value_Test()
        {
            var vm = Factory.Create<BirdViewModel>();
            var isFired = false;
            vm.Name = "Birds fly";

            vm.PropertyChanged += (s, e) => isFired = true;

            vm.Name = "Birds fly";

            Assert.AreEqual(false, isFired);
        }

        [TestMethod]
        public void NotifyPropertyChanged_Test()
        {
            var vm = Factory.Create<BirdViewModel>();
            var isFired = false;
            vm.PropertyChanged += (s, e) => isFired = true;
            vm.Name = "Birds fly";

            Assert.AreEqual(true, isFired);
        }
    }
}