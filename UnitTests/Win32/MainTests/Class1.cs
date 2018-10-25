using System;
using System.Collections.Generic;
using UnitTests.Activator;

namespace CecilatorTests
{
    public class Blub<T1, T2>
    {
        public object Create()
        {
            return new ClassWithGeneric<T1, T2>();
        }
    }

    internal class Class176<T1, T2>
    {
        public object CreateInstance(object[] array)
        {
            if (array == null || array.Length == 0)
            {
                return new ClassWithGeneric<T1, T2>();
            }
            if (array.Length == 1 && array[0] is IEnumerable<T2>)
            {
                return new ClassWithGeneric<T1, T2>(array[0] as IEnumerable<T2>);
            }
            if (array.Length == 2 && array[0] is T1 && array[1] is T2)
            {
                return new ClassWithGeneric<T1, T2>((T1)array[0], (T2)array[1]);
            }
            if (array == null || array.Length == 0)
            {
                return new ClassWithGeneric<T1, T2>();
            }
            throw new NotImplementedException("");
        }
    }
}