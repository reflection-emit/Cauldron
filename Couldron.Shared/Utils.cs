using Couldron.Core;
using System;
using System.Security;
using System.Text.RegularExpressions;

namespace Couldron
{
    /// <summary>
    /// Provides a collection of utility methods
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object, object)"/>.
        /// Then it checks all primitiv types with the == operator and as last resort uses <see cref="object.Equals(object, object)"/> to determine equality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public new static bool Equals(object a, object b)
        {
            if (a == null && b == null)
                return true;

            if (a == null) return false;

            if (object.ReferenceEquals(a, b))
                return true;

            var aType = a.GetType();

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
            if (aType == typeof(Guid)) return (Guid)a == (Guid)b;

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
    }
}