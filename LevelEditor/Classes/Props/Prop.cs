using LevelEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Props
{
    /// <summary>
    /// Represents a single Prop (interactable GameObject) in a room.
    /// </summary>
    public abstract class Prop
    {
        /// <summary>
        /// This prop's sprite.
        /// </summary>
        public Image Sprite { get; set; }
        /// <summary>
        /// This point's integer position in the room, in tile space.
        /// (0, 0) is the top-left corner of the room.
        /// </summary>
        public Point? Position { get; set; }

        /// <summary>
        /// Creates a new Prop with the given sprite and position.
        /// </summary>
        /// <param name="sprite">The sprite this Prop displays with.</param>
        /// <param name="position">The position of this Prop.</param>
        public Prop(Image sprite, Point? position = null)
        {
            // Save params without filtering
            Sprite = sprite;
            Position = position;
        }

        /// <summary>
        /// Creates a copy of this Prop at the given position.
        /// </summary>
        /// <param name="position">The position to copy this Prop to.</param>
        /// <returns>A copy of this Prop at the given position.</returns>
        public abstract Prop Instantiate(Point position);
    }
}
