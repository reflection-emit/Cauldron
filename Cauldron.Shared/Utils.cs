using Cauldron.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;

namespace Cauldron
{
    /// <summary>
    /// Provides a collection of utility methods
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// Determines whether <paramref name="a"/> is equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the == operator, then with reflection on 'op_Equality' and as last resort uses <see cref="object.Equals(object, object)"/> to determine equality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, false.</returns>
        public new static bool Equals(object a, object b)
        {
            if (a == null && b == null)
                return true;

            if (a == null) return false;
            if (b == null) return false;

            if (object.ReferenceEquals(a, b))
                return true;

            var aType = a.GetType();

            if (aType == b.GetType())
            {
                if (aType == typeof(string)) return a as string == b as string;
                if (aType == typeof(int)) return (int)a == (int)b;
                if (aType == typeof(uint)) return (uint)a == (uint)b;
                if (aType == typeof(long)) return (long)a == (long)b;
                if (aType == typeof(ulong)) return (ulong)a == (ulong)b;
                if (aType == typeof(byte)) return (byte)a == (byte)b;
                if (aType == typeof(sbyte)) return (sbyte)a == (sbyte)b;
                if (aType == typeof(float)) return (float)a == (float)b;
                if (aType == typeof(double)) return (double)a == (double)b;
                if (aType == typeof(decimal)) return (decimal)a == (decimal)b;
                if (aType == typeof(bool)) return (bool)a == (bool)b;
                if (aType == typeof(char)) return (char)a == (char)b;
                if (aType == typeof(short)) return (short)a == (short)b;
                if (aType == typeof(ushort)) return (ushort)a == (ushort)b;
                if (aType == typeof(IntPtr)) return (IntPtr)a == (IntPtr)b;
                if (aType == typeof(UIntPtr)) return (UIntPtr)a == (UIntPtr)b;
                if (aType == typeof(DateTime)) return (DateTime)a == (DateTime)b;
                if (aType == typeof(DateTimeOffset)) return (DateTimeOffset)a == (DateTimeOffset)b;
                if (aType == typeof(TimeSpan)) return (TimeSpan)a == (TimeSpan)b;
                if (aType == typeof(Guid)) return (Guid)a == (Guid)b;

                // If object a implements the IEquatable<> interface... Lets handle it in here
                var method = aType.GetMethod("Equals", new Type[] { aType });
                if (method != null)
                    return (bool)method.Invoke(a, new object[] { b });
            }

            var op = aType.GetMethod("op_Equality", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { a, b });

            // if this is null try to exchange the position of the parameters... Maybe its the other way around
            if (op == null)
                op = aType.GetMethod("op_Equality", new Type[] { b.GetType(), aType }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { b, a });

            return a.Equals(b);
        }

        /// <summary>
        /// Checks the password's strength
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>Returns <see cref="PasswordScore"/> rating</returns>
        [SecurityCritical]
        public static PasswordScore GetPasswordScore(string password)
        {
            // Origin: http://social.msdn.microsoft.com/Forums/vstudio/en-US/5e3f27d2-49af-410a-85a2-3c47e3f77fb1/how-to-check-for-password-strength
            if (string.IsNullOrEmpty(password))
                return PasswordScore.Blank;

            int score = 1;

            if (password.Length < 1)
                return PasswordScore.Blank;

            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 8)
                score++;

            if (password.Length >= 12)
                score++;

            // number only //"^\d+$" if you need to match more than one digit.
            if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?"))
                score++;

            // both, lower and upper case
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$"))
                score++;

            // ^[A-Z]+$
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]"))
                score++;

            return (PasswordScore)score;
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &gt; operator, then with reflection on 'op_GreaterThan'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is greater than <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Greater than operator cannot be applied</exception>
        public static bool GreaterThan(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return false;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return false;

            var aType = a.GetType();

            if (aType == b.GetType())
            {
                if (aType == typeof(int)) return (int)a > (int)b;
                if (aType == typeof(uint)) return (uint)a > (uint)b;
                if (aType == typeof(long)) return (long)a > (long)b;
                if (aType == typeof(ulong)) return (ulong)a > (ulong)b;
                if (aType == typeof(byte)) return (byte)a > (byte)b;
                if (aType == typeof(sbyte)) return (sbyte)a > (sbyte)b;
                if (aType == typeof(float)) return (float)a > (float)b;
                if (aType == typeof(double)) return (double)a > (double)b;
                if (aType == typeof(decimal)) return (decimal)a > (decimal)b;
                if (aType == typeof(char)) return (char)a > (char)b;
                if (aType == typeof(short)) return (short)a > (short)b;
                if (aType == typeof(ushort)) return (ushort)a > (ushort)b;
                if (aType == typeof(DateTime)) return (DateTime)a > (DateTime)b;
                if (aType == typeof(DateTimeOffset)) return (DateTimeOffset)a > (DateTimeOffset)b;
                if (aType == typeof(TimeSpan)) return (TimeSpan)a > (TimeSpan)b;
            }

            var op = aType.GetMethod("op_GreaterThan", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { a, b });

            throw new ArgumentException("The > operator cannot be applied to: " + aType.FullName);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than or equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &gt;= operator, then with reflection on 'op_GreaterThanOrEqual'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is greater than or equal to <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static bool GreaterThanOrEqual(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return true;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return true;

            var aType = a.GetType();

            if (aType == b.GetType())
            {
                if (aType == typeof(int)) return (int)a >= (int)b;
                if (aType == typeof(uint)) return (uint)a >= (uint)b;
                if (aType == typeof(long)) return (long)a >= (long)b;
                if (aType == typeof(ulong)) return (ulong)a >= (ulong)b;
                if (aType == typeof(byte)) return (byte)a >= (byte)b;
                if (aType == typeof(sbyte)) return (sbyte)a >= (sbyte)b;
                if (aType == typeof(float)) return (float)a >= (float)b;
                if (aType == typeof(double)) return (double)a >= (double)b;
                if (aType == typeof(decimal)) return (decimal)a >= (decimal)b;
                if (aType == typeof(char)) return (char)a >= (char)b;
                if (aType == typeof(short)) return (short)a >= (short)b;
                if (aType == typeof(ushort)) return (ushort)a >= (ushort)b;
                if (aType == typeof(DateTime)) return (DateTime)a >= (DateTime)b;
                if (aType == typeof(DateTimeOffset)) return (DateTimeOffset)a >= (DateTimeOffset)b;
                if (aType == typeof(TimeSpan)) return (TimeSpan)a >= (TimeSpan)b;
            }

            var op = aType.GetMethod("op_GreaterThanOrEqual", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { a, b });

            throw new ArgumentException("The >= operator cannot be applied to: " + aType.FullName);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is less than <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &lt; operator, then with reflection on 'op_LessThan'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is less than <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Lesser than operator cannot be applied</exception>
        public static bool LessThan(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return false;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return false;

            var aType = a.GetType();

            if (aType == b.GetType())
            {
                if (aType == typeof(int)) return (int)a < (int)b;
                if (aType == typeof(uint)) return (uint)a < (uint)b;
                if (aType == typeof(long)) return (long)a < (long)b;
                if (aType == typeof(ulong)) return (ulong)a < (ulong)b;
                if (aType == typeof(byte)) return (byte)a < (byte)b;
                if (aType == typeof(sbyte)) return (sbyte)a < (sbyte)b;
                if (aType == typeof(float)) return (float)a < (float)b;
                if (aType == typeof(double)) return (double)a < (double)b;
                if (aType == typeof(decimal)) return (decimal)a < (decimal)b;
                if (aType == typeof(char)) return (char)a < (char)b;
                if (aType == typeof(short)) return (short)a < (short)b;
                if (aType == typeof(ushort)) return (ushort)a < (ushort)b;
                if (aType == typeof(DateTime)) return (DateTime)a < (DateTime)b;
                if (aType == typeof(DateTimeOffset)) return (DateTimeOffset)a < (DateTimeOffset)b;
                if (aType == typeof(TimeSpan)) return (TimeSpan)a < (TimeSpan)b;
            }

            var op = aType.GetMethod("op_LessThan", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { a, b });

            throw new ArgumentException("The < operator cannot be applied to: " + aType.FullName);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is less than or equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the &lt;= operator, then with reflection on 'op_LessThanOrEqual'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static bool LessThanOrEqual(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return true;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return true;

            var aType = a.GetType();

            if (aType == b.GetType())
            {
                if (aType == typeof(int)) return (int)a <= (int)b;
                if (aType == typeof(uint)) return (uint)a <= (uint)b;
                if (aType == typeof(long)) return (long)a <= (long)b;
                if (aType == typeof(ulong)) return (ulong)a <= (ulong)b;
                if (aType == typeof(byte)) return (byte)a <= (byte)b;
                if (aType == typeof(sbyte)) return (sbyte)a <= (sbyte)b;
                if (aType == typeof(float)) return (float)a <= (float)b;
                if (aType == typeof(double)) return (double)a <= (double)b;
                if (aType == typeof(decimal)) return (decimal)a <= (decimal)b;
                if (aType == typeof(char)) return (char)a <= (char)b;
                if (aType == typeof(short)) return (short)a <= (short)b;
                if (aType == typeof(ushort)) return (ushort)a <= (ushort)b;
                if (aType == typeof(DateTime)) return (DateTime)a <= (DateTime)b;
                if (aType == typeof(DateTimeOffset)) return (DateTimeOffset)a <= (DateTimeOffset)b;
                if (aType == typeof(TimeSpan)) return (TimeSpan)a <= (TimeSpan)b;
            }

            var op = aType.GetMethod("op_LessThanOrEqual", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { a, b });

            throw new ArgumentException("The <= operator cannot be applied to: " + aType.FullName);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is unequal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all known types with the != operator, then with reflection on 'op_Inequality' and as last resort uses <see cref="object.Equals(object, object)"/> to determine unequality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if <paramref name="a"/> is unequal to <paramref name="b"/>; otherwise, false.</returns>
        public static bool UnEquals(object a, object b)
        {
            if (a == null && b == null)
                return false;

            if (a == null) return true;
            if (b == null) return true;

            if (object.ReferenceEquals(a, b))
                return false;

            var aType = a.GetType();

            if (aType == b.GetType())
            {
                if (aType == typeof(string)) return a as string != b as string;
                if (aType == typeof(int)) return (int)a != (int)b;
                if (aType == typeof(uint)) return (uint)a != (uint)b;
                if (aType == typeof(long)) return (long)a != (long)b;
                if (aType == typeof(ulong)) return (ulong)a != (ulong)b;
                if (aType == typeof(byte)) return (byte)a != (byte)b;
                if (aType == typeof(sbyte)) return (sbyte)a != (sbyte)b;
                if (aType == typeof(float)) return (float)a != (float)b;
                if (aType == typeof(double)) return (double)a != (double)b;
                if (aType == typeof(decimal)) return (decimal)a != (decimal)b;
                if (aType == typeof(bool)) return (bool)a != (bool)b;
                if (aType == typeof(char)) return (char)a != (char)b;
                if (aType == typeof(short)) return (short)a != (short)b;
                if (aType == typeof(ushort)) return (ushort)a != (ushort)b;
                if (aType == typeof(IntPtr)) return (IntPtr)a != (IntPtr)b;
                if (aType == typeof(UIntPtr)) return (UIntPtr)a != (UIntPtr)b;
                if (aType == typeof(DateTime)) return (DateTime)a != (DateTime)b;
                if (aType == typeof(DateTimeOffset)) return (DateTimeOffset)a != (DateTimeOffset)b;
                if (aType == typeof(TimeSpan)) return (TimeSpan)a != (TimeSpan)b;
                if (aType == typeof(Guid)) return (Guid)a != (Guid)b;

                // If object a implements the IEquatable<> interface... Lets handle it in here
                var method = aType.GetMethod("Equals", new Type[] { aType });
                if (method != null)
                    return !(bool)method.Invoke(a, new object[] { b });
            }

            var op = aType.GetMethod("op_Inequality", new Type[] { aType, b.GetType() }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { a, b });

            // if this is null try to exchange the position of the parameters... Maybe its the other way around
            if (op == null)
                op = aType.GetMethod("op_Inequality", new Type[] { b.GetType(), aType }, BindingFlags.Static | BindingFlags.Public);

            if (op != null)
                return (bool)op.Invoke(null, new object[] { b, a });

            return !a.Equals(b);
        }
    }
}