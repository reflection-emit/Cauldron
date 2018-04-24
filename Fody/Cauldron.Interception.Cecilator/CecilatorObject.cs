using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public abstract class CecilatorObject : ICecilatorObject
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logError;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string, SequencePoint> logErrorPoint;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logInfo;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string> logWarning;

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Action<string, SequencePoint> logWarningPoint;

        public void Log(LogTypes logTypes, Instruction instruction, MethodDefinition methodDefinition, object arg)
        {
            var next = instruction;
            while (next != null)
            {
                var result = methodDefinition.DebugInformation.GetSequencePoint(next);
                if (result != null)
                {
                    this.Log(logTypes, result, arg);
                    return;
                }

                next = next.Next;
            }

            var previous = instruction;
            while (previous != null)
            {
                var result = methodDefinition.DebugInformation.GetSequencePoint(previous);
                if (result != null)
                {
                    this.Log(logTypes, result, arg);
                    return;
                }

                previous = previous.Previous;
            }

            this.Log(logTypes, methodDefinition, arg);
        }

        public void Log(LogTypes logTypes, MethodDefinition method, object arg) => Log(logTypes, method.GetSequencePoint(), arg);

        public void Log(LogTypes logTypes, SequencePoint sequencePoint, object arg)
        {
            switch (logTypes)
            {
                case LogTypes.Error:
                    if (sequencePoint == null)
                        this.logError(arg as string ?? arg?.ToString() ?? "");
                    else
                        this.logErrorPoint(arg as string ?? arg?.ToString() ?? "", sequencePoint);

                    break;

                case LogTypes.Warning:
                    if (sequencePoint == null)
                        this.logWarning(arg as string ?? arg?.ToString() ?? "");
                    else
                        this.logWarningPoint(arg as string ?? arg?.ToString() ?? "", sequencePoint);

                    break;

                case LogTypes.Info:
                    this.logInfo(arg as string ?? arg?.ToString() ?? "");
                    break;
            }
        }

        protected void Initialize(CecilatorBase cecilatorBase)
        {
            this.logError = cecilatorBase.logError;
            this.logErrorPoint = cecilatorBase.logErrorPoint;
            this.logInfo = cecilatorBase.logInfo;
            this.logWarning = cecilatorBase.logWarning;
            this.logWarningPoint = cecilatorBase.logWarningPoint;
        }

        protected void Initialize(
            Action<string> logInfo,
            Action<string> logWarning,
            Action<string, SequencePoint> logWarningPoint,
            Action<string> logError,
            Action<string, SequencePoint> logErrorPoint)
        {
            this.logError = logError;
            this.logErrorPoint = logErrorPoint;
            this.logInfo = logInfo;
            this.logWarning = logWarning;
            this.logWarningPoint = logWarningPoint;
        }

        protected void Log(object arg) => this.logInfo(arg as string ?? arg?.ToString() ?? "");

        protected void Log(Exception e) => this.logError(e.GetStackTrace());

        protected void Log(Exception e, string message) => this.logError(e.GetStackTrace() + "\r\n" + message);
    }
}