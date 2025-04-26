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
        private float _walkingSeconds;
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
            _playerSize = new Point(sprite.Width / 4, sprite.Height / 4);
            //Create an inventory
            _inventory = new Inventory(screenSize);
            _currentHeldBox = null;

            _firstUpdate = true;
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
            else
            {
                //if we were walking and we stop pressing a key, go back to standing
                _playerState = PlayerState.Standing;
                _walkingSeconds = 0;
                //but don't change the direction you're facing
            }

            if (InputManager.GetKeyPress(Keys.Space) || InputManager.GetKeyPress(Keys.E) || InputManager.GetKeyPress(Keys.Enter))
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
            if (!_readyToMove)
                UpdateWalkingTime(deltaTime);

            if (_readyToMove && MapManager.CheckPlayerCollision(Location + movement) &&
                (!HoldingBox || MapManager.CheckPlayerCollision(_currentHeldBox.Location + movement)))
            {
                if (MapManager.CheckPlayerCollision(Location + movement) &&
                    (!HoldingBox || MapManager.CheckPlayerCollision(_currentHeldBox.Location + movement)))
                {
                    _readyToMove = false;

                    Location += movement;
                    if (_playerState != PlayerState.Walking)
                        _playerState = PlayerState.Walking;

                    //Check triggers
                    Trigger trigger = MapManager.CurrentRoom.VerifyTrigger(Location);

                    //Only trigger that cares about if it was activated right now is the Win trigger
                    //If there are more that are created this will turn into a larger if statement
                    if (trigger != null && trigger.Activate(this) && trigger is Win)
                    {
                        GameplayManager.PlayerWinTrigger();
                    }

                    SoundManager.PlaySFX(SoundManager.PlayerStepSound, -40, 40);
                }
                else
                {
                    _playerState = PlayerState.Standing;
                }
            }

            //Checkpoints are slightly slightly buggy when holding a box and walking on to them (box is saved a tile away from where it should be)
            //But it's not a big issue, and the player shouldn't be holding a box and proccing a trigger anyways
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

            // Update the player's direction
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

            // This variable allows for the time between movements to be changed depending on what state 
            // the player is coming from
            int timeAdjustment = 1;

            if (_playerState == PlayerState.Standing)
                timeAdjustment = 2;

            if (_walkingSeconds >= SecondsPerTile / timeAdjustment)
            {
                _readyToMove = true;
                _walkingSeconds -= SecondsPerTile / timeAdjustment;
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
        /// <param name="deltaTime">Time since last frame</param>
        /// <returns>Rectangle for player animation</returns>
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
                    if (_animationTimeElapsed >= SecondsPerAnimation)
                    {
                        _animationTimeElapsed -= SecondsPerAnimation;
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
            TriggerManager.CurrentCheckpoint.LoadCheckpoint(this);
            ReplayManager.ClearData();
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
        ///     int propIndex
        ///     int keyType
        /// </summary>
        /// <param name="baseFolder">Folder to save player data to</param>

        public void Save(string baseFolder)
        {

            try
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite($"{baseFolder}/PlayerData.data")))
                {
                    //Player position
                    binaryWriter.Write(Location.X);
                    binaryWriter.Write(Location.Y);

                    //Item count
                    binaryWriter.Write(_inventory.Contents.Count);

                    //Items
                    for (int i = 0; i < _inventory.Contents.Count; i++)
                    {
                        binaryWriter.Write(_inventory.Contents[i].SpriteIndex);
                        binaryWriter.Write((int)_inventory.Contents[i].ItemKeyType);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message);
            }
        }

        /// <summary>
        /// Validate the PLayerData.data file exists
        /// </summary>
        /// <param name="baseFolder">Folder to check</param>
        /// <returns>If the file exists</returns>
        public bool ValidateData(string baseFolder)
        {
            if (File.Exists($"{baseFolder}/PlayerData.data"))
                return true;
            return false;
        }

        /// <summary>
        /// Resets all relevant player values to how they were during the last checkpoint
        /// </summary>
        /// <param name="baseFolder">Folder to load player data from</param>
        public void Load(string baseFolder)
        {
            if (!File.Exists($"{baseFolder}/PlayerData.data"))
                return;

            //Clear Player's states
            ClearStates();

            try
            {
                using (BinaryReader binaryReader = new BinaryReader(File.OpenRead($"{baseFolder}/PlayerData.data")))
                {
                    //Update player position
                    Location = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());

                    //Check item count
                    int itemCount = binaryReader.ReadInt32();

                    //Give the player their items back
                    for (int i = 0; i < itemCount; i++)
                    {
                        int spriteIndex = binaryReader.ReadInt32();
                        int keyType = binaryReader.ReadInt32();

                        _inventory.AddItemToInventory(new Item(Point.Zero, AssetManager.PropTextures, spriteIndex, "TEMP_NAME", (Door.DoorKeyType)keyType));
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message);
            }
        }

        /// <summary>
        /// Returns a boolean corresponding to whether or not the play has the item with the given sprite index in their inventory
        /// </summary>
        /// <param name="index">Sprite index of the item being searched for</param>
        /// <returns>True if the player has the item, false otherwise</returns>
        public bool ContainsItem(int index)
        {
            for (int i = 0; i < _inventory.Contents.Count; i++)
            {
                if (_inventory.Contents[i].SpriteIndex == index)
                    return true;
            }
            return false;
        }
    }
}
