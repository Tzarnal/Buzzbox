using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Buzzbox_Common
{
    static class Extensions
    {
        //Randomize a given lists order.
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

        public static string CapitalizeOnlyFirstLetter(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public static string RemoveMarkup(this string str)
        {
            var output = Regex.Replace(str, @"</?.>", "");
            output = Regex.Replace(output, @"\[.\]", "");
            output = Regex.Replace(output, @"\{.*?}", "");

            return output;
        }
    }
}
