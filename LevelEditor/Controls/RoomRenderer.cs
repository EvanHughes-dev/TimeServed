using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Classes.Triggers;
using LevelEditor.Extensions;
using LevelEditor.Helpers;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace LevelEditor.Controls
{
    public class RoomRenderer : Control
    {
        #region Properties and Fields
        #region Public Properties (and their Corresponding Fields)
        private bool _showTiles;
        /// <summary>
        /// Whether the tiles of the room should be drawn.
        /// </summary>
        public bool ShowTiles
        {
            get => _showTiles;
            set
            {
                _showTiles = value;
                Invalidate();
            }
        }

        private bool _showProps;
        /// <summary>
        /// Whether the props in the room should be drawn.
        /// </summary>
        public bool ShowProps
        {
            get => _showProps; set
            {
                _showProps = value;
                Invalidate();
            }
        }

        private bool _showTriggers;
        /// <summary>
        /// Whether the triggers in the room should be drawn.
        /// </summary>
        public bool ShowTriggers
        {
            get => _showTriggers; set
            {
                _showTriggers = value;
                Invalidate();
            }
        }

        private Room _room;
        public Room Room
        {
            get => _room;
            set
            {
                // Unsubscribe from old room's events
                UnsubscribeRoomEvents();

                _room = value;
                UpdateDisplayProperties();

                SubscribeRoomEvents();
            }
        }

        #endregion
        #region Public Events
        public delegate void TileEventHandler(object? sender, TileEventArgs e);
        /// <summary>
        /// Occurs when the mouse pointer is over a tile and a mouse button is
        ///   pressed.
        /// </summary>
        public event TileEventHandler? TileMouseDown;
        /// <summary>
        /// Occurs when the mouse pointer is moved over a tile.
        /// </summary>
        public event TileEventHandler? TileMouseMove;
        /// <summary>
        /// Occurs when the mouse pointer is over a tile and a mouse button is released.
        /// </summary>
        public event TileEventHandler? TileMouseUp;
        /// <summary>
        /// Occurs when a tile is mouse clicked.
        /// </summary>
        public event TileEventHandler? TileMouseClick;
        /// <summary>
        /// Occurs when a tile is mouse double clicked.
        /// </summary>
        public event TileEventHandler? TileMouseDoubleClick;
        /// <summary>
        /// Occurs when the mouse wheel moves while the control has focus. Also includes tile data based on the mouse cursor.
        /// </summary>
        public event TileEventHandler? TileMouseWheel;

        #endregion
        #region Private Fields
        // The bounds of the room, in pixel-space, as it should be drawn to the screen
        private Rectangle _roomBounds;
        // How large each tile should be drawn, in pixels
        private int _tileSize;
        // The file path to the wire box sprite
        private const string WireBoxSpritePath = "./Sprites/prop_wirebox.png";
        // The wire box sprite itself, loaded in the constructor
        private readonly Image _wireBoxSprite;
        #endregion

        #endregion
        #region Constructors
        /// <summary>
        /// Initializes a new RoomRenderer as a child control, with specific text, size, and location.
        /// </summary>
        /// <param name="parent">The Control to be the parent of the RoomRenderer.</param>
        /// <param name="text">The text value of the RoomRenderer. Does nothing!</param>
        /// <param name="left">The X position of the control, in pixels, from the left edge of the control's container. The value is assigned to the Left property.</param>
        /// <param name="top">The Y position of the control, in pixels, from the top edge of the control's container. The value is assigned to the Top property.</param>
        /// <param name="width">The width of the control, in pixels. The value is assigned to the Width property.</param>
        /// <param name="height">The height of the control, in pixels. The value is assigned to the Height property.</param>
        public RoomRenderer(Control? parent = null, string? text = null, int left = 0, int top = 0, int width = 0, int height = 0)
            : base(parent, text, left, top, width, height)
        {
            _room = null!;

            // The internet tells me double-buffering is good for this kind of thing!
            DoubleBuffered = true;

            ShowTiles = true;
            ShowProps = true;
            ShowTriggers = true;

            // Is it clean to be doing file IO in RoomRenderer? No. Is it fine? Sure
            if (File.Exists(WireBoxSpritePath))
            {
                _wireBoxSprite = Image.FromFile(WireBoxSpritePath);    
            }
        }
        /// <summary>
        /// Initializes a new RoomRenderer with specific text, size, and location.
        /// </summary>
        /// <param name="text">The text value of the RoomRenderer. Does nothing!</param>
        /// <param name="left">The X position of the control, in pixels, from the left edge of the control's container. The value is assigned to the Left property.</param>
        /// <param name="top">The Y position of the control, in pixels, from the top edge of the control's container. The value is assigned to the Top property.</param>
        /// <param name="width">The width of the control, in pixels. The value is assigned to the Width property.</param>
        /// <param name="height">The height of the control, in pixels. The value is assigned to the Height property.</param>
        public RoomRenderer(string? text = null, int left = 0, int top = 0, int width = 0, int height = 0)
            : this(null, text, left, top, width, height) { }
        /// <summary>
        /// Initializes a new RoomRenderer as a child control, with specific text.
        /// </summary>
        /// <param name="parent">The Control to be the parent of the RoomRenderer.</param>
        /// <param name="text">The text value of the RoomRenderer. Does nothing!</param>
        public RoomRenderer(Control? parent = null, string? text = null)
            : this(parent, text, 0, 0, 0, 0) { }
        /// <summary>
        /// Initializes a new instance of the Control class with specific text.
        /// </summary>
        /// <param name="text">The text value of the RoomRenderer. Does nothing!</param>
        public RoomRenderer(string? text = null)
            : this(null, text) { }
        /// <summary>
        /// Initializes a new RoomRenderer with default settings.
        /// </summary>
        public RoomRenderer()
            : this(null) { }

        #endregion
        #region Overrides
        /// <summary>
        /// Draws the room centered within the control's bounds.
        /// </summary>
        /// <param name="e">The provided event args.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Point filtering!
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            // Offsets all sampling by half a pixel. Read https://stackoverflow.com/a/54726707 to understand why this is important,
            //   there are some very clear images that make it easy to understand!
            graphics.PixelOffsetMode = PixelOffsetMode.Half;

            if (Room != null)
            {
                if (ShowTiles) DrawTiles(graphics);
                if (ShowProps) DrawProps(graphics);
                if (ShowTriggers) DrawTriggers(graphics);
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// Updates the display properties of the RoomRenderer after the control is resized.
        /// </summary>
        /// <param name="x">The new Left property value of the control.</param>
        /// <param name="y">The new Top property value of the control.</param>
        /// <param name="width">The new Width property value of the control.</param>
        /// <param name="height">The new Height property value of the control.</param>
        /// <param name="specified">A bitwise combination of the BoundsSpecified values.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            UpdateDisplayProperties();
        }

        #region Mouse Event Overrides
        /// <summary>
        /// Raises the TileMouseDown event.
        /// </summary>
        /// <param name="e">The provided event args.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // All of these methods are literally identical except for the specific event they raise
            Point? clicked = PixelSpaceToTileSpace(e.Location);
            if (clicked is Point tile)
            {
                Prop? prop = Room.GetPropAt(tile);

                TileMouseDown?.Invoke(this, new TileEventArgs(tile, prop, e));
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the TileMouseDown event.
        /// </summary>
        /// <param name="e">The provided event args.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point? clicked = PixelSpaceToTileSpace(e.Location);
            if (clicked is Point tile)
            {
                Prop? prop = Room.GetPropAt(tile);

                TileMouseMove?.Invoke(this, new TileEventArgs(tile, prop, e));
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Raises the TileMouseDown event.
        /// </summary>
        /// <param name="e">The provided event args.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            Point? clicked = PixelSpaceToTileSpace(e.Location);
            if (clicked is Point tile)
            {
                Prop? prop = Room.GetPropAt(tile);

                TileMouseUp?.Invoke(this, new TileEventArgs(tile, prop, e));
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Raises the TileMouseDown event.
        /// </summary>
        /// <param name="e">The provided event args.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point? clicked = PixelSpaceToTileSpace(e.Location);
            if (clicked is Point tile)
            {
                Prop? prop = Room.GetPropAt(tile);

                TileMouseClick?.Invoke(this, new TileEventArgs(tile, prop, e));
            }
            base.OnMouseClick(e);
        }

        /// <summary>
        /// Raises the TileMouseDown event.
        /// </summary>
        /// <param name="e">The provided event args.</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            Point? clicked = PixelSpaceToTileSpace(e.Location);
            if (clicked is Point tile)
            {
                Prop? prop = Room.GetPropAt(tile);

                TileMouseDoubleClick?.Invoke(this, new TileEventArgs(tile, prop, e));
            }
            base.OnMouseDoubleClick(e);
        }

        /// <summary>
        /// Raises the TileMouseDown event.
        /// </summary>
        /// <param name="e">The provided event args.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Point? clicked = PixelSpaceToTileSpace(e.Location);
            if (clicked is Point tile)
            {
                Prop? prop = Room.GetPropAt(tile);

                TileMouseWheel?.Invoke(this, new TileEventArgs(tile, prop, e));
            }
            base.OnMouseWheel(e);
        }

        #endregion

        #endregion
        #region Draw Methods
        /// <summary>
        /// Draws the tile grid to the screen.
        /// </summary>
        /// <param name="graphics">The graphics object to use to draw.</param>
        private void DrawTiles(Graphics graphics)
        {
            for (int tileY = 0; tileY < Room.Height; tileY++)
            {
                for (int tileX = 0; tileX < Room.Width; tileX++)
                {
                    Tile tile = Room[tileX, tileY];

                    // This helper method does the majority of the legwork here!
                    Rectangle drawRect = TileSpaceToPixelSpace(new Point(tileX, tileY));

                    graphics.DrawImage(tile.Sprite, drawRect);
                }
            }
        }

        /// <summary>
        /// Draws all of the props to the screen, including camera view frustums.
        /// </summary>
        /// <param name="graphics">The graphics object to use to draw.</param>
        private void DrawProps(Graphics graphics)
        {
            // The simple part of drawing! This just draws all of the prop sprites to their corresponding places
            //   Please take 15-20 seconds of silence to appreciate how simple and easy to understand this code is :)
            foreach (Prop prop in _room.Props)
            {
                Debug.Assert(prop.Position != null);

                Rectangle drawRect = TileSpaceToPixelSpace((Point)prop.Position);

                graphics.DrawImage(prop.Sprite, drawRect);
            }

            // The hellish part of drawing -- camera view frustums!
            foreach (Prop prop in _room.Props)
            {
                if (prop is Camera camera)
                {
                    // Boils down to "this camera is targeting a tile that makes sense"
                    if (camera.Target.HasValue && camera.Target.Value != camera.Position)
                    {
                        // Defines how the view frustum actually looks! Right now it's 50% opacity yellow
                        Brush brush = new SolidBrush(Color.FromArgb(122, Color.Yellow));

                        // The view frustum starts and ends at the center of the two tiles
                        //   I'm not sure if it should actually end at the *center* of the target or, like, the end of the target... but this is easier!
                        Point position = TileSpaceToPixelSpace((Point)camera.Position!).GetCenter();
                        Point target = TileSpaceToPixelSpace((Point)camera.Target!).GetCenter();

                    // Listen, I'm gonna be honest... if you stare at this code for long enough you could probably understand exactly why I made these decisions
                    //   But it works, okay? Don't think too hard about it
                    Point positionToTarget = target.Subtract(position);

                        /*
                         * Alright, let's talk about this offset
                         *   It's a tile-space offset depending on the direction the camera is pointing in
                         *   and the walls around it -- if the camera is pointing down, the offset is one tile down, etc.
                         *   The camera can't start its view vector inside of a wall, however
                         * This is a reimplementation of logic that exists in the main game to make the view frustum
                         *   in the editor slightly more realistic and easier to use
                         */
                        Point cameraTile = (Point)camera.Position;

                        // First check if a nudge in the given direction would push us out of bounds
                        //   (if it would, then that's effectively a wall), then check if the given
                        //   tile is a wall
                        bool[] isNotWall = [
                            cameraTile.X > 0               && Room[cameraTile.Add(new Point(-1, 0))].IsWalkable, // left
                            cameraTile.X < Room.Width - 1  && Room[cameraTile.Add(new Point(1, 0))].IsWalkable,  // right
                            cameraTile.Y > 0               && Room[cameraTile.Add(new Point(0, -1))].IsWalkable, // up
                            cameraTile.Y < Room.Height - 1 && Room[cameraTile.Add(new Point(0, 1))].IsWalkable,  // down
                            ];

                    Point startOffset;
                    // Checks are done in left, right, up, down order to exactly mimic the order they are performed in-game
                    if (isNotWall[0] && positionToTarget.X < 0) startOffset = new Point(-1, 0);      // left
                    else if (isNotWall[1] && positionToTarget.X > 0) startOffset = new Point(1, 0);  // right
                    else if (isNotWall[2] && positionToTarget.Y < 0) startOffset = new Point(0, -1); // up
                    else if (isNotWall[3] && positionToTarget.Y > 0) startOffset = new Point(0, 1);  // down
                    // If the camera has no valid offset, show it as just being drawn from the center of the camera
                    //   Is that *really* accurate? No. But it's fine enough
                    else startOffset = new Point(0, 0);

                    Point start = position.Add(startOffset.Multiply(_tileSize));
                    Point startToTarget = target.Subtract(start);
                    float angleToTargetRads = MathF.Atan2(startToTarget.Y, startToTarget.X);

                        // Okay from here on out it's gonna be a lot of geometry and you're gonna have to trust me a little
                        // Do I have a specific algorithm I'm implementing? Hell no! I winged this shit!
                        float diameter = start.Distance(target) * 2;

                        SizeF circleSize = new SizeF(diameter, diameter);

                        RectangleF fullCircle = new RectangleF(
                            PointF.Subtract(start, circleSize / 2), // The circle's center should be directly on top of the center of the camera
                            circleSize);

                        float startAngle = float.RadiansToDegrees(angleToTargetRads - camera.RadianSpread);
                        float sweepAngle = float.RadiansToDegrees(camera.RadianSpread * 2);

                        graphics.FillPie(brush, fullCircle, startAngle, sweepAngle);
                    }

                    // Lastly, draw its wire box (just so those are visible)
                    if (camera.WireBoxPosition != null)
                    {
                        Rectangle drawRect = TileSpaceToPixelSpace((Point)camera.WireBoxPosition);
                        graphics.DrawImage(_wireBoxSprite, drawRect);   
                    }
                }
            }
        }

        /// <summary>
        /// Draws all of the triggers to the screen.
        /// NOT IMPLEMENTED. DO NOT SET <c>ShowTriggers</c> TO TRUE.
        /// </summary>
        /// <param name="graphics">The graphics object to use to draw.</param>
        /// <exception cref="NotImplementedException">ALWAYS THROWN.</exception>
        private void DrawTriggers(Graphics graphics)
        {
            // The simple part of drawing! This just draws all of the **trigger** boxes to their corresponding places
            foreach (Trigger trigger in _room.Triggers)
            {
                Debug.Assert(trigger.Bounds != null); // I am moana
                Rectangle bounds = trigger.Bounds.Value;

                Size one = new Size(1, 1);

                // a union of the tip and bottom rectangles to create one mega rectangle extravaganza
                Rectangle drawRect = Rectangle.Union(TileSpaceToPixelSpace(bounds.Location), TileSpaceToPixelSpace(bounds.Location + bounds.Size - one));

                //let me have my headcannon
                Pen moana = new Pen(trigger.Color, 4);
                Brush maoi = new SolidBrush(Color.FromArgb(63, trigger.Color));

                graphics.DrawRectangle(moana, drawRect);
                graphics.FillRectangle(maoi, drawRect);
            }
        }

        #endregion 
        #region Helpers
        /// <summary>
        /// Gets the tile underneath a given pixel-space Point.
        /// </summary>
        /// <param name="position">The position, in pixel-space.</param>
        /// <returns>The found tile, if any, in tile-space.</returns>
        private Point? PixelSpaceToTileSpace(Point position)
        {
            Point hoveredTile = position
                .Subtract(_roomBounds.Location)
                .Divide(_tileSize);

            bool outOfBounds = hoveredTile.X < 0 || hoveredTile.Y < 0 || hoveredTile.X >= Room.Width || hoveredTile.Y >= Room.Height;

            return outOfBounds ? null : hoveredTile;
        }

        /// <summary>
        /// Gets the pixel-space rectangle of a tile given that tile's tile-space coordinate.
        /// </summary>
        /// <param name="tile">The coordinate of the tile.</param>
        /// <returns>The pixel-space rectangle of that tile.</returns>
        private Rectangle TileSpaceToPixelSpace(Point tile)
        {
            // Listen, this is some math that makes sense and is correct I promise
            return new Rectangle(
                new Point(
                    _tileSize * tile.X,
                    _tileSize * tile.Y
                    ).Add(_roomBounds.Location),
                new Size(_tileSize, _tileSize)
                );
        }

        /// <summary>
        /// Updates the <c>_roomBounds</c> and <c>_tileSize</c>.
        /// </summary>
        private void UpdateDisplayProperties()
        {
            if (Room != null)
            {
                int tileSize = Math.Min(
                    Bounds.Width / Room.Width,
                    Bounds.Height / Room.Height
                    );

                Size displaySize = new Size(
                    Room.Width * tileSize,
                    Room.Height * tileSize
                    );

                Size unusedSpace = Bounds.Size - displaySize;

                _roomBounds = new Rectangle(
                    (Point)(unusedSpace / 2), // The map display should be centered within the unused space
                    displaySize
                    );

                _tileSize = tileSize;
            }
            else
            {
                // If there is no room, then just set these to some defaults
                _roomBounds = Rectangle.Empty;
                _tileSize = 0;
            }

            // Any time these values change we need to refresh the rendered screen
            Invalidate();
        }

        /// <summary>
        /// Unsubscribes from all of the room's events.
        /// </summary>
        private void UnsubscribeRoomEvents()
        {
            if (_room != null)
            {
                _room.OnTileUpdated -= OnTileUpdated;
                _room.OnPropAdded -= OnPropChange;
                _room.OnPropRemoved -= OnPropChange;
                _room.OnCameraUpdate -= OnPropChange;

                _room.OnTriggerAdded -= OnTriggerChange;
                _room.OnTriggerRemoved -= OnTriggerChange;

                _room.OnRoomResized -= OnRoomResized;
            }
        }
        /// <summary>
        /// if a trigger is added / removed (i.e. there was a change), the current fporm is invalidated
        /// </summary>
        private void OnTriggerChange(Trigger obj)
        {
            Invalidate();
        }

        /// <summary>
        /// Subscribes to all of the room's events.
        /// </summary>
        private void SubscribeRoomEvents()
        {
            if (_room != null)
            {
                _room.OnTileUpdated += OnTileUpdated;
                _room.OnPropAdded += OnPropChange;
                _room.OnPropRemoved += OnPropChange;
                _room.OnCameraUpdate += OnPropChange;

                _room.OnTriggerAdded += OnTriggerChange;
                _room.OnTriggerRemoved += OnTriggerChange;

                _room.OnRoomResized += OnRoomResized;
            }
        }

        #endregion
        #region Event Callbacks
        /// <summary>
        /// Invalidates the relevant portion of the control.
        /// </summary>
        /// <param name="prop">The prop that has changed.</param>
        private void OnPropChange(Prop prop)
        {
            switch (prop.PropType)
            {
                case ObjectType.Item:
                case ObjectType.Box:
                case ObjectType.Door:
                    Invalidate(TileSpaceToPixelSpace((Point)prop.Position!));
                    break;

                case ObjectType.Camera:
                    // Cameras have view frustums, which would be really annoying to try and calculate an
                    //   invalidation rect for... so just redraw the whole thing it's fine
                    Invalidate();
                    break;
            }
        }

        /// <summary>
        /// Invalidates the updated tile.
        /// </summary>
        /// <param name="tileUpdatedCoords">The tile that was updated.</param>
        /// <param name="newTile">The new tile.</param>
        private void OnTileUpdated(Point tileUpdatedCoords, Tile newTile)
        {
            Invalidate();
        }

        /// <summary>
        /// Updates the display properties and invalidates the render.
        /// </summary>
        private void OnRoomResized()
        {
            UpdateDisplayProperties();
            Invalidate();
        }

        #endregion
    }
}
