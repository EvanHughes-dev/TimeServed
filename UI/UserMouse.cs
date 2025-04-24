
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.UI
{
    /// <summary>
    /// A mouse to draw over the user's mouse. Keep track of hover state
    /// to change the icon if the mouse is hovering something
    /// </summary>
    internal class UserMouse
    {
        /// <summary>
        /// The mouse hover state
        /// </summary>
        public enum MouseHover
        {
            UnHovered = 0,
            Hovered = 1
        }

        private MouseHover _mouseState;

        private Texture2D[] _mouseSprites;

        private readonly Point _mouseCursorSize;
        private readonly Point _mouseCursorOffset;

        /// <summary>
        /// Create a mouse object to display the mouse location
        /// </summary>
        public UserMouse(int mouseSize)
        {
            _mouseSprites = AssetManager.CursorStates;
            _mouseState = MouseHover.UnHovered;
            _mouseCursorSize = new Point(mouseSize, mouseSize);
            _mouseCursorOffset = new Point(mouseSize / 2, mouseSize / 2);
        }

        /// <summary>
        /// Draw the mouse to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_mouseSprites[(int)_mouseState], new Rectangle(InputManager.GetMousePosition() - _mouseCursorOffset, _mouseCursorSize), Color.White);
        }

        /// <summary>
        /// Update the current hover state of the mouse
        /// </summary>
        /// <param name="newState">New hover state</param>
        public void UpdateHoverState(MouseHover newState)
        {
            _mouseState = newState;
        }
    }

}