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
using System.Diagnostics.Contracts;
using System.Collections.Generic;


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

        /// <summary>
        /// Player's current position in the world
        /// </summary>
        public Point PlayerWorldPosition { get; private set; }

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
        private const float SecondsPerAnimation = .1f;
        private const float SecondsPerPositionUpdate = .1f;
        private const float TimeUntilStand = 1.5f;

        private float _walkingSeconds;
        private float _standingSeconds;
        private bool _readyToMove;

        private float _animationTimeElapsed;
        private int _animationFrame;
        private Rectangle _playerFrameRectangle;
        private readonly Point _playerSize;

        //The player's inventory
        private Inventory _inventory;

        private Box _currentHeldBox;
        private bool _firstUpdate;
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
            //Create an inventory
            _inventory = new Inventory(screenSize);
            _currentHeldBox = null;
            _reachedDest = true;
            _justMoved = false;
            _firstUpdate = true;

            _playerSize = new Point(sprite.Width/4, sprite.Height/4);
        }

        /// <summary>
        /// Updates the player's position in world space
        /// </summary>
        /// <param name="deltaTime">The elapsed time between frames in seconds</param>
        public void Update(float deltaTime)
        {
            if (_firstUpdate)
            {
                UpdatePlayerPos();
                _firstUpdate = false;
            }
            UpdatePlayerPos();
            KeyboardInput(deltaTime);


            _inventory.Update();
        }

        #region Player Movement
        /// <summary>
        /// Gets keyboard input for player movement and moves the player in world space
        /// </summary>
        /// <param name="deltaTime">The elapsed time between frames in seconds</param>
        private void KeyboardInput(float deltaTime)
        {
            if (InputManager.GetKeyStatus(Keys.A))
            {
                PlayerMovement(deltaTime, new Point(-1, 0), Direction.Left);
            }
            else if (InputManager.GetKeyStatus(Keys.D))
            {
                PlayerMovement(deltaTime, new Point(1, 0), Direction.Right);
            }
            else if (InputManager.GetKeyStatus(Keys.W))
            {
                PlayerMovement(deltaTime, new Point(0, -1), Direction.Up);
            }
            else if (InputManager.GetKeyStatus(Keys.S))
            {
                PlayerMovement(deltaTime, new Point(0, 1), Direction.Down);
            }
            else //if (_reachedDest)
            {
                _playerState = PlayerState.Standing;
                UpdateStandingTime(deltaTime);
                _readyToMove = true;
                _walkingSeconds = 0;
                //but don't change the direction you're facing
            }

            if (InputManager.GetKeyPress(Keys.Space) || InputManager.GetKeyPress(Keys.E))
            {
                Interact();
            }
            else if (InputManager.GetMousePress(MouseButtonState.Left))
            {
                ClickToInteract(MapUtils.ScreenToTile(InputManager.GetMousePosition()));
            }
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
        /// <param name="movement">Point to move</param>
        /// <param name="directionMove">Direction of movement</param>
        private void PlayerMovement(float deltaTime, Point movement, Direction directionMove)
        {
            if (HoldingBox)
            {
                // Drop the box if the player is holding it and they attempt to move a direction the 
                // box can't be moved in
                if (IsPerpendicular(directionMove, _currentHeldBox.AttachmentDirection))
                    DropBox();
                else
                {
                    // Otherwise update need variable for the box
                    directionMove = _currentHeldBox.AttachmentDirection;
                }
            }

            if (!_readyToMove) UpdateWalkingTime(deltaTime);

            else if (_playerState == PlayerState.Walking)
            {
                if (directionMove == _playerCurrentDirection &&
                    MapManager.CheckPlayerCollision(Location + movement) &&
                    (!HoldingBox || MapManager.CheckPlayerCollision(_currentHeldBox.Location + movement)))
                {
                    _readyToMove = false;
                    Location += movement;

                    SoundManager.PlaySFX(SoundManager.PlayerStepSound, -40, 40);

                    UpdateAnimation(true);
                }
                else //Player can't move where they want to 
                {
                    _playerState = PlayerState.Standing;
                    UpdateStandingTime(deltaTime);
                    _walkingSeconds = 0;
                    //But don't reset animation yet, because that doesn't happen until the timer goes over
                }
            }
            else //player starts out standing or interacting
            {
                if (directionMove == _playerCurrentDirection)
                {
                    if (MapManager.CheckPlayerCollision(Location + movement) &&
                    (!HoldingBox || MapManager.CheckPlayerCollision(_currentHeldBox.Location + movement)))
                    {
                        _readyToMove = false;
                        Location += movement;
                        if (_playerState != PlayerState.Walking)
                            _playerState = PlayerState.Walking;

                        SoundManager.PlaySFX(SoundManager.PlayerStepSound, -40, 40);

                        UpdateAnimation(true);
                        _standingSeconds = 0;
                    }
                    else
                    {
                        _playerState = PlayerState.Standing;
                        UpdateStandingTime(deltaTime);
                        _walkingSeconds = 0;
                    }
                }
                else _readyToMove = false; //stops the player from turning around one frame and then moving the very next frame
                //They have to either stop for a frame or wait for secondspertile
            }

            // Update the player's direction
            if (_playerCurrentDirection != directionMove)
            {
                _playerCurrentDirection = directionMove;
                UpdateAnimation(false);
            }
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
        /// <summary>
        /// Updates the amount of time the player hase been not moving
        /// if they stand long enough, it resets their animation to the standing frame
        /// </summary>
        /// <param name="deltaTime"></param>
        private void UpdateStandingTime(float deltaTime)
        {
            _standingSeconds += deltaTime;

            if (_standingSeconds >= TimeUntilStand)
            {
                _animationFrame = 0;
                _playerState = PlayerState.Standing;
                _standingSeconds = 0f;
                UpdateAnimation(false);
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
        /// Updates the player's animation frame, and advances it to the next frame if necessarry
        /// </summary>
        /// <param name="advance">If true, advances the animation to the next frame</param>
        private void UpdateAnimation(bool advance)
        {
            if (advance)
            {
                _animationFrame++;
                if (_animationFrame > 3) _animationFrame = 0;
            }
            _playerFrameRectangle = new Rectangle(
                new Point(_playerSize.X * (int)_playerCurrentDirection, _playerSize.Y * _animationFrame),
                _playerSize);
        }

        /// <summary>
        /// Update the player's position regarldess of time
        /// </summary>
        private void UpdatePlayerPos()
        {
            PlayerWorldPosition = MapUtils.TileToWorld(Location);

            Point worldToScreen = MapUtils.WorldToScreen();
            PlayerScreenPosition = PlayerWorldPosition - worldToScreen + MapUtils.PixelOffset();
            _currentHeldBox?.UpdatePosition(Location);
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
            //TODO: Go back to standing when you interact regardless of the timer
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
            }

            objectToInteract?.Interact(this);
        }

        /// <summary>
        /// Check for an interactive item at a clicked point
        /// </summary>
        /// <param name="clickedPoint">Point that was clicked in tile space</param>
        public void ClickToInteract(Point clickedPoint)
        {
            if (HoldingBox)
            {
                DropBox();
                return;
            }

            int xOffset = clickedPoint.X - Location.X;
            int yOffset = clickedPoint.Y - Location.Y;
            //If the clicked tile isn't adjacent to the player, no need to proceed
            if (!(Math.Abs(xOffset) == 0 && Math.Abs(yOffset) == 1) && !(Math.Abs(xOffset) == 1 && Math.Abs(yOffset) == 0))
                return;

            Prop objectToInteract = MapManager.CheckInteractable(clickedPoint);

            objectToInteract?.Interact(this);

            if (xOffset != 0)
            {
                if (xOffset == 1)
                    _playerCurrentDirection = Direction.Right;
                else
                    _playerCurrentDirection = Direction.Left;
            }
            else
            {
                if (yOffset == 1)
                    _playerCurrentDirection = Direction.Down;
                else
                    _playerCurrentDirection = Direction.Up;
            }
        }

        /// <summary>
        /// Called to update the player's location in the new room
        /// </summary>
        /// <param name="new_location">New location for the player</param>
        public void ChangeRoom(Point new_location)
        {
            Location = new_location;
            UpdatePlayerPos();
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
            _currentHeldBox?.DropBox();
            _currentHeldBox = null;

        }

        /// <summary>
        /// Called when the player is detected by a camera
        /// </summary>
        public void Detected()
        {
            //Reset the map to the last checkpoint
            MapManager.LoadCheckpoint(TriggerManager.CurrentCheckpoint);

            //Reset the player data to the last checkpoint
            Load();
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
            UpdateAnimation(false);
        }

        /// <summary>
        /// Saves the player's position and inventory to a file. Format is as follows:
        /// int xPos
        /// int yPos
        /// 
        /// int item count
        /// 
        /// inventory:
        ///     int propIndex
        ///     int keyType
        /// </summary>
        public void Save()
        {
            //Make the folder if it doesn't already exist
            if (!Directory.Exists("./PlayerData"))
                Directory.CreateDirectory("./PlayerData");

            BinaryWriter writer = null;
            try
            {
                Stream stream = File.OpenWrite($"./PlayerData/PlayerData.data");
                writer = new BinaryWriter(stream);

                //Player position
                writer.Write(Location.X);
                writer.Write(Location.Y);

                //Item count
                writer.Write(_inventory.Contents.Count);

                //Items
                for (int i = 0; i < _inventory.Contents.Count; i++)
                {
                    writer.Write(_inventory.Contents[i].SpriteIndex);
                    writer.Write((int)_inventory.Contents[i].ItemKeyType);
                }
            }
            finally
            {
                writer.Close();
            }
        }

        /// <summary>
        /// Resets all relevant player values to how they were during the last checkpoint
        /// </summary>
        public void Load()
        {
            BinaryReader reader = null!;

            //Clear Player's states
            ClearStates();

            try
            {
                Stream stream = File.OpenRead("./PlayerData/PlayerData.data");
                reader = new BinaryReader(stream);

                //Update player position
                Location = new Point(reader.ReadInt32(), reader.ReadInt32());

                //Check item count
                int itemCount = reader.ReadInt32();

                //Give the player their items back
                for (int i = 0; i < itemCount; i++)
                {
                    int spriteIndex = reader.ReadInt32();
                    int keyType = reader.ReadInt32();

                    _inventory.AddItemToInventory(new Item(Point.Zero, AssetManager.PropTextures, spriteIndex, "TEMP_NAME", (Door.DoorKeyType)keyType));
                }
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
