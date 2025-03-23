using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// Basic button with functionality for 
    /// </summary>
    class Button
    {
        //Fields
        private Texture2D _image;
        private Texture2D _hoverImage;
        private Rectangle _rectangle;
        private bool _active;

        public event Action OnClick;

        //Properties
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        public bool Hovered
        {
            get
            {
                MouseState ms = InputManager.CurrentMouseState;
                return (ms.X >= _rectangle.Left && ms.X <= _rectangle.Right &&
                ms.Y >= _rectangle.Top && ms.Y <= _rectangle.Bottom);
            }
        }
        /// <summary>
        /// Allows the button's image to be changed
        /// </summary>
        public Texture2D Image { get { return _image; } set { _image = value; } }
        /// <summary>
        /// Allows the button's rectangle to be changed
        /// </summary>
        public Rectangle Rectangle { get { return _rectangle; } set { _rectangle = value; } }
        /// <summary>
        /// Creates a button
        /// </summary>
        /// <param name="image">Image the button displays when not hovered over</param>
        /// <param name="hoverImage">Image the button displays when hovered over</param>
        /// <param name="rectangle">Rectangle corresponding to the position of the button</param>
        /// <param name="active">Boolean determining if the button is active (can be clicked) or not</param>
        public Button(Texture2D image, Texture2D hoverImage, Rectangle rectangle, bool active)
        {
            _image = image;
            _hoverImage = hoverImage;
            _rectangle = rectangle;
            _active = active;
        }

        //Methods

        /// <summary>
        /// Draws the button, with the image changing when hovered over
        /// </summary>
        /// <param name="sb">sprite batch used for drawing the button</param>
        public virtual void Draw(SpriteBatch sb)
        {
            if (Active && Hovered)
                sb.Draw(_hoverImage, _rectangle, Color.White);
            else
                sb.Draw(_image, _rectangle, Color.White);
        }

        /// <summary>
        /// If the button is hovered over and clicked, invokes its OnClick method
        /// </summary>
        public virtual void Update()
        {
            if (Hovered && InputManager.GetMousePress(MouseButtonState.Left))
                OnClick?.Invoke();
        }
    }
}
