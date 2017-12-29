using Cauldron.Core.Interceptors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System.IO;
using System.Linq;

namespace Win32_Interceptors_Test
{
    [TestClass]
    public class RegistryAttribute_Tests
    {
        [TestMethod]
        public void ByteArrayProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.ByteArrayProperty = new byte[] { 9, 244, 10, 55, 34 };

            var checkClass = new RegistryAttributeTestClass();
            Assert.IsTrue(new byte[] { 9, 244, 10, 55, 34 }.SequenceEqual(checkClass.ByteArrayProperty));
        }

        [TestCleanup]
        public void CleanUp()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.Clean();
        }

        [TestMethod]
        public void Default_Value_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.Clean();

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual("", checkClass.StringProperty);
            Assert.AreEqual((uint)0, checkClass.UintProperty);
            Assert.AreEqual((ulong)0, checkClass.UlongProperty);
            Assert.AreEqual(0, checkClass.LongProperty);
            Assert.AreEqual(0, checkClass.IntProperty);
        }

        [TestMethod]
        public void DirectoryInfoProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.DirectoryInfoProperty = new DirectoryInfo(Path.GetTempPath());

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual(new DirectoryInfo(Path.GetTempPath()).FullName, testClass.DirectoryInfoProperty.FullName);
        }

        [TestMethod]
        public void FileInfoProperty_Test()
        {
            var filename = Path.Combine(Path.GetTempPath(), "toast.txt");
            var testClass = new RegistryAttributeTestClass();
            testClass.FileInfoProperty = new FileInfo(filename);

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual(new FileInfo(filename).FullName, testClass.FileInfoProperty.FullName);
        }

        [TestMethod]
        public void IntProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.IntProperty = int.MaxValue;

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual(int.MaxValue, checkClass.IntProperty);
        }

        [TestMethod]
        public void LongProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.LongProperty = long.MaxValue;

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual(long.MaxValue, checkClass.LongProperty);
        }

        [TestMethod]
        public void StringArrayProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.StringArrayProperty = new string[]
            {
                "First Line",
                "Second line",
                "Third line"
            };

            var checkClass = new RegistryAttributeTestClass();
            Assert.IsTrue(new string[]
            {
                "First Line",
                "Second line",
                "Third line"
            }.SequenceEqual(checkClass.StringArrayProperty));
        }

        [TestMethod]
        public void StringProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.StringProperty = "Hello Hello Hello";

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual("Hello Hello Hello", checkClass.StringProperty);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.Clean();
        }

        [TestMethod]
        public void UintProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.UintProperty = uint.MaxValue;

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual(uint.MaxValue, checkClass.UintProperty);
        }

        [TestMethod]
        public void ULongProperty_Test()
        {
            var testClass = new RegistryAttributeTestClass();
            testClass.UlongProperty = ulong.MaxValue;

            var checkClass = new RegistryAttributeTestClass();
            Assert.AreEqual(ulong.MaxValue, checkClass.UlongProperty);
        }

        [RegistryClass(RegistryHive.CurrentUser, "Software\\Cauldron\\UnitTest")]
        public class RegistryAttributeTestClass
        {
            public byte[] ByteArrayProperty { get; set; }
            public DirectoryInfo DirectoryInfoProperty { get; set; }
            public FileInfo FileInfoProperty { get; set; }
            public int IntProperty { get; set; }
            public long LongProperty { get; set; }
            public string[] StringArrayProperty { get; set; }
            public string StringProperty { get; set; }
            public uint UintProperty { get; set; }
            public ulong UlongProperty { get; set; }

            public void Clean()
            {
                this.ByteArrayProperty = null;
                this.IntProperty = 0;
                this.LongProperty = 0;
                this.DirectoryInfoProperty = null;
                this.FileInfoProperty = null;
                this.StringArrayProperty = null;
                this.StringProperty = null;
                this.UintProperty = 0;
                this.UlongProperty = 0;
            }
        }
    }
}