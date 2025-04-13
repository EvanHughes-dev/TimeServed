using System;
using System.Collections.Generic;
using System.IO;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.GameObjects.Triggers;
using Microsoft.Xna.Framework.Content;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

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

    public enum TriggerTypes
    {
        PlayerSpawn = 0,
        Checkpoint = 1
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
        //I originally wanted to make this a generic list of all the props in the room that might need to be updated
        //But most props don't have an update function, so it's better to have this list only be concerned with cameras because they can actually update
        /// <summary>
        /// Get the list of cameras in the room
        /// </summary>
        public List<Camera> Cameras { get; private set; }
        private List<Trigger> _triggersInRoom;
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
            _triggersInRoom = new List<Trigger> { };
            Cameras = new List<Camera>();
            Doors = new List<Door> { };
            ParseData(filePath);

            //DELETE THIS
            Checkpoint c = new Checkpoint(new Point(1, 1), null, 1, 1, 1);
            _triggersInRoom.Add(c);
            TriggerManager.AddCheckpoint(c);
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
            foreach (Prop propToDraw in _itemsInRoom)
            {
                Point propPosition = propToDraw.Location;

                if (
                    propPosition.X >= screenMinX
                    && propPosition.X <= screenMaxX
                    && propPosition.Y >= screenMinY
                    && propPosition.Y <= screenMaxY
                )
                {
                    propToDraw.Draw(sb, worldToScreen, pixelOffset);
                }
                //Cameras are excluded from this check and are always drawn if they are in the room
                //This is done to allow the camera's vision cone to be seen even when the camera is not on the screen
                else if (propToDraw is Camera)
                {
                    propToDraw.Draw(sb, worldToScreen, pixelOffset);
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
                    doorToDraw.Draw(sb, worldToScreen, pixelOffset);
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
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // save the file path for debug uses
            FilePath = filePath;

            try
            {
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(filePath)))
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
                    *       2 = Box
                    *       3 = Door 
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
                    *       int destRoom
                    *       int outputPosX
                    *       int outputPosY
                    *  if objectType == 1
                    *       Point centerPoint point of a camera vision cone
                    *       float radian value of camera spread
                    */

                // Define the size of the current room and loop to populate tiles
                int tileMapWidth = binaryReader.ReadInt32();
                int tileMapHeight = binaryReader.ReadInt32();

                _map = new Tile[tileMapWidth, tileMapHeight];
                MapSize = new Point(tileMapWidth, tileMapHeight);
                for (int tileYIndex = 0; tileYIndex < tileMapHeight; tileYIndex++)
                {
                    for (int tileXIndex = 0; tileXIndex < tileMapWidth; tileXIndex++)
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

                    Point tileLocation = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());

                        ObjectTypes objectType = (ObjectTypes)binaryReader.ReadInt32();
                        switch (objectType)
                        {
                            case ObjectTypes.Item:
                                Door.DoorKeyType keyTypeItem = (Door.DoorKeyType)binaryReader.ReadInt32();

                                Item newItemInRoom = new Item(
                                    tileLocation,
                                    AssetManager.PropTextures,
                                    propIndex,
                                    "TEMP_NAME",
                                    keyTypeItem
                                );

                                newItemInRoom.OnItemPickup += RemoveGameObject;
                                _itemsInRoom.Add(newItemInRoom);
                                break;
                            case ObjectTypes.Camera:
                                Point centerCastPoint = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                                Camera cam = new Camera(tileLocation, AssetManager.CameraTextures, propIndex, this, centerCastPoint, (float)binaryReader.ReadDouble());
                                _itemsInRoom.Add(cam);
                                Cameras.Add(cam);
                                break;
                            case ObjectTypes.Box:
                                _itemsInRoom.Add(new Box(tileLocation, AssetManager.Boxes, propIndex));
                                break;
                            case ObjectTypes.Door:
                                Door.DoorKeyType keyTypeDoor = (Door.DoorKeyType)binaryReader.ReadInt32();

                                // Parse a door from the file
                                // Next three values correspond to the needed data
                                Door doorFromFile = new Door(
                                    binaryReader.ReadInt32(),//Read destination room
                                    new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32()),// Read the destination point in the new room
                                    keyTypeDoor,
                                    tileLocation,
                                    AssetManager.DoorTexture,
                                    propIndex
                                );
                                // When this door is interacted with, transition the player
                                doorFromFile.OnDoorInteract += TransitionPlayer;
                                Doors.Add(doorFromFile);
                                break;
                        }
                        //Define number of triggers in the room
                        //TODO: update files so that triggers are implemented
                        //For rooms with no triggers we can slap a 0 at the end and everything will be fine
                        //As of right now they are not updated, so the following line has been commented out
                        //int numberOfTriggers = binaryReader.ReadInt32();
                        int numberOfTriggers = 0; //DELETE THIS

                        //Parse all needed triggers from file
                        while (numberOfTriggers > 0)
                        {
                            Point triggerPos = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                            int triggerWidth = binaryReader.ReadInt32();
                            int triggerHeight = binaryReader.ReadInt32();

                            TriggerTypes triggerType = (TriggerTypes)binaryReader.ReadInt32();

                            if (triggerType == TriggerTypes.PlayerSpawn)
                            {
                                _triggersInRoom.Add(new PlayerSpawn(triggerPos, null));
                            }
                            else if (triggerType == TriggerTypes.Checkpoint)
                            {
                                Checkpoint checkpoint = new Checkpoint(triggerPos, null, binaryReader.ReadInt32(), triggerWidth, triggerHeight);
                                _triggersInRoom.Add(checkpoint);

                                //And add it to the trigger manager!
                                TriggerManager.AddCheckpoint(checkpoint);
                            }
                        }
                     
                        numberOfGameObjects--;
                        //TODO: ask evan why adding "binaryReader.Close()" here makes cameras not show up
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message);
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
        /// <returns>If the tile is walkable. True means the tile is walkable</returns>
        public bool VerifyWalkable(Point pointToCheck)
        {
            foreach (GameObject gameObject in _itemsInRoom)
            {
                // If the object is a box that is held and in the square, do not let the player enter it
                if (gameObject is Box && ((Box)gameObject).AttachmentDirection == Players.Direction.None
                 && gameObject.Location == pointToCheck)
                {
                    return false;
                }
            }

            return _map[pointToCheck.X, pointToCheck.Y].IsWalkable;
        }

        /// <summary>
        /// Verifies that the tile the player is looking at contains an interactable item
        /// </summary>
        /// <param name="playerFacing">The location of the tile the player is facing</param>
        /// <returns></returns>
        public Prop VerifyInteractable(Point playerFacing)
        {
            foreach (Prop obj in _itemsInRoom)
            {
                if (obj is Prop prop)
                {
                    if (playerFacing == prop.Location)
                    {
                        return prop;
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
        /// Checks if the tile the player is currently standing on is a trigger.
        /// </summary>
        /// <param name="playerPosition">Player's current position</param>
        /// <returns>The trigger the player is standing on, or null if no such trigger exists.</returns>
        public Trigger VerifyTrigger(Point playerPosition)
        {
            foreach (Trigger trigger in _triggersInRoom)
            {
                if (playerPosition.X >= trigger.Location.X && playerPosition.X < trigger.Location.X + trigger.Width &&
                    playerPosition.Y >= trigger.Location.Y && playerPosition.Y < trigger.Location.Y + trigger.Height)
                    return trigger;
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

        /// <summary>
        /// Saves the current room to a file
        /// </summary>
        /// <param name="filepath">filepath for the file</param>
        public void SaveRoom(string filepath)
        {
            //Uses the same form of room data as the ParseData function, which is...
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
                    *       2 = Box
                    *       3 = Door 
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
                    *       int destRoom
                    *       int outputPosX
                    *       int outputPosY
                    *  if objectType == 1
                    *       Point centerPoint point of a camera vision cone
                    *       float radian value of camera spread
                    *       
                    *       and then a bunch of trigger stuff that isn't implemented yet
                    */
            BinaryWriter binaryWriter = null;
            try
            {
                Stream stream = File.OpenWrite(Path.Join(filepath, $"{RoomName}.room"));
                binaryWriter = new BinaryWriter(stream);

                //Write dimensions of room
                binaryWriter.Write(MapSize.X);
                binaryWriter.Write(MapSize.Y);

                //Write out all the tiles
                for (int x = 0; x < MapSize.X; x++)
                {
                    for (int y = 0; y < MapSize.Y; y++)
                    {
                        binaryWriter.Write(_map[x, y].IsWalkable);
                        binaryWriter.Write(_map[x, y].SpriteIndex);
                    }
                }

                //Write prop count
                binaryWriter.Write(_itemsInRoom.Count + Doors.Count);

                //TODO: make sure the amount of times this loops is correct
                //Honestly it might be better to just have writing out each type of prop be its own thing
                //Write out all non-door props
                for (int i = 0; i < _itemsInRoom.Count - (Doors.Count + Cameras.Count); i++)
                {
                    binaryWriter.Write(_itemsInRoom[i].SpriteIndex);
                    binaryWriter.Write(_itemsInRoom[i].Location.X);
                    binaryWriter.Write(_itemsInRoom[i].Location.Y);

                    //If statements to determine what kind of prop this is, and if it needs extra data
                    if (_itemsInRoom[i] is Item)
                    {
                        binaryWriter.Write((int)ObjectTypes.Item);
                        binaryWriter.Write((int)((Item)_itemsInRoom[i]).ItemKeyType);
                    }
                    else if (_itemsInRoom[i] is Camera)
                        binaryWriter.Write((int)ObjectTypes.Camera);
                    else if (_itemsInRoom[i] is Box)
                        binaryWriter.Write((int)ObjectTypes.Box);
                }

                //Write out all doors
                for (int i = 0; i < Doors.Count; i++)
                {
                    //Standard game object parameters
                    binaryWriter.Write(Doors[i].SpriteIndex);
                    binaryWriter.Write(Doors[i].Location.X);
                    binaryWriter.Write(Doors[i].Location.Y);
                    binaryWriter.Write((int)ObjectTypes.Door);

                    //Unique to doors
                    binaryWriter.Write((int)Doors[i].KeyType);
                    binaryWriter.Write(Doors[i].DestRoom);
                    binaryWriter.Write(Doors[i].DestinationTile.X);
                    binaryWriter.Write(Doors[i].DestinationTile.Y);
                }

                //TODO: Write out all cameras


                //Write out all triggers
                binaryWriter.Write(_triggersInRoom.Count);
                for (int i = 0; i < _triggersInRoom.Count; i++)
                {
                    binaryWriter.Write(_triggersInRoom[i].Location.X);
                    binaryWriter.Write(_triggersInRoom[i].Location.Y);
                    binaryWriter.Write(_triggersInRoom[i].Width);
                    binaryWriter.Write(_triggersInRoom[i].Height);

                    //If statements to determine trigger type
                    if (_triggersInRoom[i] is PlayerSpawn)
                        binaryWriter.Write((int)TriggerTypes.PlayerSpawn);
                    if (_triggersInRoom[i] is Checkpoint)
                    {
                        binaryWriter.Write((int)TriggerTypes.Checkpoint);
                        binaryWriter.Write(((Checkpoint)_triggersInRoom[i]).Index);
                    }
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message);
            }
            finally
            {
                binaryWriter.Close();
            }
        }
    }
}
