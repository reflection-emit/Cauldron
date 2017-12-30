using Cauldron.Interception.Cecilator;
using System;
using System.Diagnostics;

namespace Cauldron.Interception.Fody
{
    public sealed class StopwatchLog : IDisposable
    {
        private Stopwatch stopwatch;
        private string text;
        private WeaverBase weaver;

        public StopwatchLog(WeaverBase weaver, string text)
        {
            this.weaver = weaver;
            this.stopwatch = Stopwatch.StartNew();
            this.text = text;
        }

        public void Dispose()
        {
            stopwatch.Stop();
            weaver.Log(LogTypes.Info, $"----- Implementing {this.text} interceptors took {this.stopwatch.Elapsed.TotalMilliseconds}ms -----");
        }
    }
}