
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.Map;

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

        /// <summary>
        /// Get the mouse's current position in tile space
        /// </summary>
        public Point CursorTilePosition { get; private set; }

        private MouseHover _mouseState;

        private Texture2D[] _mouseSprites;

        private readonly Point _mouseCursorSize;
        private readonly Point _mouseCursorOffset;

        private bool _uiHover;
        private Point _mouseScreenPos;

        /// <summary>
        /// Create a mouse object to display the mouse location
        /// </summary>
        public UserMouse(int mouseSize)
        {
            _mouseSprites = AssetManager.CursorStates;
            _mouseState = MouseHover.UnHovered;
            _mouseCursorSize = new Point(mouseSize, mouseSize);
            _mouseCursorOffset = new Point(mouseSize / 2, mouseSize / 2);
            _mouseScreenPos = Point.Zero;
            CursorTilePosition = Point.Zero;
            _uiHover = false;
        }

        /// <summary>
        /// Update the mouse's state and tile position
        /// </summary>
        public void Update()
        {
            _mouseScreenPos = InputManager.GetMousePosition() - _mouseCursorOffset;
            CursorTilePosition = MapUtils.ScreenToTile(_mouseScreenPos);

            // If the cursor is hovering over the ui, don't override that hover state
            if (_uiHover || MapManager.CurrentRoom == null)
                return;
            // If there isn't a item under the cursor then un-hover the mouse
            if (MapManager.CurrentRoom.VerifyInteractable(CursorTilePosition) == null)
                _mouseState = MouseHover.UnHovered;
            else
                _mouseState = MouseHover.Hovered;

        }

        /// <summary>
        /// Draw the mouse to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(_mouseSprites[(int)_mouseState], new Rectangle(_mouseScreenPos, _mouseCursorSize), Color.White);
        }

        /// <summary>
        /// Update the current hover state of the mouse
        /// </summary>
        /// <param name="newState">New hover state</param>
        public void UpdateHoverState(MouseHover newState)
        {
            _mouseState = newState;
            _uiHover = _mouseState == MouseHover.Hovered;
        }
    }

}