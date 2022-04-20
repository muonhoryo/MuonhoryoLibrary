
using System;

namespace MuonhoryoLibrary
{
    public static class ArrayExtensions
    {
        public static bool Contains<T>(this T[] array,T value)
        {
            if(array == null)
            {
                throw new ArgumentNullException("array cannot be null");
            }
            if(value == null)
            {
                throw new ArgumentNullException("value cannot be null");
            }
            if(array.Length == 0)
            {
                return false;
            }
            foreach(var item in array)
            {
                if (item.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
