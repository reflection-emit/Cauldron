using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public abstract class WeaverBase
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static IEnumerable<TypeDefinition> AllTypes { get; internal set; }

        public Builder Builder { get; private set; }
        public Action<string> LogError { get; set; }
        public Action<string> LogInfo { get; set; }
        public Action<string> LogWarning { get; set; }
        public ModuleDefinition ModuleDefinition { get; set; }
        public List<string> ReferenceCopyLocalPaths { get; set; }

        public void AfterWeaving()
        {
            AllTypes = null;

            this.Builder = null;
            this.LogError = null;
            this.LogInfo = null;
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
            this.Builder = this.CreateBuilder();
            this.OnExecute();
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