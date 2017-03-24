using Cauldron.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Cauldron.Interception.Test
{
#if DESKTOP

    [TestClass]
    public class CachedAttributeTest
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

        private void HUHU()
        {
            var ff = Task.FromResult(22);
        }

        [Cache]
        private string PerfectString(int index)
        {
            Thread.Sleep(4000);

            return index.ToString();
        }

        [Cache]
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