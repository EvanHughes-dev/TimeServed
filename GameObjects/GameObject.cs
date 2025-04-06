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
        public int SpriteIndex { get; private set; }

        public GameObject(Point location, Texture2D[] spriteArray, int spriteIndex)
        {
            Location = location;
            if (spriteArray == null)
                Sprite = null;
            else
                Sprite = spriteArray[spriteIndex];
            SpriteIndex = spriteIndex;
        }

        //Additional constructor for the player
        public GameObject(Point location, Texture2D sprite)
        {
            Location = location;
            Sprite = sprite;
            SpriteIndex = -1;
        }
    }
}
