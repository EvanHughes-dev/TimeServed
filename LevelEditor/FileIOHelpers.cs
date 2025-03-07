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

        public static bool SaveLevel(Level level, string folderPath)
        {
            throw new NotImplementedException();
        }

        public static Level? LoadLevel(string folderPath)
        {
            throw new NotImplementedException();
        }

        public static bool SaveRoom(Room room, string folderPath)
        {
            throw new NotImplementedException();
        }

        public static Room? LoadRoom(string filePath)
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
    }
}
