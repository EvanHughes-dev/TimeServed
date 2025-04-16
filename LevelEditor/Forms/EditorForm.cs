// Leah Crain
// 2/9/25
// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on

using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using LevelEditor.Forms.Prompts;

namespace LevelEditor
{
    /// <summary>
    /// Keeps track of tab state
    /// </summary>
    internal enum TabState
    {
        Tiles,
        Props
    };

    /// <summary>
    /// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on.
    /// </summary>
    public partial class EditorForm : Form
    {
        #region Fields and Properties
        // A reference to the MainForm that created this EditorForm
        private readonly MainForm _mainForm;

        /// <summary>
        /// The Room this editor is editing.
        /// </summary>
        private Room Room { get; }

        /// <summary>
        /// Which tab is currently open within the EditorForm
        /// </summary>
        private TabState TabState { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of grid tiles the user can draw on.
        /// </summary>
        private TileBox[,] TileGrid { get; set; }

        /// <summary>
        /// Gets or sets the color the user currently has selected from the palette.
        /// </summary>
        private Tile SelectedTile { get; set; }
        /// <summary>
        /// The palette of tiles the user may select from.
        /// </summary>
        private Tile[,] TilePalette { get; }

        /// <summary>
        /// Gets or sets the prop the user currently has selected from the palette.
        /// </summary>
        private Prop SelectedProp { get; set; }
        /// <summary>
        /// Gets the palette of props the user may select from
        /// </summary>
        private Prop[,] PropPalette { get; }

        /// <summary>
        /// Holds the prop box 
        /// </summary>
        private PropBox _currentlySelectedPropBox;
        /// <summary>
        /// Holds the prop boxes that exist in the room
        /// </summary>
        private readonly List<PropBox> _propBoxesInRoom;

        #endregion
        #region Constructors
        /// <summary>
        /// Creates a new EditorForm, editing an existing room.
        /// </summary>
        private EditorForm(MainForm mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;

            TabState = TabState.Tiles;

            // All the palettes are created in a very similar way, but this lets us have extra control should we need it
            TilePalette = CreatePalette<Tile, TileBox>(_mainForm.Tiles, 3, tabPageTiles,
                (tileBox, tile) =>
                {
                    // This will be run once for every TileBox in the palette!
                    tileBox.Tile = tile;
                    tileBox.Click += Swatch_Click;

                    // Ensures that the tile sprite will be shrunk or enlarged to fit within the TileBox
                    tileBox.SizeMode = PictureBoxSizeMode.Zoom;
                });

            // Sets the selected tile to some sort of reasonable default -- _tilePalette[0, 0] is the only place we can guarantee there's a tile in the palette
            SelectedTile = TilePalette[0, 0];


            PropPalette = CreatePalette<Prop, PropBox>(_mainForm.Props, 3, tabPageProps,
                (propBox, prop) =>
                {
                    propBox.Prop = prop;
                    propBox.Click += PropSwatch_Click;

                    propBox.SizeMode = PictureBoxSizeMode.Zoom;
                });

            // Sets the selected prop to some sort of reasonable default -- _propPalette[0, 0] is the only place we can guarantee there's a prop in the palette
            SelectedProp = PropPalette[0, 0];

            _propBoxesInRoom = [];
        }

        /// <summary>
        /// Creates a new EditorForm by creating a new room.
        /// </summary>
        /// <param name="mainForm">A reference to the MainForm.</param>
        /// <param name="name">The name of the new room.</param>
        /// <param name="width">The width of the new room.</param>
        /// <param name="height">The height of the new room.</param>
        public EditorForm(MainForm mainForm, string name, int width, int height)
            : this(mainForm)
        {
            Room = new Room(name, width, height, mainForm.Tiles.ElementAt(0));
            InitializeMap(Room);

            _mainForm.AddNewRoom(Room);
        }

        /// <summary>
        /// Creates a new EditorForm with an existing Room.
        /// </summary>
        /// <param name="mainForm">A reference to the MainForm.</param>
        /// <param name="room">An existing Room to edit.</param>
        public EditorForm(MainForm mainForm, Room room)
            : this(mainForm)
        {
            Room = room;

            RoomRenderer renderer = new RoomRenderer();
            renderer.Room = room;
            groupBoxMap.Controls.Add(renderer);
            renderer.Bounds = PadRectInwards(groupBoxMap.ClientRectangle, 5, 5, 20, 5);
        }

        #endregion
        #region Swatch Events
        /// <summary>
        /// When a "swatch" button is clicked, select its color.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Button sender.</exception>
        private void Swatch_Click(object? sender, EventArgs e)
        {
            if (sender is not TileBox swatch) throw new Exception();

            SelectedTile = swatch.Tile;
        }
        /// <summary>
        /// When a prop button is clicked, select its object.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Button sender.</exception>
        private void PropSwatch_Click(object? sender, EventArgs e)
        {
            if (sender is not PropBox prop) throw new Exception("Invalid call to PropSwatch_Click");

            if (_currentlySelectedPropBox != null) _currentlySelectedPropBox.BackColor = Color.Transparent;
            prop.BackColor = Color.Honeydew;
            _currentlySelectedPropBox = prop;
            SelectedProp = prop.Prop;
        }

        #endregion
        #region Tile Events
        /// <summary>
        /// When a tile is clicked, either change its color or select its color (for left and right click, respectively).
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Control sender.</exception>
        private void Tile_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not TileBox tile) throw new Exception();

            if (TabState == TabState.Tiles)
            {
                // We have to disable this control capturing the mouse so that future MouseMove events can be fired on *other* tiles
                //   (to make clicking and dragging work)
                tile.Capture = false;

                (int tileY, int tileX) = TileGrid.IndexesOf(tile);

                if (e.Button == MouseButtons.Left)
                {
                    tile.Tile = SelectedTile;

                    Room.Tiles[tileY, tileX] = SelectedTile;
                }

                // Right click picks color! Because that's convenient and I wanted it to be a feature!
                if (e.Button == MouseButtons.Right)
                    SelectedTile = tile.Tile;
            }
            else if (TabState == TabState.Props)
            {
                // We have to disable this control capturing the mouse so that future MouseMove events can be fired on *other* tiles
                //   (to make clicking and dragging work)
                //tile.Capture = false;

                (int y, int x) = TileGrid.IndexesOf(tile);

                if (e.Button == MouseButtons.Left)
                {
                    Prop createdProp = null!;

                    // Different props have to be created in different ways
                    switch (SelectedProp.PropType)
                    {
                        case ObjectType.Item: // Items and boxes are created the same
                        case ObjectType.Box:
                            createdProp = SelectedProp.Instantiate(new Point(x, y));
                            break;

                        case ObjectType.Door:
                            Door door = (Door)SelectedProp;

                            Room room = RoomSelectForm.Prompt(_mainForm.GetAllRooms());
                            if (room == null) return;

                            Point? destination = PositionSelectForm.Prompt(room);
                            if (destination == null) return;

                            createdProp = door.Instantiate(new Point(x, y), (Point)destination, room.Id);
                            break;

                        case ObjectType.Camera:
                            Camera camera = (Camera)SelectedProp;

                            CameraForm.Prompt(Room, camera, new Point(x, y));

                            break;
                    }

                    //Propy is ALive!! They gets all of tile's crap like a younger sibling does
                    PropBox proppy = new PropBox()
                    {
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Parent = tile,
                        Location = new Point(0, 0),
                        Size = tile.Size,
                        BackColor = Color.Transparent, //and their parent is trans apparently
                        Prop = createdProp
                    };

                    tile.Controls.Add(proppy);

                    proppy.BringToFront();
                    proppy.MouseDown += PropBox_MouseDown;

                    Room.Props.Add(proppy.Prop);
                    _propBoxesInRoom.Add(proppy);
                }
            }
        }

        /// <summary>
        /// When clicking on a PropBox
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-PropBox sender</exception>
        private void PropBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not PropBox proppy) throw new Exception("Invalid sender to PropBox_MouseDown");

            if (e.Button == MouseButtons.Right) //Right click will...
            {
                proppy.Parent?.Controls.Remove(proppy); //Remove proppy form controls
                Room.Props.Remove(proppy.Prop);   //Remove proppy from the actual room list
                _propBoxesInRoom.Remove(proppy);  //Remove proppy form the form's room list
                proppy.Dispose();                 //REMOVES PROPPY FROM LIFE!!! MWAHAHAHAHHA
            }
        }

        /// <summary>
        /// When the mouse moves over top of a tile, if left click is pressed, change its color.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Control sender.</exception>
        private void Tile_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not TileBox tile) throw new Exception("Invalid sender to Tile_MouseMove");
            if (TabState == TabState.Tiles)
            {
                (int y, int x) = TileGrid.IndexesOf(tile);

                if (e.Button == MouseButtons.Left)
                {
                    tile.Tile = SelectedTile;

                    Room.Tiles[y, x] = SelectedTile;
                }
            }
        }

        #endregion
        #region Tab Events
        /// <summary>
        /// Changes the state based on the current tab selected and decides what to do with that info
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void tabControlTilesProps_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (sender is not TabControl tabby) throw new Exception("Invalid sender to tabControlTilesProps_SelectedIndexChanged");

            TabState newState = (TabState)tabby.SelectedIndex;

            switch (newState)
            {
                case TabState.Tiles:
                    foreach (PropBox proppy in _propBoxesInRoom)
                    {
                        proppy.Enabled = false; //So that tiles can be drawn underneath the props
                    }
                    break;
                case TabState.Props:
                    foreach (PropBox proppy in _propBoxesInRoom)
                    {
                        proppy.Enabled = true; //Props can now be interacted with
                    }
                    break;
            }

            TabState = newState;
        }
        #endregion
        #region Helpers
        /// <summary>
        /// Creates a palette the user can utilize to select different elements.
        /// </summary>
        /// <typeparam name="TValue">The value of the data used for the palette.</typeparam>
        /// <typeparam name="TControl">The type of control the palette should be built out of.</typeparam>
        /// <param name="elements">The elements of the palette.</param>
        /// <param name="columns">How many columns the palette should have.</param>
        /// <param name="parent">The common parent that each control of the palette should have.</param>
        /// <param name="setupCallback">A callback to call to setup each control of the palette.</param>
        /// <returns>The values of the created palette.</returns>
        private TValue[,] CreatePalette<TValue, TControl>(IEnumerable<TValue> elements, int columns, Control parent, Action<TControl, TValue> setupCallback)
            where TControl : Control, new()
        {
            int count = elements.Count();

            // Divides the given elements into several rows with the given number of columns
            //   For example, if there are 10 elements and we should have 4 columns, ceil(10 / 4) = 3 rows
            int numOfRows = (int)Math.Ceiling((float)count / columns);
            TValue[,] palette = new TValue[numOfRows, columns];

            // Linearly copies every element from the given elements to the new 2D palette array
            for (int i = 0; i < count; i++)
            {
                int row = (int)Math.Floor((float)i / columns);
                int column = i % columns;

                palette[row, column] = elements.ElementAt(i);
            }

            // Next, we have to create the necessary controls so the user can interact with the palette
            int height = numOfRows;
            int width = columns;

            // Padding is currently hardcoded because I didn't feel like adding 5 more arguments to this function...
            //   it's not the cleanest but it's fine for now
            Rectangle bounds = PadRectInwards(parent.ClientRectangle, 5, 5, 20, 5);
            TControl[,] swatches = GenerateGrid<TControl>(width, height, bounds, 5, parent);

            // Setup each swatch using the user-provided callback
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    setupCallback(swatches[y, x], palette[y, x]);
                }
            }

            return palette;
        }

        /// <summary>
        /// Initializes the grid of TileBoxes and sets their display to facilitate editing of a given Room.
        /// </summary>
        /// <param name="room">The room to be edited.</param>
        private void InitializeMap(Room room)
        {
            int height = room.Tiles.GetLength(0);
            int width = room.Tiles.GetLength(1);

            // If Tiles isn't null, then this function has been called before and we need to delete (or re-use?) all of the prior existing tiles
            if (TileGrid != null)
            {
                // It would be best to reuse all of the existing tiles (with like a pool system) but this is easier
                foreach (TileBox tile in TileGrid)
                {
                    // I'm not totally sure what the best thing to do is to destroy these tiles, and this does still seem to cause a memory leak...
                    //   but this is good enough I suppose
                    Controls.Remove(tile);
                    tile.Dispose();
                }
            }

            // We need the overall form window to adapt to the size of the map, so if we measure how much the groupBoxMap changes
            //   width then we can apply that same width change to the form window
            int priorWidth = groupBoxMap.Width;

            groupBoxMap.Width = groupBoxMap.Height / height * width;

            int widthChange = groupBoxMap.Width - priorWidth;
            Width += widthChange;

            // Padding of 5 units on each side and 20 units on the top just happens to look good
            Rectangle mapBounds = PadRectInwards(groupBoxMap.ClientRectangle, 5, 5, 20, 5);
            TileGrid = GenerateGrid<TileBox>(width, height, mapBounds, parent: groupBoxMap);

            // Now, lastly, we set up every individual TileBox to show the proper tile
            //   and respond properly when clicked on or dragged over
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileBox tileBox = TileGrid[y, x];
                    tileBox.Tile = Room.Tiles[y, x];
                    tileBox.MouseDown += Tile_MouseDown;
                    tileBox.MouseMove += Tile_MouseMove;

                    tileBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }

            foreach (Prop prop in Room.Props)
            {
                if (prop.Position.HasValue)
                {
                    Point propPosition = new Point(prop.Position.Value.X, prop.Position.Value.Y);
                    TileBox tile = TileGrid[propPosition.Y, propPosition.X];
                    //Propy is ALive!! They gets all of tile's crap like a younger sibling does
                    PropBox proppy = new PropBox()
                    {
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Parent = tile,
                        Location = new Point(0, 0),
                        Size = tile.Size,
                        BackColor = Color.Transparent //and their parent is trans apparently
                    };


                    tile.Controls.Add(proppy);

                    proppy.Prop = prop;
                    proppy.BringToFront();
                    proppy.MouseDown += PropBox_MouseDown;

                    _propBoxesInRoom.Add(proppy);
                }
            }

        }

        /// <summary>
        /// Creates a uniform grid of controls following a number of parameters.
        /// </summary>
        /// <typeparam name="T">The type of control to create a grid of.</typeparam>
        /// <param name="width">The width of the grid to create, in "number of controls".</param>
        /// <param name="height">The height of the grid to create, in "number of controls".</param>
        /// <param name="rect">The rectangle to fit the grid in. The individual controls will be resized to fit the grid entirely within this rectangle.</param>
        /// <param name="padding">The padding to apply between each individual control.</param>
        /// <param name="parent">A Control to make all of the grid elements a child of.</param>
        /// <returns>A 2D array with all of the controls in the grid.</returns>
        private T[,] GenerateGrid<T>(int width, int height, Rectangle rect, int padding = 0, Control? parent = null) where T : Control, new()
        {
            T[,] controls = new T[height, width];

            int controlSize = Math.Min(rect.Width / width, rect.Height / height);

            // The grid is generated in columns, from left to right and top to bottom
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    T control = new T();
                    Controls.Add(control);

                    // Doesn't matter that the parent argument might be null here -- control.Parent is nullable already!
                    control.Parent = parent;

                    control.SetBounds((x * controlSize) + rect.Left + padding,
                        (y * controlSize) + rect.Top + padding,
                        controlSize - padding,
                        controlSize - padding);

                    controls[y, x] = control;
                }
            }

            return controls;
        }

        /// <summary>
        /// Takes a rectangle and pads each individual side inwards by the given amounts.
        /// </summary>
        /// <param name="rect">The rectangle to pad.</param>
        /// <param name="padLeft">How far to move the left side inwards.</param>
        /// <param name="padRight">How far to move the right side inwards.</param>
        /// <param name="padTop">How far to move the top side inwards.</param>
        /// <param name="padBottom">How far to move the bottom side inwards.</param>
        /// <returns>The padded rectangle.</returns>
        private static Rectangle PadRectInwards(Rectangle rect, int padLeft, int padRight, int padTop, int padBottom)
        {
            return new Rectangle(rect.Left + padLeft,
                rect.Top + padTop,
                rect.Width - padLeft - padRight,
                rect.Height - padTop - padBottom);
        }

        #endregion

        private void EditorForm_Resize(object sender, EventArgs e)
        {
            
        }
    }
}
