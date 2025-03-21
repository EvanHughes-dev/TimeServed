using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects
{
    internal abstract class GameObject
    {
        /// <summary>
        /// Current location of the object in tile space
        /// </summary>
        public Point Location { get; protected set; }
        public Texture2D Sprite { get; private set; }

        public GameObject(Point location, Texture2D sprite)
        {
            Location = location;
            Sprite = sprite;
        }
    }
}
