using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Props
{
    /// <summary>
    /// Represents a single door within a room that leads to another room.
    /// </summary>
    public class Door : Prop
    {
        /// <summary>
        /// The type of key necessary to open this door, if any.
        /// </summary>
        public KeyType KeyToOpen { get; set; }
        /// <summary>
        /// The direction this door is facing. A north-facing door would be placed on a south wall.
        /// </summary>
        public Orientation Facing { get; set; }

        /// <summary>
        /// The Room that this door should connect to.
        /// </summary>
        public Room? ConnectedTo { get; set; }

        /// <summary>
        /// The coordinates that the door should lead to in the room it is connected to.
        /// </summary>
        public Point Destination { get; set; }

        /// <summary>
        /// Creates a new Door with the given sprite, position, KeyType, and facing direction.
        /// </summary>
        /// <param name="sprite">The sprite this Prop displays with.</param>
        /// <param name="position">The position of this Prop.</param>
        /// <param name="imageIndex"> Index of this image</param>
        /// <param name="keyToOpen">
        ///     The type of key that can be used to open this door. 
        ///     Set to None to make this door unlocked.
        /// </param>
        /// <param name="facing">The direction this door faces.</param>
        public Door(Image sprite, Point position, int propIndex, KeyType keyToOpen, Orientation facing)
            : base(sprite, position, propIndex, ObjectType.Door)
        {
            // Save params
            KeyToOpen = keyToOpen;
            Facing = facing;
        }
    }
}
