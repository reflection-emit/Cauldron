using Cauldron.Interception.Test.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Cauldron.Interception.Test
{
    [TestClass]
    public class Field_Interception_Code_Validation_Tests
    {
        [TestFieldInterceptor]
        private string fieldOne;

        [TestMethod]
        public void Instance_Method_Field_Interception()
        {
            fieldOne = "Hi";
        }
    }
}