using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensions
{
    /// <summary>
    /// A static class of extension method(s) that have to do with arrays.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Finds both indexes of a given value within this 2D array.
        /// </summary>
        /// <typeparam name="T">The type of data stored in this array.</typeparam>
        /// <param name="array">This array, the array to search.</param>
        /// <param name="toFind">The value to find the indexes of.</param>
        /// <returns>The two indexes of the found value, or (-1, -1) if it was not found.</returns>
        public static (int, int) IndexesOf<T>(this T[,] array, T toFind)
        {
            for (int y = 0; y < array.GetLength(0); y++)
            {
                for (int x = 0; x < array.GetLength(1); x++)
                {
                    if (array[y, x]!.Equals(toFind))
                        return (y, x);
                }
            }

            return (-1, -1);
        }
    }
}
