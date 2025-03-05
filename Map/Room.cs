using System;
using System.Collections.Generic;
using System.IO;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map.Tiles;
using Microsoft.Xna.Framework;
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

        private const int TileSize = 128;

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
        /// <param name="player">Reference to the player</param>
        /// <param name="screenSize">Size of the screen</param>
        public void Draw(SpriteBatch sb, Player player, Vector2 screenSize)
        {
            /*
            * Draw order
            *
            * Tiles
            * GameObjects
            * Doors (type of GameObject)
            */
            // Find the player's position in pixels, not tiles
            Vector2 playerPos = new Vector2(
                player.PlayerWorldPos.X * TileSize + TileSize / 2,
                player.PlayerWorldPos.Y * TileSize + TileSize / 2
            );
            Vector2 worldToScreen = playerPos - screenSize / 2;

            worldToScreen = new Vector2(
                MathHelper.Clamp(worldToScreen.X, 0, _map.GetLength(0) * TileSize - screenSize.X),
                MathHelper.Clamp(worldToScreen.Y, 0, _map.GetLength(1) * TileSize - screenSize.Y)
            );

            // Find the coordinates of the four corners to figure out which
            // tiles and objects need to be displayed

            int screenMinX = (int)worldToScreen.X;
            int screenMinY = (int)worldToScreen.Y;
            float screenMaxX = worldToScreen.X + 0;
            float screenMaxY = 0;

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
                *   bool isWalkable
                *   int textureIndex
                *
                * int gameObjectCount
                *
                * GameObject:
                *   int propIndex
                *   int positionX
                *   int positionY
                *
                *   boolean isDoor
                *   if True
                *       int entranceIndex
                *       int destRoom
                *       int destDoor
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
                    int propIndex = binaryReader.ReadInt32();
                    int posX = binaryReader.ReadInt32();
                    int posY = binaryReader.ReadInt32();

                    if (binaryReader.ReadBoolean())
                    {
                        // Next three values correspond to the needed data
                        _doors.Add(
                            new Door(
                                binaryReader.ReadInt32(),
                                binaryReader.ReadInt32(),
                                binaryReader.ReadInt32(),
                                Door.DoorKeyType.None,
                                new Point(posX, posY),
                                AssetManager.PropTextures[propIndex]
                            )
                        );
                    }
                    else
                    {
                        _props.Add(
                            new Item(new Point(posX, posY), AssetManager.PropTextures[propIndex])
                        );
                    }

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
