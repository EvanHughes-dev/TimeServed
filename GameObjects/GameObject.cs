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
        public Texture2D Sprite { get; protected set; }
        /// <summary>
        /// The index of this object's sprite in its spriteArray
        /// </summary>
        public int SpriteIndex { get; protected set; }

        /// <summary>
        /// Create a new GameObject at a location and with an image
        /// </summary>
        /// <param name="location">Object's location</param>
        /// <param name="spriteArray">Array of sprites containing this GameObject's sprite</param>
        /// <param name="spriteIndex">Index of this GameObject's sprite in its spriteArray</param>
        public GameObject(Point location, Texture2D[] spriteArray, int spriteIndex)
        {
            Location = location;
            Sprite = spriteArray[spriteIndex];
            SpriteIndex = spriteIndex;
        }

        /// <summary>
        /// Create a new GameObject at a location and with an image, without needing a spriteIndex
        /// </summary>
        /// <param name="location">Object's location</param>
        /// <param name="sprite">Image of the object</param>
        public GameObject(Point location, Texture2D sprite)
        {
            Location = location;
            Sprite = sprite;
            SpriteIndex = -1;
        }
    }
}
