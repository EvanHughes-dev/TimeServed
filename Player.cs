using System;
using System.Collections.Generic;
using MakeEveryDayRecount.Debug;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        public Point PlayerScreenPosition { get; private set; }

        private Direction _playerCurrentDirection;
        private PlayerState _playerState;

        /// <summary>
        /// Get the player's current state
        /// </summary>
        public PlayerState CurrentPlayerState
        {
            get { return _playerState; }
        }

        /// <summary>
        /// Get the direction the player is facing
        /// </summary>
        public Direction PlayerCurrentDirection
        {
            get { return _playerCurrentDirection; }
        }

        private readonly float _secondsPerTile = .2f;
        private float _walkingSeconds;
        private bool _readyToMove;

        //A reference to the gameplay manager which has a reference
        //to the map which lets the player know what's near them
        private readonly GameplayManager _gameplayManager;

        private List<GameObject> _inventory;

        private Rectangle _sourceRectangle;
        private Texture2D _playerTextures; //Probably a sprite sheet

        public Player(Point location, Texture2D sprite, GameplayManager gameplayManager)
            : base(location, sprite)
        {
            //NOTE: For now, the player's screen position is always in the middle
            _walkingSeconds = 0;
            _gameplayManager = gameplayManager;
            _readyToMove = true;
        }

        /// <summary>
        /// Updates the player's position in world space
        /// </summary>
        /// <param name="deltaTimeS">The elapsed time between frames in seconds</param>
        public void Update(float deltaTimeS)
        {
            KeyboardInput(deltaTimeS);
            UpdatePlayerPos();
        }

        /// <summary>
        /// Draws the player in the center of the screen
        /// </summary>
        /// <param name="sb">The instance of spritebatch to be used to draw the player</param>
        public void Draw(SpriteBatch sb)
        {
            //REMEMBER THEY ONLY DRAW AT THE CENTER TILE
            //TODO add the ability for the player to walk up to but into through the walls
            sb.Draw(
                Sprite,
                new Rectangle(PlayerScreenPosition, AssetManager.TileSize),
                Color.White
            );
        }

        /// <summary>
        /// Gets keyboard input for player movement and moves the player in world space
        /// </summary>
        /// <param name="deltaTime">The elapsed time between frames in seconds</param>
        private void KeyboardInput(float deltaTime)
        {
            if (InputManager.GetKeyStatus(Keys.Left) || InputManager.GetKeyStatus(Keys.A))
            {
                PlayerMovement(deltaTime, new Point(-1, 0), Direction.Left);
            }
            else if (InputManager.GetKeyStatus(Keys.Right) || InputManager.GetKeyStatus(Keys.D))
            {
                PlayerMovement(deltaTime, new Point(1, 0), Direction.Right);
            }
            else if (InputManager.GetKeyStatus(Keys.Up) || InputManager.GetKeyStatus(Keys.W))
            {
                PlayerMovement(deltaTime, new Point(0, -1), Direction.Up);
            }
            else if (InputManager.GetKeyStatus(Keys.Down) || InputManager.GetKeyStatus(Keys.S))
            {
                PlayerMovement(deltaTime, new Point(0, 1), Direction.Down);
            }
            //if we were walking and we stop pressing a key, go back to standing
            else
            {
                _playerState = PlayerState.Standing;
                if (!_readyToMove)
                    UpdateWalkingTime(deltaTime);
                //but don't change the direction you're facing
            }
        }

        /// <summary>
        /// Move the player in the direction they need to so long
        /// as there isn't an issue with collision
        /// </summary>
        /// <param name="deltaTime">Time since last frame</param>
        /// <param name="movement">Vector to move</param>
        /// <param name="directionMove">Direction of movement</param>
        private void PlayerMovement(float deltaTime, Point movement, Direction directionMove)
        {
            if (!_readyToMove)
                UpdateWalkingTime(deltaTime);
            if (_readyToMove && _gameplayManager.Map.CheckPlayerCollision(Location + movement))
            {
                Location += movement;
                _readyToMove = false;
            }

            // Update the player's walking state if needed
            if (_playerCurrentDirection != directionMove)
                _playerCurrentDirection = directionMove;

            if (_playerState == PlayerState.Standing)
                _playerState = PlayerState.Walking;
        }

        /// <summary>
        /// Update the time value in between each movements
        /// </summary>
        /// <param name="deltaTime">Time that has elapsed since last frame</param>
        private void UpdateWalkingTime(float deltaTime)
        {
            _walkingSeconds += deltaTime;
            if (_walkingSeconds >= _secondsPerTile)
            {
                _readyToMove = true;
                _walkingSeconds = 0;
            }
        }

        /// <summary>
        /// Convert from the player's tile position to world position
        /// </summary>
        private void UpdatePlayerPos()
        {
            Point playerWorldPos = MapUtils.TileToWorld(Location);
            Point worldToScreen = MapUtils.WorldToScreen();

            PlayerScreenPosition = playerWorldPos - worldToScreen;
        }

        public bool ContainsKey(Door.DoorKeyType keyType)
        {
            throw new NotImplementedException();
        }
    }
}
