using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.UI
{
    /// <summary>
    /// An image to display on the menu. No interaction needed, only a visual
    /// </summary>
    internal class Image : InterfaceElement
    {
        private Texture2D _sprite;

        /// <summary>
        /// Initialize an image to be displayed at a point with a texture
        /// </summary>
        /// <param name="origin">Origin of the image</param>
        /// <param name="size">Size of the image</param>
        /// <param name="color">Color to draw the image in</param>
        /// <param name="texture">Texture of the image</param>
        public Image(Point origin, Point size, Color color, Texture2D texture) : base(origin, size, color)
        {

            _sprite = texture;
        }

        /// <summary>
        /// Initialize an image to be displayed at a point with a texture
        /// </summary>
        /// <param name="origin">Origin of the image</param>
        /// <param name="size">Size of the image</param>
        /// <param name="texture">Texture of the image</param>
        public Image(Point origin, Point size, Texture2D texture) : base(origin, size)
        {

            _sprite = texture;
        }

        /// <summary>
        /// Draw this image to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(_sprite, new Rectangle(_origin, _size), Color.White);
        }
    }
}