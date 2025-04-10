using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;
using System.IO;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    internal class Checkpoint : Trigger
    {
        //Fields
        private int _index;
        private bool _active;

        //Properties
        public int Index
        {
            get { return _index; }
        }

        //TODO:
        //Make it so checkpoints can only trigger in sequence (checkpoint 3 can't happen unless checkpoint 2 has tripped)

        public Checkpoint(Point location, Texture2D sprite, int index, int width, int height)
            : base(location, sprite, width, height)
        {
            _active = true;
        }


        //Methods

        //TODO: Maybe add a method to display that progress is being saved in some way? Low priority

        public override void Activate(Player player)
        {
            if (_active)
            {
                string baseFolder = "./CheckpointData";

                //Always want to overwrite the folder if it exists
                if (Directory.Exists(baseFolder))
                    Directory.Delete(baseFolder);

                //(Re)make the folder
                Directory.CreateDirectory(baseFolder);

                //Save map and player data
                MapManager.SaveMap(baseFolder);
                player.Save(baseFolder);

                //Call the replay manager function
                ReplayManager.SaveData(GameplayManager.Level, _index);

                //Deactivate the checkpoint
                _active = false;
            }
        }
    }
}
