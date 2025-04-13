using LevelEditor.Classes;
using LevelEditor.Classes.Props;

namespace LevelEditor
{
    /// <summary>
    /// A static helper class to handle all necessary file IO with different formats.
    /// *ALL* methods within this class are UNSAFE and MUST be called within a TRY-CATCH!
    /// </summary>
    internal static class FileIOHelpers
    {

        #region File Saving

        /// <summary>
        /// Saves a given level to the given path.
        /// This method is UNSAFE and MUST be called within a TRY-CATCH!
        /// </summary>
        /// <param name="level">The level to save.</param>
        /// <param name="folderPath">The path to save to.</param>
        /// <param name="allTiles">All possible tiles, so this method can save the rooms with the proper tile indexes.</param>
        public static void SaveLevel(Level level, string folderPath, IEnumerable<Tile> allTiles)
        {
            /*
             * THE .level FILE FORMAT:
             *  - int roomCount
             *  - Several rooms, with the following format:
             *    - string roomName
             *    - int roomID
             */

            // Path.Join is technically safer than $"{folderPath}/level.level" since different OSs use different path join characters
            // Does that *really* matter for this program's use case? Not really! But it's good practice
            string levelPath = Path.Join(folderPath, "level.level");
            BinaryWriter writer = new(new FileStream(levelPath, FileMode.Create));

            int roomCount = level.Rooms.Count;

            writer.Write(roomCount);

            foreach (Room room in level.Rooms)
            {
                writer.Write(room.Name);
                writer.Write(room.Id);
            }

            writer.Close();

            // A level can't really be played without its rooms, so it seems good to just save those too
            foreach (var room in level.Rooms)
            {
                SaveRoom(room, folderPath, allTiles);
            }
        }

        /// <summary>
        /// Saves a room file to a given folder.
        /// This method is UNSAFE and MUST be called within a TRY-CATCH!
        /// </summary>
        /// <param name="room">The room to save.</param>
        /// <param name="folderPath">The folder to save it in.</param>
        /// <param name="allTiles">All possible tiles, so this method can save the room with the proper tile indexes.</param>
        public static void SaveRoom(Room room, string folderPath, IEnumerable<Tile> allTiles)
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
             *       int destRoomId
             *       int destX
             *       int destY
             */

            Tile[] tilesArray = [.. allTiles];


            string roomPath = Path.Join(folderPath, $"{room.Name}.room");
            BinaryWriter writer = new(new FileStream(roomPath, FileMode.Create));

            int height = room.Tiles.GetLength(0);
            int width = room.Tiles.GetLength(1);

            writer.Write(width);
            writer.Write(height);

            // Tiles are written in rows, from the top to the bottom and left to right
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Tile toWrite = room.Tiles[y, x];
                    writer.Write(toWrite.IsWalkable);
                    writer.Write(Array.IndexOf(tilesArray, toWrite));
                }
            }

            writer.Write(room.Props.Count);

            foreach (Prop prop in room.Props)
            {
                writer.Write(prop.ImageIndex);
                if (prop.Position.HasValue)
                {
                    writer.Write(prop.Position.Value.X);
                    writer.Write(prop.Position.Value.Y);
                }
                writer.Write((int)prop.PropType);
                switch (prop.PropType)
                {
                    case ObjectType.Item:
                        Item item = (Item)prop;
                        writer.Write((int)item.KeyType);
                        break;
                    case ObjectType.Camera:
                        // TODO Add camera saving when the file format has been decided
                        break;
                    case ObjectType.Box:
                        // Don't need to save any extra data for the box
                        break;
                    case ObjectType.Door:
                        Door door = (Door)prop;
                        writer.Write((int)door.KeyToOpen);
                        if (door.ConnectedTo != null)
                            writer.Write(door.ConnectedTo.Value);
                        if (door.Destination.HasValue)
                        {
                            writer.Write(door.Destination.Value.X);
                            writer.Write(door.Destination.Value.Y);
                        }
                        else
                        {
                            throw new NullReferenceException("Doors must have a destination value to save");
                        }
                        break;
                }
            }

            writer.Close();
        }

        #endregion

        #region  File Loading

        /// <summary>
        /// Loads the level at the given file path and returns it.
        /// This method is UNSAFE and MUST be called within a TRY-CATCH!
        /// </summary>
        /// <param name="filePath">The path to the .level file.</param>
        /// <param name="allTiles">A reference to the tile array, so the rooms can be loaded properly.</param>
        /// <param name="allProps">A reference to the prop array so the room can load any props that have been saved</param>
        /// <returns>The loaded level, including all of its contained rooms.</returns>
        public static Level LoadLevel(string filePath, IEnumerable<Tile> allTiles, IEnumerable<Prop> allProps)
        {
            /*
             * THE .level FILE FORMAT:
             *  - int roomCount
             *  - Several rooms, with the following format:
             *    - string roomName
             *    - int roomID
             */
            Level level = new();

            BinaryReader reader = new(new FileStream(filePath, FileMode.Open));

            int roomCount = reader.ReadInt32();

            for (int i = 0; i < roomCount; i++)
            {
                string roomName = reader.ReadString();
                int roomIndex = reader.ReadInt32();

                string folder = Path.GetDirectoryName(filePath)!;

                string roomPath = Path.Join(folder, $"{roomName}.room");

                // If the room doesn't exist... actually just don't try and load it
                //   This way, you can easily remove rooms by just deleting their .room file
                if (File.Exists(roomPath))
                {
                    level.Rooms.Add(
                        LoadRoom(roomPath, allTiles, allProps)
                        );
                    level.Rooms[level.Rooms.Count-1].Id = roomIndex;
                }

            }

            reader.Close();

            return level;
        }

        /// <summary>
        /// Loads the room at the given file path.
        /// This method is UNSAFE and MUST be called within a TRY-CATCH!
        /// </summary>
        /// <param name="filePath">The path to the .room file.</param>
        /// <param name="id">id assigned to the room</param>
        /// <param name="allTiles">A reference to the tile array, so it can be loaded properly.</param>
        /// <param name="allProps">A reference to the prop array so the room can load any props that have been saved</param>
        /// <returns>The loaded room.</returns>
        public static Room LoadRoom(string filePath, IEnumerable<Tile> allTiles, IEnumerable<Prop> allProps)
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
            *       int destRoomId
            *       int destX
            *       int destY
            */

            BinaryReader reader = new(new FileStream(filePath, FileMode.Open));

            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            string roomName = Path.GetFileNameWithoutExtension(filePath);

            Room room = new Room(roomName, width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // For loading the tiles, we actually don't care whether they were saved
                    //   as being walkable or not. We treat the tile array as gospel!
                    _ = reader.ReadBoolean();

                    int tileIndex = reader.ReadInt32();
                    Tile tile = allTiles.ElementAt(tileIndex);

                    room.Tiles[y, x] = tile;
                }
            }

            // TODO: ADD LOADING OF PROPS
            int numOfProps = reader.ReadInt32();

            while (numOfProps > 0)
            {
                int imageIndex = reader.ReadInt32();
                Point propPosition = new Point(reader.ReadInt32(), reader.ReadInt32());
                ObjectType objectType = (ObjectType)reader.ReadInt32();

                switch (objectType)
                {
                    case ObjectType.Item:
                        _ = reader.ReadInt32();// Don't need the key type
                        room.Props.Add(allProps.ElementAt(imageIndex).Instantiate(propPosition));
                        break;
                    case ObjectType.Box:
                        room.Props.Add(allProps.ElementAt(imageIndex + 5).Instantiate(propPosition));
                        break;
                    case ObjectType.Door:
                        _ = reader.ReadInt32(); //Don't need the key type
                        int destRoom = reader.ReadInt32();
                        Point destPoint = new Point(reader.ReadInt32(), reader.ReadInt32());
                        room.Props.Add(((Door)allProps.ElementAt(imageIndex + 6)).Instantiate(propPosition, destPoint, destRoom));
                        break;
                    case ObjectType.Camera:
                        break;
                }

                numOfProps--;
            }

            reader.Close();

            return room;
        }

        #endregion

        #region Loading Assets

        /// <summary>
        /// UNIMPLEMENTED
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Camera LoadCamera() => throw new NotImplementedException("CAMERAS ARE NOT IMPLEMENTED YELL AT LEAH");

        /// <summary>
        /// Loads a door from disk given the name of the sprite file and the necessary data about the door.
        /// </summary>
        /// <param name="spriteName">The name of the sprite file in the Sprites/Props folder, including file extension.</param
        /// <param name="imageIndex"> Index of this image inside the coresponding array in the game</prarm>
        /// <param name="keyToOpen">The type of key necessary to open this door, if any.</param>
        /// <param name="facing">The direction that this door is facing (a north-facing door would be placed on a south wall).</param>
        /// <returns>The loaded door.</returns>
        public static Door LoadDoor(string spriteName, int imageIndex, KeyType keyToOpen)
        {
            // These loaded props are fundamentally positionless -- they aren't in a room!
            return new Door(LoadPropSprite(spriteName), imageIndex, keyToOpen);
        }

        /// <summary>
        /// Loads an item from disk given the name of the sprite file and the necessary data about the item.
        /// </summary>
        /// <param name="spriteName">The name of the sprite file in the Sprites/Props folder, including file extension.</param>
        /// <param name="imageIndex"> Index of this image inside the coresponding array in the game</prarm>
        /// <param name="keyType">The type of key that this item is, if any.</param>
        /// <returns>The loaded item.</returns>
        public static Item LoadItem(string spriteName, int imageIndex, KeyType keyType)
        {
            return new Item(LoadPropSprite(spriteName), imageIndex, keyType);
        }

        /// <summary>
        /// Loads a box from disk given the name of the sprite file.
        /// </summary>
        /// <param name="spriteName">The name of the sprite file in the Sprites/Props folder, including file extension.</param>
        /// <param name="imageIndex"> Index of this image inside the coresponding array in the game</prarm>
        /// <returns>The loaded box.</returns>
        public static Box LoadBox(string spriteName, int imageIndex)
        {
            return new Box(LoadPropSprite(spriteName), imageIndex, null);
        }

        /// <summary>
        /// Loads a sprite with a given name from the folder containing the prop sprites.
        /// </summary>
        /// <param name="spriteName">The file name of the sprite to load, including file extension (e.g. sprite.png)</param>
        /// <returns>The loaded image.</returns>
        private static Image LoadPropSprite(string spriteName)
        {
            const string PropFolderPath = "./Sprites/Props";

            return Image.FromFile(Path.Join(PropFolderPath, spriteName));
        }

        /// <summary>
        /// Loads a tile from disk given the name of the sprite file in the Tiles folder and whether that tile is walkable.
        /// This method is UNSAFE and MUST be called within a TRY-CATCH!
        /// </summary>
        /// <param name="spriteName">The name of the sprite file in the Sprites/Tiles folder, including file extension.</param>
        /// <param name="isWalkable">Whether this tile should be allowed to be walked on.</param>
        /// <returns>The loaded tile.</returns>
        public static Tile LoadTile(string spriteName, bool isWalkable)
        {
            const string TileFolderPath = "./Sprites/Tiles";

            return new(Image.FromFile(Path.Join(TileFolderPath, spriteName)), isWalkable);
        }

        #endregion

        #region Useful File Macros
        /// <summary>
        /// Automatically update the Content.MGCB file to contain ALL files inside the Levels
        /// folder to be included in the build. This function assumes that the folder locations 
        /// for the Level Editor, Content.MGCB, and Levels folder do not change.
        /// </summary>
        public static void UpdateContentMGCB()
        {
            string pathToContent = "../../../../Content/";
            string levelsPath = Path.Combine(pathToContent, "Levels");
            string contentFilePath = Path.Combine(pathToContent, "Content.mgcb");

            List<string> fileLines = new List<string>(File.ReadAllLines(contentFilePath));
            List<string> existingEntries = new List<string>();
            List<int> commentLines = new List<int>();

            // Collect existing level references and mark their line positions
            for (int i = 0; i < fileLines.Count; i++)
            {
                string line = fileLines[i].TrimStart();
                if (line.StartsWith("#begin Levels"))
                {
                    commentLines.Add(i);
                    existingEntries.Add(line);
                }
            }

            // Iterate through all level folders and files
            foreach (string folder in Directory.GetDirectories(levelsPath).Select(Path.GetFileName)!)
            {
                foreach (string fileName in FindAllFiles(Path.Combine(levelsPath, folder!)))
                {
                    string entry = $"#begin Levels/{folder}/{fileName}";

                    if (!existingEntries.Contains(entry))
                    {
                        fileLines.Add("");
                        fileLines.Add(entry);
                        fileLines.Add($"/copy:Levels/{folder}/{fileName}");
                    }
                    else
                    {
                        commentLines.RemoveAll(index => fileLines[index] == entry);
                    }
                }
            }

            // Remove obsolete entries
            for (int i = commentLines.Count - 1; i >= 0; i--)
            {
                int index = commentLines[i];
                if (index + 1 < fileLines.Count && fileLines[index + 1].StartsWith("/copy:"))
                {
                    fileLines.RemoveAt(index + 1);
                }
                fileLines.RemoveAt(index);
            }

            // Save the updated file
            File.WriteAllLines(contentFilePath, fileLines);
        }
        /// <summary>
        /// Find all the *.level and *.room files in a folder
        /// </summary>
        /// <param name="folderPath">Path to the folder</param>
        /// <returns>List<string> with all the file names</returns>
        private static List<string> FindAllFiles(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.level").Concat(Directory.GetFiles(folderPath, "*.room"))
                            .Select(Path.GetFileName)
                            .ToList()!;
        }

        /// <summary>
        /// Copy all the contents of a folder from one location to another
        /// </summary>
        /// <param name="sourceFolder">Folder to copy the contents of</param>
        /// <param name="destFolder">Folder to copy to</param>
        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            foreach (string file in Directory.GetFiles(sourceFolder))
            {
                string destFile = Path.Combine(destFolder, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceFolder))
            {
                string destSubDir = Path.Combine(destFolder, Path.GetFileName(subDir));
                CopyFolder(subDir, destSubDir);
            }
        }

        #endregion

    }
}
