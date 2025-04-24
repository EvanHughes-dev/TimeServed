using MakeEveryDayRecount.GameObjects.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// The win trigger of the current level
        /// </summary>
        public static Win WinTrigger { get; private set; }

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
            WinTrigger = null;

            //...and all the other lists of triggers, once they're implemented
        }

        /// <summary>
        /// Adds the given checkpoint to the list of all checkpoints in the current level, if its the first time its being added
        /// </summary>
        /// <param name="checkpoint">Checkpoint to be added</param>
        public static void AddUniqueCheckpoint(Checkpoint checkpoint)
        {
            if (!Checkpoints.Contains(checkpoint))
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
        /// JTODO: comment code
        /// </summary>
        /// <param name="win"></param>
        public static void SetWinTrigger(Win win)
        {
            WinTrigger = win;
        }
    }
}
