using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Cauldron.Interception.Cecilator
{
    public sealed class FieldCollection : IEnumerable<Field>
    {
        [EditorBrowsable(EditorBrowsableState.Never), DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private InnerFieldCollection innerCollection = new InnerFieldCollection();

        internal FieldCollection(BuilderType type, Mono.Collections.Generic.Collection<FieldDefinition> fields)
        {
            if (fields == null || fields.Count == 0)
                return;

            for (int i = 0; i < fields.Count; i++)
                this.innerCollection.Add(new Field(type, fields[i]));
        }

        public Field this[int index] { get { return this.innerCollection[index]; } }

        public Field this[string key] { get { return this.innerCollection[key]; } }

        public bool Contains(string key) => this.innerCollection.Contains(key);

        public IEnumerator<Field> GetEnumerator() => this.innerCollection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.innerCollection.GetEnumerator();

        internal void Add(Field field) => this.innerCollection.Add(field);

        private sealed class InnerFieldCollection : KeyedCollection<string, Field>
        {
            protected override string GetKeyForItem(Field item) => item.Name;
        }
    }
}