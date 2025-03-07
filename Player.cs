using System;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.Map.Tiles;
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
        private readonly float _secondsPerTile = 1f;
        private float _walkingSeconds;

        //A reference to the gameplay manager which has a reference to the map which lets the player know what's near them
        private readonly GameplayManager _gameplayManager;
        //This is a variable that lets me easily get the current map manager so I can verify that tiles are walkable
        private MapManager _currentMap;

        private List<GameObject> _inventory;

        private Rectangle _sourceRectangle;
        private Texture2D _playerTextures; //Probably a sprite sheet

        public Player(Point location, Texture2D sprite, GameplayManager gameplayManager)
            : base(location, sprite)
        {
            //NOTE: For now, the player's screen position is always in the middle
            _walkingSeconds = 0;
            _gameplayManager = gameplayManager;
            _currentMap = _gameplayManager.Map;
        }

        public void Update(float deltaTimeS)
        {
            KeyboardInput(deltaTimeS);
        }

        public void Draw(SpriteBatch sb)
        {
            //REMEMBER THEY ONLY DRAW AT THE MIDDLE OF THE SCREEN
            sb.Draw(Sprite, new Rectangle(300, 250, Sprite.Width, Sprite.Height), Color.White);
        }

        private void KeyboardInput(float deltaTimeS)
        {
            //if we were walking already
            //if we're going in the same direction we were just going
            //increment the counter
            //if the counter is high enough, move by one in our current direction and reduce the counter by the threshold amount

            //if our direction has changed
            //reset the counter
            //move by 1 in the new direction
            //change the player's direction

            //NOTE: Check collision every time we try to move
            if (_playerState == PlayerState.Walking)
            {
                if (InputManager.GetKeyStatus(Keys.Left) || InputManager.GetKeyStatus(Keys.A))
                {
                    if (_playerCurrentDirection == Direction.Left)
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile && _currentMap.CheckPlayerCollision(Location + new Point(-1, 0)))
                        {
                            Location += new Point(-1, 0);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    else if (_currentMap.CheckPlayerCollision(Location + new Point(-1, 0)))
                    {
                        _walkingSeconds = 0;
                        Location += new Point(-1, 0);
                        _playerCurrentDirection = Direction.Left;
                    }
                }
                else if (InputManager.GetKeyStatus(Keys.Right) || InputManager.GetKeyStatus(Keys.D))
                {
                    if (_playerCurrentDirection == Direction.Right && _currentMap.CheckPlayerCollision(Location + new Point(1, 0)))
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile)
                        {
                            Location += new Point(1, 0);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    else if (_currentMap.CheckPlayerCollision(Location + new Point(1, 0)))
                    {
                        _walkingSeconds = 0;
                        Location += new Point(1, 0);
                        _playerCurrentDirection = Direction.Right;
                    }
                }
                else if (InputManager.GetKeyStatus(Keys.Up) || InputManager.GetKeyStatus(Keys.W))
                {
                    if (_playerCurrentDirection == Direction.Up && _currentMap.CheckPlayerCollision(Location + new Point(0, -1)))
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile)
                        {
                            Location += new Point(0, -1);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    else if (_currentMap.CheckPlayerCollision(Location + new Point(0, -1)))
                    {
                        _walkingSeconds = 0;
                        Location += new Point(0, -1);
                        _playerCurrentDirection = Direction.Up;
                    }
                }
                else if (InputManager.GetKeyStatus(Keys.Down) || InputManager.GetKeyStatus(Keys.S))
                {
                    if (_playerCurrentDirection == Direction.Down && _currentMap.CheckPlayerCollision(Location + new Point(0, 1)))
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile)
                        {
                            Location += new Point(0, 1);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    else if (_currentMap.CheckPlayerCollision(Location + new Point(0, 1)))
                    {
                        _walkingSeconds = 0;
                        Location += new Point(0, 1);
                        _playerCurrentDirection = Direction.Down;
                    }
                }
                //if we were walking and we stop pressing a key, go back to standing
                else
                {
                    _playerState = PlayerState.Standing;
                    _walkingSeconds = 0;
                    //but don't change the direction you're facing
                }
            }
            //if we're standing
            if (_playerState == PlayerState.Standing)
            {

                //if some key is pressed, move in the corresponding direction and increment the walking counter
                if (InputManager.GetKeyStatus(Keys.Left) || InputManager.GetKeyStatus(Keys.A) && _currentMap.CheckPlayerCollision(Location + new Point(-1, 0)))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Left;
                    Location += new Point(-1, 0);
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Right) || InputManager.GetKeyStatus(Keys.D) && _currentMap.CheckPlayerCollision(Location + new Point(1, 0)))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Right;
                    Location += new Point(1, 0);
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Up) || InputManager.GetKeyStatus(Keys.W) && _currentMap.CheckPlayerCollision(Location + new Point(0, 1)))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Up;
                    Location += new Point(0, -1);
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Down) || InputManager.GetKeyStatus(Keys.S) && _currentMap.CheckPlayerCollision(Location + new Point(0, 1)))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Down;
                    Location += new Point(0, 1);
                    _walkingSeconds += deltaTimeS;
                }
            }
        }

        public bool ContainsKey(Door.DoorKeyType keyType)
        {
            throw new NotImplementedException();
        }
    }
}
