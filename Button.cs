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

        //JAMES NOTE
        //The methods above and below have two different ways of getting mouse position.
        //Bottom one is more verbose and goes through input manager.
        //I don't know which is better code.

        /// <summary>
        /// 
        /// </summary>
        public void Click()
        {
            //JAMES NOTE
            //Should I be checking position in this method or outside of it?
            
            Point mousePos = InputManager.GetMousePosition();
            //If the mouse is within the button and the left mouse button was clicked this frame
            if (mousePos.X >= _rectangle.Left && mousePos.X <= _rectangle.Right &&
                mousePos.Y >= _rectangle.Top && mousePos.Y <= _rectangle.Bottom &&
                InputManager.GetMousePress(MouseButtonState.Left))
                OnClick?.Invoke();
        }
    }
}
