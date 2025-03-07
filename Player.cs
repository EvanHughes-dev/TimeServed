using System;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
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
        private readonly float _secondsPerTile = 2f;
        private float _walkingSeconds;

        private List<GameObject> _inventory;

        private Rectangle _sourceRectangle;
        private Texture2D _playerTextures; //Probably a sprite sheet

        public Player(Point location, Texture2D sprite)
            : base(location, sprite)
        {
            //NOTE: For now, the player's screen position is always in the middle
            _walkingSeconds = 0;
        }

        /// <summary>
        /// Updates the player's position in world space
        /// </summary>
        /// <param name="deltaTimeS">The elapsed time between frames in seconds</param>
        public void Update(float deltaTimeS)
        {
            KeyboardInput(deltaTimeS);
        }

        /// <summary>
        /// Draws the player in the center of the screen
        /// </summary>
        /// <param name="sb">The instance of spritebatch to be used to draw the player</param>
        public void Draw(SpriteBatch sb)
        {
            //REMEMBER THEY ONLY DRAW AT THE MIDDLE OF THE SCREEN
            sb.Draw(Sprite, new Rectangle(300, 250, Sprite.Width, Sprite.Height), Color.White);
        }

        /// <summary>
        /// Gets keyboard input for player movement and moves the player in world space
        /// </summary>
        /// <param name="deltaTimeS">The elapsed time between frames in seconds</param>
        private void KeyboardInput(float deltaTimeS)
        {
            //if we were walking already
            if (_playerState == PlayerState.Walking)
            {
                if (InputManager.GetKeyStatus(Keys.Left) || InputManager.GetKeyStatus(Keys.A))
                {
                    //if we're going in the same direction we were just going
                    //increment the counter
                    //if the counter is high enough, move by one in our current direction and reduce the counter by the threshold amount
                    if (_playerCurrentDirection == Direction.Left)
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile)
                        {
                            Location += new Point(-1, 0);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    //if our direction has changed
                    //reset the counter
                    //move by 1 in the new direction
                    //change the player's direction
                    else
                    {
                        _walkingSeconds = 0;
                        Location += new Point(-1, 0);
                        _playerCurrentDirection = Direction.Left;
                    }
                    //this structure is the same for all the keys
                }
                else if (InputManager.GetKeyStatus(Keys.Right) || InputManager.GetKeyStatus(Keys.D))
                {
                    if (_playerCurrentDirection == Direction.Right)
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile)
                        {
                            Location += new Point(1, 0);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    else
                    {
                        _walkingSeconds = 0;
                        Location += new Point(1, 0);
                        _playerCurrentDirection = Direction.Right;
                    }
                }
                else if (InputManager.GetKeyStatus(Keys.Up) || InputManager.GetKeyStatus(Keys.W))
                {
                    if (_playerCurrentDirection == Direction.Up)
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile)
                        {
                            Location += new Point(0, -1);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    else
                    {
                        _walkingSeconds = 0;
                        Location += new Point(0, -1);
                        _playerCurrentDirection = Direction.Up;
                    }
                }
                else if (InputManager.GetKeyStatus(Keys.Down) || InputManager.GetKeyStatus(Keys.S))
                {
                    if (_playerCurrentDirection == Direction.Down)
                    {
                        _walkingSeconds += deltaTimeS;
                        if (_walkingSeconds >= _secondsPerTile)
                        {
                            Location += new Point(0, 1);
                            _walkingSeconds -= _secondsPerTile;
                        }
                    }
                    else
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
                if (InputManager.GetKeyStatus(Keys.Left) || InputManager.GetKeyStatus(Keys.A))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Left;
                    Location += new Point(-1, 0);
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Right) || InputManager.GetKeyStatus(Keys.D))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Right;
                    Location += new Point(1, 0);
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Up) || InputManager.GetKeyStatus(Keys.W))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Up;
                    Location += new Point(0, -1);
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Down) || InputManager.GetKeyStatus(Keys.S))
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

        public void Interact() 
        {
            Prop thing; //change the name of this variable later

            switch (_playerCurrentDirection)
            {
                case Direction.Left:
                    //thing = .CheckInteractable(Location + new Point(-1, 0));
                    break;
                case Direction.Up:
                    break;
                case Direction.Right:
                    break;
                case Direction.Down:
                    break;
            }
        }
    }
}
