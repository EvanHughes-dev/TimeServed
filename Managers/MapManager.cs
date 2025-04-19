using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.GameObjects.Triggers;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text.RegularExpressions;

namespace MakeEveryDayRecount.Managers
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
    internal static class MapManager
    {
        public static event OnRoomUpdate OnRoomUpdate;

        private static Room _currentRoom;

        /// <summary>
        /// Get the current room on the map
        /// </summary>
        public static Room CurrentRoom
        {
            get => _currentRoom;
        }
        private static Room[] _rooms;

        private readonly GameplayManager _gameplayManager;

        /// <summary>
        /// Initialize the map manger and make rooms
        /// </summary>
        /// <param name="gameplayManager">Reference to the gamePlayManager</param>
        public static void Initialize()
        {
            _rooms = LoadMapData(GameplayManager.Level);
            _currentRoom = _rooms[0];
            OnRoomUpdate?.Invoke(_currentRoom);
            foreach (Room room in rooms)
            {
                room.DoorTransition += TransitionRoom;
                _rooms.Add(room.RoomIndex, room);
            }
        }
        /// <summary>
        /// Transition the player to the new room
        /// </summary>
        /// <param name="transDoor">Door to transition from</param>
        /// <param name="destRoom"> Destination room for transition </param>
        public static void TransitionRoom(Door transDoor, int destRoom)
        { 
             GameplayManager.PlayerObject.ChangeRoom(transDoor.DestinationTile);
            ChangeRoom(destRoom);

        }

        /// <summary>
        /// Change to the given room
        /// </summary>
        /// <param name="destRoom">Room index to change to</param>
        public static void ChangeRoom(int destRoom)
        {
            _currentRoom = _rooms[destRoom];
            OnRoomUpdate?.Invoke(_currentRoom);
        }

        /// <summary>
        /// Draw the current room to the screen
        /// </summary>
        /// <param name="batch">Batch of sprites to add to</param>
        public static void Draw(SpriteBatch batch)
        {
            _currentRoom.Draw(batch);
        }

        /// <summary>
        /// Check if a player is allowed to move to a certain tile
        /// </summary>
        /// <param name="playerDest">Tile player wants to move to</param>
        /// <returns>If the player is allowed to move there</returns>
        public static bool CheckPlayerCollision(Point playerDest)
        {
            return _currentRoom.VerifyWalkable(playerDest);
        }

        /// <summary>
        /// Check if the tile in front of the player contains an interactable item
        /// </summary>
        /// <param name="playerFacing">The tile the player wants to interact with</param>
        /// <returns>Current item to interact with in the provided tile</returns>
        public static Prop CheckInteractable(Point playerFacing)
        {
            return _currentRoom.VerifyInteractable(playerFacing);
        }

        public static Trigger CheckTrigger(Point playerPosition)
        {
            return _currentRoom.VerifyTrigger(playerPosition);
        }

        /// <summary>
        /// Load all the needed data relating to each room
        /// from the corresponding files and format them
        /// </summary>
        /// <param name="currentLevel">Current level of the game</param>
        /// <returns>Formatted data loaded from files</returns>
        private static Room[] LoadMapData(int currentLevel, string alternative = "")
        {
            // only read binary data here
            // each room is in charge of parsing itself
            string folderPath;
            if (alternative != "")
            {
                folderPath = alternative;
            }
            else
            {
                folderPath = $"./Content/Levels/Level{currentLevel}";
            }

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
            if (File.Exists(folderPath + "/level.level"))
            {
                Stream streamReader = File.OpenRead(folderPath + "/level.level");
                BinaryReader binaryReader = new BinaryReader(streamReader);

                int roomCount = binaryReader.ReadInt32();
                rooms = new Room[roomCount];

                for (int currentRoom = 0; currentRoom < roomCount; currentRoom++)
                {
                    string roomName = binaryReader.ReadString();
                    string roomFilePath = $"{folderPath}/{roomName}.room";
                    int roomIndex = binaryReader.ReadInt32();
                    Room room = new Room(roomFilePath, roomName, roomIndex);
                    rooms[currentRoom] = room;
                }

                binaryReader.Close();
            }
            else
            {
                // Provide more details about what level the error occurred on
                throw new FileNotFoundException(
                    $"No Level.level file located for level {currentLevel}. File path: {folderPath}/level.level"
                );
            }
            return rooms;
        }

        /// <summary>
        /// Switch to the level in gameplayManager
        /// </summary>
        public static void ChangeLevel()
        {
            Room[] rooms = LoadMapData(_gameplayManager.Level);
            _rooms.Clear();
            _currentRoom = rooms[0];
            OnRoomUpdate?.Invoke(_currentRoom);
            foreach (Room room in rooms)
            {
                room.DoorTransition += TransitionRoom;
            }
        }

        public static void LoadCheckpoint(Checkpoint c)
        {
            _rooms = LoadMapData(1, "./CheckpointData");
            ChangeRoom(c.RoomIndex);
            foreach (Room room in _rooms)
            {
                room.DoorTransition += TransitionRoom;
            }  
        }

        public static void SaveMap(string baseFolder)
        {
            //Make the .level file
            BinaryWriter writer = new BinaryWriter(File.OpenWrite(baseFolder + "/level.level"));

            //Save number of rooms
            writer.Write(_rooms.Length);

            //Write data for each room
            for (int i = 0; i < _rooms.Length; i++)
            {
                //Write name
                writer.Write(_rooms[i].RoomName);

                //Write index
                writer.Write(_rooms[i].RoomIndex);
            }

            //Save all the rooms
            for (int i = 0; i < _rooms.Length; i++)
            {
                _rooms[i].SaveRoom(baseFolder);
            }

            //Close the writer
            writer.Close();
        }
    }
}
