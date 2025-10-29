using System;
using System.Collections.Generic;

namespace CardGame.Utils
{
    public static class ShuffleExtensions
    {
        private static readonly Random _rng = new Random();
        public static void ShuffleInPlace<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}
