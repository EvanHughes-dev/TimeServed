using System;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            None,
            Screwdriver,
            Card
        }

        private DoorKeyType _keyType;

        /// <summary>
        /// Get the position of this door
        /// </summary>
        public int SourceDoor { get; private set; }

        /// <summary>
        /// Get the room this door leads to
        /// </summary>
        public int DestRoom { get; private set; }

        /// <summary>
        /// Get the door this door leads to
        /// </summary>
        public int DestDoor { get; private set; }

        public event DoorTransition DoorInteracted;

        /// <summary>
        ///Create the door object
        /// </summary>
        /// <param name="sourceDoor">This door's index</param>
        /// <param name="destRoom">Room index that this door leads to</param>
        /// <param name="destDoor">Door's index that this door goes to</param>
        /// <param name="keyType">Type of key this door leads to</param>
        /// <param name="location">Position in room</param>
        /// <param name="sprite">Sprite to display</param>
        public Door(
            int sourceDoor,
            int destRoom,
            int destDoor,
            DoorKeyType keyType,
            Point location,
            Texture2D sprite
        )
            : base(location, sprite)
        {
            SourceDoor = sourceDoor;
            DestRoom = destRoom;
            DestDoor = destDoor;
            _keyType = keyType;
        }

        /// <summary>
        /// Check to see if this door can be interacted with
        /// </summary>
        /// <returns>If the door can be interacted</returns>
        public override void Interact(Player player)
        {
            if (_keyType != DoorKeyType.None && player.ContainsKey(_keyType))
            {
                DoorInteracted(this);
            }
        }

        public override void Update(float deltaTimeS)
        {
            throw new NotImplementedException();
        }
    }
}
