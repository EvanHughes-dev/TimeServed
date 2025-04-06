using MakeEveryDayRecount.Managers;
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
        /// <summary>
        /// The current sprite of the GameObject
        /// </summary>
        public Texture2D Sprite { get; private set; }

        /// <summary>
        /// Create a new GameObject at a location and with an image
        /// </summary>
        /// <param name="location">Location of the object</param>
        /// <param name="sprite">Image of the object</param>
        public GameObject(Point location, Texture2D sprite)
        {
            Location = location;
            Sprite = sprite;
        }
    }
}
