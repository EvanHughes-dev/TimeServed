using System;
using System.IO;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// Call to update the current room that is active on the map
    /// </summary>
    /// <param name="newRoom">Room that is now active</param>
    delegate void OnRoomUpdate(Room newRoom);

    /// <summary>
    /// Keep track of the room the player is currently
    /// in and load the needed map
    /// </summary>
    internal class MapManager
    {
        public event OnRoomUpdate OnRoomUpdate;

        private Room _currentRoom;

        /// <summary>
        /// Get the current room on the map
        /// </summary>
        public Room CurrentRoom
        {
            get => _currentRoom;
        }
        private Room[] _rooms;

        private readonly GameplayManager _gameplayManager;

        /// <summary>
        /// Initialize the map manger and make rooms
        /// </summary>
        /// <param name="gameplayManager">Reference to the gamePlayManager</param>
        public MapManager(GameplayManager gameplayManager)
        {
            _gameplayManager = gameplayManager;
            _rooms = LoadMapData(_gameplayManager.Level);
            _currentRoom = _rooms[0];
            OnRoomUpdate?.Invoke(_currentRoom);
            foreach (Room room in _rooms)
            {
                room.DoorTransition += TransitionRoom;
            }
        }

        public void TransitionRoom(Door transDoor)
        {
            _currentRoom = _rooms[transDoor.DestRoom];
            OnRoomUpdate?.Invoke(_currentRoom);
            // Figure out how to update the player's position
            throw new NotImplementedException("TransitionRoom not been created yet in MapManager");
        }

        /// <summary>
        /// Draw the current room to the screen
        /// </summary>
        /// <param name="batch">Batch of sprites to add to</param>
        public void Draw(SpriteBatch batch)
        {
            _currentRoom.Draw(batch);
        }

        /// <summary>
        /// Check if a player is allowed to move to a certain tile
        /// </summary>
        /// <param name="playerDest">Tile player wants to move to</param>
        /// <returns>If the player is allowed to move there</returns>
        public bool CheckPlayerCollision(Point playerDest)
        {
            return _currentRoom.VerifyWalkable(playerDest);
        }

        /// <summary>
        /// Load all the needed data relating to each room
        /// from the corresponding files and format them
        /// </summary>
        /// <param name="currentLevel">Current level of the game</param>
        /// <returns>Formatted data loaded from files</returns>
        private Room[] LoadMapData(int currentLevel)
        {
            // only read binary data here
            // each room is in charge of parsing itself
            string folderPath = $"./Content/Level{currentLevel}";

            // Level.level should exist in every Level folder
            // Acts as a config file for the .room files
            // File is structured in the following form:

            /*
            * int roomCount
            *   Rooms:
            *   string roomName
            *   int roomIndex
            *
            * Loop through and pass path and index to a new room object
            */
            Room[] rooms = new Room[0];
            if (File.Exists(folderPath + "/Level.level"))
            {
                Stream streamReader = File.OpenRead(folderPath + "/Level.level");
                BinaryReader binaryReader = new BinaryReader(streamReader);

                int roomCount = binaryReader.ReadInt32();
                rooms = new Room[roomCount];

                for (int currentRoom = 0; currentRoom < roomCount; currentRoom++)
                {
                    string roomName = binaryReader.ReadString();
                    string roomFilePath = $"{folderPath}/{roomName}.room";
                    int roomIndex = binaryReader.ReadInt32();
                    rooms[currentRoom] = new Room(roomFilePath, roomName, roomIndex);
                }

                binaryReader.Close();
            }
            else
            {
                // Provide more details about what level the error occurred on
                throw new FileNotFoundException(
                    $"No Level.level file located for level {currentLevel}. File path: {folderPath}/Level.level"
                );
            }
            return rooms;
        }
    }
}
