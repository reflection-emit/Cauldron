using Mono.Cecil.Cil;

namespace Cauldron.Interception.Cecilator
{
    /// <summary>
    /// Holds all position required for adding IL code to the async state machine.
    /// </summary>
    public sealed class AsyncStateMachinePositions
    {
        internal readonly Position catchBegin;
        internal readonly Position catchEnd;
        internal readonly ExceptionHandler exceptionHandler;
        internal readonly Position lastResult;
        internal readonly Position tryBegin;
        internal readonly Position tryEnd;

        internal AsyncStateMachinePositions(Method method)
        {
            var exceptionBlock = method.AsyncMethodHelper.GetAsyncStateMachineExceptionBlock();
            var tryBlock = method.AsyncMethodHelper.GetAsyncStateMachineTryBlock();

            this.lastResult = method.AsyncMethodHelper.GetAsyncStateMachineLastGetResult();
            this.exceptionHandler = exceptionBlock.Item2;
            this.catchEnd = exceptionBlock.Item1.End;
            this.catchBegin = exceptionBlock.Item1.Beginning;
            this.tryEnd = tryBlock.End;
            this.tryBegin = tryBlock.Beginning;
        }
    }
}