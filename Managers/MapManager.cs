using System.Collections.Generic;
using System.IO;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.GameObjects.Triggers;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        //private static Room[] _rooms;
        private static Dictionary<int, Room> _rooms;

        /// <summary>
        /// Initialize the map manger and make rooms
        /// </summary>
        public static void Initialize()
        {
            _rooms = new Dictionary<int, Room>();
        }

        /// <summary>
        /// Draw the current room to the screen
        /// </summary>
        /// <param name="batch">Batch of sprites to add to</param>
        public static void Draw(SpriteBatch batch)
        {
            _currentRoom.Draw(batch);
        }

        #region Level Loading

        /// <summary>
        /// Switch to the level in gameplayManager
        /// </summary>
        public static void ChangeLevel(int level)
        {
            //It might be better to just initialize MapManager again rather than have this method

            Room[] rooms = LoadMapData(level);
            _rooms.Clear();
            _currentRoom = rooms[0];
            OnRoomUpdate?.Invoke(_currentRoom);
            foreach (Room room in rooms)
            {
                room.DoorTransition += TransitionRoom;
                _rooms.Add(room.RoomIndex, room);
            }
        }

        /// <summary>
        /// Load all the needed data relating to each room
        /// from the corresponding files and format them
        /// </summary>
        /// <param name="currentLevel">Integer value of the current level of the game</param>
        /// <param name="alternative">A string that acts as an alternative way to load the map data, 
        /// such as if map data isn't being loaded from a level with an integer value</param>
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
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(folderPath + "/level.level")))
                {
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
                }
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

        #region Checkpoints

        /// <summary>
        /// Loads the map data for the given checkpoint
        /// </summary>
        /// <param name="baseFolder">Base folder to load data from</param>
        /// <param name="roomIndex">Index of the room to load the player into</param>
        /// <param name="level"> Current level loading </param>
        public static void LoadCheckpoint(string baseFolder, int roomIndex, int level)
        {

            Room[] rooms = LoadMapData(level, baseFolder);
            _rooms.Clear();
            foreach (Room room in rooms)
            {
                room.DoorTransition += TransitionRoom;
                room.DoorTransition += TransitionRoom;
                _rooms.Add(room.RoomIndex, room);
            }
            ChangeRoom(roomIndex);
        }

        /// <summary>
        /// Saves the map's data to the baseFolder via creation of a level.level file and all the .room files
        /// </summary>
        /// <param name="baseFolder">Folder that the Map data gets saved to</param>
        public static void SaveMap(string baseFolder)
        {
            //Make the .level file
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(baseFolder + "/level.level")))
            {
                //Save number of rooms
                writer.Write(_rooms.Count);

                //Write data for each room
                foreach (KeyValuePair<int, Room> kvp in _rooms)
                {
                    //Write name
                    writer.Write(_rooms[kvp.Key].RoomName);

                    //Write index
                    writer.Write(_rooms[kvp.Key].RoomIndex);
                }

                //Save all the rooms
                foreach (KeyValuePair<int, Room> kvp in _rooms)
                {
                    _rooms[kvp.Key].SaveRoom(baseFolder);
                }

            }
        }

        /// <summary>
        /// Validate that the .level file exists and all the needed .room files are there.
        /// If a .room file is stored in properly, that will not be detected
        /// </summary>
        /// <param name="baseFolder">Folder to check</param>
        /// <returns>If the files are valid to load the map</returns>
        public static bool ValidateSavedData(string baseFolder)
        {
            if (!File.Exists(baseFolder + "/level.level"))
                return false;

            try
            {
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(baseFolder + "/level.level")))
                {
                    int roomCount = binaryReader.ReadInt32();

                    for (int currentRoom = 0; currentRoom < roomCount; currentRoom++)
                    {
                        string roomName = binaryReader.ReadString();
                        _ = binaryReader.ReadInt32();
                        if (!File.Exists($"{baseFolder}/{roomName}.room"))
                            return false;
                    }
                }
            }
            catch
            {
                // If any error is throw, assume data was invalid
                return false;
            }
            return true;
        }
        #endregion

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

        #endregion


        #region  Tile Checking

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

        #endregion

    }
}
