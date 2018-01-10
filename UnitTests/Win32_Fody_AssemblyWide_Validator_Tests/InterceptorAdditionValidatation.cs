using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Win32_Fody_AssemblyWide_Validator_Tests
{
    public struct HHH
    {
    }

    [TestClass]
    public class InterceptorAdditionValidatation
    {
        public DateTime Bla { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            try
            {
                if (Toast() | Toast() | Toast())
                    throw new Exception();
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }

        [TestMethod]
        public int TestMethod2()
        {
            try
            {
                if (Toast() | Toast() | Toast())
                    throw new Exception();

                return 8;
            }
            catch (Exception)
            {
                return 9;
            }
            finally
            {
            }
        }

        public HHH uiu()
        {
            try
            {
                return new HHH();
            }
            catch (Exception)
            {
                return default(HHH);
            }
        }

        private bool Toast() => false;
    }
}