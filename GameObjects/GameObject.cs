using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects
{
    internal abstract class GameObject
    {
        public Point Location { get; private set; }
        public Texture2D Sprite { get; private set; }

        public GameObject(Point location, Texture2D sprite)
        {
            Location = location;
            Sprite = sprite;
        }
    }
}
