using System.Collections.Generic;
using System.IO;
using TimeServed.Managers.Replay;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text.RegularExpressions;

namespace TimeServed.Managers
{
    /// <summary>
    /// Keep track of the important keys the player presses every frame
    /// and store them in ram until the player reaches a checkout then
    /// save to a file. Read these files to play the game in reverse
    /// </summary>
    internal static class ReplayManager
    {
        /// <summary>
        /// Get the current ReplayState for this frame 
        /// </summary>
        public static ReplayState CurrentReplayState { get; private set; }

        /// <summary>
        /// Get if the replay is active
        /// </summary>
        public static bool PlayingReplay { get; private set; }

        private static readonly string BaseFolder = "./Replay";

        private static Queue<ReplayState> _currentReplyStates;

        private static int _currentLevel;

        /// <summary>
        /// Initialize the replay manager
        /// </summary>
        public static void Initialize()
        {
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);

            _currentReplyStates = new Queue<ReplayState> { };
            PlayingReplay = false;

            CurrentReplayState = null;
        }

        /// <summary>
        /// Save the current state of the keyboard
        /// </summary>
        /// <param name="deltaTime">Time since last frame</param>
        public static void SaveState(float deltaTime)
        {
            _currentReplyStates.Enqueue(new ReplayState(Keyboard.GetState(), Mouse.GetState(), deltaTime));
        }

        /// <summary>
        /// Clear the data saved so far
        /// </summary>
        public static void ClearData()
        {
            _currentReplyStates.Clear();
        }

        /// <summary>
        /// Clear all the saved replay data from the file io
        /// </summary>
        public static void ClearSavedData()
        {
            RecursiveDelete(BaseFolder);
        }

        /// <summary>
        /// Save the current states to a file
        /// </summary>
        /// <param name="level">Level to save to</param>
        /// <param name="checkpoint">Checkpoint number to save under</param>
        public static void SaveData(int level, int checkpoint)
        {
            string folderPath = BaseFolder + $"/Level{level}";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(folderPath + $"/checkpoint{checkpoint}.replay"));
            binaryWriter.Write(_currentReplyStates.Count);
            while (_currentReplyStates.Count > 0)
            {
                _currentReplyStates.Dequeue().SaveToFile(binaryWriter);
            }
            binaryWriter.Close();

            _currentReplyStates.Clear();
        }

        /// <summary>
        /// Begin the replay process
        /// </summary>
        public static void BeginReplay()
        {
            _currentLevel = GameplayManager.HighestLevel;
            LoadLevel(_currentLevel);
            PlayingReplay = true;
            CurrentReplayState = _currentReplyStates.Dequeue();
        }

        /// <summary>
        /// End this replay and clear all data
        /// </summary>
        public static void EndReplay()
        {
            PlayingReplay = false;
            CurrentReplayState = null;
            _currentReplyStates.Clear();
        }

        /// <summary>
        /// Load the state of the next frame
        /// </summary>
        public static bool NextFrame()
        {
            if (_currentReplyStates.Count == 0)
                return false;
            CurrentReplayState = _currentReplyStates.Dequeue();
            return true;

        }

        /// <summary>
        /// Load all the files for the next level
        /// </summary>
        public static void NextLevel()
        {
            _currentLevel--;
            LoadLevel(_currentLevel);
            CurrentReplayState = _currentReplyStates.Dequeue();
        }

        /// <summary>
        /// Load the given level files
        /// </summary>
        /// <param name="level">Level to load</param>
        /// <exception cref="DirectoryNotFoundException">Directory wasn't found</exception>
        private static void LoadLevel(int level)
        {
            string folderPath = BaseFolder + $"/Level{level}";
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"No directory found at path {folderPath}");

            // Get the list of filenames in the order they were created
            List<string> fileNames = SortCheckpoints(Directory.GetFiles(folderPath, "*.replay").Select(Path.GetFileName).ToList());

            // Parse each file and add the contents to the list
            // The contents of the files are saved in ascending order (frame 2 follows frame 1) so no need to order
            foreach (string fileName in fileNames)
            {
                BinaryReader binaryReader = new BinaryReader(File.OpenRead(folderPath + "/" + fileName));
                int numberOfReplay = binaryReader.ReadInt32();

                for (int i = 0; i < numberOfReplay; i++)
                {
                    _currentReplyStates.Enqueue(new ReplayState(binaryReader));

                }
                binaryReader.Close();
            }
        }

        /// <summary>
        /// Parse the number from the string and order the list
        /// </summary>
        /// <param name="checkpoints">list of all checkpoint files</param>
        /// <returns>Ordered list of all checkpoints</returns>
        private static List<string> SortCheckpoints(List<string> checkpoints)
        {
            return checkpoints.OrderBy(x => int.Parse(Regex.Match(x, "\\d+").Value)).ToList();
        }


        /// <summary>
        /// Recursively delete all contents of a folder
        /// </summary>
        /// <param name="folderPath">Folder to delete</param>
        private static void RecursiveDelete(string folderPath)
        {

            if (!Directory.Exists(folderPath))
                return;

            foreach (string file in Directory.GetFiles(folderPath))
                File.Delete(file);

            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                RecursiveDelete(folder);
            }

            Directory.Delete(folderPath);
        }

    }

}