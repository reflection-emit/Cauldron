using PostSharp.Aspects;
using System;
using System.Diagnostics;

namespace Couldron.Aspects
{
    /// <summary>
    /// Used for internal performance debugging only
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    [Serializable]
    internal sealed class ExecutionSpeedDebugAttribute : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var startTime = DateTime.Now;
            base.OnInvoke(args);
            Debug.WriteLine("Execution of " + args.Method.Name + ": " + DateTime.Now.Subtract(startTime).TotalMilliseconds.ToString() + "ms");
        }
    }
}