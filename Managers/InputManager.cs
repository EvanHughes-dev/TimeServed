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
    /// James Young
    /// Holds keyboard state for this frame and keyboard state for the prior frame.
    /// Methods allow checking if a key was pressed or released this frame, or what state a key is right now.
    /// </summary>
    public static class InputManager
    {
        private static KeyboardState PriorKeyboardState { get; set; }
        private static KeyboardState CurrentKeyboardState { get; set; }
        private static MouseState PriorMouseState { get; set; }
        public static MouseState CurrentMouseState { get; set; }

        /// <summary>
        /// Updates prior and current states.
        /// </summary>
        public static void Update()
        {
            //Set the Prior states to the current states
            PriorKeyboardState = CurrentKeyboardState;
            PriorMouseState = CurrentMouseState;

            //Get the current states
            CurrentKeyboardState = Keyboard.GetState();
            CurrentMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Checks if the key was pressed this frame.
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if CurrentState is down and PriorState is up, false otherwise</returns>
        public static bool GetKeyPress(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && PriorKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the key was released this frame
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if CurrentState is up and PriorState is down, false otherwise</returns>
        public static bool GetKeyRelease(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key) && PriorKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns the current state of the given key
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if the key is down, false otherwise</returns>
        public static bool GetKeyStatus(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns a boolean related to the current mouse state
        /// </summary>
        /// <param name="mouseState">Mouse state being checked</param>
        /// <returns>True if the given mouse state was started this frame, false otherwise</returns>
        public static bool GetMousePress(MouseButtonState mbs)
        {
            switch (mbs)
            {
                case MouseButtonState.Left:
                    return CurrentMouseState.LeftButton == ButtonState.Pressed &&
                        PriorMouseState.LeftButton == ButtonState.Released;
                case MouseButtonState.Right:
                    return CurrentMouseState.RightButton == ButtonState.Pressed &&
                        PriorMouseState.RightButton == ButtonState.Released;
                case MouseButtonState.Middle:
                    return CurrentMouseState.MiddleButton == ButtonState.Pressed &&
                        PriorMouseState.MiddleButton == ButtonState.Released;
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
            switch (mbs)
            {
                case MouseButtonState.Left:
                    return CurrentMouseState.LeftButton == ButtonState.Released &&
                        PriorMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtonState.Right:
                    return CurrentMouseState.RightButton == ButtonState.Released &&
                        PriorMouseState.RightButton == ButtonState.Pressed;
                case MouseButtonState.Middle:
                    return CurrentMouseState.MiddleButton == ButtonState.Released &&
                        PriorMouseState.MiddleButton == ButtonState.Pressed;
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
            switch (mbs)
            {
                case MouseButtonState.Left:
                    return CurrentMouseState.LeftButton == ButtonState.Pressed;
                case MouseButtonState.Right:
                    return CurrentMouseState.RightButton == ButtonState.Pressed;
                case MouseButtonState.Middle:
                    return CurrentMouseState.MiddleButton == ButtonState.Pressed;
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
            return new Point(CurrentMouseState.X, CurrentMouseState.Y);
        }
    }
}
