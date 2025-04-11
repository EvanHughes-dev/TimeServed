using System.Collections.Generic;
using System.IO;
using MakeEveryDayRecount.Managers.Replay;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Text.RegularExpressions;

namespace MakeEveryDayRecount.Managers
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

        private static List<ReplayState> _currentReplyStates;

        private static int _selectedReplayState;
        private static int _currentLevel;

        /// <summary>
        /// Initialize the replay manager
        /// </summary>
        public static void Initialize()
        {
            if (Directory.Exists(BaseFolder))
                ClearSavedData();
            else
                Directory.CreateDirectory(BaseFolder);

            _currentReplyStates = new List<ReplayState> { };
            _selectedReplayState = 0;
            PlayingReplay = false;
        }

        /// <summary>
        /// Clear any binary saved data 
        /// </summary>
        private static void ClearSavedData()
        {
            ClearDirectory(BaseFolder);
        }

        /// <summary>
        /// Clear all files and sub directories from the provided directory
        /// </summary>
        /// <param name="directory">Directory to clear</param>
        private static void ClearDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
                File.Delete(file);
            string[] directories = Directory.GetDirectories(directory);
            foreach (string clearDirectory in directories)
                ClearDirectory(clearDirectory);
        }

        /// <summary>
        /// Save the current state of the keyboard
        /// </summary>
        public static void SaveState()
        {
            _currentReplyStates.Add(new ReplayState(Keyboard.GetState(), Mouse.GetState()));
        }

        /// <summary>
        /// Clear the data saved so far
        /// </summary>
        public static void ClearData()
        {
            _currentReplyStates.Clear();
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
            for (int i = 0; i < _currentReplyStates.Count; i++)
            {
                _currentReplyStates[i].SaveToFile(binaryWriter);
            }
            binaryWriter.Close();

            _currentReplyStates.Clear();
        }

        /// <summary>
        /// Begin the replay process
        /// </summary>
        public static void BeginReplay()
        {
            _selectedReplayState = 0;
            _currentLevel = 1;
            LoadLevel(_currentLevel);
            PlayingReplay = true;
            CurrentReplayState = _currentReplyStates[_selectedReplayState];
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
            _selectedReplayState++;
            if (_selectedReplayState >= _currentReplyStates.Count)
                return false;
            CurrentReplayState = _currentReplyStates[_selectedReplayState];
            return true;

        }

        /// <summary>
        /// Load all the files for the next level
        /// </summary>
        public static void NextLevel()
        {
            _selectedReplayState = 0;
            _currentLevel++;
            LoadLevel(_currentLevel);
            CurrentReplayState = _currentReplyStates[_selectedReplayState];
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
                    _currentReplyStates.Add(new ReplayState(binaryReader));

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

    }

}