using System;
using System.Reflection;

namespace Couldron.Core
{
    /// <summary>
    /// Provides static methods for common mathematical functions.
    /// </summary>
    public static class Mathc
    {
        /// <summary>
        /// Adds <paramref name="a"/> to <paramref name="b"/>
        /// <para/>
        /// If <paramref name="a"/> and <paramref name="b"/> are null then null is returned.
        /// If <paramref name="a"/> is null then <paramref name="b"/> is returned.
        /// If <paramref name="b"/> is null then <paramref name="a"/> is returned.
        /// <para/>
        /// Tries to cast primitiv <see cref="Type"/> and use the + operator.
        /// If the <see cref="Type"/> is unknown then reflection is used to determin the operator.
        /// </summary>
        /// <param name="a">The first summand</param>
        /// <param name="b">The second summand</param>
        /// <returns>Returns the sum of the addition</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static object Add(object a, object b)
        {
            if (a == null && b == null)
                return null;

            if (a == null) return b;
            if (b == null) return a;

            var aType = a.GetType();

            if (aType == typeof(int)) return (int)a + (int)b;
            if (aType == typeof(uint)) return (uint)a + (uint)b;
            if (aType == typeof(long)) return (long)a + (long)b;
            if (aType == typeof(ulong)) return (ulong)a + (ulong)b;
            if (aType == typeof(byte)) return (byte)a + (byte)b;
            if (aType == typeof(sbyte)) return (sbyte)a + (sbyte)b;
            if (aType == typeof(float)) return (float)a + (float)b;
            if (aType == typeof(double)) return (double)a + (double)b;
            if (aType == typeof(decimal)) return (decimal)a + (decimal)b;
            if (aType == typeof(char)) return (char)a + (char)b;
            if (aType == typeof(short)) return (short)a + (short)b;
            if (aType == typeof(ushort)) return (ushort)a + (ushort)b;
            if (aType == typeof(TimeSpan)) return (TimeSpan)a + (TimeSpan)b;

            var op = aType.GetMethod("op_Addition", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return op.Invoke(null, new object[] { a, b });

            op = aType.GetMethod("op_Addition", new Type[] { b.GetType(), aType }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return op.Invoke(null, new object[] { b, a });

            throw new ArgumentException("The + operator cannot be applied to: " + aType.FullName);
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value the parameter <paramref name="value"/> can have</param>
        /// <param name="max">The maximum value the parameter <paramref name="value"/> can have</param>
        /// <returns>The clamped value</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (value > max)
                value = max;

            if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Clamps a value between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="min">The minimum value the parameter <paramref name="value"/> can have</param>
        /// <param name="max">The maximum value the parameter <paramref name="value"/> can have</param>
        /// <returns>The clamped value</returns>
        public static int Clamp(int value, int min, int max)
        {
            if (value > max)
                value = max;

            if (value < min)
                value = min;

            return value;
        }

        /// <summary>
        /// Divides <paramref name="a"/> with <paramref name="b"/>
        /// <para/>
        /// If <paramref name="a"/> and <paramref name="b"/> are null then null is returned.
        /// If <paramref name="a"/> is null then 0 is returned.
        /// If <paramref name="b"/> is null then <paramref name="a"/> is returned;
        /// <para/>
        /// Tries to cast primitiv <see cref="Type"/> and use the / operator.
        /// If the <see cref="Type"/> is unknown then reflection is used to determin the operator.
        /// </summary>
        /// <param name="a">The dividend</param>
        /// <param name="b">The divisor</param>
        /// <returns>Returns the quotient of the division</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static object Divide(object a, object b)
        {
            if (a == null && b == null)
                return null;

            if (a == null) return 0;
            if (b == null) return a;

            var aType = a.GetType();

            if (aType == typeof(int)) return (int)a / (int)b;
            if (aType == typeof(uint)) return (uint)a / (uint)b;
            if (aType == typeof(long)) return (long)a / (long)b;
            if (aType == typeof(ulong)) return (ulong)a / (ulong)b;
            if (aType == typeof(byte)) return (byte)a / (byte)b;
            if (aType == typeof(sbyte)) return (sbyte)a / (sbyte)b;
            if (aType == typeof(float)) return (float)a / (float)b;
            if (aType == typeof(double)) return (double)a / (double)b;
            if (aType == typeof(decimal)) return (decimal)a / (decimal)b;
            if (aType == typeof(char)) return (char)a / (char)b;
            if (aType == typeof(short)) return (short)a / (short)b;
            if (aType == typeof(ushort)) return (ushort)a / (ushort)b;

            var op = aType.GetMethod("op_Division", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return op.Invoke(null, new object[] { a, b });

            throw new ArgumentException("The / operator cannot be applied to: " + aType.FullName);
        }

        /// <summary>
        /// Multiplies <paramref name="a"/> with <paramref name="b"/>
        /// <para/>
        /// If <paramref name="a"/> and <paramref name="b"/> are null then null is returned.
        /// If <paramref name="a"/> is null then <paramref name="b"/> is returned.
        /// If <paramref name="b"/> is null then <paramref name="a"/> is returned.
        /// <para/>
        /// Tries to cast primitiv <see cref="Type"/> and use the * operator.
        /// If the <see cref="Type"/> is unknown then reflection is used to determin the operator.
        /// </summary>
        /// <param name="a">The multiplier</param>
        /// <param name="b">The multiplicand</param>
        /// <returns>Returns the product of the multiplication</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static object Multiply(object a, object b)
        {
            if (a == null && b == null)
                return null;

            if (a == null) return b;
            if (b == null) return a;

            var aType = a.GetType();

            if (aType == typeof(int)) return (int)a * (int)b;
            if (aType == typeof(uint)) return (uint)a * (uint)b;
            if (aType == typeof(long)) return (long)a * (long)b;
            if (aType == typeof(ulong)) return (ulong)a * (ulong)b;
            if (aType == typeof(byte)) return (byte)a * (byte)b;
            if (aType == typeof(sbyte)) return (sbyte)a * (sbyte)b;
            if (aType == typeof(float)) return (float)a * (float)b;
            if (aType == typeof(double)) return (double)a * (double)b;
            if (aType == typeof(decimal)) return (decimal)a * (decimal)b;
            if (aType == typeof(char)) return (char)a * (char)b;
            if (aType == typeof(short)) return (short)a * (short)b;
            if (aType == typeof(ushort)) return (ushort)a * (ushort)b;

            var op = aType.GetMethod("op_Multiply", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return op.Invoke(null, new object[] { a, b });

            op = aType.GetMethod("op_Multiply", new Type[] { b.GetType(), aType }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return op.Invoke(null, new object[] { b, a });

            throw new ArgumentException("The + operator cannot be applied to: " + aType.FullName);
        }

        /// <summary>
        /// Substracts <paramref name="b"/> from <paramref name="a"/>
        /// <para/>
        /// If <paramref name="a"/> and <paramref name="b"/> are null then null is returned.
        /// If <paramref name="a"/> is null then 0 is returned.
        /// If <paramref name="b"/> is null then <paramref name="a"/> is returned;
        /// <para/>
        /// Tries to cast primitiv <see cref="Type"/> and use the - operator.
        /// If the <see cref="Type"/> is unknown then reflection is used to determin the operator.
        /// </summary>
        /// <param name="a">The minuend</param>
        /// <param name="b">The substrahend</param>
        /// <returns>Returns the difference of the substraction</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static object Substract(object a, object b)
        {
            if (a == null && b == null)
                return null;

            if (a == null) return 0;
            if (b == null) return a;

            var aType = a.GetType();

            if (aType == typeof(int)) return (int)a - (int)b;
            if (aType == typeof(uint)) return (uint)a - (uint)b;
            if (aType == typeof(long)) return (long)a - (long)b;
            if (aType == typeof(ulong)) return (ulong)a - (ulong)b;
            if (aType == typeof(byte)) return (byte)a - (byte)b;
            if (aType == typeof(sbyte)) return (sbyte)a - (sbyte)b;
            if (aType == typeof(float)) return (float)a - (float)b;
            if (aType == typeof(double)) return (double)a - (double)b;
            if (aType == typeof(decimal)) return (decimal)a - (decimal)b;
            if (aType == typeof(char)) return (char)a - (char)b;
            if (aType == typeof(short)) return (short)a - (short)b;
            if (aType == typeof(ushort)) return (ushort)a - (ushort)b;
            if (aType == typeof(TimeSpan)) return (TimeSpan)a - (TimeSpan)b;

            var op = aType.GetMethod("op_Subtraction", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return op.Invoke(null, new object[] { a, b });

            throw new ArgumentException("The - operator cannot be applied to: " + aType.FullName);
        }
    }
}