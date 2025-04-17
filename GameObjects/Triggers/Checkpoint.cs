using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;
using System.IO;
using System.Runtime.CompilerServices;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    internal class Checkpoint : Trigger
    {
        //Fields
        private int _index;
        private bool _active;
        private int _roomIndex;

        //Properties
        public int Index
        {
            get { return _index; }
        }
        public bool Active
        {
            get { return _active; }
        }
        public int RoomIndex
        {
            get { return _roomIndex; }
        }

        public Checkpoint(Point location, Texture2D sprite, int index, int width, int height)
            : base(location, sprite, width, height)
        {
            _active = true;
            _index = index;
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
            if (Index != 1)
                if (!TriggerManager.Checkpoints[Index - 1].Active)
                    return;

            if (_active)
            {
                string baseFolder = "./CheckpointData";

                //Always want to overwrite the folder if it exists
                if (Directory.Exists(baseFolder))
                    RecursiveDelete(baseFolder);

                //(Re)make the folder
                Directory.CreateDirectory(baseFolder);

                //Save map and player data
                MapManager.SaveMap(baseFolder);
                player.Save(baseFolder);

                //Save the room this checkpoint is located in
                _roomIndex = MapManager.CurrentRoom.RoomIndex;

                //Update the current checkpoint
                TriggerManager.CurrentCheckpoint = this;

                //Call the replay manager function
                ReplayManager.SaveData(GameplayManager.Level, _index);

                //Deactivate the checkpoint after it has saved the data
                _active = false;
            }
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
