using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;
using System.IO;
using System.Runtime.CompilerServices;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    /// <summary>
    /// Trigger that saves the player's progress when they trigger it
    /// </summary>
    internal class Checkpoint : Trigger
    {
        //Properties

        /// <summary>
        /// Index of this checkpoint
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Whether or not this checkpoint can be activated
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        /// Index of the room this checkpoint is housed in
        /// </summary>
        public int RoomIndex { get; private set; }

        /// <summary>
        /// Constructs a checkpoint
        /// </summary>
        /// <param name="location">Location of the top left-most point of the checkpoint in its room</param>
        /// <param name="index">Index of the checkpoint</param>
        /// <param name="width">Width of this checkpoint</param>
        /// <param name="height">Height of this checkpoint</param>
        /// <param name="active">Whether or not this checkpoint can be activated</param>
        public Checkpoint(Point location, int index, int width, int height, bool active)
            : base(location, width, height)
        {
            Index = index;
            Active = active;
        }


        //Methods

        //TODO: Maybe add a method to display that progress is being saved in some way? Low priority

        /// <summary>
        /// Saves the map and player data
        /// </summary>
        /// <param name="player">The player object</param>
        public override void Activate(Player player)
        {
            //Makes sure the prior checkpoint has been activatated
            //unless this is the first checkpoint, in which case it's always good to activate
            if (Index != 0)
                if (!TriggerManager.Checkpoints[Index - 1].Active)
                    return;
            if (!Active)
                return;
            
            string baseFolder = "./CheckpointData";

            //Deactivate the checkpoint, need to before it has saved the data
            Active = false;

            //Always want to overwrite the folder if it exists
            if (Directory.Exists(baseFolder))
                RecursiveDelete(baseFolder);

            //(Re)make the folder
            Directory.CreateDirectory(baseFolder);

            //Save map and player data
            MapManager.SaveMap(baseFolder);
            player.Save(baseFolder);

            //Save the room this checkpoint is located in
            RoomIndex = MapManager.CurrentRoom.RoomIndex;

            //Update the current checkpoint
            TriggerManager.CurrentCheckpoint = this;

            //Call the replay manager function
            ReplayManager.SaveData(GameplayManager.Level, Index);
        }

        /// <summary>
        /// Recursively delete all contents of a folder
        /// </summary>
        /// <param name="folderPath">Folder to delete</param>
        private void RecursiveDelete(string folderPath)
        {

            foreach (string file in Directory.GetFiles(folderPath))
                File.Delete(file);

            foreach(string folder in Directory.GetDirectories(folderPath))
            {
                RecursiveDelete(folder);
            }

            Directory.Delete(folderPath);
        }
    }
}
