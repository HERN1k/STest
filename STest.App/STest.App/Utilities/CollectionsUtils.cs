using System;
using System.Collections.Generic;

namespace STest.App.Utilities
{
    public class CollectionsUtils
    {
        public static void Shuffle<T>(IList<T> list)
        {
            var random = new Random();

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);

                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}