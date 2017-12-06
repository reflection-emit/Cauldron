using Cauldron.Core.Extensions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#if WINDOWS_UWP

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

namespace Cauldron.Interception.Test
{
#if DESKTOP

    [TestClass]
    public class TimedCachedAttributeTest
    {
        [TestMethod]
        public void CacheTest_1()
        {
            var stopWatch = Stopwatch.StartNew();

            var firstString = this.PerfectString(40);
            stopWatch.Stop();

            var firstResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var secondString = this.PerfectString(40);
            stopWatch.Stop();

            var secondResult = stopWatch.Elapsed.TotalMilliseconds;

            Assert.AreEqual(true, firstResult > 3000);
            Assert.AreEqual(true, secondResult < 500);
            Assert.AreEqual("40", firstString);
            Assert.AreEqual("40", secondString);
        }

        [TestMethod]
        public void CacheTest_2()
        {
            var stopWatch = Stopwatch.StartNew();
            var firstString = this.PerfectString(80);
            stopWatch.Stop();
            var firstResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var secondString = this.PerfectString(80);
            stopWatch.Stop();
            var secondResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var thirdString = this.PerfectString(60);
            stopWatch.Stop();
            var thirdResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var forthString = this.PerfectString(60);
            stopWatch.Stop();
            var forthResult = stopWatch.Elapsed.TotalMilliseconds;

            Assert.AreEqual(true, firstResult > 3000);
            Assert.AreEqual(true, secondResult < 500);
            Assert.AreEqual("80", firstString);
            Assert.AreEqual("80", secondString);

            Assert.AreEqual(true, thirdResult > 3000);
            Assert.AreEqual(true, forthResult < 500);
            Assert.AreEqual("60", thirdString);
            Assert.AreEqual("60", forthString);
        }

        [TestMethod]
        public void CacheTest_Async_1()
        {
            var stopWatch = Stopwatch.StartNew();

            var firstString = this.PerfectStringAsync(40).RunSync();
            stopWatch.Stop();

            var firstResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var secondString = this.PerfectStringAsync(40).RunSync();
            stopWatch.Stop();

            var secondResult = stopWatch.Elapsed.TotalMilliseconds;

            Assert.AreEqual(true, firstResult > 3000);
            Assert.AreEqual(true, secondResult < 500);
            Assert.AreEqual("40", firstString);
            Assert.AreEqual("40", secondString);
        }

        [TestMethod]
        public void CacheTest_Async_2()
        {
            var stopWatch = Stopwatch.StartNew();
            var firstString = this.PerfectStringAsync(80).RunSync();
            stopWatch.Stop();
            var firstResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var secondString = this.PerfectStringAsync(80).RunSync();
            stopWatch.Stop();
            var secondResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var thirdString = this.PerfectStringAsync(60).RunSync();
            stopWatch.Stop();
            var thirdResult = stopWatch.Elapsed.TotalMilliseconds;

            stopWatch = Stopwatch.StartNew();
            var forthString = this.PerfectStringAsync(60).RunSync();
            stopWatch.Stop();
            var forthResult = stopWatch.Elapsed.TotalMilliseconds;

            Assert.AreEqual(true, firstResult > 3000);
            Assert.AreEqual(true, secondResult < 500);
            Assert.AreEqual("80", firstString);
            Assert.AreEqual("80", secondString);

            Assert.AreEqual(true, thirdResult > 3000);
            Assert.AreEqual(true, forthResult < 500);
            Assert.AreEqual("60", thirdString);
            Assert.AreEqual("60", forthString);
        }

        [TimedCache]
        private string PerfectString(int index)
        {
            Thread.Sleep(4000);

            return index.ToString();
        }

        [TimedCache]
        private async Task<string> PerfectStringAsync(int index)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(4000);
            });
            return index.ToString();
        }

        private async Task<string> PerfectStringAsync2(int index, string io, Guid iiii, double erte)
        {
            if (index == 0)
                return await this.PerfectStringAsync(index);
            else
                return await Task.FromResult("88");
        }
    }

#endif
}