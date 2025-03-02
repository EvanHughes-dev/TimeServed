using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace MakeEveryDayRecount
{
    internal class GameplayManager
    {
        private int _currentLevel;
        private Player _player;

        private MapManager _map;

        public GameplayManager(Player player, Texture2D[] tileMap)
        {
            _player = player;
            _map = new MapManager(tileMap);
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
