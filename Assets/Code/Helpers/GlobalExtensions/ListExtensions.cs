using System.Collections.Generic;
using UnityEngine;

namespace Code.Helpers.GlobalExtensions
{
    public static class ListExtensions
    {
        public static T GetRandomElement<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }

            var randomIndex = Random.Range(0, list.Count);
            return list[randomIndex];
        }
    }
}