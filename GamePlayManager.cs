using System;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// Called when the player object is updated in memory
    /// </summary>
    /// <param name="player">New player object</param>
    delegate void OnPlayerUpdate(Player player);

    /// <summary>
    /// Manager of Player and the Map Manager.
    /// </summary>
    internal class GameplayManager
    {
        /// <summary>
        /// The current level being played
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Access the reference to the Player
        /// </summary>
        public Player PlayerObject { get; private set; }

        /// <summary>
        /// Access the current MapManager
        /// </summary>
        public MapManager Map { get; private set; }

        public OnPlayerUpdate OnPlayerUpdate;

        /// <summary>
        /// Initialize GameplayManager
        /// </summary>
        /// <param name="screenSize">Size of the screen</param>
        public GameplayManager()
        {
            Level = 1;
            PlayerObject = new Player(new Point(3, 3), AssetManager.PlayerTexture, this);
            Map = new MapManager(this);
            OnPlayerUpdate?.Invoke(PlayerObject);
        }

        public void Update(GameTime gameTime)
        {
            //Update Player
            PlayerObject.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            //Check for pause
            
        }

        /// <summary>
        /// Draws the map and the player.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Draw(SpriteBatch sb)
        {
            //Draw the map
            Map.Draw(sb);

            //Draw the player
            PlayerObject.Draw(sb);
        }
    }
}
