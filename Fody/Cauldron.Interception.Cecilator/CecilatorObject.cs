using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public abstract class CecilatorObject
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

        protected void Log(LogTypes logTypes, Instruction instruction, MethodDefinition methodDefinition, object arg)
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

        protected void Log(LogTypes logTypes, Property property, object arg) => Log(logTypes, property.Getter ?? property.Setter, arg);

        protected void Log(LogTypes logTypes, Method method, object arg) => Log(logTypes, GetSequencePoint(method.methodDefinition), arg);

        protected void Log(LogTypes logTypes, MethodDefinition method, object arg) => Log(logTypes, GetSequencePoint(method), arg);

        protected void Log(LogTypes logTypes, BuilderType type, object arg) => Log(logTypes, type.GetRelevantConstructors().FirstOrDefault() ?? type.Methods.FirstOrDefault(), arg);

        protected void Log(LogTypes logTypes, SequencePoint sequencePoint, object arg)
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

        protected void Log(object arg) => this.logInfo(arg as string ?? arg?.ToString() ?? "");

        protected void Log(Exception e) => this.logError(e.GetStackTrace());

        protected void Log(Exception e, string message) => this.logError(e.GetStackTrace() + "\r\n" + message);

        private SequencePoint GetSequencePoint(MethodDefinition methodDefinition)
        {
            if (methodDefinition == null || methodDefinition.Body == null || methodDefinition.Body.Instructions == null)
                return null;

            foreach (var instruction in methodDefinition.Body.Instructions)
            {
                var result = methodDefinition.DebugInformation.GetSequencePoint(instruction);
                if (result == null)
                    continue;

                return result;
            }

            return null;
        }
    }
}