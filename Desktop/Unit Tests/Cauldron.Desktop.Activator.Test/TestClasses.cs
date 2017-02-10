using Cauldron.Activator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cauldron.Desktop.Activator.Test
{
    public class KeyedTestList<TKey, TItem2, TItem> : KeyedCollection<string, TItem>
    {
        //public void AddRange(IEnumerable<TItem> items)
        //{
        //}

        //public void AddRange(IEnumerable items)
        //{
        //}

        protected override string GetKeyForItem(TItem item)
        {
            return item.ToString();
        }
    }

    [Component(typeof(ITestInterface))]
    public class TestClass : ITestInterface
    {
        public double? Height { get; set; }

        public int GetFish(string name)
        {
            return Convert.ToInt32(name);
        }
    }

    public class TestList<T> : List<T>
    {
    }
}