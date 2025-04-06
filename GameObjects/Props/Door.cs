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
        /// Get the direction the player will be outputted in when entering this door 
        /// </summary>
        public Point DestinationTile { get; private set; }
        /// <summary>
        /// Get the index of this door
        /// </summary>
        public int DoorIndex { get; private set; }
        /// <summary>
        /// Get the room this door leads to
        /// </summary>
        public int DestRoom { get; private set; }

        /// <summary>
        /// Get the door this door leads to
        /// </summary>
        public int _destDoorIndex { get; private set; }

        /// <summary>
        /// Called when the player successfully interacts with a door
        /// </summary>
        public event DoorTransition OnDoorInteract;

        private Door _destDoor;

        /// <summary>
        ///Create the door object
        /// </summary>
        /// <param name="sourceDoor">This door's index</param>
        /// <param name="destRoom">Room index that this door leads to</param>
        /// <param name="_destDoor">Door's index that this door goes to</param>
        /// <param name="keyType">Type of key this door leads to</param>
        /// <param name="location">Position in room</param>
        /// <param name="sprite">Sprite to display</param>
        public Door(
            Point outPosition,
            int sourceDoor,
            int destRoom,
            int _destDoor,
            DoorKeyType keyType,
            Point location,
            Texture2D sprite
        )
            : base(location, sprite)
        {
            DoorIndex = sourceDoor;
            DestRoom = destRoom;
            _destDoorIndex = _destDoor;
            DestinationTile = outPosition + location;
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
                OnDoorInteract?.Invoke(_destDoor, DestRoom);
            }
        }

        /// <summary>
        /// Assign the door to which this door leads 
        /// </summary>
        /// <param name="doorAssignment">Door this door leads to</param>
        public void AssignDoor(Door doorAssignment)
        {
            _destDoor = doorAssignment;
        }
    }
}
