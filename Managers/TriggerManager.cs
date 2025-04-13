using MakeEveryDayRecount.GameObjects.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount.Managers
{
    internal static class TriggerManager
    {
        public static List<Checkpoint> Checkpoints { get; private set; }

        //TODO: This probably shouldn't be fully public, ask for feedback
        public static Checkpoint CurrentCheckpoint { get; set; }

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

    }
}
