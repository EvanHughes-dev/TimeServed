using System;
using System.IO;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// Keep track of the room the player is currently
    /// in and load the needed map
    /// </summary>
    internal class MapManager
    {
        private Room _currentRoom;
        private Room[] _rooms;
        private Texture2D[] _tileSprites;

        private GameplayManager _gameplayManager;

        /// <summary>
        /// Initialize the map manger and make rooms
        /// </summary>
        /// <param name="gameplayManager">Reference to the gamePlayManager</param>
        public MapManager(GameplayManager gameplayManager)
        {
            _tileSprites = AssetManager.TileMap;
            _gameplayManager = gameplayManager;
            _rooms = LoadMapData(_gameplayManager.Level);
            _currentRoom = _rooms[0];

            foreach (Room room in _rooms)
            {
                room.DoorTransition += TransitionRoom;
            }
        }

        public void TransitionRoom(Door transDoor)
        {
            throw new NotImplementedException("TransitionRoom not been created yet in MapManager");
        }

        /// <summary>
        /// Draw the current room to the screen
        /// </summary>
        /// <param name="batch">Batch of sprites to add to</param>
        public void Draw(SpriteBatch batch)
        {
            _currentRoom.Draw(batch, _gameplayManager.PlayerObject, _gameplayManager.ScreenSize);
        }

        /// <summary>
        /// Check if a player is allowed to move to a certain tile
        /// </summary>
        /// <param name="playerDest">Tile player wants to move to</param>
        /// <returns>If the player is allowed to move there</returns>
        public bool CheckPlayerCollision(Point playerDest)
        {
            throw new NotImplementedException(
                "CheckPlayerCollision has not been created yet in MapManager"
            );
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
