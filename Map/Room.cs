using System;
using System.Collections.Generic;
using System.IO;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.Map
{
    /// <summary>
    /// Transition from one room to another
    /// </summary>
    /// <param name="destinationDoor">Door that was interacted with</param>
    /// <param name="destRoomIndex">Destination room</param>
    delegate void DoorTransition(Door destinationDoor, int destRoomIndex);

    /// <summary>
    /// Pickup an object from the room
    /// </summary>
    /// <param name="objectToPickup">Object that was picked up</param>
    delegate void ItemPickup(Item objectToPickup);

    public enum ObjectTypes
    {
        Item = 0,
        Camera = 1,
        Box = 2,
        Door = 3
    }

    /// <summary>
    /// Hold all the information relating to what should be
    /// draw in each given room. Also controls which room should
    /// be entered when the Player uses a door
    /// </summary>
    internal class Room
    {
        /// <summary>
        /// Called when a valid door is interacted with
        /// </summary>
        public event DoorTransition DoorTransition;

        /// <summary>
        /// Get the current room
        /// </summary>
        public int RoomIndex { get; private set; }

        /// <summary>
        /// Get the path to this room's file
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Get the room's name
        /// </summary>
        public string RoomName { get; private set; }

        /// <summary>
        /// Get the number of items in the current room
        /// </summary>
        public int ItemCount
        {
            get => _itemsInRoom.Count;
        }
        private Tile[,] _map;
        private List<Prop> _itemsInRoom;
        public List<Door> Doors
        { get; private set; }

        /// <summary>
        /// Size of current map
        /// </summary>
        public Point MapSize { get; private set; }

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

            _map = new Tile[,] { };
            _itemsInRoom = new List<Prop> { };
            Doors = new List<Door> { };
            ParseData(filePath);
        }

        #region  Drawing Logic

        /// <summary>
        /// Draw all tiles and GameObjects in the current room
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public void Draw(SpriteBatch sb)
        {
            /*
            * Draw order
            *
            * Tiles
            * GameObjects
            * Doors (type of GameObject)
            */

            Point worldToScreen = MapUtils.WorldToScreen();

            Point TileSize = AssetManager.TileSize;

            // Find the coordinates of the four corners to figure out which
            // tiles and objects need to be displayed in tile positions
            // By casting to an int, we make sure we get all partial tiles
            // If any edge of a tile is on the screen, it still is displayed
            int screenMinX = worldToScreen.X / TileSize.X;
            int screenMinY = worldToScreen.Y / TileSize.Y;
            int screenMaxX = (worldToScreen.X + MapUtils.ScreenSize.X) / TileSize.X;
            int screenMaxY = (worldToScreen.Y + MapUtils.ScreenSize.Y) / TileSize.Y;

            Point pixelOffset = MapUtils.PixelOffset();

            // Display all tiles that are on screen by looping between the screenMin and screenMax on each axis
            for (int xTile = screenMinX; xTile <= screenMaxX; xTile++)
            {
                for (int yTile = screenMinY; yTile <= screenMaxY; yTile++)
                {
                    if (xTile >= _map.GetLength(0) || yTile >= _map.GetLength(1) || xTile < 0 || yTile < 0)
                        continue;
                    Tile currentTile = _map[xTile, yTile];
                    Point screenPos =
                        MapUtils.TileToWorld(xTile, yTile) - worldToScreen + pixelOffset;
                    sb.Draw(
                        AssetManager.TileMap[currentTile.SpriteIndex],
                        new Rectangle(screenPos, TileSize),
                        Color.White
                    );
                }
            }

            // Display all GameObjects on screen
            foreach (GameObject propToDraw in _itemsInRoom)
            {
                Point propPosition = propToDraw.Location;

                if (
                    propPosition.X >= screenMinX
                    && propPosition.X <= screenMaxX
                    && propPosition.Y >= screenMinY
                    && propPosition.Y <= screenMaxY
                )
                {
                    sb.Draw(
                        propToDraw.Sprite,
                        new Rectangle(
                            MapUtils.TileToWorld(propPosition) - worldToScreen + pixelOffset,
                            TileSize
                        ),
                        Color.White
                    );
                }
            }

            // Display all doors
            foreach (Door doorToDraw in Doors)
            {
                Point propPosition = doorToDraw.Location;

                if (
                    propPosition.X >= screenMinX
                    && propPosition.X <= screenMaxX
                    && propPosition.Y >= screenMinY
                    && propPosition.Y <= screenMaxY
                )
                {
                    sb.Draw(
                        doorToDraw.Sprite,
                        new Rectangle(MapUtils.TileToWorld(propPosition) - worldToScreen + pixelOffset, TileSize),
                        Color.White
                    );
                }
            }
        }

        #endregion

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
                // save the file path for debug uses
                FilePath = filePath;
                BinaryReader binaryReader = null;
                try
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
                    *   int objectType 
                    *       0 = Item
                    *       1 = Camera
                    *       2 = Door
                    *       3 = Box 
                    * 
                    *   if objectType == 0 || objectType == 3
                    *   int keyType
                    *       0 = None
                    *       1 = Key card
                    *       2 = Screwdriver
                    *       For doors, this is the key that can unlock them
                    *       For items, this is the key type they are
                    * 
                    *   if objectType == 3
                    *       int facing
                    *           0 = North/Up
                    *           1 = East/Right
                    *           2 = South/Down
                    *           3 = West/Left
                    *       int entranceIndex
                    *       int destRoom
                    *       int destDoor
                    */
                    Stream stream = File.OpenRead(filePath);
                    binaryReader = new BinaryReader(stream);

                    // Define the size of the current room and loop to populate tiles
                    int tileMapWidth = binaryReader.ReadInt32();
                    int tileMapHeight = binaryReader.ReadInt32();

                    _map = new Tile[tileMapWidth, tileMapHeight];
                    MapSize = new Point(tileMapWidth, tileMapHeight);
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
                    Dictionary<int, Point> direction = new Dictionary<int, Point> { { 0, new Point(0, -1) }, { 1, new Point(1, 0) }, { 2, new Point(0, 1) }, { 3, new Point(-1, 0) } };

                    // Parse all needed GameObjects from the file
                    while (numberOfGameObjects > 0)
                    {
                        int propIndex = binaryReader.ReadInt32();
                        int posX = binaryReader.ReadInt32();
                        int posY = binaryReader.ReadInt32();

                        ObjectTypes objectType = (ObjectTypes)binaryReader.ReadInt32();
                        if (objectType == ObjectTypes.Item|| objectType == ObjectTypes.Door)
                        {
                            Door.DoorKeyType keyType = (Door.DoorKeyType)binaryReader.ReadInt32();

                            if (objectType == ObjectTypes.Door)
                            {
                                // Parse a door from the file
                                // Next three values correspond to the needed data
                                Door doorFromFile = new Door(
                                    direction[binaryReader.ReadInt32()],
                                    binaryReader.ReadInt32(),
                                    binaryReader.ReadInt32(),
                                    binaryReader.ReadInt32(),
                                    keyType,
                                    new Point(posX, posY),
                                    AssetManager.DoorTexture[propIndex]
                                );
                                // When this door is interacted with, transition the player
                                doorFromFile.OnDoorInteract += TransitionPlayer;
                                Doors.Add(doorFromFile);
                            }
                            else
                            {
                                // Parse a prop from the file

                                Item newItemInRoom = new Item(
                                    new Point(posX, posY),
                                    AssetManager.PropTextures[propIndex],
                                    "TEMP_NAME",
                                    keyType
                                );
                                // When this item is picked up, remove it from this room
                                newItemInRoom.OnItemPickup += RemoveGameObject;
                                _itemsInRoom.Add(newItemInRoom);
                            }
                        }
                        else if (objectType == ObjectTypes.Camera)
                        {
                            _itemsInRoom.Add(new Camera(new Point(posX, posY), AssetManager.Cameras[propIndex]));
                        }
                        else if (objectType == ObjectTypes.Box)
                        {
                            _itemsInRoom.Add(new Box(new Point(posX, posY), AssetManager.Boxes[propIndex]));
                        }
                        numberOfGameObjects--;
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Write(e.Message);
                }
                finally
                {
                    binaryReader.Close();
                }
            }
            else
            {
                throw new FileNotFoundException(
                    $"File could not be found when loading room {RoomName}. File path {filePath}"
                );
            }
        }

        /// <summary>
        /// Transition the player from one room to another
        /// </summary>
        /// <param name="doorToTravelTo">Door that the player will exit</param>
        /// <param name="destRoom">Destination room </param>
        private void TransitionPlayer(Door doorToTravelTo, int destRoom)
        {
            DoorTransition(doorToTravelTo, destRoom);
        }

        /// <summary>
        /// Return if a tile can be walk on
        /// </summary>
        /// <param name="pointToCheck">Tile to check</param>
        /// <returns>If the tile is walkable</returns>
        public bool VerifyWalkable(Point pointToCheck)
        {
            return _map[pointToCheck.X, pointToCheck.Y].IsWalkable;
        }

        /// <summary>
        /// Verifies that the tile the player is looking at contains an interactable item
        /// </summary>
        /// <param name="playerFacing">The location of the tile the player is facing</param>
        /// <returns></returns>
        public Prop VerifyInteractable(Point playerFacing)
        {
            foreach (GameObject prop in _itemsInRoom)
            {
                if (prop is Prop)
                {
                    if (playerFacing == prop.Location)
                    {
                        return (Prop)prop;
                    }
                }
            }
            foreach (Door door in Doors)
            {
                if (playerFacing == door.Location)
                {
                    return door;
                }
            }
            return null;
        }

        /// <summary>
        /// Remove an object from the room's inventory
        /// </summary>
        /// <param name="itemToRemove">Item to remove</param>
        public void RemoveGameObject(Item itemToRemove)
        {
            _itemsInRoom.Remove(itemToRemove);
        }
    }
}
