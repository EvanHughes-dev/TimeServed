using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount
{
    class Button
    {
        //Fields
        private Texture2D _image;
        private Texture2D _hoverImage;
        private Rectangle _rectangle;

        public event Action OnClick;

        //Constructor
        public Button(Texture2D image, Texture2D hoverImage, Rectangle rectangle)
        {
            _image = image;
            _hoverImage = hoverImage;
            _rectangle = rectangle;
        }

        //Methods

        /// <summary>
        /// Draws the button, with the image changing when hovered over
        /// </summary>
        /// <param name="sb">sprite batch used for drawing the button</param>
        public void Draw(SpriteBatch sb)
        {
            MouseState ms = Mouse.GetState();
            if (ms.X >= _rectangle.Left && ms.X <= _rectangle.Right &&
                ms.Y >= _rectangle.Top && ms.Y <= _rectangle.Bottom)
            {
                sb.Draw(_hoverImage, _rectangle, Color.White);
            }

            else
            {
                sb.Draw(_image, _rectangle, Color.White);
            }
        }

        public void Click()
        {
            OnClick?.Invoke();
        }
    }
}
