using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.GameObjects.Props
{
    /// <summary>
    /// A door that has the same properties of a
    /// GameObject but all has the ability to be opened
    /// and direct the user to another room
    /// </summary>
    internal class Door : Prop
    {
        /// <summary>
        /// Track which key is required to access the door/vent
        /// </summary>
        public enum DoorKeyType
        {
            None = 0,
            Screwdriver = 1,
            Card = 2
        }

        private DoorKeyType _keyType;

        /// <summary>
        /// Get the direction the player will be outputted in when interacting with this door
        /// </summary>
        public Point DestinationTile { get; private set; }

        /// <summary>
        /// Get the room this door leads to
        /// </summary>
        public int DestRoom { get; private set; }

        /// <summary>
        /// Called when the player successfully interacts with a door
        /// </summary>
        public event DoorTransition OnDoorInteract;

        /// <summary>
        ///Create the door object
        /// </summary>
        /// <param name="outPosition">Position to output the player when they interact with the door</param>
        /// <param name="destRoom">Room index that this door leads to</param>
        /// <param name="keyType">Type of key this door leads to</param>
        /// <param name="location">Position in room</param>
        /// <param name="spriteArray">Sprite array of the door</param>
        /// <param name="spriteIndex">index of the sprite in the sprite array</param>
        public Door(
            int destRoom,
            Point outPosition,
            DoorKeyType keyType,
            Point location,
            Texture2D[] spriteArray,
            int spriteIndex
        )
            : base(location, spriteArray, spriteIndex)
        {
            DestRoom = destRoom;
            DestinationTile = outPosition;
            _keyType = keyType;
        }

        /// <summary>
        /// Check to see if this door can be interacted with
        /// </summary>
        /// <returns>If the door can be interacted</returns>
        public override void Interact(Player player)
        {
            if (_keyType == DoorKeyType.None || player.ContainsKey(_keyType))
            {
                OnDoorInteract?.Invoke(this, DestRoom);
            }
        }
    }
}
