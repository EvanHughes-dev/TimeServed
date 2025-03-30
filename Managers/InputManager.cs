using MakeEveryDayRecount.Managers.Replay;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MakeEveryDayRecount.Managers
{
    //Enum for different mouse button states
    //JAMES NOTE
    //I think it needs a better name
    public enum MouseButtonState
    {
        Left,
        Right,
        Middle
    }

    /// <summary>
    /// Holds keyboard state for this frame and keyboard state for the prior frame.
    /// Methods allow checking if a key was pressed or released this frame, or what state a key is right now.
    /// </summary>
    public static class InputManager
    {
        private static KeyboardState _priorKeyboardState;
        private static KeyboardState _currentKeyboardState;
        private static MouseState _priorMouseState;
        private static MouseState _currentMouseState;

        private static ReplayState _priorReplayState;
        private static ReplayState _currentReplayState;

        /// <summary>
        /// Updates prior and current states.
        /// </summary>
        public static void Update()
        {
            //Set the Prior states to the current states
            _priorKeyboardState = _currentKeyboardState;
            _priorMouseState = _currentMouseState;

            //Get the current states
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
        }

        public static void ReplayUpdate()
        {
            _priorReplayState = _currentReplayState;
            _currentReplayState = ReplayManager.CurrentReplayState;
        }

        /// <summary>
        /// Checks if the key was pressed this frame.
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if CurrentState is down and PriorState is up, false otherwise</returns>
        public static bool GetKeyPress(Keys key)
        {
            if (ReplayManager.PlayingReplay)
            {
                if (_priorReplayState == null)
                    return _currentReplayState.IsKeyDown(key);
                return _currentReplayState.IsKeyDown(key) && _priorReplayState.IsKeyUp(key);
            }
            return _currentKeyboardState.IsKeyDown(key) && _priorKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the key was released this frame
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if CurrentState is up and PriorState is down, false otherwise</returns>
        public static bool GetKeyRelease(Keys key)
        {
            if (ReplayManager.PlayingReplay)
            {
                if (_priorReplayState == null)
                    return false;
                return _currentReplayState.IsKeyUp(key) && _priorReplayState.IsKeyDown(key);
            }
            return _currentKeyboardState.IsKeyUp(key) && _priorKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns the current state of the given key
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if the key is down, false otherwise</returns>
        public static bool GetKeyStatus(Keys key)
        {
            if (ReplayManager.PlayingReplay)
                return _currentReplayState.IsKeyDown(key);
            return _currentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns a boolean related to the current mouse state
        /// </summary>
        /// <param name="mouseState">Mouse state being checked</param>
        /// <returns>True if the given mouse state was started this frame, false otherwise</returns>
        public static bool GetMousePress(MouseButtonState mbs)
        {
            if (ReplayManager.PlayingReplay)
            {
                if (_priorReplayState == null)
                    return _currentReplayState.GetMouseState(mbs) == ButtonState.Pressed;
                return _currentReplayState.GetMouseState(mbs) != _priorReplayState.GetMouseState(mbs)
                       && _currentReplayState.GetMouseState(mbs) == ButtonState.Pressed;
            }
            switch (mbs)
            {
                case MouseButtonState.Left:
                    return _currentMouseState.LeftButton == ButtonState.Pressed &&
                        _priorMouseState.LeftButton == ButtonState.Released;
                case MouseButtonState.Right:
                    return _currentMouseState.RightButton == ButtonState.Pressed &&
                        _priorMouseState.RightButton == ButtonState.Released;
                case MouseButtonState.Middle:
                    return _currentMouseState.MiddleButton == ButtonState.Pressed &&
                        _priorMouseState.MiddleButton == ButtonState.Released;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a boolean related to the current mouse state
        /// </summary>
        /// <param name="mbs">Mouse state being checked</param>
        /// <returns>True if the given mouse state was ended this frame, false otherwise</returns>
        public static bool GetMouseRelease(MouseButtonState mbs)
        {
            if (ReplayManager.PlayingReplay)
            {
                if (_priorReplayState == null)
                    return false;
                return _currentReplayState.GetMouseState(mbs) != _priorReplayState.GetMouseState(mbs)
                       && _currentReplayState.GetMouseState(mbs) == ButtonState.Released;
            }
            switch (mbs)
            {
                case MouseButtonState.Left:
                    return _currentMouseState.LeftButton == ButtonState.Released &&
                        _priorMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtonState.Right:
                    return _currentMouseState.RightButton == ButtonState.Released &&
                        _priorMouseState.RightButton == ButtonState.Pressed;
                case MouseButtonState.Middle:
                    return _currentMouseState.MiddleButton == ButtonState.Released &&
                        _priorMouseState.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a boolean related to the current mouse state
        /// </summary>
        /// <param name="mbs">Mouse state being checked</param>
        /// <returns>True if the given mouse state is active, false otherwise</returns>
        public static bool GetMouseStatus(MouseButtonState mbs)
        {
            if (ReplayManager.PlayingReplay)
                return _currentReplayState.GetMouseState(mbs) == ButtonState.Pressed;
            switch (mbs)
            {
                case MouseButtonState.Left:
                    return _currentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtonState.Right:
                    return _currentMouseState.RightButton == ButtonState.Pressed;
                case MouseButtonState.Middle:
                    return _currentMouseState.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a point with the mouse's current X and Y positions
        /// </summary>
        /// <returns>A point with the mouse's current X and Y positions</returns>
        public static Point GetMousePosition()
        {
            if (ReplayManager.PlayingReplay)
                return _currentReplayState.CurrentMousePosition;
            return new Point(_currentMouseState.X, _currentMouseState.Y);
        }
    }
}
