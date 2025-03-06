using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace MakeEveryDayRecount
{
    /// <summary>
    /// Manager of Player and the Map Manager.
    /// </summary>
    internal class GameplayManager
    {
        public int CurrentLevel { get; private set; }
        public Player PlayerObject { get; private set; }

        private MapManager _map;

        public GameplayManager(Player player, Texture2D[] tileMap)
        {
            PlayerObject = player;
            _map = new MapManager(tileMap);
        }

        /// <summary>
        /// Updates the player (and maybe additional stuff later)
        /// </summary>
        /// <param name="deltaTimeS">How much time has ellapsed since the last update</param>
        public void Update(float deltaTimeS)
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
