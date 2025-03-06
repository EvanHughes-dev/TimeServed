using System;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// Manager of Player and the Map Manager.
    /// </summary>
    internal class GameplayManager
    {
        /// <summary>
        /// Access the player's current level
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Access the reference to the Player
        /// </summary>
        public Player PlayerObject { get; private set; }

        /// <summary>
        /// Get the size of the screen
        /// </summary>
        public Vector2 ScreenSize { get; private set; }
        private MapManager _map;

        /// <summary>
        /// Initialize GameplayManager
        /// </summary>
        /// <param name="screenSize">Size of the screen</param>
        public GameplayManager(Vector2 screenSize)
        {
            Level = 1;
            PlayerObject = new Player(Point.Zero, AssetManager.PlayerTexture);
            _map = new MapManager(this);
            ScreenSize = screenSize;
        }

        public void Update(GameTime gameTime)
        {
            //Update Player
            PlayerObject.Update(deltaTimeS);
        }

        /// <summary>
        /// Draws the map and the player.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Draw(SpriteBatch sb)
        {
            //Draw the map
            _map.Draw(sb);

            //Draw the player
            PlayerObject.Draw(sb);
        }
    }
}
