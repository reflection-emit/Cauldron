using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public class LocalVariableCollection : IEnumerable<LocalVariable>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private InnerLocalVariableCollection innerCollection = new InnerLocalVariableCollection();

        internal LocalVariableCollection(BuilderType type, Mono.Collections.Generic.Collection<VariableDefinition> localVariables)
        {
            if (localVariables == null || localVariables.Count == 0)
                return;

            for (int i = 0; i < localVariables.Count; i++)
            {
                if (string.IsNullOrEmpty(localVariables[i].Name) || this.innerCollection.Contains(localVariables[i].Name))
                    localVariables[i].Name = "<>var_" + CecilatorBase.GenerateName();

                this.innerCollection.Add(new LocalVariable(type, localVariables[i]));
            }
        }

        public LocalVariable this[int index] { get { return this.innerCollection[index]; } }

        public LocalVariable this[string key] { get { return this.innerCollection[key]; } }

        public bool Contains(string key) => this.innerCollection.Contains(key);

        public IEnumerator<LocalVariable> GetEnumerator() => this.innerCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.innerCollection.GetEnumerator();

        internal void Add(LocalVariable localVariable) => this.innerCollection.Add(localVariable);

        private sealed class InnerLocalVariableCollection : KeyedCollection<string, LocalVariable>
        {
            protected override string GetKeyForItem(LocalVariable item) => item.Name;
        }
    }
}