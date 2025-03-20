using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.DebugModes
{
    /// <summary>
    /// Provides a centralized debugging system for the game.
    /// This class allows any other class to add values to the debug display,
    /// making it easier to monitor game variables in real-time.
    ///
    /// More specialized debug features can be built on top of this class,
    /// but this serves as a general-purpose debugging utility.
    ///
    /// <para><b>Usage:</b></para>
    /// To add a value to the debug display, call <see cref="AddObject"/>
    /// and provide a label along with a lambda function that returns the variable's value.
    ///
    /// <example>
    /// <code>
    /// DebugMode.AddObject("Player Tile", () => Location);
    /// </code>
    /// </example>
    /// </summary>
    internal static class GlobalDebug
    {
        private static Dictionary<string, Func<object>> _objectsToDisplay;

        private static SpriteFont _spriteFont;
        private static int _yAxisIncrement;

        /// <summary>
        /// Initializes the debug mode system.
        /// This must be called before adding objects or attempting to draw.
        /// </summary>
        public static void Initialize()
        {
            _objectsToDisplay = new Dictionary<string, Func<object>>();
            _spriteFont = AssetManager.TimesNewRoman20;
            _yAxisIncrement = _spriteFont.LineSpacing;
        }

        /// <summary>
        /// Renders the stored debug values to the screen.
        /// The debug information is displayed in a column on the left side of the screen.
        ///
        /// <para>
        /// This method must be called within the game's <c>Draw</c> method.
        /// Ensure that <see cref="SetFont"/> has been called before calling this method.
        /// </para>
        /// </summary>
        /// <param name="sb">The <see cref="SpriteBatch"/> used to draw the text.</param>
        public static void Draw(SpriteBatch sb)
        {
            if (_spriteFont == null)
                return;

            Vector2 drawPoint = new Vector2(10, 10);
            sb.DrawString(_spriteFont, "Debug Mode: Global", drawPoint, Color.Black);
            drawPoint.Y += _yAxisIncrement;
            foreach (KeyValuePair<string, Func<object>> entry in _objectsToDisplay)
            {
                string displayValue = $"{entry.Key}: {entry.Value()}";

                sb.DrawString(_spriteFont, displayValue, drawPoint, Color.Black);
                drawPoint.Y += _yAxisIncrement;
            }
        }

        /// <summary>
        /// Adds an object to the debug display.
        /// The value will automatically update each frame without needing to be re-added.
        /// </summary>
        /// <param name="displayName">The label used to display the value.</param>
        /// <param name="displayObject">
        /// A function returning the current value of the variable.
        /// This allows the debug system to update dynamically.
        /// </param>

        public static void AddObject(string displayName, Func<object> displayObject)
        {
            _objectsToDisplay[displayName] = displayObject;
        }

        /// <summary>
        /// Removes an object from the debug display.
        /// </summary>
        /// <param name="displayName">The label of the object to remove.</param>
        /// <returns>
        /// <c>true</c> if the object was successfully removed;
        /// <c>false</c> if the object was not found in the debug display.
        /// </returns>
        public static bool RemoveObject(string displayName)
        {
            return _objectsToDisplay.Remove(displayName);
        }
    }
}
