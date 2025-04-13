using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.Players.InventoryFiles;
using MakeEveryDayRecount.GameObjects.Triggers;
using System.IO;
using System;


namespace MakeEveryDayRecount.Players
{

    /// <summary>
    /// The direction of an object
    /// </summary>
    public enum Direction
    {
        Down = 0,
        Right = 1,
        Up = 2,
        Left = 3,
        None
    }

    /// <summary>
    /// The player that can move around the map and interact
    /// </summary>
    internal class Player : GameObject
    {
        /// <summary>
        /// The state the player is in
        /// </summary>
        public enum PlayerState
        {
            Standing = 0,
            Walking = 1,
            Interacting = 2
        }



        /// <summary>
        /// Player's current position on the screen
        /// </summary>
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

        private const float SecondsPerTile = .2f;
        private float _walkingSeconds;
        private bool _readyToMove;

        private float _animationTimeElapsed;
        private int _animationFrame;
        private Rectangle _playerFrameRectangle;
        private readonly Point _playerSize;

        //The player's inventory
        private Inventory _inventory;

        private Box _currentHeldBox;

        /// <summary>
        /// Get if the player is holing a box 
        /// </summary>
        private bool HoldingBox { get { return _currentHeldBox != null; } }

        /// <summary>
        /// Create the player with all the needed information
        /// </summary>
        /// <param name="location">Location of the player</param>
        /// <param name="sprite">Image of the player</param>
        /// <param name="screenSize">Size of the screen in pixels</param>
        public Player(Point location, Texture2D sprite, Point screenSize)
            : base(location, sprite)
        {
            _walkingSeconds = 0;
          
            _animationFrame = 0;
            _playerSize = new Point(sprite.Width / 4, sprite.Height / 4);
            //Create an inventory
            _inventory = new Inventory(screenSize);
            _currentHeldBox = null;
        }

        /// <summary>
        /// Updates the player's position in world space
        /// </summary>
        /// <param name="deltaTime">The elapsed time between frames in seconds</param>
        public void Update(float deltaTime)
        {
            KeyboardInput(deltaTime);
            UpdatePlayerPos();
            _playerFrameRectangle = AnimationUpdate(deltaTime);
            _inventory.Update();
        }

        #region Player Movement
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
            else
            {
                //if we were walking and we stop pressing a key, go back to standing
                _playerState = PlayerState.Standing;
                _walkingSeconds = 0;
                //but don't change the direction you're facing
            }

            if (InputManager.GetKeyPress(Keys.E))
                Interact();
            if (InputManager.GetKeyPress(Keys.I))
                Detected();
            
        }

        /// <summary>
        /// Get if two directions are perpendicular to each other
        /// </summary>
        /// <param name="directionToCheck">First direction</param>
        /// <param name="directionToCheckAgainst">Second direction</param>
        /// <returns>If the directions are at right angles</returns>
        private bool IsPerpendicular(Direction directionToCheck, Direction directionToCheckAgainst)
        {
            if (directionToCheck == Direction.None || directionToCheckAgainst == Direction.None)
                return false;
            // Directions Up and Down % 2 = 0, while Left and Right return 1
            // If the modulus values are different than they are perpendicular
            return (int)directionToCheck % 2 != (int)directionToCheckAgainst % 2;
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
            // Drop the box if the player is holding it and they attempt to move a direction the 
            // box can't be moved in
            if (HoldingBox && IsPerpendicular(directionMove, _currentHeldBox.AttachmentDirection))
                DropBox();

            // Stop the player from turing around if they're holding the box
            if (HoldingBox)
                directionMove = _currentHeldBox.AttachmentDirection;

            if (!_readyToMove)
                UpdateWalkingTime(deltaTime);

            if (_readyToMove && MapManager.CheckPlayerCollision(Location + movement) &&
                (!HoldingBox || MapManager.CheckPlayerCollision(_currentHeldBox.Location + movement)))
            {

                Location += movement;

                //Check for triggers
                Trigger trigger = null;
                trigger = MapManager.CheckTrigger(Location);
                if (trigger != null)
                    trigger.Activate(this);

                _readyToMove = false;
                if (_playerState == PlayerState.Standing)
                    _playerState = PlayerState.Walking;
                if (HoldingBox)
                    _currentHeldBox.UpdatePosition(Location);
                SoundManager.PlaySFX(SoundManager.PlayerStepSound, -20, 20);
            }

            // Update the player's walking state if needed
            if (_playerCurrentDirection != directionMove)
                _playerCurrentDirection = directionMove;
        }

        /// <summary>
        /// Update the time value in between each movements
        /// </summary>
        /// <param name="deltaTime">Time that has elapsed since last frame</param>
        private void UpdateWalkingTime(float deltaTime)
        {
            _walkingSeconds += deltaTime;
            if (_walkingSeconds >= SecondsPerTile)
            {
                _readyToMove = true;
                _walkingSeconds -= SecondsPerTile;
            }
        }
        #endregion

        #region Drawing Logic

        /// <summary>
        /// Draws the player in the center of the screen
        /// </summary>
        /// <param name="sb">The instance of spritebatch to be used to draw the player</param>
        public void Draw(SpriteBatch sb)
        {

            sb.Draw(
                Sprite,
                new Rectangle(PlayerScreenPosition, AssetManager.TileSize),
                _playerFrameRectangle,
                Color.White
            );

            //Draw the inventory. If the player were to ever overlap the inventory it will disappear behind it
            //Because nothing in the game should be drawn on top of the UI
            _inventory.Draw(sb, MapUtils.ScreenSize);
        }

        /// <summary>
        /// Set the current Rectangle that represents the player's current image.
        /// As it is setup now, the player changes walking animation once per tile
        /// at the same rate the player walks.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        private Rectangle AnimationUpdate(float deltaTime)
        {
            // Change the animation based on what state the player is currently in
            switch (_playerState)
            {
                case PlayerState.Standing:
                    // Reset the animation timer to zero so the player doesn't look like
                    // they're walking of they turn while in the same tile
                    _animationFrame = 0;
                    _animationTimeElapsed = 0;
                    break;
                case PlayerState.Walking:
                    _animationTimeElapsed += deltaTime;
                    // Check if the animation is ready to update
                    if (_animationTimeElapsed >= SecondsPerTile)
                    {
                        _animationTimeElapsed -= SecondsPerTile;
                        _animationFrame++;
                        // Walking animations range from 0-3 in the Sprite Sheet
                        // _animationFrame being < 0 is probably not going to happen
                        // but its easy enough to check for so might as well
                        if (_animationFrame >= 4 || _animationFrame < 0)
                            _animationFrame = 0;
                    }
                    break;
                case PlayerState.Interacting:
                    // TODO Add animation for picking up/interacting
                    break;
            }

            return new Rectangle(
                new Point(
                    _playerSize.X * (int)_playerCurrentDirection,
                    _playerSize.Y * _animationFrame
                ),
                _playerSize
            );
        }

        /// <summary>
        /// Convert from the player's tile position to screen position
        /// </summary>
        private void UpdatePlayerPos()
        {
            Point playerWorldPos = MapUtils.TileToWorld(Location);
            Point worldToScreen = MapUtils.WorldToScreen();

            PlayerScreenPosition = playerWorldPos - worldToScreen + MapUtils.PixelOffset();
        }

        #endregion

        #region Interaction
        /// <summary>
        /// Determines if the player's inventory contains a key of the specified type
        /// </summary>
        /// <param name="keyType">The key type to look for</param>
        /// <returns>True if a suitable key is found, false otherwise</returns>
        public bool ContainsKey(Door.DoorKeyType keyType)
        {
            return _inventory.SelectedItem != null
                && _inventory.SelectedItem.ItemKeyType == keyType;
        }

        /// <summary>
        /// Adds an item to the player's inventory
        /// </summary>
        /// <param name="item">The item to add to the inventory</param>
        public void PickUpItem(Item item)
        {
            //add the item to your inventory
            _inventory.AddItemToInventory(item);
        }
        /// <summary>
        /// Player asks map manager to check if the tile it's looking at has an interactable object
        /// If so, the player then interacts with that thing.
        /// </summary>
        public void Interact()
        {
            if (HoldingBox)
            {
                DropBox();
                return;
            }
            Prop objectToInteract = null;

            switch (_playerCurrentDirection)
            {
                case Direction.Left:
                    objectToInteract = MapManager.CheckInteractable(Location + new Point(-1, 0));
                    break;
                case Direction.Up:
                    objectToInteract = MapManager.CheckInteractable(Location + new Point(0, -1));
                    break;
                case Direction.Right:
                    objectToInteract = MapManager.CheckInteractable(Location + new Point(1, 0));
                    break;
                case Direction.Down:
                    objectToInteract = MapManager.CheckInteractable(Location + new Point(0, 1));
                    break;
                    //add code that makes the interaction happen
            }

            if (objectToInteract == null)
                return;

            objectToInteract.Interact(this);
        }

        /// <summary>
        /// Called to update the player's location in the new room
        /// </summary>
        /// <param name="new_location">New location for the player</param>
        public void ChangeRoom(Point new_location)
        {
            Location = new_location;
        }

        /// <summary>
        /// Pickup this box and attach it to the player
        /// </summary>
        /// <param name="boxToPickup">Box to attach</param>
        public void PickupBox(Box boxToPickup)
        {
            _currentHeldBox = boxToPickup;
        }

        /// <summary>
        /// Release the box the player is holding
        /// </summary>
        private void DropBox()
        {
            if(HoldingBox)
             _currentHeldBox.DropBox();
            _currentHeldBox = null;
        }

        /// <summary>
        /// Called when the player is detected by a camera
        /// </summary>
        public void Detected(){
            // TODO rather than restarting the level, just reset the player to the last checkpoint
             GameplayManager.LevelReset();
        }

        #endregion

        /// <summary>
        /// Reset player to default states
        /// </summary>
        public void ClearStates()
        {
            _playerCurrentDirection = Direction.Down;
            _playerState = PlayerState.Standing;
            DropBox();
            _inventory.ClearInventory();
        }

        /// <summary>
        /// Saves the player's position and inventory to a file. Format is as follows:
        /// int xPos
        /// int yPos
        /// 
        /// int item count
        /// 
        /// inventory:
        ///     string itemName
        /// </summary>
        public void Save(string baseFolder)
        {
            //Make the folder if it doesn't already exist
            if (!Directory.Exists("./PlayerData"))
                Directory.CreateDirectory("./PlayerData");

            BinaryWriter writer = null;
            try
            {
                Stream stream = File.OpenWrite($"{baseFolder}/PlayerData");
                writer = new BinaryWriter(stream);

                //Player position
                writer.Write(Location.X);
                writer.Write(Location.Y);

                //Item count
                writer.Write(_inventory.Contents.Count);

                //Items
                for (int i = 0; i < _inventory.Contents.Count; i++)
                    writer.Write(_inventory.Contents[i].ItemName);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message);
            }
            finally
            {
                writer.Close();
            }
        }
    }
}
