using System.Collections.Generic;

namespace middler.Common.ExtensionMethods
{
    public static class ListExtensions
    {
        public static List<T> EnsureList<T>(this List<T> list)
        {
            return list ?? new List<T>();
        }
    }
}
