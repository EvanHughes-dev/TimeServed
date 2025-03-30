using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.UI
{

    /// <summary>
    /// Basic button with functionality for menus and to be extend for inventory
    /// </summary>
    class Button : InterfaceElement
    {
        //Fields
        private Texture2D _image;
        private Texture2D _hoverImage;
        private bool _isHovered;
        protected bool _isInteractive;

        public event Action OnClick;
        public event HoverChange OnHoverChange;
        //Properties

        /// <summary>
        /// Get if this button is active
        /// </summary>
        public bool Active
        {
            get { return _isInteractive; }
            set { _isInteractive = value; }
        }

        /// <summary>
        /// Get if this button is hovered this from
        /// </summary>
        protected bool Hovered
        {
            get
            {
                MouseState ms = InputManager.CurrentMouseState;
                Rectangle displayRect = DisplayRect;
                return (ms.X >= displayRect.Left && ms.X <= displayRect.Right &&
                ms.Y >= displayRect.Top && ms.Y <= displayRect.Bottom);
            }
        }

        /// <summary>
        /// Creates a button with a rectangle, color, image, hover image and is hoverable state
        /// </summary>
        /// <param name="rectangle">Rectangle corresponding to the position of the button</param>
        /// <param name="color">Color of the button</param>
        /// <param name="image">Image the button displays when not hovered over</param>
        /// <param name="hoverImage">Image the button displays when hovered over</param>
        /// <param name="isHoverable">Boolean determining if the button is active (can be hovered) or not</param>
        public Button(Rectangle rectangle, Texture2D image, Color color, Texture2D hoverImage, bool isHoverable) : base(rectangle, color)
        {
            _image = image;
            _hoverImage = hoverImage;
            _isInteractive = isHoverable;
            _isHovered = false;

        }

        /// <summary>
        /// Creates a button with a rectangle, image, hover image and is hoverable state. Color defaulted to white
        /// </summary> 
        /// <param name="rectangle">Rectangle corresponding to the position of the button</param>
        /// <param name="image">Image the button displays when not hovered over</param>
        /// <param name="hoverImage">Image the button displays when hovered over</param>
        /// <param name="isHoverable">Boolean determining if the button is active (can be hovered) or not</param>
        public Button(Rectangle rectangle, Texture2D image, Texture2D hoverImage, bool isHoverable) : base(rectangle)
        {
            _image = image;
            _hoverImage = hoverImage;
            _isInteractive = isHoverable;
            _isHovered = false;
        }

        //Methods

        /// <summary>
        /// Draws the button, with the image changing when hovered over
        /// </summary>
        /// <param name="sb">sprite batch used for drawing the button</param>
        public override void Draw(SpriteBatch sb)
        {
            if (_isHovered)
                sb.Draw(_hoverImage, DisplayRect, Color.White);
            else
                sb.Draw(_image, DisplayRect, Color.White);
        }

        /// <summary>
        /// If the button is hovered over and clicked, invokes its OnClick method
        /// </summary>
        public virtual void Update()
        {
            if (Hovered && Active && !_isHovered)
            {
                _isHovered = true;
                OnHoverChange?.Invoke(UserMouse.MouseHover.Hovered);
            }
            else if (!Hovered && _isHovered)
            {
                _isHovered = false;
                OnHoverChange?.Invoke(UserMouse.MouseHover.UnHovered);
            }

            if (_isHovered && InputManager.GetMousePress(MouseButtonState.Left))
            {
                OnHoverChange?.Invoke(UserMouse.MouseHover.UnHovered);
                OnClick?.Invoke();
            }


        }
    }
}
