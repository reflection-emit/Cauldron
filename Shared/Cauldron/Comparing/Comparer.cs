using System;
using static Cauldron.Core.ComparingOperatorCache;
using static Cauldron.Core.IEquatableCache;

namespace Cauldron
{
    /// <summary>
    /// Provides methods for comparing objects
    /// </summary>
    public static class Comparer
    {
        /// <summary>
        /// Determines whether <paramref name="a"/> is equal to <paramref name="b"/>
        /// </summary>
        /// <typeparam name="T">The type of the objects to be compared</typeparam>
        /// <typeparam name="TValue">The values of the object used to compare them (e.g. Hash)</typeparam>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <param name="selector">The value selector which will be used for the compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, false.
        /// </returns>
        public static bool Equals<T, TValue>(T a, T b, Func<T, TValue> selector)
        {
            if (object.Equals(a, b))
                return true;

            if ((a != null && b == null) || (a == null && b != null))
                return false;

            return Comparer.Equals(selector(a), selector(b));
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object,
        /// object)"/>. Then it checks all known types with the == operator, then with reflection on
        /// 'op_Equality' and as last resort uses <see cref="object.Equals(object, object)"/> to
        /// determine equality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is equal to <paramref name="b"/>; otherwise, false.
        /// </returns>
        public new static bool Equals(object a, object b)
        {
            if (a == null && b == null)
                return true;

            if (a == null) return false;
            if (b == null) return false;

            if (object.ReferenceEquals(a, b))
                return true;

            switch (a)
            {
                case string a_ when b is string b_: /**/                 return a_ == b_;
                case int a_ when b is int b_: /**/                       return a_ == b_;
                case int a_ when b is long b_: /**/                      return a_ == b_;
                case long a_ when b is int b_: /**/                      return a_ == b_;
                case uint a_ when b is uint b_: /**/                     return a_ == b_;
                case long a_ when b is long b_: /**/                     return a_ == b_;
                case ulong a_ when b is ulong b_: /**/                   return a_ == b_;
                case byte a_ when b is byte b_: /**/                     return a_ == b_;
                case sbyte a_ when b is sbyte b_: /**/                   return a_ == b_;
                case float a_ when b is float b_: /**/                   return a_ == b_;
                case float a_ when b is double b_: /**/                  return a_ == b_;
                case double a_ when b is double b_: /**/                 return a_ == b_;
                case double a_ when b is float b_: /**/                  return a_ == b_;
                case decimal a_ when b is decimal b_: /**/               return a_ == b_;
                case char a_ when b is char b_: /**/                     return a_ == b_;
                case int a_ when b is char b_: /**/                      return a_ == b_;
                case char a_ when b is int b_: /**/                      return a_ == b_;
                case byte a_ when b is char b_: /**/                     return a_ == b_;
                case char a_ when b is byte b_: /**/                     return a_ == b_;
                case short a_ when b is short b_: /**/                   return a_ == b_;
                case ushort a_ when b is ushort b_: /**/                 return a_ == b_;
                case DateTime a_ when b is DateTime b_: /**/             return a_ == b_;
                case DateTimeOffset a_ when b is DateTimeOffset b_: /**/ return a_ == b_;
                case TimeSpan a_ when b is TimeSpan b_: /**/             return a_ == b_;
                case IntPtr a_ when b is IntPtr b_: /**/                 return a_ == b_;
                case UIntPtr a_ when b is UIntPtr b_: /**/               return a_ == b_;
                case Guid a_ when b is Guid b_: /**/                     return a_ == b_;
            }

            var aType = a.GetType();
            var bType = b.GetType();

            if (ComparingOperatorCache.Get(ComparingOperatorCache.Operator.Equal, aType, bType) is ComparerOperator op1)
                return op1(a, b);

            if (ComparingOperatorCache.Get(ComparingOperatorCache.Operator.Equal, bType, aType) is ComparerOperator op2)
                return op2(b, a);

            if (IEquatableCache.Get(aType, bType) is EqualsMethod op3)
                return op3(a, b);

            if (IEquatableCache.Get(bType, aType) is EqualsMethod op4)
                return op4(b, a);

            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object,
        /// object)"/>. Then it checks all known types with the &gt; operator, then with reflection
        /// on 'op_GreaterThan'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is greater than <paramref name="b"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">Greater than operator cannot be applied</exception>
        public static bool GreaterThan(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return false;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return false;

            switch (a)
            {
                case int a_ when b is int b_: /**/                       return a_ > b_;
                case uint a_ when b is uint b_: /**/                     return a_ > b_;
                case long a_ when b is long b_: /**/                     return a_ > b_;
                case ulong a_ when b is ulong b_: /**/                   return a_ > b_;
                case byte a_ when b is byte b_: /**/                     return a_ > b_;
                case sbyte a_ when b is sbyte b_: /**/                   return a_ > b_;
                case float a_ when b is float b_: /**/                   return a_ > b_;
                case double a_ when b is double b_: /**/                 return a_ > b_;
                case decimal a_ when b is decimal b_: /**/               return a_ > b_;
                case char a_ when b is char b_: /**/                     return a_ > b_;
                case short a_ when b is short b_: /**/                   return a_ > b_;
                case ushort a_ when b is ushort b_: /**/                 return a_ > b_;
                case DateTime a_ when b is DateTime b_: /**/             return a_ > b_;
                case DateTimeOffset a_ when b is DateTimeOffset b_: /**/ return a_ > b_;
                case TimeSpan a_ when b is TimeSpan b_: /**/             return a_ > b_;
            }

            var aType = a.GetType();
            var bType = b.GetType();

            var op = ComparingOperatorCache.Get(ComparingOperatorCache.Operator.GreaterThan, aType, bType) ??
                throw new ArgumentException("The > operator cannot be applied to: " + aType.FullName);

            return op(a, b);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than or equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object,
        /// object)"/>. Then it checks all known types with the &gt;= operator, then with reflection
        /// on 'op_GreaterThanOrEqual'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is greater than or equal to <paramref name="b"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static bool GreaterThanOrEqual(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return true;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return true;

            switch (a)
            {
                case int a_ when b is int b_: /**/                       return a_ >= b_;
                case uint a_ when b is uint b_: /**/                     return a_ >= b_;
                case long a_ when b is long b_: /**/                     return a_ >= b_;
                case ulong a_ when b is ulong b_: /**/                   return a_ >= b_;
                case byte a_ when b is byte b_: /**/                     return a_ >= b_;
                case sbyte a_ when b is sbyte b_: /**/                   return a_ >= b_;
                case float a_ when b is float b_: /**/                   return a_ >= b_;
                case double a_ when b is double b_: /**/                 return a_ >= b_;
                case decimal a_ when b is decimal b_: /**/               return a_ >= b_;
                case char a_ when b is char b_: /**/                     return a_ >= b_;
                case short a_ when b is short b_: /**/                   return a_ >= b_;
                case ushort a_ when b is ushort b_: /**/                 return a_ >= b_;
                case DateTime a_ when b is DateTime b_: /**/             return a_ >= b_;
                case DateTimeOffset a_ when b is DateTimeOffset b_: /**/ return a_ >= b_;
                case TimeSpan a_ when b is TimeSpan b_: /**/             return a_ >= b_;
            }

            var aType = a.GetType();
            var bType = b.GetType();

            var op = ComparingOperatorCache.Get(ComparingOperatorCache.Operator.GreaterThanOrEqual, aType, bType) ??
                throw new ArgumentException("The >= operator cannot be applied to: " + aType.FullName);

            return op(a, b);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is less than <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object,
        /// object)"/>. Then it checks all known types with the &lt; operator, then with reflection
        /// on 'op_LessThan'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is less than <paramref name="b"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">Lesser than operator cannot be applied</exception>
        public static bool LessThan(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return false;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return false;

            switch (a)
            {
                case int a_ when b is int b_: /**/                       return a_ < b_;
                case uint a_ when b is uint b_: /**/                     return a_ < b_;
                case long a_ when b is long b_: /**/                     return a_ < b_;
                case ulong a_ when b is ulong b_: /**/                   return a_ < b_;
                case byte a_ when b is byte b_: /**/                     return a_ < b_;
                case sbyte a_ when b is sbyte b_: /**/                   return a_ < b_;
                case float a_ when b is float b_: /**/                   return a_ < b_;
                case double a_ when b is double b_: /**/                 return a_ < b_;
                case decimal a_ when b is decimal b_: /**/               return a_ < b_;
                case char a_ when b is char b_: /**/                     return a_ < b_;
                case short a_ when b is short b_: /**/                   return a_ < b_;
                case ushort a_ when b is ushort b_: /**/                 return a_ < b_;
                case DateTime a_ when b is DateTime b_: /**/             return a_ < b_;
                case DateTimeOffset a_ when b is DateTimeOffset b_: /**/ return a_ < b_;
                case TimeSpan a_ when b is TimeSpan b_: /**/             return a_ < b_;
            }

            var aType = a.GetType();
            var bType = b.GetType();

            var op = ComparingOperatorCache.Get(ComparingOperatorCache.Operator.LessThan, aType, bType) ??
                throw new ArgumentException("The < operator cannot be applied to: " + aType.FullName);

            return op(a, b);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is less than or equal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object,
        /// object)"/>. Then it checks all known types with the &lt;= operator, then with reflection
        /// on 'op_LessThanOrEqual'
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">Operator cannot be applied</exception>
        public static bool LessThanOrEqual(object a, object b)
        {
            if (a == null && b == null) // if they are both null then they are eventually equal
                return true;

            if (a == null) return false; // same behaviour like nullable types
            if (b == null) return false;

            if (object.ReferenceEquals(a, b)) // They are also equal here
                return true;

            switch (a)
            {
                case int a_ when b is int b_: /**/                       return a_ <= b_;
                case uint a_ when b is uint b_: /**/                     return a_ <= b_;
                case long a_ when b is long b_: /**/                     return a_ <= b_;
                case ulong a_ when b is ulong b_: /**/                   return a_ <= b_;
                case byte a_ when b is byte b_: /**/                     return a_ <= b_;
                case sbyte a_ when b is sbyte b_: /**/                   return a_ <= b_;
                case float a_ when b is float b_: /**/                   return a_ <= b_;
                case double a_ when b is double b_: /**/                 return a_ <= b_;
                case decimal a_ when b is decimal b_: /**/               return a_ <= b_;
                case char a_ when b is char b_: /**/                     return a_ <= b_;
                case short a_ when b is short b_: /**/                   return a_ <= b_;
                case ushort a_ when b is ushort b_: /**/                 return a_ <= b_;
                case DateTime a_ when b is DateTime b_: /**/             return a_ <= b_;
                case DateTimeOffset a_ when b is DateTimeOffset b_: /**/ return a_ <= b_;
                case TimeSpan a_ when b is TimeSpan b_: /**/             return a_ <= b_;
            }

            var aType = a.GetType();
            var bType = b.GetType();

            var op = ComparingOperatorCache.Get(ComparingOperatorCache.Operator.LessThanOrEqual, aType, bType) ??
                throw new ArgumentException("The <= operator cannot be applied to: " + aType.FullName);

            return op(a, b);
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is unequal to <paramref name="b"/>
        /// </summary>
        /// <typeparam name="T">The type of the objects to be compared</typeparam>
        /// <typeparam name="TValue">The values of the object used to compare them (e.g. Hash)</typeparam>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <param name="selector">The value selector which will be used for the compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is unequal to <paramref name="b"/>; otherwise, false.
        /// </returns>
        public static bool UnEquals<T, TValue>(T a, T b, Func<T, TValue> selector)
        {
            if ((a == null && b != null) || (a != null && b == null))
                return true;

            if (a == null && b == null)
                return false;

            return Comparer.UnEquals(selector(a), selector(b));
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is unequal to <paramref name="b"/>
        /// <para/>
        /// Checks reference equality first with <see cref="object.ReferenceEquals(object,
        /// object)"/>. Then it checks all known types with the != operator, then with reflection on
        /// 'op_Inequality' and as last resort uses <see cref="object.Equals(object, object)"/> to
        /// determine unequality
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns>
        /// true if <paramref name="a"/> is unequal to <paramref name="b"/>; otherwise, false.
        /// </returns>
        public static bool UnEquals(object a, object b)
        {
            if (a == null && b == null)
                return false;

            if (a == null) return true;
            if (b == null) return true;

            if (object.ReferenceEquals(a, b))
                return false;

            switch (a)
            {
                case string a_ when b is string b_: /**/                 return a_ != b_;
                case int a_ when b is int b_: /**/                       return a_ != b_;
                case int a_ when b is long b_: /**/                      return a_ != b_;
                case int a_ when b is char b_: /**/                      return a_ != b_;
                case uint a_ when b is uint b_: /**/                     return a_ != b_;
                case long a_ when b is long b_: /**/                     return a_ != b_;
                case long a_ when b is int b_: /**/                      return a_ != b_;
                case ulong a_ when b is ulong b_: /**/                   return a_ != b_;
                case byte a_ when b is byte b_: /**/                     return a_ != b_;
                case byte a_ when b is char b_: /**/                     return a_ != b_;
                case sbyte a_ when b is sbyte b_: /**/                   return a_ != b_;
                case float a_ when b is float b_: /**/                   return a_ != b_;
                case float a_ when b is double b_: /**/                  return a_ != b_;
                case double a_ when b is double b_: /**/                 return a_ != b_;
                case double a_ when b is float b_: /**/                  return a_ != b_;
                case decimal a_ when b is decimal b_: /**/               return a_ != b_;
                case char a_ when b is char b_: /**/                     return a_ != b_;
                case char a_ when b is int b_: /**/                      return a_ != b_;
                case char a_ when b is byte b_: /**/                     return a_ != b_;
                case short a_ when b is short b_: /**/                   return a_ != b_;
                case ushort a_ when b is ushort b_: /**/                 return a_ != b_;
                case DateTime a_ when b is DateTime b_: /**/             return a_ != b_;
                case DateTimeOffset a_ when b is DateTimeOffset b_: /**/ return a_ != b_;
                case TimeSpan a_ when b is TimeSpan b_: /**/             return a_ != b_;
                case IntPtr a_ when b is IntPtr b_: /**/                 return a_ != b_;
                case UIntPtr a_ when b is UIntPtr b_: /**/               return a_ != b_;
                case Guid a_ when b is Guid b_: /**/                     return a_ != b_;
            }

            var aType = a.GetType();
            var bType = b.GetType();

            if (ComparingOperatorCache.Get(ComparingOperatorCache.Operator.Inequality, aType, bType) is ComparerOperator op1)
                return op1(a, b);

            if (ComparingOperatorCache.Get(ComparingOperatorCache.Operator.Inequality, bType, aType) is ComparerOperator op2)
                return op2(b, a);

            if (IEquatableCache.Get(aType, bType) is EqualsMethod op3)
                return !op3(a, b);

            if (IEquatableCache.Get(bType, aType) is EqualsMethod op4)
                return !op4(b, a);

            return !a.Equals(b);
        }
    }
}