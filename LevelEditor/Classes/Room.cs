using LevelEditor.Classes.Props;
using LevelEditor.Classes.Triggers;
using LevelEditor.Extensions;
using System.Collections.ObjectModel;

namespace LevelEditor.Classes
{
    /// <summary>
    /// A Room, with a name, ID, and grid of tiles.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// The room's name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The room's randomly-generated ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The grid of tiles the room is made of.
        /// </summary>
        private Tile[,] _tiles;

        /// <summary>
        /// Get the current save state of this form
        /// </summary>
        public SavedState SavedState { get; set; }

        private readonly List<Prop> _props;
        private readonly List<Trigger> _triggers;
        /// <summary>
        /// The props that have been placed in this room, read-only.
        /// </summary>
        public ReadOnlyCollection<Prop> Props => _props.AsReadOnly();
        /// <summary>
        /// The triggers that have been placed in this room, read-only.
        /// </summary>
        public ReadOnlyCollection<Trigger> Triggers => _triggers.AsReadOnly();

        /// <summary>
        /// Gets or sets the tile in the room at the given coordinate.
        /// </summary>
        /// <param name="x">The X coordinate of the tile to set. Larger values are further to the right.</param>
        /// <param name="y">The Y coordinate of the tile to set. Larger values are further down.</param>
        /// <returns>The found tile.</returns>
        public Tile this[int x, int y]
        {
            // Don't really need to do index validation because, if it would ever be relevant, the array
            //   would just throw an IndexOutOfRange exception itself
            get => _tiles[y, x];
            set
            {
                this[new Point(x, y)] = value;
            }
        }

        /// <summary>
        /// Gets or sets the tile at the given point.
        /// </summary>
        /// <param name="tileCoords">The point, in tilespace, of the tile to get or set.</param>
        /// <returns>The found tile.</returns>
        public Tile this[Point tileCoords]
        {
            get => this[tileCoords.X, tileCoords.Y];
            set
            {
                _tiles[tileCoords.Y, tileCoords.X] = value;
                OnTileUpdated?.Invoke(tileCoords, value);
            }
        }

        /// <summary>
        /// How wide this room is, in tiles.
        /// </summary>
        public int Width => _tiles.GetLength(1);
        /// <summary>
        /// How tall this room is, in tiles.
        /// </summary>
        public int Height => _tiles.GetLength(0);
        /// <summary>
        /// The dimensions of this room, in tiles.
        /// </summary>
        public Size Dimensions
        {
            get => new Size(Width, Height);
        }

        /// <summary>
        /// Called whenever a tile in the room is changed.
        /// </summary>
        public event Action<Point, Tile>? OnTileUpdated;
        /// <summary>
        /// Called whenever a prop is added to the room.
        /// </summary>
        public event Action<Prop>? OnPropAdded;
        /// <summary>
        /// Called whenever a prop is removed from the room.
        /// </summary>
        public event Action<Prop>? OnPropRemoved;
        /// <summary>
        /// Called whenever a trigger is added to the room.
        /// </summary>
        public event Action<Trigger>? OnTriggerAdded;
        /// <summary>
        /// Called whenever a trigger is removed from the room.
        /// </summary>
        public event Action<Trigger>? OnTriggerRemoved;
        /// <summary>
        /// Called whenever a camera in this room updates its view frustum.
        /// </summary>
        public event Action<Camera>? OnCameraUpdate;
        /// <summary>
        /// Called whenever this room has its bounds resized.
        /// </summary>
        public event Action? OnRoomResized;

        /// <summary>
        /// Creates a new Room with a name, dimensions, and optional Tile to fill the grid with.
        /// </summary>
        /// <param name="name">The room's name.</param>
        /// <param name="width">The room's width, in tiles.</param>
        /// <param name="height">The room's height, in tiles.</param>
        /// <param name="bg">
        ///   If provided, every tile in the Room will be set to this tile.
        ///   Should only be excluded if you're planning to immediately set every tile manually!
        /// </param>
        public Room(string name, int width, int height, Tile? bg = null, int? id = null)
        {
            Name = name;

            _tiles = new Tile[height, width];

            if (bg != null)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        _tiles[y, x] = (Tile)bg;
                    }
                }
            }

            _props = new List<Prop>();
            _triggers = new List<Trigger>();
            Id = id == null ? Program.Random.Next() : (int)id;
            SavedState = SavedState.Saved;
        }

        /// <summary>
        /// Adds a new prop to this Room.
        /// </summary>
        /// <param name="prop">The prop to add. Must not be null, and must have a set position.</param>
        /// <exception cref="ArgumentException">Thrown when prop.Position is null.</exception>
        public void AddProp(Prop prop)
        {
            ArgumentNullException.ThrowIfNull(prop);
            if (prop.Position == null)
                throw new ArgumentException("Rooms cannot contain positionless props.", nameof(prop));

            _props.Add(prop);

            if (prop is Camera camera)
            {
                camera.CameraUpdate += HandleCameraUpdate;
            }

            OnPropAdded?.Invoke(prop);
        }

        /// <summary>
        /// Invokes the OnCameraUpdate event.
        /// </summary>
        /// <param name="sender"></param>
        private void HandleCameraUpdate(Camera sender)
        {
            OnCameraUpdate?.Invoke(sender);
        }

        /// <summary>
        /// Removes the given prop from this room.
        /// </summary>
        /// <param name="prop">The prop to remove.</param>
        /// <returns>True if the prop was found and removed, false if the prop was not found.</returns>
        public bool RemoveProp(Prop prop)
        {
            bool removalSuccessful = _props.Remove(prop);

            if (removalSuccessful)
            {
                if (prop is Camera camera)
                {
                    camera.CameraUpdate -= HandleCameraUpdate;
                }

                OnPropRemoved?.Invoke(prop);
            }

            return removalSuccessful;
        }

        /// <summary>
        /// Removes the prop at the given position.
        /// </summary>
        /// <param name="tilePosition">The position, in tilespace, of the prop to remove.</param>
        /// <returns>True if a prop at that position was found and removed, false otherwise.</returns>
        public bool RemovePropAt(Point tilePosition)
        {
            return RemoveProp(GetPropAt(tilePosition)!);
        }

        /// <summary>
        /// Gets the prop at the given position, if one exists.
        /// </summary>
        /// <param name="tilePosition">The position to find the prop at.</param>
        /// <returns>The prop, if one is found, or null if no such prop exists.</returns>
        public Prop? GetPropAt(Point tilePosition)
        {
            return _props.Find(prop => prop.Position == tilePosition);
        }

        /// <summary>
        /// Adds a new trigger to this Room.
        /// </summary>
        /// <param name="trigger">The trigger to add. Must not be null, and must have a set bounds.</param>
        /// <exception cref="ArgumentException">Thrown when trigger.Bounds is null.</exception>
        public void AddTrigger(Trigger trigger)
        {
            ArgumentNullException.ThrowIfNull(trigger);
            if (trigger.Bounds == null)
                throw new ArgumentException("Rooms cannot contain boundless triggers.", nameof(trigger));

            _triggers.Add(trigger);

            OnTriggerAdded?.Invoke(trigger);
        }

        /// <summary>
        /// Removes the given trigger from this room.
        /// </summary>
        /// <param name="trigger">The trigger to remove.</param>
        /// <returns>True if the trigger was found and removed, false if the trigger was not found.</returns>
        public bool RemoveTrigger(Trigger trigger)
        {
            bool removalSuccessful = _triggers.Remove(trigger);

            if (removalSuccessful)
            {

                OnTriggerRemoved?.Invoke(trigger);
            }

            return removalSuccessful;
        }

        /// <summary>
        /// Removes the trigger at the given position.
        /// </summary>
        /// <param name="tilePosition">The position, in tilespace, of the trigger to remove.</param>
        /// <returns>True if a trigger at that position was found and removed, false otherwise.</returns>
        public bool RemoveTriggerAt(Point tilePosition)
        {
            return RemoveTrigger(GetTriggerAt(tilePosition)!);
        }

        /// <summary>
        /// Gets the trigger at the given position, if one exists.
        /// </summary>
        /// <param name="tilePosition">The position to find the trigger at.</param>
        /// <returns>The trigger, if one is found, or null if no such trigger exists.</returns>
        public Trigger? GetTriggerAt(Point tilePosition)
        {
            return _triggers.Find(trigger => trigger.Bounds!.Value.Contains(tilePosition));
        }

        /// <summary>
        /// Resizes the room by adjusting each side by the given amounts. Positive numbers push the side outwards,
        ///   negative numbers pull the side inwards.
        /// </summary>
        /// <param name="top">How much to adjust the top by.</param>
        /// <param name="bottom">How much to adjust the bottom by.</param>
        /// <param name="left">How much to adjust the left by.</param>
        /// <param name="right">How much to adjust the right by.</param>
        /// <param name="bg">The background tile to fill each new tile with.</param>
        public void Resize(int top, int bottom, int left, int right, Tile bg)
        {
            Rectangle newBounds = new Rectangle(Point.Empty, Dimensions).NudgeSides(top, bottom, left, right);
            Tile[,] tiles = new Tile[newBounds.Height, newBounds.Width];

            int verticalOffset = newBounds.Top;
            int horizontalOffset = newBounds.Left;

            for (int newY = 0; newY < tiles.GetLength(0); newY++)
            {
                for (int newX = 0; newX < tiles.GetLength(1); newX++)
                {
                    int tileY = newY + verticalOffset;
                    int tileX = newX + horizontalOffset;

                    Tile tile =
                        tileY < Height && tileY >= 0
                        && tileX < Width && tileX >= 0
                        ? this[tileX, tileY] : bg;

                    tiles[newY, newX] = tile;
                }
            }

            _tiles = tiles;

            OnRoomResized?.Invoke();
        }
    }
}
