// Made with <3 by Ryan Boyer http://ryanjboyer.com

using System;
using System.Linq;
using System.Collections.Generic;

namespace BurstLines
{
    public static partial class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }

        public static void For<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            T[] array = collection.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }
        }

        public static void For(this int length, Action<int> action)
        {
            for (int i = 0; i < length; i++)
            {
                action(i);
            }
        }
    }
}