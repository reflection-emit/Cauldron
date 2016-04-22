using Cauldron.Test.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cauldron.Test
{
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

            DispatcherUtil.DoEvents();

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

            DispatcherUtil.DoEvents();

            Assert.AreEqual(false, isFired);
        }

        [TestMethod]
        public void NotifyPropertyChanged_Test()
        {
            var vm = Factory.Create<BirdViewModel>();
            var isFired = false;
            vm.PropertyChanged += (s, e) => isFired = true;
            vm.Name = "Birds fly";

            DispatcherUtil.DoEvents();

            Assert.AreEqual(true, isFired);
        }
    }
}