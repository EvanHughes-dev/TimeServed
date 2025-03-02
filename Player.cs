using MakeEveryDayRecount.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;


namespace MakeEveryDayRecount
{
    internal class Player: GameObject
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
        private readonly float _secondsPerTile = 0.2f;
        private float _walkingSeconds;
        private List<GameObject> _inventory;

        private Rectangle _sourceRectangle;
        private Texture2D _playerTextures; //Probably a sprite sheet

        public Player(Point location, Texture2D sprite) : base(location, sprite)
        {

            _walkingSeconds = 0;
        }

        public override void Update(float deltaTimeS) {
            throw new NotImplementedException("Update has not been created yet in Player");
            KeyboardInput(deltaTimeS);
        }
        public void Draw(SpriteBatch sb) {
            throw new NotImplementedException("Draw has not been created yet in Player");
        }
        private void KeyboardInput(float deltaTimeS)
        {
            throw new NotImplementedException("KeyBoardInput has not been created yet in Player");

            //if we were walking already
            if (_playerState == PlayerState.Walking)
            {
                //if we're going in the same direction we were just going
                //increment the counter
                //if the counter is high enough, move by one in our current direction and reduce the counter by the threshold amount

                //if our direction has changed
                //reset the counter
                //move by 1 in the new direction
                //change the player's direction
            }
            //if we just started walking
            if (_playerState == PlayerState.Standing)
            {
                //reset the walking counter to 0
                _walkingSeconds = 0;
                //if some key is pressed, move in the corresponding direction and increment the walking counter
                if (InputManager.GetKeyStatus(Keys.Left) || InputManager.GetKeyStatus(Keys.A))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Left;
                    _location.X = _location.X - 1;
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Right) || InputManager.GetKeyStatus(Keys.D))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Right;
                    _location.X = _location.X + 1;
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Up) || InputManager.GetKeyStatus(Keys.W))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Up;
                    _location.Y = _location.Y - 1;
                    _walkingSeconds += deltaTimeS;
                }
                else if (InputManager.GetKeyStatus(Keys.Down) || InputManager.GetKeyStatus(Keys.S))
                {
                    _playerState = PlayerState.Walking;
                    _playerCurrentDirection = Direction.Down;
                    _location.Y = _location.Y + 1;
                    _walkingSeconds += deltaTimeS;
                }
            }
            //If there's no key pressed, walkingseconds = 0, state = standing
        }
    }
}
