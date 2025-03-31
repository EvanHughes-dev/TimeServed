using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.UI
{
    /// <summary>
    /// Create a base class for all ui elements to inherit from. Contain
    /// the origin point, size, and color of the element and require a draw
    /// function. 
    /// </summary>
    internal abstract class InterfaceElement
    {
        protected Point _origin;
        protected Point _size;
        protected Color _color;

        /// <summary>
        /// Get the rectangle for the current element
        /// </summary>
        public Rectangle DisplayRect { get => new Rectangle(_origin, _size); }

        /// <summary>
        /// Create a new interface element with a origin point and size
        /// </summary>
        /// <param name="origin">Origin of the element</param>
        /// <param name="size">Size in pixels of the element</param>
        /// <param name="color">Color to display the element in</param>
        public InterfaceElement(Point origin, Point size, Color color)
        {
            _origin = origin;
            _size = size;
            _color = color;
        }

        /// <summary>
        /// Create an element with just a position and size. Default color to white
        /// </summary>
        /// <param name="origin">Origin of the element</param>
        /// <param name="size">Size in pixels of the element</param>
        public InterfaceElement(Point origin, Point size) : this(origin, size, Color.White) { }

        /// <summary>
        /// Create an element with just a position and color. Defaults size to (0,0)
        /// </summary>
        /// <param name="origin">Origin of the element</param>
        /// <param name="color">Color to display the element in</param>
        public InterfaceElement(Point origin, Color color) : this(origin, Point.Zero, color) { }

        /// <summary>
        /// Create an element with just a position. Assign size of (0,0) and color as white
        /// </summary>
        /// <param name="origin">Origin of the element</param>
        public InterfaceElement(Point origin) : this(origin, Point.Zero, Color.White) { }

        /// <summary>
        /// Create an element from a passed in rectangle and color
        /// </summary>
        /// <param name="rectangle">Rectangle to read data from</param>
        /// <param name="color">Color to display the element in</param>
        public InterfaceElement(Rectangle rectangle, Color color) : this(rectangle.Location, rectangle.Size, color) { }

        /// <summary>
        /// Create an element from a passed in rectangle. Default color to white
        /// </summary>
        /// <param name="rectangle">Rectangle to read data from</param>
        public InterfaceElement(Rectangle rectangle) : this(rectangle.Location, rectangle.Size, Color.White) { }

        /// <summary>
        /// Draw the current element
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        public abstract void Draw(SpriteBatch sb);

        /// <summary>
        /// Update the location of the element
        /// </summary>
        /// <param name="newPoint">New origin</param>
        public void ChangeOrigin(Point newPoint)
        {
            _origin = newPoint;
        }

        /// <summary>
        /// Update the size of the element
        /// </summary>
        /// <param name="newSize">New size of the element</param>
        public void ChangeSize(Point newSize)
        {
            _size = newSize;
        }
    }
}