using System;
using System.Collections.Generic;

namespace Cauldron.Interception.Cecilator
{
    internal sealed class ListEx<T> : List<T>
    {
        public event EventHandler Changed;

        public new void Add(T item)
        {
            base.Add(item);
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        public new void Clear()
        {
            base.Clear();
            this.Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}