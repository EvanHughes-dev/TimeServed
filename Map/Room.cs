using System;
using System.Collections.Generic;
using System.IO;
using TimeServed.GameObjects.Props;
using TimeServed.Map.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeServed.Managers;
using TimeServed.GameObjects.Triggers;

namespace TimeServed.Map
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

    /// <summary>
    /// Type of prop
    /// </summary>
    public enum ObjectTypes
    {
        Item = 0,
        Camera = 1,
        Box = 2,
        Door = 3
    }


    /// <summary>
    /// Type of trigger's that are possible
    /// </summary>
    public enum TriggerTypes
    {
        Checkpoint = 0,
        Win = 1
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

        private TileMap _map;


        /// <summary>
        /// Get the list of cameras in the room
        /// </summary>
        public List<Camera> Cameras { get; private set; }


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

            _map = new TileMap();

            Cameras = new List<Camera>();

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
            */

            Point worldToScreen = MapUtils.WorldToScreen;

            Point TileSize = AssetManager.TileSize;

            // Find the coordinates of the four corners to figure out which
            // tiles and objects need to be displayed in tile positions
            // By casting to an int, we make sure we get all partial tiles
            // If any edge of a tile is on the screen, it still is displayed
            int screenMinX = worldToScreen.X / TileSize.X;
            int screenMinY = worldToScreen.Y / TileSize.Y;
            int screenMaxX = (worldToScreen.X + MapUtils.ScreenSize.X) / TileSize.X;
            int screenMaxY = (worldToScreen.Y + MapUtils.ScreenSize.Y) / TileSize.Y;

            // Display all tiles that are on screen by looping between the screenMin and screenMax on each axis
            for (int xTile = screenMinX; xTile <= screenMaxX; xTile++)
            {
                for (int yTile = screenMinY; yTile <= screenMaxY; yTile++)
                {
                    if (!_map.WithinBounds(xTile, yTile))
                        continue;
                    Tile currentTile = _map[yTile, xTile];

                    currentTile.Draw(sb);
                }
            }

        }

        /// <summary>
        /// Draw the debug version of the map
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public void DebugDraw(SpriteBatch sb)
        {
            Point worldToScreen = MapUtils.WorldToScreen;

            Point TileSize = AssetManager.TileSize;

            // Find the coordinates of the four corners to figure out which
            // tiles and objects need to be displayed in tile positions
            // By casting to an int, we make sure we get all partial tiles
            // If any edge of a tile is on the screen, it still is displayed
            int screenMinX = worldToScreen.X / TileSize.X;
            int screenMinY = worldToScreen.Y / TileSize.Y;
            int screenMaxX = (worldToScreen.X + MapUtils.ScreenSize.X) / TileSize.X;
            int screenMaxY = (worldToScreen.Y + MapUtils.ScreenSize.Y) / TileSize.Y;

            // Display all tiles that are on screen by looping between the screenMin and screenMax on each axis
            for (int xTile = screenMinX; xTile <= screenMaxX; xTile++)
            {
                for (int yTile = screenMinY; yTile <= screenMaxY; yTile++)
                {
                    if (!_map.WithinBounds(xTile, yTile))
                        continue;
                    Tile currentTile = _map[yTile, xTile];

                    currentTile.Draw(sb);
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
                    */

                    // Define the size of the current room and loop to populate tiles
                    int tileMapWidth = binaryReader.ReadInt32();
                    int tileMapHeight = binaryReader.ReadInt32();

                    _map = new TileMap(tileMapWidth, tileMapHeight);
                    MapSize = new Point(tileMapWidth, tileMapHeight);

                    for (int tileYIndex = 0; tileYIndex < tileMapHeight; tileYIndex++)
                    {
                        for (int tileXIndex = 0; tileXIndex < tileMapWidth; tileXIndex++)
                        {
                            _map[tileYIndex, tileXIndex] = new Tile(
                                binaryReader.ReadBoolean(),
                                binaryReader.ReadInt32(),
                                new Point(tileXIndex, tileYIndex)
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
                        Prop newProp = null;
                        switch (objectType)
                        {

                            case ObjectTypes.Item:
                                Door.DoorKeyType keyTypeItem = (Door.DoorKeyType)binaryReader.ReadInt32();

                                Item newItemInRoom = new Item(
                                    tileLocation,
                                    AssetManager.PropTextures,
                                    propIndex,
                                    keyTypeItem
                                );

                                newItemInRoom.OnItemPickup += RemoveGameObject;
                                newProp = newItemInRoom;
                                break;
                            case ObjectTypes.Camera:
                                Point centerCastPoint = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                                float spread = (float)binaryReader.ReadDouble();
                                Point wireBoxPoint = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());

                                Camera cam;
                                if (wireBoxPoint != new Point(-1, -1))
                                    cam = new Camera(
                                        tileLocation,
                                        AssetManager.CameraTextures,
                                        propIndex,
                                        this,
                                        centerCastPoint,
                                        spread,
                                        wireBoxPoint);
                                else
                                    cam = new Camera(
                                        tileLocation,
                                        AssetManager.CameraTextures,
                                        propIndex,
                                        this,
                                        centerCastPoint,
                                        spread);
                                Cameras.Add(cam);
                                newProp = cam;

                                break;
                            case ObjectTypes.Box:
                                newProp = new Box(tileLocation, AssetManager.Boxes, propIndex);
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
                                newProp = doorFromFile;
                                break;
                        }
                        _map[tileLocation].PropHeld = newProp;

                        numberOfGameObjects--;
                    }

                    //Define number of triggers in the room
                    int numberOfTriggers = binaryReader.ReadInt32();

                    //Parse all needed triggers from file
                    while (numberOfTriggers > 0)
                    {
                        Trigger triggerFound = null;

                        Point triggerPos = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                        int triggerWidth = binaryReader.ReadInt32();
                        int triggerHeight = binaryReader.ReadInt32();

                        TriggerTypes triggerType = (TriggerTypes)binaryReader.ReadInt32();
                        // Figure out what type of trigger to create 

                        switch (triggerType)
                        {
                            case TriggerTypes.Checkpoint:
                                Checkpoint checkpoint = new Checkpoint(triggerPos, binaryReader.ReadInt32(), triggerWidth, triggerHeight, binaryReader.ReadBoolean());
                                checkpoint.RoomIndex = RoomIndex;

                                TriggerManager.AddCheckpoint(checkpoint);

                                //If it's the first checkpoint, make that the player's spawn
                                if (checkpoint.Index == 0)
                                {
                                    TriggerManager.SetPlayerSpawn(checkpoint);
                                }
                                triggerFound = checkpoint;
                                break;

                            case TriggerTypes.Win:
                                int itemIndex = binaryReader.ReadInt32();
                                triggerFound = new Win(triggerPos, itemIndex, triggerWidth, triggerHeight);
                                break;
                        }

                        // Add the created trigger to all tiles it spans
                        for (int xIndex = 0; xIndex < triggerWidth; xIndex++)
                            for (int yIndex = 0; yIndex < triggerHeight; yIndex++)
                                _map[yIndex + triggerPos.Y, xIndex + triggerPos.X].TriggerHeld = triggerFound;

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
            if (!_map.WithinBounds(pointToCheck))
                return false;

            Prop prop;
            if ((prop = _map[pointToCheck].PropHeld) != null)
            {
                if (prop is Box box)
                {
                    // The position is unwalkable if the player isn't holding it or the item requesting the check is a camera
                    if (isCamera || box.AttachmentDirection == Players.Direction.None)
                        return false;

                    return true;
                }
                else if (!isCamera)
                {

                    return false;
                }
            }

            return _map[pointToCheck].IsWalkable;
        }

        /// <summary>
        /// Verifies that the tile the player is looking at contains an interactable item
        /// </summary>
        /// <param name="playerFacing">The location of the tile the player is facing</param>
        /// <returns>Prop to interact with</returns>
        public Prop VerifyInteractable(Point playerFacing)
        {
            if (!_map.WithinBounds(playerFacing))
                return null;
            return _map[playerFacing].PropHeld;
        }

        /// <summary>
        /// Checks if the tile the player is currently standing on is a trigger.
        /// </summary>
        /// <param name="playerPosition">Player's current position</param>
        /// <returns>The trigger the player is standing on, or null if no such trigger exists.</returns>
        public Trigger VerifyTrigger(Point playerPosition)
        {
            return _map[playerPosition].TriggerHeld;
        }


        /// <summary>
        /// Remove an object from the room's inventory
        /// </summary>
        /// <param name="itemToRemove">Item to remove</param>
        public void RemoveGameObject(Item itemToRemove)
        {
            _map[itemToRemove.Location].PropHeld = null;
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

                List<Prop> propsInRoom = new List<Prop> { };
                List<Trigger> triggersInRoom = new List<Trigger> { };

                //Write dimensions of room
                binaryWriter.Write(MapSize.X);
                binaryWriter.Write(MapSize.Y);

                //Write out all the tiles
                for (int y = 0; y < MapSize.Y; y++)
                {
                    for (int x = 0; x < MapSize.X; x++)
                    {
                        Tile tile = _map[y, x];
                        binaryWriter.Write(tile.IsWalkable);
                        binaryWriter.Write(tile.SpriteIndex);

                        if (tile.PropHeld != null && !(tile.PropHeld is WireBox))
                            propsInRoom.Add(tile.PropHeld);
                        if (tile.TriggerHeld != null)
                            triggersInRoom.Add(tile.TriggerHeld);
                    }
                }

                //Write prop count
                binaryWriter.Write(propsInRoom.Count);

                //Write out all non-door non-camera props
                foreach (Prop propToSave in propsInRoom)
                {


                    binaryWriter.Write(propToSave.SpriteIndex);
                    binaryWriter.Write(propToSave.Location.X);
                    binaryWriter.Write(propToSave.Location.Y);

                    //If statements to determine what kind of prop this is, and if it needs extra data
                    if (propToSave is Item)
                    {
                        binaryWriter.Write((int)ObjectTypes.Item);
                        binaryWriter.Write((int)((Item)propToSave).ItemKeyType);
                    }
                    else if (propToSave is Box)
                    {
                        binaryWriter.Write((int)ObjectTypes.Box);
                    }
                    else if (propToSave is Door door)
                    {
                        binaryWriter.Write((int)ObjectTypes.Door);

                        //Unique to doors
                        binaryWriter.Write((int)door.KeyType);
                        binaryWriter.Write(door.DestRoom);
                        binaryWriter.Write(door.DestinationTile.X);
                        binaryWriter.Write(door.DestinationTile.Y);
                    }
                    else if (propToSave is Camera cam)
                    {
                        binaryWriter.Write((int)ObjectTypes.Camera);

                        //Unique to cameras
                        binaryWriter.Write(cam.CenterPoint.X);
                        binaryWriter.Write(cam.CenterPoint.Y);
                        binaryWriter.Write((double)cam.Spread);
                        WireBox camWireBox = cam.WireBox;
                        if (camWireBox == null)
                        {
                            binaryWriter.Write(-1);
                            binaryWriter.Write(-1);
                        }
                        else
                        {
                            binaryWriter.Write(camWireBox.Location.X);
                            binaryWriter.Write(camWireBox.Location.Y);
                        }
                    }
                    else
                    {
                        throw new Exception("Attempted to save an unknown prop");
                    }
                }

                //Write out all triggers
                binaryWriter.Write(triggersInRoom.Count);
                foreach (Trigger trigger in triggersInRoom)
                {
                    binaryWriter.Write(trigger.Location.X);
                    binaryWriter.Write(trigger.Location.Y);
                    binaryWriter.Write(trigger.Width);
                    binaryWriter.Write(trigger.Height);
                    //If statements to determine trigger type
                    if (trigger is Checkpoint)
                    {
                        binaryWriter.Write((int)TriggerTypes.Checkpoint);
                        binaryWriter.Write(((Checkpoint)trigger).Index);
                        binaryWriter.Write(((Checkpoint)trigger).Active);
                    }
                    else if (trigger is Win)
                    {
                        binaryWriter.Write((int)TriggerTypes.Win);
                        binaryWriter.Write(((Win)trigger).ItemIndex);
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

        /// <summary>
        /// Get the name of the texture 2d for a tile
        /// </summary>
        /// <param name="tileLocation">Location of the tile</param>
        /// <returns>Name of the texture 2d</returns>
        public string GetTileSpriteName(Point tileLocation)
        {
            return AssetManager.TileMap[_map[tileLocation.Y, tileLocation.X].SpriteIndex].Name;
        }

        /// <summary>
        /// Add a prop in a new location
        /// </summary>
        /// <param name="prop">Prop to add</param>
        public void AddProp(Prop prop)
        {
            _map[prop.Location].PropHeld = prop;
        }

        /// <summary>
        /// Get the tile at a location
        /// </summary>
        public Tile GetTile(Point location)
        {
            if(_map.WithinBounds(location))
            return _map[location];
            else return null;
        }
    }
}
