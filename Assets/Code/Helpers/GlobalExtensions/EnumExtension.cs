using System;
using Random = UnityEngine.Random;

namespace Code.Helpers.GlobalExtensions
{
    public static class EnumExtension
    {
        public static T GetRandom<T>(int ignoreFromStart = 1, int ignoreFromEnd = 0) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            if (ignoreFromStart > values.Length)
                ignoreFromStart = 1;
            
            var length = values.Length - ignoreFromEnd;
            if (ignoreFromStart > length)
                length = values.Length;
            
            return (T)values.GetValue(Random.Range(ignoreFromStart, length));
        }
    }
}