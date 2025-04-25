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

        private Text _displayText;

        public event Action OnClick;
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
                Point ms = InputManager.GetMousePosition();
                Rectangle displayRect = DisplayRect;
                return (ms.X >= displayRect.Left && ms.X <= displayRect.Right &&
                ms.Y >= displayRect.Top && ms.Y <= displayRect.Bottom);
            }
        }

        /// <summary>
        /// Get if this button is hovered this from
        /// </summary>
        protected bool ReplayHovered
        {
            get
            {
                Point ms = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                Rectangle displayRect = DisplayRect;
                return (ms.X >= displayRect.Left && ms.X <= displayRect.Right &&
                ms.Y >= displayRect.Top && ms.Y <= displayRect.Bottom);
            }
        }

        private bool _mousePressed;

        /// <summary>
        /// Creates a button with a rectangle, color, image, hover image and is hoverable state
        /// </summary>
        /// <param name="rectangle">Rectangle corresponding to the position of the button</param>
        /// <param name="color">Color of the button</param>
        /// <param name="image">Image the button displays when not hovered over</param>
        /// <param name="hoverImage">Image the button displays when hovered over</param>
        /// <param name="isHoverable">Boolean determining if the button is active (can be hovered) or not</param>
        /// <param name="textElement">Text to display on button</param>
        public Button(Rectangle rectangle, Texture2D image, Color color, Texture2D hoverImage, bool isHoverable, Text textElement) : base(rectangle, color)
        {
            _image = image;
            _hoverImage = hoverImage;
            _isInteractive = isHoverable;
            _isHovered = false;
            _displayText = textElement;
            _mousePressed = false;
        }

        /// <summary>
        /// Creates a button with a rectangle, color, image, hover image and is hoverable state without a text field
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
            _displayText = null;
            _mousePressed = false;
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
            _displayText = null;
            _mousePressed = false;
        }

        /// <summary>
        /// Creates a button with a rectangle, image, hover image and is hoverable state. Color defaulted to white
        /// </summary> 
        /// <param name="rectangle">Rectangle corresponding to the position of the button</param>
        /// <param name="image">Image the button displays when not hovered over</param>
        /// <param name="hoverImage">Image the button displays when hovered over</param>
        /// <param name="isHoverable">Boolean determining if the button is active (can be hovered) or not</param>
        public Button(Rectangle rectangle, Texture2D image, Texture2D hoverImage, bool isHoverable, string text, SpriteFont sf) : base(rectangle)
        {
            _image = image;
            _hoverImage = hoverImage;
            _isInteractive = isHoverable;
            _isHovered = false;
            _displayText = null;
            _mousePressed = false;

            Vector2 textPoint = rectangle.Location.ToVector2() + ((rectangle.Size.ToVector2() - sf.MeasureString(text)) / 2);
            _displayText = new Text(textPoint.ToPoint(), text, sf);
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

            _displayText?.Draw(sb);
        }

        /// <summary>
        /// If the button is hovered over and clicked, invokes its OnClick method
        /// </summary>
        public virtual void Update()
        {
            if (Hovered && Active)
            {
                _isHovered = true;
                InterfaceManager.HoverModeChange(UserMouse.MouseHover.Hovered);
            }
            else if (!Hovered && _isHovered)
            {
                _isHovered = false;
                InterfaceManager.HoverModeChange(UserMouse.MouseHover.UnHovered);
            }

            if (_isHovered && InputManager.GetMousePress(MouseButtonState.Left))
            {
                InterfaceManager.HoverModeChange(UserMouse.MouseHover.UnHovered);
                OnClick?.Invoke();
            }
        }

        /// <summary>
        /// Called during replay to get the actual mouse position
        /// </summary>
        public void ReplayUpdate()
        {
            if (ReplayHovered && Active)
            {
                _isHovered = true;
            }
            else if (!ReplayHovered && _isHovered)
            {
                _isHovered = false;
            }

            if (_isHovered && Mouse.GetState().LeftButton == ButtonState.Pressed && !_mousePressed)
            {
                OnClick?.Invoke();
            }

            _mousePressed = Mouse.GetState().LeftButton == ButtonState.Pressed;
        }
    }
}
