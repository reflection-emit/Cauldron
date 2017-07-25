using Cauldron.Activator;
using Cauldron.Core.Extensions;
using Cauldron.XAML.Validation;
using Cauldron.XAML.Validation.ViewModels;
using System.Collections;
using System.Linq;

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Cauldron.Test
{
    public class SparrowViewModel : ValidatableViewModelBase
    {
        [StringLength(5, 10, "key")]
        public string Caption { get; set; }

        [IsMandatory("mandatory")]
        public string Name { get; set; }

        [Equality(nameof(Password2), "password does not match")]
        public string Password { get; set; }

        [Equality(nameof(Password), "password does not match 2")]
        public string Password2 { get; set; }

        [StringLength(10, "key")]
        public string Text { get; set; }
    }

    [TestClass]
    public class ValidationTest
    {
        [TestMethod]
        public void Equality_Validator_Error_Test()
        {
            var vm = Factory.Create<SparrowViewModel>();
            vm.Password = "Password123";
            vm.Password2 = "ko";

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
            vm.ValidateAsync().RunSync();

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
            vm.ValidateAsync().RunSync();
            var errorMessage = vm.GetErrors(nameof(SparrowViewModel.Name)).Cast<string>().ToArray().Join("\n");

            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
        }
    }
}