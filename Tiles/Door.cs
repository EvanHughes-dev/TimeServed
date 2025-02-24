using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount.Tiles
{
    /// <summary>
    /// A door that has the same properties of a 
    /// tile but all has the ability to be opened 
    /// and direct the user to another room
    /// </summary>
    internal class Door : Tile
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

        private int _sourceDoor;
        private int _destRoom;
        private int _destDoor;
        private DoorKeyType _keyType;

        /// <summary>
        /// Get the position of this door
        /// </summary>
        public int SourceDoor { get => _sourceDoor; }
        /// <summary>
        /// Get the room this door leads to
        /// </summary>
        public int DestRoom { get => _destRoom; }
        /// <summary>
        /// Get the door this door leads to
        /// </summary>
        public int DestDoor { get => _destDoor; }
        /// <summary>
        /// Get the required key for this door
        /// </summary>
        public DoorKeyType KeyType { get => _keyType; }

        /// <summary>
        /// Create an instance of a door or vent with the needed data
        /// </summary>
        /// <param name="sourceDoor">Door in the current room</param>
        /// <param name="destRoom">Room the door leads to</param>
        /// <param name="destDoor">Door the door leads to in the next room</param>
        /// <param name="keyType">Type of key required to access the door</param>
        /// <param name="isWalkable">If the tile can be walked on</param>
        /// <param name="spriteIndex">The sprite index of the tile</param>
        public Door(int sourceDoor, int destRoom, int destDoor, DoorKeyType keyType,
                    bool isWalkable, int spriteIndex) : base(isWalkable, spriteIndex)
        {
            _sourceDoor = sourceDoor;
            _destRoom = destRoom;
            _destDoor = destDoor;
            _keyType = keyType;
        }

        /// <summary>
        /// Check to see if this door can be interacted with
        /// </summary>
        /// <returns>If the door can be interacted</returns>
        public bool Interact() {
            throw new NotImplementedException("Interact has not been created yet in Door");
        }

    }
}
