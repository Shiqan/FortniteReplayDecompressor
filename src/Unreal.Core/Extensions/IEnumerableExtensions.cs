using System;
using System.Collections.Generic;
using System.Linq;

namespace Unreal.Core.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> enumerable, Func<T, bool> func, T replacement)
        {
            return enumerable.Select(i => func(i) ? replacement : i);
        }
    }
}
