using System;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects;
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
        private Direction _playerCurrentDirection;

        private PlayerState _playerState;
        private readonly double _tilesPerSecond;
        private List<GameObject> _inventory;

        private Rectangle _sourceRectangle;
        private Texture2D _playerTextures;

        public Player(Point location, Texture2D sprite)
            : base(location, sprite) { }

        public override void Update(float gameTime)
        {
            throw new NotImplementedException("Update has not been created yet in Player");
        }

        private void KeyboardInput()
        {
            throw new NotImplementedException("KeyBoardInput has not been created yet in Player");
        }
    }
}
