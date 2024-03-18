
using System;

namespace MuonhoryoLibrary
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Return true if array contains value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Contains<T>(this T[] array,T value)
        {
            if(array == null)
            {
                throw new ArgumentNullException("Array cannot be null");
            }
            if(value == null)
            {
                throw new ArgumentNullException("Value cannot be null");
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
