using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount
{
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
            //Check for input

            throw new NotImplementedException("Update has not been created yet in GamePlayManager");
        }

        public void Draw(SpriteBatch sb)
        {
            _map.Draw(sb);
        }
    }
}
