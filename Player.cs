using System;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MakeEveryDayRecount.Map;

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

        private readonly float _secondsPerTile = .2f;
        private float _walkingSeconds;
        private bool _readyToMove;

        private List<GameObject> _inventory;

        private Rectangle _sourceRectangle;
        private Texture2D _playerTextures; //Probably a sprite sheet

        private GameplayManager _gamePlayManager;

        public Player(Point location, Texture2D sprite, GameplayManager gameplayManager)
            : base(location, sprite)
        {
            //NOTE: For now, the player's screen position is always in the middle
            _walkingSeconds = 0;
            _gamePlayManager = gameplayManager;
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
                new Rectangle(
                    PlayerScreenPosition,
                    AssetManager.TileSize
                ),
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
                     UpdateWalkingTime(deltaTime);
                    if (_readyToMove)
                    {
                        Location += new Point(-1, 0);
                        _readyToMove = false;
                    }

                // Update the player's walking state if needed
                if (_playerCurrentDirection!=Direction.Left)
                        _playerCurrentDirection = Direction.Left;
                    if (_playerState == PlayerState.Standing)
                        _playerState = PlayerState.Walking;

                }
                else if (InputManager.GetKeyStatus(Keys.Right) || InputManager.GetKeyStatus(Keys.D))
                {
                    UpdateWalkingTime(deltaTime);
                    if (_readyToMove)
                    {
                        Location += new Point(1, 0);
                        _readyToMove = false;
                    }

                // Update the player's walking state if needed
                if (_playerCurrentDirection != Direction.Right)
                        _playerCurrentDirection = Direction.Right;
                    if (_playerState == PlayerState.Standing)
                        _playerState = PlayerState.Walking;
                }
                else if (InputManager.GetKeyStatus(Keys.Up) || InputManager.GetKeyStatus(Keys.W))
                {
                    UpdateWalkingTime(deltaTime);
                    if (_readyToMove)
                    {
                        Location += new Point(0, -1);
                        _readyToMove = false;
                    }

                // Update the player's walking state if needed
                if (_playerCurrentDirection != Direction.Up)
                        _playerCurrentDirection = Direction.Up;
                    if (_playerState == PlayerState.Standing)
                        _playerState = PlayerState.Walking;
                }
                else if (InputManager.GetKeyStatus(Keys.Down) || InputManager.GetKeyStatus(Keys.S))
                {
                    UpdateWalkingTime(deltaTime);
                    if (_readyToMove)
                    {
                        Location += new Point(0, 1);
                        _readyToMove = false;
                    }

                    // Update the player's walking state if needed
                    if (_playerCurrentDirection != Direction.Down)
                        _playerCurrentDirection = Direction.Down;
                    if (_playerState == PlayerState.Standing)
                        _playerState = PlayerState.Walking;
                }
                //if we were walking and we stop pressing a key, go back to standing
                else
                {
                    _playerState = PlayerState.Standing;
                    _walkingSeconds = 0;
                    _readyToMove = true;
                    //but don't change the direction you're facing
                }
                       
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
                _walkingSeconds -= _secondsPerTile;
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
