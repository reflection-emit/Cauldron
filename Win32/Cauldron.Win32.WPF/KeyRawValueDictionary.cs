using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Cauldron.XAML
{
    /// <summary>
    /// Represents a collection of key and <see cref="RawValue"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class KeyRawValueDictionary : Dictionary<string, RawValue>
    {
        /// <summary>
        /// Adds the specified key and value to the dictionary
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, int value) => this.Add(key, new RawValue(value.ToBytes()));

        /// <summary>
        /// Adds the specified key and value to the dictionary
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, long value) => this.Add(key, new RawValue(value.ToBytes()));

        /// <summary>
        /// Adds the specified key and value to the dictionary
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, double value) => this.Add(key, new RawValue(value.ToBytes()));

        /// <summary>
        /// Adds the specified key and value to the dictionary
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, float value) => this.Add(key, new RawValue(value.ToBytes()));

        /// <summary>
        /// Adds the specified key and value to the dictionary
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, string value) => this.Add(key, new RawValue(Encoding.UTF8.GetBytes(value)));

        /// <summary>
        /// Adds the specified key and value to the dictionary
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(string key, bool value) => this.Add(key, new RawValue(value ? new byte[] { 1 } : new byte[] { 0 }));

        /// <summary>
        /// Deserialize the serialized <see cref="RawValue"/> and adds these value to the existing items.
        /// </summary>
        /// <param name="bytes">The bytes representation of the <see cref="RawValue"/>s</param>
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

            return result.ToArray().Flatten();
        }
    }

    /// <summary>
    /// Represents the raw values that can be freely converted to int, long, double, float, bool or string
    /// </summary>
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

        /// <exclude/>
        public static implicit operator RawValue(int value) => new RawValue(value.ToBytes());

        /// <exclude/>
        public static implicit operator RawValue(long value) => new RawValue(value.ToBytes());

        /// <exclude/>
        public static implicit operator RawValue(string value) => new RawValue(Encoding.UTF8.GetBytes(value));

        /// <exclude/>
        public static implicit operator RawValue(double value) => new RawValue(value.ToBytes());

        /// <exclude/>
        public static implicit operator RawValue(float value) => new RawValue(value.ToBytes());

        /// <exclude/>
        public static implicit operator RawValue(bool value) => new RawValue(value ? new byte[] { 1 } : new byte[] { 0 });

        /// <summary>
        /// Determines whether the specified object are equal
        /// </summary>
        /// <param name="x">The first object of type <see cref="RawValue"/> to compare</param>
        /// <param name="y">The second object of type <see cref="RawValue"/> to compare</param>
        /// <returns>True if the specified objects are equal; otherwise, false</returns>
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

        /// <summary>
        /// Determines whether the specified object are equal
        /// </summary>
        /// <param name="obj">The object of type <see cref="RawValue"/> or byte[] to compare</param>
        /// <returns>True if the specified objects are equal; otherwise, false</returns>
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

        /// <summary>
        /// Returns a hash code for the specified object
        /// </summary>
        /// <param name="obj">The <see cref="RawValue"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object</returns>
        public int GetHashCode(RawValue obj) => obj.raw.GetHashCode();

        /// <summary>
        /// Returns a hash code for this object
        /// </summary>
        public override int GetHashCode() => this.raw.GetHashCode();

        /// <summary>
        /// Converts the raw bytes value to bool
        /// </summary>
        /// <returns></returns>
        public bool ToBool() => this.raw[0] == 1;

        /// <summary>
        /// Converts the raw bytes value to double
        /// </summary>
        /// <returns>The converted value</returns>
        public double ToDouble() => this.raw.ToDouble();

        /// <summary>
        /// Converts the raw bytes value to float
        /// </summary>
        /// <returns>The converted value</returns>
        public float ToFloat() => this.raw.ToFloat();

        /// <summary>
        /// Converts the raw bytes value to integer
        /// </summary>
        /// <returns>The converted value</returns>
        public int ToInteger() => this.raw.ToInteger();

        /// <summary>
        /// Converts the raw bytes value to long
        /// </summary>
        /// <returns>The converted value</returns>
        public long ToLong() => this.raw.ToLong();

        /// <summary>
        /// Converts the raw bytes value to string
        /// </summary>
        /// <returns>The converted value</returns>
        public override string ToString() => Encoding.UTF8.GetString(this.raw);
    }
}