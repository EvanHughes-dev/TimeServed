using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Props
{

    public enum ObjectType
    {
        Item = 0,
        Camera = 1,
        Box = 2,
        Door = 3
    }

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
        public Point Position { get; set; }

        /// <summary>
        /// Creates a new Prop with the given sprite and position.
        /// </summary>
        /// <param name="sprite">The sprite this Prop displays with.</param>
        /// <param name="position">The position of this Prop.</param>
        public Prop(Image sprite, Point position)
        {
            // Save params without filtering
            Sprite = sprite;
            Position = position;
        }
    }
}
