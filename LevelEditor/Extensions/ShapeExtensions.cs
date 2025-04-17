using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensions
{
    /// <summary>
    /// A collection of extension methods for the shapes in the System.Drawing namespace.
    /// </summary>
    public static class ShapeExtensions
    {
        /*
         *     /$$$$$$$  /$$$$$$$$  /$$$$$$  /$$$$$$$        /$$      /$$ /$$$$$$$$
         *    | $$__  $$| $$_____/ /$$__  $$| $$__  $$      | $$$    /$$$| $$_____/
         *    | $$  \ $$| $$      | $$  \ $$| $$  \ $$      | $$$$  /$$$$| $$      
         *    | $$$$$$$/| $$$$$   | $$$$$$$$| $$  | $$      | $$ $$/$$ $$| $$$$$   
         *    | $$__  $$| $$__/   | $$__  $$| $$  | $$      | $$  $$$| $$| $$__/   
         *    | $$  \ $$| $$      | $$  | $$| $$  | $$      | $$\  $ | $$| $$      
         *    | $$  | $$| $$$$$$$$| $$  | $$| $$$$$$$/      | $$ \/  | $$| $$$$$$$$
         *    |__/  |__/|________/|__/  |__/|_______/       |__/     |__/|________/
         * 
         * Hello there, generous team member who wants to contribute to this extension class!
         *   (note: if you are not a generous team member who wants to contribute to this extension class, please disregard)
         *   
         * I'm glad you're doing so! Extension methods are really cool and a great way to keep code clean (and a great thing to 
         *   get in the practice of using!) -- if you haven't used them before, absolutely feel free to reach out to me with questions!
         *   
         * For this extension class though, I have a special rule, which is that, unless for some reason it makes absolutely no sense, 
         *   *every method must have a version for both the integer and float version of the type.*
         *   That means that if you define an extension method for Point, you must also define an identical method for PointF and vice versa
         *   (same for Rectangle/RectangleF and Size/SizeF)
         *   Put the two methods directly next to each other so it's easy to just change both whenever changes are needed!
         *   
         *   ***PRs THAT BREAK THE ABOVE RULE SHOULD GENERALLY BE REJECTED***
         *   
         * -Leah
         */

        #region Point / PointF
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

        /// <summary>
        /// Returns a new point that is the component-wise subtraction of another point from this point.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to subtract from this point.</param>
        /// <returns>The result of the component-wise subtraction.</returns>
        public static Point Subtract(this Point point, Point other) => point.Add(other.Negate());
        /// <summary>
        /// Returns a new point that is the component-wise subtraction of another point from this point.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to subtract from this point.</param>
        /// <returns>The result of the component-wise subtraction.</returns>
        public static PointF Subtract(this PointF point, PointF other) => point.Add(other.Negate());

        /// <summary>
        /// Negates both components of this point.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <returns><c>new Point(-point.X, -point.Y);</c></returns>
        public static Point Negate(this Point point) => new Point(-point.X, -point.Y);
        /// <summary>
        /// Negates both components of this point.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <returns><c>new PointF(-point.X, -point.Y);</c></returns>
        public static PointF Negate(this PointF point) => new PointF(-point.X, -point.Y);

        /// <summary>
        /// Multiplies both components of this point by a given integer.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="multiplier">The integer to multiply this point by.</param>
        /// <returns><c>new Point(point.X * multiplier, point.Y * multiplier);</c></returns>
        public static Point Multiply(this Point point, int multiplier)
        {
            return new Point(point.X * multiplier, point.Y * multiplier);
        }
        /// <summary>
        /// Multiplies both components of this point by a given float.
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="multiplier">The float to multiply this point by.</param>
        /// <returns><c>new PointF(point.X * multiplier, point.Y * multiplier);</c></returns>
        public static PointF Multiply(this PointF point, float multiplier)
        {
            return new PointF(point.X * multiplier, point.Y * multiplier);
        }

        /// <summary>
        /// Returns a new point that is the component-wise integer division of this point by another point.
        ///   For float division, cast this point to a PointF!
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to divide this point by.</param>
        /// <returns>The result of the division.</returns>
        public static Point Divide(this Point point, Point other)
        {
            return new Point(point.X / other.X, point.Y / other.Y);
        }
        /// <summary>
        /// Returns a new point that is the component-wise float division of this point by another point.
        ///   For integer division, cast both PointFs to a Point!
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to divide this point by.</param>
        /// <returns>The result of the division.</returns>
        public static PointF Divide(this PointF point, PointF other)
        {
            return new PointF(point.X / other.X, point.Y / other.Y);
        }

        /// <summary>
        /// Returns a new point which is the quotient of each of this point's components divided by the given integer.
        ///   For float division, cast this point to a PointF!
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="divisor">The integer to divide both components by.</param>
        /// <returns>The calculated quotient.</returns>
        public static Point Divide(this Point point, int divisor)
        {
            return new Point(point.X / divisor, point.Y / divisor);
        }
        /// <summary>
        /// Returns a new point which is the quotient of each of this point's components divided by the given float.
        ///   For integer division, cast this PointF to a Point!
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="divisor">The float to divide both components by.</param>
        /// <returns>The calculated quotient.</returns>
        public static PointF Divide(this PointF point, float divisor)
        {
            return new PointF(point.X / divisor, point.Y / divisor);
        }

        /// <summary>
        /// Calculates the squared distance between this point and another point.
        ///   Why squared distance? Because square roots are slow, and squared distance can still be used for relative calculations
        ///   (i.e. <, >, and/or ==)
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to calculate distance with.</param>
        /// <returns>The calculated distance, squared.</returns>
        public static int SqrDistance(this Point point, Point other)
        {
            Point difference = point.Subtract(other);

            return difference.X * difference.X + difference.Y * difference.Y;
        }
        /// <summary>
        /// Calculates the squared distance between this point and another point.
        ///   Why squared distance? Because square roots are slow, and squared distance can still be used for relative calculations
        ///   (i.e. <, >, and/or ==)
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to calculate distance with.</param>
        /// <returns>The calculated distance, squared.</returns>
        public static float SqrDistance(this PointF point, PointF other)
        {
            PointF difference = point.Subtract(other);

            return difference.X * difference.X + difference.Y * difference.Y;
        }

        /// <summary>
        /// Calculates the distance between this point and another point.
        ///   If you don't actually need the exact number, use SqrDistance()!
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to calculate distance with.</param>
        /// <returns>The calculated distance.</returns>
        public static float Distance(this Point point, Point other)
        {
            return MathF.Sqrt(point.SqrDistance(other));
        }
        /// <summary>
        /// Calculates the distance between this point and another point.
        ///   If you don't actually need the exact number, use SqrDistance()!
        /// </summary>
        /// <param name="point">This point.</param>
        /// <param name="other">The point to calculate distance with.</param>
        /// <returns>The calculated distance.</returns>
        public static float Distance(this PointF point, PointF other)
        {
            return MathF.Sqrt(point.SqrDistance(other));
        }

        #endregion
        #region Rectangle / RectangleF
        /// <summary>
        /// Nudges each individual side of this rectangle inwards or outwards. Positive values mean to move that side outwards, negative values mean to move that side inwards.
        /// </summary>
        /// <param name="rect">This rectangle.</param>
        /// <param name="top">How much to nudge the top side by.</param>
        /// <param name="bottom">How much to nudge the bottom side by.</param>
        /// <param name="left">How much to nudge the left side by.</param>
        /// <param name="right">How much to nudge the right side by.</param>
        /// <returns>The adjusted rectangle.</returns>
        public static Rectangle NudgeSides(this Rectangle rect, int top, int bottom, int left, int right)
        {
            return new Rectangle(
                rect.Left - left,
                rect.Top - top,
                rect.Width + left + right,
                rect.Height + top + bottom);
        }
        /// <summary>
        /// Nudges each individual side of this rectangle inwards or outwards. Positive values mean to move that side outwards, negative values mean to move that side inwards.
        /// </summary>
        /// <param name="rect">This rectangle.</param>
        /// <param name="top">How much to nudge the top side by.</param>
        /// <param name="bottom">How much to nudge the bottom side by.</param>
        /// <param name="left">How much to nudge the left side by.</param>
        /// <param name="right">How much to nudge the right side by.</param>
        /// <returns>The adjusted rectangle.</returns>
        public static RectangleF NudgeSides(this RectangleF rect, float top, float bottom, float left, float right)
        {
            return new RectangleF(
                rect.Left - left,
                rect.Top - top,
                rect.Width + left + right,
                rect.Height + top + bottom);
        }

        /// <summary>
        /// Gets the center point of this rectangle.
        /// </summary>
        /// <param name="rect">This rectangle.</param>
        /// <returns><c>return Point.Add(rect.Location, rect.Size / 2);</c></returns>
        public static Point GetCenter(this Rectangle rect)
        {
            return Point.Add(rect.Location, rect.Size / 2);
        }
        /// <summary>
        /// Gets the center point of this rectangle.
        /// </summary>
        /// <param name="rect">This rectangle.</param>
        /// <returns><c>return PointF.Add(rect.Location, rect.Size / 2);</c></returns>
        public static PointF GetCenter(this RectangleF rect)
        {
            return PointF.Add(rect.Location, rect.Size / 2);
        }

        #endregion
        #region Size / SizeF
        // None yet!

        #endregion
    }
}
