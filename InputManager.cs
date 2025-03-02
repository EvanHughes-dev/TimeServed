using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// James Young
    /// Holds keyboard state for this frame and keyboard state for the prior frame.
    /// Methods allow checking if a key was pressed or released this frame, or what state a key is right now.
    /// </summary>
    public static class InputManager
    {        
        private static KeyboardState PriorState { get; set; }
        private static KeyboardState CurrentState { get; set; }

        /// <summary>
        /// Updates prior and current state.
        /// </summary>
        public static void Update()
        {
            //Set the Prior state to the current state
            PriorState = CurrentState;

            //Get the current Keyboard state
            CurrentState = Keyboard.GetState();
        }

        /// <summary>
        /// Checks if the key was pressed this frame.
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if CurrentState is down and PriorState is up, false otherwise</returns>
        public static bool GetKeyPress(Keys key)
        {
            return CurrentState.IsKeyDown(key) && PriorState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the key was released this frame
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if CurrentState is up and PriorState is down, false otherwise</returns>
        public static bool GetKeyRelease(Keys key)
        {
            return CurrentState.IsKeyUp(key) && PriorState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns the current state of the given key
        /// </summary>
        /// <param name="key">Key being checked</param>
        /// <returns>True if the key is down, false otherwise</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool GetKeyStatus(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }
    }
}
