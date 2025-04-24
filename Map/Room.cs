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
        Checkpoint = 0
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
        /// <summary>
        /// A public property for the items in this room.
        /// You can only add items to the room through this property, you can't remove them
        /// This property references a private list, so you can't directly use its methods
        /// </summary>
        public List<Prop> ItemsInRoom
        {
            get { return _itemsInRoom; }
            set
            {
                if (value.Count > _itemsInRoom.Count)
                {
                    _itemsInRoom = value;
                }
            }
        }
        //I originally wanted to make this a generic list of all the dynamic props in the room that might need to be updated
        //But most props don't have an update function, so it's better to have this list only be concerned with cameras
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
                if (propToDraw is Camera)
                    continue;
                if (
                    propPosition.X >= screenMinX
                    && propPosition.X <= screenMaxX
                    && propPosition.Y >= screenMinY
                    && propPosition.Y <= screenMaxY
                )
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

            foreach (Camera cam in Cameras)
            {
                cam.Draw(sb, worldToScreen, pixelOffset);
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

                        numberOfGameObjects--;
                    }

                    //Define number of triggers in the room
                    int numberOfTriggers = binaryReader.ReadInt32();

                    //Parse all needed triggers from file
                    while (numberOfTriggers > 0)
                    {
                        Point triggerPos = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                        int triggerWidth = binaryReader.ReadInt32();
                        int triggerHeight = binaryReader.ReadInt32();

                        TriggerTypes triggerType = (TriggerTypes)binaryReader.ReadInt32();

                        if (triggerType == TriggerTypes.Checkpoint)
                        {
                            Checkpoint checkpoint = new Checkpoint(triggerPos, binaryReader.ReadInt32(), triggerWidth, triggerHeight, binaryReader.ReadBoolean());
                            _triggersInRoom.Add(checkpoint);

                            //Add it to the trigger manager if it's the first time its been added
                            TriggerManager.AddUniqueCheckpoint(checkpoint);

                            //If it's the first checkpoint, make that the player's spawn
                            if (TriggerManager.Checkpoints.Count == 1)
                            {
                                TriggerManager.SetPlayerSpawn(checkpoint);
                            }
                        }
                        numberOfTriggers--;
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
        /// <param name="isCamera">Is from camera</param>
        /// <returns>If the tile is walkable. True means the tile is walkable</returns>
        public bool VerifyWalkable(Point pointToCheck, bool isCamera = false)
        {
            foreach (Prop prop in _itemsInRoom)
            {
                // If the object is a box that is held and in the square, check if it is unwalkable in the context
                if (prop.Location.Equals(pointToCheck))
                {
                    if (prop is Box)
                    {
                        // The position is unwalkable if the player isn't holding it or the item requesting the check is a camera
                        if (((Box)prop).AttachmentDirection == Players.Direction.None || isCamera)
                            return false;

                        return true;
                    }
                    else if (!isCamera)
                    {

                        return false;
                    }
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
                for (int y = 0; y < MapSize.Y; y++)
                {
                    for (int x = 0; x < MapSize.X; x++)
                    {
                        binaryWriter.Write(_map[x, y].IsWalkable);
                        binaryWriter.Write(_map[x, y].SpriteIndex);
                    }
                }

                //Write prop count
                binaryWriter.Write(_itemsInRoom.Count + Doors.Count);

                //Write out all non-door non-camera props
                for (int i = 0; i < _itemsInRoom.Count; i++)
                {
                    if (_itemsInRoom[i] is Camera)
                        continue;

                    binaryWriter.Write(_itemsInRoom[i].SpriteIndex);
                    binaryWriter.Write(_itemsInRoom[i].Location.X);
                    binaryWriter.Write(_itemsInRoom[i].Location.Y);

                    //If statements to determine what kind of prop this is, and if it needs extra data
                    if (_itemsInRoom[i] is Item)
                    {
                        binaryWriter.Write((int)ObjectTypes.Item);
                        binaryWriter.Write((int)((Item)_itemsInRoom[i]).ItemKeyType);
                    }
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

                //Write out all the cameras
                for (int i = 0; i < Cameras.Count; i++)
                {
                    //Standard game object parameters
                    binaryWriter.Write(Cameras[i].SpriteIndex);
                    binaryWriter.Write(Cameras[i].Location.X);
                    binaryWriter.Write(Cameras[i].Location.Y);
                    binaryWriter.Write((int)ObjectTypes.Camera);

                    //Unique to cameras
                    binaryWriter.Write(Cameras[i].CenterPoint.X);
                    binaryWriter.Write(Cameras[i].CenterPoint.Y);
                    binaryWriter.Write((double)Cameras[i].Spread);
                }

                //Write out all triggers
                binaryWriter.Write(_triggersInRoom.Count);
                for (int i = 0; i < _triggersInRoom.Count; i++)
                {
                    binaryWriter.Write(_triggersInRoom[i].Location.X);
                    binaryWriter.Write(_triggersInRoom[i].Location.Y);
                    binaryWriter.Write(_triggersInRoom[i].Width);
                    binaryWriter.Write(_triggersInRoom[i].Height);


                    //JTODO: I think the triggers get saved wrong in here, specifically the index
                    //If statements to determine trigger type
                    if (_triggersInRoom[i] is Checkpoint)
                    {
                        binaryWriter.Write((int)TriggerTypes.Checkpoint);
                        binaryWriter.Write(((Checkpoint)_triggersInRoom[i]).Index);
                        binaryWriter.Write(((Checkpoint)_triggersInRoom[i]).Active);
                    }
                }
            }
            catch (Exception e)
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
