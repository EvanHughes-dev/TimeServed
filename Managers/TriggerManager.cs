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
        public static List<Checkpoint> Checkpoints { get; }



        //TODO: Finish implementation
        //Reset Method called for each new level
        //Wheneber a trigger is loaded (presumably in map manager or room I think) add it to the list

        public static void Reset()
        {
            //reset all the fields
        }

        public static void AddCheckpoint(Checkpoint checkpoint)
        {
            Checkpoints.Add(checkpoint);
        }

    }
}
