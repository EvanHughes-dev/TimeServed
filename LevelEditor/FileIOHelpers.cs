using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    /// <summary>
    /// A static helper class to handle all necessary file IO with different formats.
    /// *ALL* methods within this class are UNSAFE and MUST be called within a TRY-CATCH!
    /// </summary>
    internal static class FileIOHelpers
    {
        /// <summary>
        /// Saves a given level to the given path.
        /// </summary>
        /// <param name="level">The level to save.</param>
        /// <param name="filePath">The path to save to.</param>
        public static void SaveLevel(Level level, string filePath)
        {
            /*
             * THE .level FILE FORMAT:
             *  - int roomCount
             *  - Several rooms, with the following format:
             *    - string roomName
             *    - int roomID
             */

            BinaryWriter writer = new(new FileStream(filePath, FileMode.Create));

            int roomCount = level.Rooms.Count;

            writer.Write(roomCount);

            foreach (Room room in level.Rooms)
            {
                writer.Write(room.Name);
                writer.Write(room.Id);
            }

            writer.Close();
        }

        /// <summary>
        /// Saves a given level to the given path.
        /// </summary>
        /// <param name="level">The level to save.</param>
        /// <param name="filePath">The path to save to.</param>
        public static Level LoadLevel(string filePath)
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

                level.Rooms.Add(LoadRoom($"./{roomName}.room"));
            }

            reader.Close();

            return level;
        }

        public static bool SaveRoom(Room room, string filePath)
        {
            throw new NotImplementedException();
        }

        public static Room LoadRoom(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads all PNG images within a given folder. Does not look in any child folders.
        /// This method is UNSAFE and MUST be called within a TRY-CATCH!
        /// </summary>
        /// <param name="folderPath">A relative or absolute path to the files to load.</param>
        /// <returns>An array of the loaded images.</returns>
        public static Image[] LoadImages(string folderPath)
        {
            // In theory, I don't think there's anything stopping me from adding support for
            //   loading other image formats as well (as long as they are supported by Image.FromFile),
            //   but I am lazy and I'm pretty sure we're only going to be using PNGs
            string[] imagePaths = Directory.GetFiles(folderPath, "*.png");

            Image[] images = new Image[imagePaths.Length];
            for (int i = 0; i < imagePaths.Length; i++)
            {
                images[i] = Image.FromFile(imagePaths[i]);
            }

            return images;
        }

        /// <summary>
        /// Loads a tile from disk given the name of the sprite file in the Tiles folder and whether that tile is walkable.
        /// </summary>
        /// <param name="spriteName">The name of the sprite file in the Tiles folder, including file extension.</param>
        /// <param name="isWalkable">Whether this tile should be allowed to be walked on.</param>
        /// <returns>The loaded tile.</returns>
        public static Tile LoadTile(string spriteName, bool isWalkable)
        {
            const string TileFolderPath = "./Tiles";

            return new(Image.FromFile(Path.Join(TileFolderPath, spriteName)), isWalkable);
        }
    }
}
