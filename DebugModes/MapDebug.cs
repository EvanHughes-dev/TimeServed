using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.DebugModes
{
    /// <summary>
    /// Debug information relating to the map such as what
    /// room the player is in, how big the room is, and where
    /// the file for that room is
    /// </summary>
    internal class MapDebug : BaseDebug
    {
        private readonly MapManager _map;
        private Room _currentRoom;

        /// <summary>
        /// Initialize debug mode map
        /// </summary>
        public MapDebug()
            : base()
        {
            _map =  GameplayManager.Map;
            _currentRoom = _map.CurrentRoom;
            _map.OnRoomUpdate += UpdateCurrentRoom;
            AddMapDebugInfo();
        }

        /// <summary>
        /// Add all relevant data that should be debugged
        /// </summary>
        private void AddMapDebugInfo()
        {
            _objectsToDisplay.Add("Room Size", () => _currentRoom.MapSize);
            _objectsToDisplay.Add("Room Name", () => _currentRoom.RoomName);
            _objectsToDisplay.Add("Room Index", () => _currentRoom.RoomIndex);
            _objectsToDisplay.Add("Room File", () => _currentRoom.FilePath);
        }

        /// <summary>
        /// Draw all debug tiles before the text
        /// </summary>
        /// <param name="sb">Sprite batch to draw to</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb, "Map");
        }

        /// <summary>
        /// Update the room debug data is drawn from
        /// </summary>
        /// <param name="newRoom">New room to have selected</param>
        private void UpdateCurrentRoom(Room newRoom)
        {
            _currentRoom = newRoom;
        }
    }
}
