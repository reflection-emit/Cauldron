using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Collections
{
    internal sealed class FastDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator<TValue>
    {
        private int currentIndex = -1;
        private FastDictionaryEntry<TKey, TValue>[] entries;

        internal FastDictionaryEnumerator(FastDictionaryEntry<TKey, TValue>[] entries) =>
            this.entries = entries.Where(x => x != null).ToArray();

        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                var result = this.entries[this.currentIndex];
                return new KeyValuePair<TKey, TValue>(result.key, result.value);
            }
        }

        TValue IEnumerator<TValue>.Current
        {
            get
            {
                var result = this.entries[this.currentIndex];
                return result.value;
            }
        }

        object IEnumerator.Current => entries[this.currentIndex];

        public void Dispose() => this.entries = null;

        public IEnumerable<KeyValuePair<TKey, TValue>> GetItems()
        {
            foreach (var item in this.entries)
                yield return new KeyValuePair<TKey, TValue>(item.key, item.value);
        }

        public bool MoveNext()
        {
            this.currentIndex++;
            return this.currentIndex < this.entries.Length;
        }

        public void Reset() => this.currentIndex = -1;
    }
}