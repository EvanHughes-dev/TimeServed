using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    internal abstract class Trigger : GameObject
    {
        //Fields
        protected int _width;
        protected int _height;

        //Properties
        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        //Sprite should always be null, only included so it extends from gameobject
        protected Trigger(Point location, Texture2D sprite, int width, int height)
            : base(location, sprite) 
        {
            _width = width;
            _height = height;
        }

        public abstract void Activate(Player player);
    }
}
