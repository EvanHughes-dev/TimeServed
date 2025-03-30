using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    internal class Checkpoint : Trigger
    {
        //Fields
        private int _index;

        //Properties
        

        public Checkpoint(Point location, Texture2D sprite, int index, int width, int height)
            : base(location, sprite, width, height)
        {
            _height = height;
        }


        //Methods

        //TODO: Add a method that saves the player's progress when they hit the checkpoint
        //TODO: Maybe add a method to display that progress is being saved in some way? Low priority

        public override void Activate(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
