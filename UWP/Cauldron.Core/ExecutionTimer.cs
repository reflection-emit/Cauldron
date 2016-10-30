using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cauldron.Core
{
    /// <summary>
    /// Provides a simple performace measurement of a code block
    /// <code>
    /// using(var perf = new ExecutionTimer())
    /// {
    /// }
    /// </code>
    /// </summary>
    public sealed class ExecutionTimer : DisposableBase
    {
        private string memberName;
        private Stopwatch startTime;

        /// <summary>
        /// Initializes a new instance of <see cref="ExecutionTimer"/>
        /// </summary>
        /// <param name="memberName">The name of the calling method</param>
        public ExecutionTimer([CallerMemberName] string memberName = "")
        {
            this.memberName = memberName;
            this.startTime = Stopwatch.StartNew();
            Output.WriteLineInfo("Start execution of '{0}'", this.memberName);
        }

        /// <summary>
        /// Occures after <see cref="IDisposable.Dispose"/> has been invoked
        /// </summary>
        /// <param name="disposeManaged">true if managed resources requires disposing</param>
        protected override void OnDispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                this.startTime.Stop();
                Output.WriteLineInfo("Execution of '{0}': {1}ms", this.memberName, this.startTime.Elapsed.TotalMilliseconds);
            }
        }
    }
}