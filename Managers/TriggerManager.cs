using MakeEveryDayRecount.GameObjects.Triggers;
using System.Collections.Generic;
using System.IO;
using System;

namespace MakeEveryDayRecount.Managers
{
    /// <summary>
    /// Keeps track of important triggers
    /// </summary>
    internal static class TriggerManager
    {
        /// <summary>
        /// List of all checkpoints in the current level
        /// </summary>
        public static List<Checkpoint> Checkpoints { get; private set; }

        /// <summary>
        /// List of the most recent checkpoint the player has activated
        /// </summary>
        public static Checkpoint CurrentCheckpoint { get; set; }

        /// <summary>
        /// The checkpoint that the player spawns at at the start of a level
        /// </summary>
        public static Checkpoint PlayerSpawn { get; private set; }

        /// <summary>
        /// Initializes important properties (currently just the list of checkpoints)
        /// </summary>
        public static void Initialize()
        {
            Checkpoints = new List<Checkpoint>();
        }

        /// <summary>
        /// Resets all lists of all triggers
        /// </summary>
        public static void Reset()
        {
            Checkpoints.Clear();
            PlayerSpawn = null;

            //...and all the other lists of triggers, once they're implemented
        }

        /// <summary>
        /// Adds the given checkpoint to the list of all checkpoints in the current level, if its the first time its being added
        /// </summary>
        /// <param name="checkpoint">Checkpoint to be added</param>
        public static void AddUniqueCheckpoint(Checkpoint checkpoint)
        {
            //A checkpoint is designated unique if its position is distinct, since that cannot be altered during runtime
            foreach (Checkpoint c in Checkpoints)
            {
                if (checkpoint.Location == c.Location)
                    return;
            }
            Checkpoints.Add(checkpoint);
        }

        /// <summary>
        /// Sets the player's spawn to the given checkpoint
        /// </summary>
        /// <param name="spawn">Checkpoint that will be the player's spawn</param>
        public static void SetPlayerSpawn(Checkpoint spawn)
        {
            PlayerSpawn = spawn;
        }

        /// <summary>
        /// Ensure that all data in the checkpoints folder is valid
        /// </summary>
        /// <returns>If the data saved is valid</returns>
        private static bool ValidateCheckpointData()
        {
            string baseFolder = Checkpoint.BaseFolder;
            return File.Exists($"{baseFolder}/level.data") && GameplayManager.PlayerObject.ValidateData(baseFolder)
             && MapManager.ValidateSavedData(baseFolder);
        }

        /// <summary>
        /// Load data from a saved checkpoint file
        /// </summary>
        public static void LoadCheckpoint()
        {
            if (!ValidateCheckpointData())
                return;
            int level;
            int roomIndex;
            int selectedCheckpointIndex;
            string baseFolder = Checkpoint.BaseFolder;

            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead($"{baseFolder}/level.data")))
            {
                level = binaryReader.ReadInt32();
                roomIndex = binaryReader.ReadInt32();
                selectedCheckpointIndex = binaryReader.ReadInt32();
            }

            MapManager.LoadCheckpoint(baseFolder, roomIndex, level);
            GameplayManager.PlayerObject.Load(baseFolder);

            CurrentCheckpoint = Checkpoints[selectedCheckpointIndex];
            for (int i = 0; i < selectedCheckpointIndex; i++)
            {
                Checkpoints[i].Active = false;
            }

            GameplayManager.LoadLevelFromCheckpoint(level);
        }


        public static void UpdateCheckpoint(Checkpoint checkpoint)
        {
            for (int i = 0; i < Checkpoints.Count; i++)
            {
                if (checkpoint.Location == Checkpoints[i].Location)
                {
                    Checkpoints[i] = checkpoint;
                    return;
                }
            }
            //Shouldn't ever run
            throw new Exception("This checkpoint isn't in triggerArray");
        }
    }
}
