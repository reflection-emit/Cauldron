using Cauldron.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cauldron.Core
{
    public sealed class KeyRawValueDictionary : Dictionary<string, RawValue>
    {
        public void Add(string key, int value) => this.Add(key, new RawValue(value.ToBytes()));

        public void Add(string key, double value) => this.Add(key, new RawValue(value.ToBytes()));

        public void Add(string key, float value) => this.Add(key, new RawValue(value.ToBytes()));

        public void Add(string key, string value) => this.Add(key, new RawValue(Encoding.UTF8.GetBytes(value)));

        public void Deserialize(byte[] bytes)
        {
            var currentPosition = 0;

            do
            {
                // Key length
                var keyLength = bytes.GetBytes(currentPosition, 4).ToInteger();
                currentPosition += 4;
                var key = Encoding.UTF8.GetString(bytes.GetBytes(currentPosition, keyLength));
                currentPosition += keyLength;

                var valueLength = bytes.GetBytes(currentPosition, 4).ToInteger();
                currentPosition += 4;

                this.Add(key, new RawValue(bytes.GetBytes(currentPosition, valueLength)));
                currentPosition += valueLength;
            } while (currentPosition + 4 < bytes.Length);
        }

        /// <summary>
        /// Serializes the dictionary to a byte array.
        /// </summary>
        /// <returns>A byte array</returns>
        public byte[] Serialize()
        {
            var result = new List<byte[]>();

            foreach (var item in this)
            {
                // The length of the key
                result.Add(item.Key.Length.ToBytes());
                // The key itself
                result.Add(Encoding.UTF8.GetBytes(item.Key));

                var value = item.Value.Raw;
                result.Add(value.Length.ToBytes());
                result.Add(value);
            }

            return result.ToArray().Concat();
        }
    }

    public sealed class RawValue : IEqualityComparer<RawValue>
    {
        private byte[] raw;

        internal RawValue(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            this.raw = value;
        }

        internal byte[] Raw { get { return this.raw; } }

        public bool Equals(RawValue x, RawValue y)
        {
            if (x == null && y == null)
                return true;

            if (x != null && y == null)
                return false;

            if (x == null && y != null)
                return false;

            return x.raw.SequenceEqual(y.raw);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is RawValue)
                return this.Equals(this, obj as RawValue);

            if (obj is byte[])
                return this.raw.SequenceEqual(obj as byte[]);

            return false;
        }

        public int GetHashCode(RawValue obj) => obj.raw.GetHashCode();

        public override int GetHashCode() => this.raw.GetHashCode();

        public double ToDouble() => this.raw.ToDouble();

        public float ToFloat() => this.raw.ToFloat();

        public int ToInteger() => this.raw.ToInteger();

        public override string ToString() => Encoding.UTF8.GetString(this.raw);
    }
}