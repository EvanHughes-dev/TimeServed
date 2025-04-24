using MakeEveryDayRecount.GameObjects.Triggers;
using System.Collections.Generic;
using System.IO;

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
            PlayerSpawn = null; //TODO: is this needed?
            //...and all the other lists of triggers, once they're implemented
        }

        /// <summary>
        /// Adds the given checkpoint to the list of all checkpoints in the current level
        /// </summary>
        /// <param name="checkpoint">Checkpoint to be added</param>
        public static void AddCheckpoint(Checkpoint checkpoint)
        {
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

        public static void LoadCheckpoint()
        {
            if (!ValidateCheckpointData())
                return;

        }

    }
}
