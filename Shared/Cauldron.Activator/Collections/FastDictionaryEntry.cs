namespace Cauldron.Collections
{
    internal sealed class FastDictionaryEntry<TKey, TValue>
    {
        public uint hashcode;
        public TKey key;
        public int next;
        public TValue value;
    }
}