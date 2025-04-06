using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Props
{
    /// <summary>
    /// Represents a single Item in a room.
    /// </summary>
    public class Item : Prop
    {
        /// <summary>
        /// The type of key that this Item is, if any.
        /// </summary>
        public KeyType KeyType { get; set; }

        /// <summary>
        /// Creates a new Item with the given sprite, position, and KeyType.
        /// </summary>
        /// <param name="sprite">The sprite this Item displays with.</param>
        /// <param name="position">The position of this Item.</param>
        /// <param name="imageIndex"> Index of this image</param>
        /// <param name="keyType">
        ///     The kind of key that this Item is. Will open Doors with a matching KeyType.
        ///     Set to None to make this Item inable to open any locked doors.
        /// </param>
        public Item(Image sprite, Point position, int imageIndex, KeyType keyType)
            : base(sprite, position, imageIndex, ObjectType.Item)
        {
            // Save params
            KeyType = keyType;
        }
    }
}
