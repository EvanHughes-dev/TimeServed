using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;


namespace MakeEveryDayRecount.GameObjects
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

       
        public Door(int sourceDoor, int destRoom, int destDoor, DoorKeyType keyType, 
            Point location, Texture2D sprite): base(location, sprite)
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
        public override void Interact(Player player)
        {
            throw new NotImplementedException("Interact has not been created yet in Door");
        }

        public override void Update(float deltaTimeS)
        {
            throw new NotImplementedException("Update has not been created yet in Door");
        }
    }
}
