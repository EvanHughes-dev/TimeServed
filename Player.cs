using System;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount
{
    internal class Player : GameObject
    {
        public enum PlayerState
        {
            Standing = 0,
            Walking = 1,
            Interacting
        }

        public enum Direction
        {
            Left = 0,
            Up = 1,
            Right = 2,
            Down = 3
        }

        public Point PlayerPos { get; private set; }
        public Point PlayerWorldPos { get; private set; }
        private Direction _playerCurrentDirection;

        private PlayerState _playerState;
        private readonly double _tilesPerSecond;
        private List<GameObject> _inventory;

        private Rectangle _sourceRectangle;
        private Texture2D _playerTextures;

        public Player(Point location, Texture2D sprite)
            : base(location, sprite) { }

        public void Update(float gameTime)
        {
            throw new NotImplementedException("Update has not been created yet in Player");
        }

        public void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException("Draw has not been created yet in Player");
        }

        private void KeyboardInput()
        {
            throw new NotImplementedException("KeyBoardInput has not been created yet in Player");
        }

        /// <summary>
        /// Search the player's inventory to see if they have a key
        /// of a certain type
        /// </summary>
        /// <param name="doorKeyType">Type of key to search for</param>
        /// <returns></returns>
        public bool ContainsKey(Door.DoorKeyType doorKeyType)
        {
            throw new NotImplementedException("ContainsKey has not been created yet in Player");
        }
    }
}
