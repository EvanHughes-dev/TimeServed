using System;
using System.Collections.Generic;
using System.IO;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.Map.Tiles;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.Map
{
    /// <summary>
    /// Hold all the information relating to what should be
    /// draw in each given room. Also controls which room should
    /// be entered when the Player uses a door
    /// </summary>
    internal class Room
    {
        private Tile[,] _map;
        private List<GameObject> _props;
        private List<Door> _doors;
        public int RoomIndex { get; private set; }
        public string RoomName { get; private set; }

        /// <summary>
        /// Establish the room object
        /// </summary>
        /// <param name="filePath">Path to the file to generate the room data</param>
        /// <param name="roomName">Name of the current room/param>
        /// <param name="roomIndex">Index in the room array</param>
        public Room(string filePath, string roomName, int roomIndex)
        {
            RoomIndex = roomIndex;
            RoomName = roomName;
            ParseData(filePath);
        }

        /// <summary>
        /// Draw all tiles and GameObjects in the current room
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Draw(SpriteBatch sb)
        {
            /*
            * Draw order
            *
            * Tiles
            * GameObjects
            * Doors (type of GameObject)
            */

            throw new NotImplementedException("Draw not been created yet in Room");
        }

        /// <summary>
        /// Parse the binary file that contains the structure for the
        /// current room and what should populate it
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <exception cref="FileNotFoundException">File path could not be found</exception>
        private void ParseData(string filePath)
        {
            if (File.Exists(filePath))
            {
                /*
                * Form of the room data is as follows
                *
                * int tileMapWidth
                * int tileMapHeight
                *
                * Tiles:
                *   bit isWalkable 0 -> false 1 -> true
                *   int textureIndex
                *
                * int gameObjectCount
                *
                * GameObject:
                *   int propIndex
                *   int positionX
                *   int positionY
                *
                *   bit isDoor 0 -> false 1 -> true
                *   int entranceIndex
                *   int destRoom
                *   int destDoor
                *
                */
                Stream stream = File.OpenRead(filePath);
                BinaryReader binaryReader = new BinaryReader(stream);

                // Define the size of the current room and loop to populate tiles
                int tileMapWidth = binaryReader.ReadInt32();
                int tileMapHeight = binaryReader.ReadInt32();

                _map = new Tile[tileMapWidth, tileMapHeight];

                for (int tileXIndex = 0; tileXIndex < tileMapWidth; tileXIndex++)
                {
                    for (int tileYIndex = 0; tileYIndex < tileMapHeight; tileYIndex++)
                    {
                        _map[tileXIndex, tileYIndex] = new Tile(
                            binaryReader.ReadBoolean(),
                            binaryReader.ReadInt32()
                        );
                    }
                }

                // Define the number of GameObjects in the room
                // Could be a door
                int numberOfGameObjects = binaryReader.ReadInt32();

                while (numberOfGameObjects > 0)
                {
                    //TODO finish

                    int propIndex = binaryReader.ReadInt32();
                    int posX = binaryReader.ReadInt32();
                    int posY = binaryReader.ReadInt32();

                    numberOfGameObjects--;
                }

                binaryReader.Close();
            }
            else
            {
                throw new FileNotFoundException(
                    $"File could not be found when loading room {RoomName}. File path {filePath}"
                );
            }
        }
    }
}
