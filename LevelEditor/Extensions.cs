using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    /// <summary>
    /// A static class of whatever extension methods happen to be useful for this project.
    /// </summary>
    public static class Extensions
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

        /// <summary>
        /// Gets the form-relative location of this control, irrespective of any parents this control may have.
        /// </summary>
        /// <param name="control">This control.</param>
        /// <returns>The form-relative location of this control.</returns>
        public static Point GetAbsoluteLocation(this Control control)
        {
            // Recursive method!
            //   Goes up to the highest parent, and then adds all of the found locations together
            //   to find the form-relative location of this control (rather than just the parent-relative location)
            if (control.Parent != null)
                return control.Parent.GetAbsoluteLocation().Add(control.Location);
            else
                return control.Location;
        }

        /// <summary>
        /// Returns a new point that is the component-wise combination of this point and another point.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to add to this point.</param>
        /// <returns>The combination of the points, after adding them (component-wise).</returns>
        public static Point Add(this Point point, Point other)
        {
            // Why, on god's green earth, is this not built-in?
            return new Point(
                point.X + other.X,
                point.Y + other.Y);
        }

        /// <summary>
        /// Returns a new point that is the component-wise combination of this point and another point.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to add to this point.</param>
        /// <returns>The combination of the points, after adding them (component-wise).</returns>
        public static PointF Add(this PointF point, PointF other)
        {
            // Oh come *ON!*
            return new PointF(
                point.X + other.X,
                point.Y + other.Y);
        }
    }
}
