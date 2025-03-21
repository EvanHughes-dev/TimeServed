using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.DebugModes
{
    /// <summary>
    /// Base for all other debug modes besides global debug. Create
    /// a format for all debug modes to work with and display information
    /// the same way
    /// </summary>
    internal abstract class BaseDebug
    {
        protected Dictionary<string, Func<object>> _objectsToDisplay;
        protected SpriteFont _spriteFont;
        protected GameplayManager _gameplayManager;

        private readonly int _yAxisIncrement;

        /// <summary>
        /// Initializes the basic debug system
        /// </summary>
        /// <param name="spriteFont">The font used for debug text</param>
        /// <param name="gameplayManager">Reference to the game's manager for accessing player data</param>
        public BaseDebug(GameplayManager gameplayManager)
        {
            _spriteFont = AssetManager.TimesNewRoman20;
            _gameplayManager = gameplayManager;
            _yAxisIncrement = _spriteFont.LineSpacing;
            _objectsToDisplay = new Dictionary<string, Func<object>> { };
        }

        /// <summary>
        /// Debug all text saved in _objectsToDisplay
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        /// <param name="debugMode">Mode of current debug to display</param>
        protected void Draw(SpriteBatch sb, string debugMode)
        {
            if (_spriteFont == null)
                return;

            Vector2 drawPoint = new Vector2(10, 10);

            sb.DrawString(_spriteFont, $"Debug Mode: {debugMode}", drawPoint, Color.Black);
            drawPoint.Y += _yAxisIncrement;
            foreach (KeyValuePair<string, Func<object>> entry in _objectsToDisplay)
            {
                string displayValue = $"{entry.Key}: {entry.Value()}";

                sb.DrawString(_spriteFont, displayValue, drawPoint, Color.Black);
                drawPoint.Y += _yAxisIncrement;
            }
        }

        /// <summary>
        /// Draw all information ralting to the specific debug mode
        /// </summary>
        /// <param name="sb">Sprite batch used for rendering.</param>
        public abstract void Draw(SpriteBatch sb);
    }
}
