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
        /// The Room that this door should connect to.
        /// </summary>
        public int? ConnectedTo { get; set; }

        /// <summary>
        /// The coordinates that the door should lead to in the room it is connected to.
        /// </summary>
        public Point? Destination { get; set; }

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
        /// <param name="destination">Destination the player will be at when they interact</param>
        public Door(Image sprite, int imageIndex, KeyType keyToOpen, Point? position=null, Point? destination = null, int? roomIndex=null)
            : base(sprite,  imageIndex, ObjectType.Door, position)
        {
            // Save params
            KeyToOpen = keyToOpen;
            Destination = destination;
            ConnectedTo = roomIndex;
        }


        /// <summary>
        /// Creates a copy of this Door at the given position.
        /// </summary>
        /// <param name="position">The position to copy this Door to.</param>
        /// <returns>A copy of this Door at the given position.</returns>
        public override Door Instantiate(Point position)
        {
            return new Door(Sprite, ImageIndex, KeyToOpen,  position);
        }

        /// <summary>
        /// Creates a copy of this Door at the given position.
        /// </summary>
        /// <param name="position">The position to copy this Door to.</param>
        /// <param name="destination">Destination point of this door</param>
        /// <param name="roomIndex">Index this room coresponds to</param>
        /// <returns>A copy of this Door at the given position.</returns>
        public Door Instantiate(Point position, Point destination, int roomIndex)
        {
            return new Door(Sprite, ImageIndex, KeyToOpen, position, destination, roomIndex);
        }
    }
}
