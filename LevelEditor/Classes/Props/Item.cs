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
        /// <param name="keyType">
        ///     The kind of key that this Item is. Will open Doors with a matching KeyType.
        ///     Set to None to make this Item inable to open any locked doors.
        /// </param>
        /// <param name="sprite">The sprite this Item displays with.</param>
        /// <param name="position">The position of this Item.</param>
        public Item(KeyType keyType, Image sprite, Point? position = null)
            : base(sprite, position)
        {
            // Save params
            KeyType = keyType;
        }

        /// <summary>
        /// Returns a copy of this Item at the given position.
        /// </summary>
        /// <param name="position">The position to "instantiate" the Item at.</param>
        /// <returns>A copy of this Item at the given position.</returns>
        public override Item Instantiate(Point position)
        {
            return new Item(KeyType, Sprite, position);
        }
    }
}
