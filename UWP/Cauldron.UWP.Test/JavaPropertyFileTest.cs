#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

using Cauldron.Core.Java;

namespace Cauldron.Test
{
    [TestClass]
    public class JavaPropertyFileTest
    {
        [TestMethod]
        public void ReadPropertyFile_Test()
        {
            var file =
                "#Fri Jan 17 22:37:45 MYT 2014\r\n" +
                "! Comment\r\n" +
                "# Comment\r\n" +
                "!Comment\r\n" +
                "dbpassword=password\n" +
                "dbpassword2=he\\tjki\n" +
                "database=localhost\\u0394\n" +
                "Hong\\ Kong = Near China\n" +
                "a-key = a-value \\u2640 people\n" +
                "b\\u2126key : a-value\n" +
                "c-key=a-value\r\n" +
                "d-key a-value\r\n" +
                "a-longer-key-example = a really long value that is \\\r\n" +
                "    split over two lines.\r" +
                "dbuser=mkyong\r\n";

            var properties = JavaProperties.Read(file);

            Assert.AreEqual(10, properties.Count);
            Assert.AreEqual("password", properties["dbpassword"].Value);
            Assert.AreEqual("localhostΔ", properties["database"].Value);
            Assert.AreEqual("Hong Kong", properties["Hong Kong"].Key);
            Assert.AreEqual("bΩkey", properties["bΩkey"].Key);
            Assert.AreEqual("a really long value that is  split over two lines.", properties["a-longer-key-example"].Value);
        }

        [TestMethod]
        public void WritePropertyFile_Test()
        {
            var file = new JavaProperties();

            file.Add(new PropertyLine("dbpassword", "password"));
            file.Add(new PropertyLine("database", "localhost ♣ st"));
            file.Add(new PropertyLine("King Kong", "a really long value ioio"));

            var properties = JavaProperties.Read(file.ToString());

            Assert.AreEqual("password", properties["dbpassword"].Value);
            Assert.AreEqual("localhost ♣ st", properties["database"].Value);
            Assert.AreEqual("King Kong", properties["King Kong"].Key);
            Assert.AreEqual("a really long value ioio", properties["King Kong"].Value);
        }
    }
}