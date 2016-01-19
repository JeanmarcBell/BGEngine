using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Fills an array with a value.
        /// WARNING: If the value is a reference, then will fill with that reference.
        /// </summary>
        public static void Fill<T>(this T[] array, T value)
        {
            for(int i=0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
    }
}
