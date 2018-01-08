using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public abstract class WeaverBase : CecilatorObject
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static IEnumerable<TypeDefinition> AllTypes { get; internal set; }

        public string AssemblyFilePath { get; set; }
        public Builder Builder { get; private set; }
        public List<string> DefineConstants { get; set; }
        public Action<string> LogError { get; set; }
        public Action<string, SequencePoint> LogErrorPoint { get; set; }
        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        public Action<string, SequencePoint> LogWarningPoint { get; set; }
        public ModuleDefinition ModuleDefinition { get; set; }
        public string ProjectDirectoryPath { get; set; }
        public List<string> ReferenceCopyLocalPaths { get; set; }
        public string SolutionDirectoryPath { get; set; }

        public void AfterWeaving()
        {
            AllTypes = null;

            this.Builder = null;
            this.LogErrorPoint = null;
            this.LogError = null;
            this.LogInfo = null;
            this.LogWarningPoint = null;
            this.LogWarning = null;
            this.ReferenceCopyLocalPaths.Clear();
            this.ModuleDefinition.Dispose();
            this.ModuleDefinition = null;

            Method.variableDictionary.Clear();
            this.OnAfterWeaving();
        }

        public void Cancel()
        {
            Method.variableDictionary.Clear();
            this.OnCancel();
        }

        public void Execute()
        {
            this.Initialize(this.LogInfo, this.LogWarning, this.LogWarningPoint, this.LogError, this.LogErrorPoint);

            try
            {
                this.Builder = this.CreateBuilder();
                this.OnExecute();
            }
            catch (Exception e)
            {
                this.LogError(e.GetStackTrace());
                throw;
            }
        }

        public virtual void OnAfterWeaving()
        {
        }

        public virtual void OnCancel()
        {
        }

        public virtual void OnExecute()
        {
        }
    }
}