using Cauldron.Core.Collections;
using Cauldron.Core.Extensions;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

#endif

namespace Cauldron.Core.Test
{
    [TestClass]
    public class KeyRawValueDictionary_Tests
    {
        [TestMethod]
        public void Serialize_Deserialize_Dictionary_Test()
        {
            var testData = new KeyRawValueDictionary();
            testData.Add("Key1", 490);
            testData.Add("Key2", 291.732874);
            testData.Add("Random Key", 3.5f);
            testData.Add("OMG", "A string is great");
            testData.Add("How it works", 88.668 / 66);
            testData.Add("Test Data", 33332);

            var data = testData.Serialize();

            var newCollection = new KeyRawValueDictionary();
            newCollection.Deserialize(data);

            Assert.AreEqual(testData["Key1"].ToInteger(), newCollection["Key1"].ToInteger());
            Assert.AreEqual(testData["Key2"].ToDouble(), newCollection["Key2"].ToDouble());
            Assert.AreEqual(testData["Random Key"].ToFloat(), newCollection["Random Key"].ToFloat());
            Assert.AreEqual(testData["OMG"].ToString(), newCollection["OMG"].ToString());
            Assert.AreEqual(testData["How it works"].ToDouble(), newCollection["How it works"].ToDouble());
            Assert.AreEqual(testData["Test Data"], newCollection["Test Data"]);
        }
    }
}