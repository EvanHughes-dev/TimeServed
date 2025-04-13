using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Props
{
    /// <summary>
    /// Represents a single pushable Box in a room.
    /// </summary>
    public class Box : Prop
    {
        /// <summary>
        /// Creates a new Box with the given sprite and position.
        /// </summary>
        /// <param name="sprite">The sprite this Box displays with.</param>
        /// <param name="imageIndex"> Index of this image</param>
        /// <param name="position">The position of this Box.</param>
        public Box(Image sprite, int imageIndex, Point? position = null) 
            : base(sprite, imageIndex, ObjectType.Box, position ) { }

        /// <summary>
        /// Creates a copy of this Box at the given position.
        /// </summary>
        /// <param name="position">The position to copy this Box to.</param>
        /// <returns>A copy of this Box at the given position.</returns>
        public override Box Instantiate(Point position)
        {
            return new Box(Sprite, ImageIndex, position);
        }
    }
}
