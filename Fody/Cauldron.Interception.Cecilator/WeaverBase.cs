using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Cauldron.Interception.Cecilator
{
    public abstract class WeaverBase : BaseModuleWeaver, ICecilatorObject
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static IEnumerable<TypeDefinition> AllTypes { get; internal set; }

        public Builder Builder { get; private set; }

        public string ProjectName => this.ProjectDirectoryPath
            .With(x => x.Substring(x.LastIndexOf('\\', this.ProjectDirectoryPath.Length - 2) + 1))
            .Replace("\\", "");

        public override void AfterWeaving()
        {
            AllTypes = null;

            this.Builder = null;
            this.LogErrorPoint = null;
            this.LogError = null;
            this.LogInfo = null;
            this.LogWarningPoint = null;
            this.LogWarning = null;
            this.ModuleDefinition.Dispose();
            this.ModuleDefinition = null;
        }

        public override void Execute()
        {
            if (bool.TryParse(this.Config.Attribute("Verbose")?.Value?.ToString() ?? "true", out bool result))
                IsVerbose = result;
            else
                IsVerbose = true;

            this.Initialize(this.LogInfo, this.LogWarning, this.LogWarningPoint, this.LogError, this.LogErrorPoint);

            this.Builder = this.CreateBuilder();
            this.OnExecute();
        }

        /// <summary>
        /// Returns all assemblies that is referenced by the defined assembly <paramref name="assemblyDefinition"/> and its reference assemblies recursively.
        /// </summary>
        /// <returns>A collection of <see cref="AssemblyDefinition"/>.</returns>
        public IEnumerable<AssemblyDefinition> GetAllReferencedAssemblies(AssemblyDefinition assemblyDefinition)
        {
            var result = new Collection<AssemblyDefinition>();

            void getAssemblyDefinition(IEnumerable<AssemblyNameReference> assemblyNameReferences)
            {
                if (assemblyNameReferences == null)
                    return;

                foreach (var assemblyNameReference in assemblyNameReferences)
                    addAssemblyDefinition(assemblyNameReference);
            }

            void addAssemblyDefinition(AssemblyNameReference assemblyNameReference)
            {
                var resolvedAssembly = this.Resolve(assemblyNameReference);
                if (resolvedAssembly != null && !result.Contains(resolvedAssembly, new AssemblyDefinitionEqualityComparer()))
                {
                    result.Add(resolvedAssembly);

                    if (resolvedAssembly.MainModule != null)
                        getAssemblyDefinition(resolvedAssembly.MainModule.AssemblyReferences);
                }
            }

            if (assemblyDefinition != null && !result.Contains(assemblyDefinition, new AssemblyDefinitionEqualityComparer()))
            {
                result.Add(assemblyDefinition);

                if (assemblyDefinition.MainModule != null)
                    getAssemblyDefinition(assemblyDefinition.MainModule.AssemblyReferences);
            }

            return result;
        }

        /// <summary>
        /// Returns all assemblies that is referenced by the defined assemblies <paramref name="assemblyDefinitions"/> and its reference assemblies recursively.
        /// </summary>
        /// <returns>A collection of <see cref="AssemblyDefinition"/>.</returns>
        public IEnumerable<AssemblyDefinition> GetAllReferencedAssemblies(IEnumerable<AssemblyDefinition> assemblyDefinitions)
        {
            foreach (var item in assemblyDefinitions)
                foreach (var result in this.GetAllReferencedAssemblies(item))
                    yield return result;
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield break;
        }

        /// <summary>
        /// Loads the assembly using the Mono.Cecil default resolver.
        /// </summary>
        /// <param name="assemblyNameReferences">The assembly names of the assemblies to be resolved</param>
        /// <returns>The <see cref="AssemblyDefinition"/> of the assembly.</returns>
        public IEnumerable<AssemblyDefinition> Resolve(IEnumerable<AssemblyNameReference> assemblyNameReferences)
        {
            foreach (var item in assemblyNameReferences)
            {
                var result = this.Resolve(item);
                if (result != null)
                    yield return result;
            }
        }

        /// <summary>
        /// Loads the assembly using the Mono.Cecil default resolver.
        /// </summary>
        /// <param name="assemblyNameReference">The assembly name of the assembly to be resolved</param>
        /// <returns>The <see cref="AssemblyDefinition"/> of the assembly.</returns>
        public AssemblyDefinition Resolve(AssemblyNameReference assemblyNameReference)
        {
            try
            {
                return this.ModuleDefinition.AssemblyResolver.Resolve(assemblyNameReference);
            }
            catch
            {
                return null;
            }
        }

        protected abstract void OnExecute();

        #region Implementation from CecilatorObject due to breaking changes in FOdy 3.0.0

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

        public static bool IsVerbose { get; private set; }

        public void Log(LogTypes logTypes, Instruction instruction, MethodDefinition methodDefinition, object arg)
        {
            if (!IsVerbose && logTypes != LogTypes.Error)
                return;

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

        public void Log(LogTypes logTypes, MethodDefinition method, object arg) => this.Log(logTypes, method.GetSequencePoint(), arg);

        public void Log(LogTypes logTypes, SequencePoint sequencePoint, object arg)
        {
            if (!IsVerbose && logTypes != LogTypes.Error)
                return;

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

        #endregion Implementation from CecilatorObject due to breaking changes in FOdy 3.0.0
    }
}