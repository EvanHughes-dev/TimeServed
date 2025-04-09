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
        private bool active;

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
            active = true;
        }


        //Methods

        //TODO: update the SaveRoom method in SaveMap when doors are fixed
        //TODO: Maybe add a method to display that progress is being saved in some way? Low priority
        //TODO: when a checkpoint gets activated, call one of replay manager's functions (ask evan for more details when merging main)

        public override void Activate(Player player)
        {
            MapManager.SaveMap();
            player.Save();
        }
    }
}
