using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace Couldron.Test
{
    [TestClass]
    public class LocalizationTest
    {
        [TestMethod]
        public void LocalizationSource_Test()
        {
            var lang = Factory.Create<Localization>();
            lang.CultureInfo = new CultureInfo("de-de");

            Assert.AreEqual("Erster text", lang["testText"]);
            Assert.AreEqual("Über uns", lang["aboutus"]);
        }
    }
}