using System;
using System.Collections.Generic;

namespace Cauldron.Activator
{
    /*
         Based on the implementation of Adam Horvath (CustomDictionary)
         http://blog.teamleadnet.com/2012/07/ultra-fast-hashtable-dictionary-with.html
    */

    internal sealed class FactoryStringDictionary<TValue>
        where TValue : class
    {
        private const int initialsize = 89;

        private int[] buckets;
        private FactoryDictionaryEntry[] entries;
        private int nextfree;

        public FactoryStringDictionary() => Initialize();

        public int Count => nextfree;

        public TValue this[string key]
        {
            get
            {
                uint pos = (uint)key.GetHashCode() % (uint)buckets.Length;
                int entryLocation = buckets[pos];

                if (entryLocation == -1)
                    return null;

                int nextpos = entryLocation;

                do
                {
                    var entry = entries[nextpos];

                    if (key == entry.key)
                        return entries[nextpos].value;

                    nextpos = entry.next;
                }
                while (nextpos != -1);

                return null;
            }
        }

        public void Add(string key, TValue value)
        {
            if (nextfree >= entries.Length)
                Resize();

            uint hash = (uint)key.GetHashCode();
            uint hashPos = hash % (uint)buckets.Length;
            int entryLocation = buckets[hashPos];
            int storePos = nextfree;

            if (entryLocation != -1)
            {
                int currEntryPos = entryLocation;

                do
                {
                    var entry = entries[currEntryPos];

                    if (key == entry.key)
                        return;

                    currEntryPos = entry.next;
                }
                while (currEntryPos > -1);

                nextfree++;
            }
            else
                nextfree++;

            buckets[hashPos] = storePos;
            entries[storePos] = new FactoryDictionaryEntry
            {
                next = entryLocation,
                key = key,
                value = value,
                hashcode = hash
            };
        }

        public void Clear() => Initialize();

        public bool ContainsKey(string key)
        {
            uint hash = (uint)key.GetHashCode();
            uint pos = hash % (uint)buckets.Length;
            int entryLocation = buckets[pos];

            if (entryLocation == -1)
                return false;

            int nextpos = entryLocation;

            do
            {
                var entry = entries[nextpos];

                if (key == entry.key)
                    return true;

                nextpos = entry.next;
            } while (nextpos != -1);

            return false;
        }

        public IEnumerable<TValue> GetValues()
        {
            for (int i = 0; i < entries.Length; i++)
                if (entries[i] != null)
                    yield return entries[i].value;
        }

        public bool Remove(string key)
        {
            uint hash = (uint)key.GetHashCode();
            uint pos = hash % (uint)buckets.Length;
            int entryLocation = buckets[pos];

            if (entryLocation == -1)
                return false;

            int nextpos = entryLocation;

            do
            {
                var entry = entries[nextpos];

                if (key == entry.key)
                {
                    nextfree--;
                    buckets[pos] = -1;
                    entries[nextpos] = null;
                    return true;
                }
                nextpos = entry.next;
            }
            while (nextpos != -1);

            return false;
        }

        private uint FindNewSize()
        {
            uint roughsize = (uint)buckets.Length * 2 + 1;

            for (int i = 0; i < FactoryDictionary.primeSizes.Length; i++)
                if (FactoryDictionary.primeSizes[i] >= roughsize)
                    return FactoryDictionary.primeSizes[i];

            throw new NotImplementedException("Too large array");
        }

        private void Initialize()
        {
            this.buckets = new int[initialsize];
            this.entries = new FactoryDictionaryEntry[initialsize];
            nextfree = 0;

            for (int i = 0; i < entries.Length; i++)
                buckets[i] = -1;
        }

        private void Resize()
        {
            var newsize = FindNewSize();
            var newhashes = new int[newsize];
            var newentries = new FactoryDictionaryEntry[newsize];

            Array.Copy(entries, newentries, nextfree);

            for (int i = 0; i < newsize; i++)
                newhashes[i] = -1;

            for (int i = 0; i < nextfree; i++)
            {
                uint pos = newentries[i].hashcode % newsize;
                int prevpos = newhashes[pos];
                newhashes[pos] = i;

                if (prevpos != -1)
                    newentries[i].next = prevpos;
            }

            buckets = newhashes;
            entries = newentries;
        }

        private class FactoryDictionaryEntry
        {
            public uint hashcode;
            public string key;
            public int next;
            public TValue value;
        }
    }
}