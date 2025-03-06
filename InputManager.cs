using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount
{
    //Write an enum for mosue states
    
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
        private static MouseState CurrentMouseState { get; set; }

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
        /// <exception cref="NotImplementedException"></exception>
        public static bool GetKeyStatus(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        public static bool GetMousePress(Keys key)
        {
            return CurrentMouseState.mouse
        }
    }
}
