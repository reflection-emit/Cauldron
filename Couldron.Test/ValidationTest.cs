using Cauldron.Test.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cauldron.Test
{
    [TestClass]
    public sealed class ValidationTest
    {
        [TestMethod]
        public void Equality_Validator_Error_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Password = "Password123";

            Assert.AreEqual(true, vm.HasErrors);
        }

        [TestMethod]
        public void Equality_Validator_NoError_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Password = "Password123";
            vm.Password2 = "Password123";

            Assert.AreEqual(false, vm.HasErrors);
        }

        [TestMethod]
        public void Simple_Validation_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Name = "";

            Assert.AreEqual(true, vm.HasErrors);
        }

        [TestMethod]
        public void StringLength_Length_Validator_Error_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Text = "Blabla";

            Assert.AreEqual(true, vm.HasErrors);
        }

        [TestMethod]
        public void StringLength_Length_Validator_NoErrors_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Text = "0123456789";

            Assert.AreEqual(false, vm.HasErrors);
        }

        [TestMethod]
        public void StringLength_Max_Validator_Error_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Caption = "01234567890";

            Assert.AreEqual(true, vm.HasErrors);
        }

        [TestMethod]
        public void StringLength_Min_Validator_Error_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Caption = "0123";

            Assert.AreEqual(true, vm.HasErrors);
        }

        [TestMethod]
        public void StringLength_MinMax_Validator_NoErrors_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Caption = "012345678";

            Assert.AreEqual(false, vm.HasErrors);
        }

        [TestMethod]
        public void Validation_Message_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Name = "";
            var errorMessage = string.Join("\n", vm.GetErrors(nameof(SparrowViewModel.Name)).ToArray<string>());

            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
        }
    }
}