using System;
using System.Collections.Generic;
using System.Text;

namespace Buzzbox_Common
{
    static class Extensions
    {
        //Source: http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        private static readonly Random Rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
