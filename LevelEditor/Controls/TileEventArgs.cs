using LevelEditor.Classes.Props;

namespace LevelEditor.Controls
{
    /// <summary>
    /// Extension of MouseEventArgs that provides data for all Tile events.
    /// </summary>
    public class TileEventArgs : MouseEventArgs
    {
        /// <summary>
        /// The tile that the mouse is hovered over.
        /// </summary>
        public Point Tile { get; }
        /// <summary>
        /// The prop on the tile that the mouse is hovered over, if any.
        /// </summary>
        public Prop? Prop { get; }

        /// <summary>
        /// Creates a new TileEventArgs.
        /// </summary>
        /// <param name="tile">The tile that the mouse is hovered over.</param>
        /// <param name="prop">The prop on the tile that the mouse is hovered over, if any.</param>
        /// <param name="button">One of the MouseButtons values that indicate which mouse button was pressed.</param>
        /// <param name="clicks">The number of times a mouse button was pressed.</param>
        /// <param name="mouseX">The x-coordinate of a mouse click, in pixels.</param>
        /// <param name="mouseY">The y-coordinate of a mouse click, in pixels.</param>
        /// <param name="delta">A signed count of the number of detents the wheel has rotated.</param>
        public TileEventArgs(Point tile, Prop? prop, MouseButtons button, int clicks, int mouseX, int mouseY, int delta)
            : base(button, clicks, mouseX, mouseY, delta)
        {
            // Save the tile and prop params
            Tile = tile;
            Prop = prop;
        }

        /// <summary>
        /// Creates a new TileEventArgs using a given MouseEventArgs as a base.
        /// </summary>
        /// <param name="tile">The tile that the mouse is hovered over.</param>
        /// <param name="prop">The prop on the tile that the mouse is hovered over, if any.</param>
        /// <param name="baseArgs">The MouseEventArgs to encapsulate.</param>
        public TileEventArgs(Point tile, Prop? prop, MouseEventArgs baseArgs)
            : this(tile, prop, baseArgs.Button, baseArgs.Clicks, baseArgs.X, baseArgs.Y, baseArgs.Delta) { }
    }
}