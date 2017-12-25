using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

#if NETFX_CORE

using System.Runtime.Serialization;

#else
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace Win32_Fody_Assembly_Validation_Tests
{
    [TestClass]
    public class Serializer_Tests
    {
        [TestMethod]
        public void Serialize_Instance_With_Field_Interceptors()
        {
            var data = new SerializableClass_Properties();
            this.Serialize(data);
        }

        [TestMethod]
        public void Serialize_Instance_With_Method_Interceptors()
        {
            var data = new SerializableClass_Properties();
            this.Serialize(data);
        }

        [TestMethod]
        public void Serialize_Instance_With_Property_Interceptors()
        {
            var data = new SerializableClass_Properties();
            this.Serialize(data);
        }

        private void Serialize(object data)
        {
#if NETFX_CORE

            using (var writer = new FileStream(Path.Combine(Path.GetTempPath(), data.GetType().ToString()), FileMode.Create))
            {
                var serializer = new DataContractSerializer(data.GetType());
                serializer.WriteObject(writer, data);
            }

#else
            using (var stream = File.Open(data.GetType().ToString(), FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, data);
                stream.Close();
            }
#endif
        }

        #region Resources

#if NETFX_CORE

        [DataContract]
#else
        [Serializable]
#endif
        public class SerializableClass_Fields
        {
            [TestPropertyInterceptor]
            private string data1;

            [TestPropertyInterceptor]
            private int data2;
        }

#if NETFX_CORE

        [DataContract]
#else
        [Serializable]
#endif
        public class SerializableClass_Fields_And_Methods
        {
            public string data1;

            public int data2;

            [TestMethodInterceptor]
            public void TestMethod()
            {
                Debug.WriteLine("huhu");
            }
        }

#if NETFX_CORE

        [DataContract]
#else
        [Serializable]
#endif
        public class SerializableClass_Properties
        {
            [TestPropertyInterceptor]
            public string Data1 { get; set; }

            [TestPropertyInterceptor]
            public int Data2 { get; set; }
        }

        #endregion Resources
    }
}