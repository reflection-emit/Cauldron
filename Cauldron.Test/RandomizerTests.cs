using Cauldron.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Couldron.Test
{
    [TestClass]
    public class RandomizerTests
    {
        [TestMethod]
        public void Boolean_False_Test()
        {
            var rnd = new Randomizer_Low();

            Assert.AreEqual(false, rnd.NextBoolean());
        }

        [TestMethod]
        public void Boolean_True_Test()
        {
            var rnd = new Randomizer_High();

            Assert.AreEqual(true, rnd.NextBoolean());
        }

        [TestMethod]
        public void BottomArray_Test()
        {
            var theArray = new int[] { 9, 5, 2, 4 };
            var rnd = new Randomizer_Low();

            Assert.AreEqual(9, rnd.Next(theArray));
        }

        [TestMethod]
        public void Byte_Max_Test()
        {
            var rnd = new Randomizer_High();

            Assert.AreEqual(byte.MaxValue, rnd.NextByte());
        }

        [TestMethod]
        public void Byte_Min_Test()
        {
            var rnd = new Randomizer_Low();

            Assert.AreEqual(0, rnd.NextByte());
        }

        [TestMethod]
        public void Double_Max_Test()
        {
            var rnd = new Randomizer_High();

            Assert.AreEqual(566.0, rnd.NextDouble(0, 567));
        }

        [TestMethod]
        public void Double_Min_Test()
        {
            var rnd = new Randomizer_Low();

            Assert.AreEqual(0.0, rnd.NextDouble(0, 567));
        }

        [TestMethod]
        public void TopArray_Test()
        {
            var theArray = new int[] { 9, 5, 2, 4 };
            var rnd = new Randomizer_High();

            Assert.AreEqual(4, rnd.Next(theArray));
        }

        #region Resources

        public class Randomizer_High : Randomizer
        {
            protected override Random CreateRandomInstance()
            {
                return new RandomMock_High();
            }
        }

        public class Randomizer_Low : Randomizer
        {
            protected override Random CreateRandomInstance()
            {
                return new RandomMock_Low();
            }
        }

        public class RandomMock_High : Random
        {
            public override int Next()
            {
                return int.MinValue;
            }

            public override int Next(int minValue, int maxValue)
            {
                return maxValue - 1;
            }

            public override double NextDouble()
            {
                return 1.0;
            }
        }

        public class RandomMock_Low : Random
        {
            public override int Next()
            {
                return int.MaxValue;
            }

            public override int Next(int minValue, int maxValue)
            {
                return minValue;
            }

            public override double NextDouble()
            {
                return 0.0;
            }
        }

        #endregion Resources
    }
}