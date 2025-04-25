using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.UI
{
    /// <summary>
    /// Create a text element to draw its string value when called
    /// </summary>
    internal class Text : InterfaceElement
    {
        private SpriteFont _sf;
        private string _text;
        private Vector2 _offset;

        /// <summary>
        /// Create a new text with a origin, color, string, and SpriteFont
        /// </summary>
        /// <param name="origin">Origin of the text</param>
        /// <param name="color">Color of the text</param>
        /// <param name="text">Text to display</param>
        /// <param name="sf">SpriteFont to draw the text with</param>
        public Text(Point origin, Color color, string text, SpriteFont sf) : base(origin, color)
        {
            _text = text;
            _sf = sf;
        }


        /// <summary>
        /// Create a new text with a origin, string, and SpriteFont. Color defaulted to white
        /// </summary>
        /// <param name="origin">Origin of the text</param>
        /// <param name="text">Text to display</param>
        /// <param name="sf">SpriteFont to draw the text with</param>
        /// <param name="useOffset"> If the offset should be used</param>
        public Text(Point origin, string text, SpriteFont sf, bool useOffset = false) : base(origin)
        {
            _text = text;
            _sf = sf;
            if (useOffset)
            {
                Vector2 offset = _sf.MeasureString(_text);
                _offset = new Vector2(offset.X, offset.Y / 4);
            }
            else
                _offset = Vector2.Zero;
        }

        /// <summary>
        /// Draw the text to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        public override void Draw(SpriteBatch sb)
        {

            sb.DrawString(_sf, _text, _origin.ToVector2() + _offset, _color);
        }

        /// <summary>
        /// Update the text for this element
        /// </summary>
        /// <param name="text">New text</param>
        public void UpdateText(string text)
        {
            _text = text;
        }
    }
}