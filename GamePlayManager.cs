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

        private MapManager _map;

        public GameplayManager()
        {
            _map = new MapManager(this);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException("Update has not been created yet in GamePlayManager");
        }

        public void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException("Draw has not been created yet in GamePlayManager");
        }
    }
}
