using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Activator;
using Cauldron.XAML.ViewModels;
using PropertyChanged;
using Cauldron.Core;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Cauldron.Test
{
    public class BirdViewModel : ViewModelBase
    {
        public double FlightHeight { get; set; }

        public string Name { get; set; }

        [AlsoNotifyFor(nameof(FlightHeight))]
        public double Speed { get; set; }
    }

    [TestClass]
    public class ViewModelTest
    {
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

            DispatcherEx.Current.ProcessEvents();

            Assert.AreEqual(true, isFlightChanged);
            Assert.AreEqual(true, isSpeedChanged);
        }

        [TestMethod]
        public void NotifyPropertyChanged_Same_Value_Test()
        {
            var vm = Factory.Create<BirdViewModel>();
            var isFired = false;
            vm.Name = "Birds fly";
            DispatcherEx.Current.ProcessEvents();

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

            DispatcherEx.Current.ProcessEvents();

            Assert.AreEqual(true, isFired);
        }
    }
}