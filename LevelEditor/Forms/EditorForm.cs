// Leah Crain
// 2/9/25
// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on

using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LevelEditor
{
    /// <summary>
    /// Keeps track of tab state
    /// </summary>
    enum TabState
    {
        Tiles,
        Props
    };

    /// <summary>
    /// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on.
    /// </summary>
    public partial class EditorForm : Form
    {
        // A reference to the MainForm that created this EditorForm
        private readonly MainForm _mainForm;

        private TabState _tabState;

        /// <summary>
        /// Gets or sets the 2D array of grid tiles the user can draw on.
        /// </summary>
        private TileBox[,] TileGrid { get; set; }

        /// <summary>
        /// Gets or sets the Room this editor is editing.
        /// </summary>
        private Room Room { get; set; }

        // <summary>
        /// Gets or sets the color the user currently has selected from the palette.
        /// </summary>
        private Tile SelectedTile { get; set; }
        /// <summary>
        /// Gets the palette of tiles the user may select from.
        /// </summary>
        private Tile[,] Palette { get; }

        /// <summary>
        /// Gets or sets the porp texture the user currently has selected from the palette.
        /// </summary>
        private Prop SelectedProp { get; set; }
        /// <summary>
        /// Gets the palette of props the user may select from
        /// </summary>
        private Prop[,] PropPalette { get; }
        /// <summary>
        /// Holds the prop boxes that exist in the room
        /// </summary>
        private List<PropBox> _propBoxesInRoom;
        /// <summary>
        /// Creates a new EditorForm, editing an existing room.
        /// </summary>
        private EditorForm(MainForm mainForm)
        {
            InitializeComponent();

            _mainForm = mainForm;

            _tabState = TabState.Tiles;

            int numOfTiles = _mainForm.Tiles.Count;

            int numOfRows = (int)Math.Ceiling((float)numOfTiles / 2);
            Palette = new Tile[numOfRows, 2];

            for (int i = 0; i < numOfTiles; i++)
            {
                int y = (int)Math.Floor((float)i / 2);
                int x = i % 2;

                Palette[y, x] = _mainForm.Tiles.ElementAt(i);
            }

            // Creates all of the buttons in the palette
            CreatePaletteTileBoxes();

            // Sets the selected color to some sort of reasonable default -- Palette[0, 0] is the only place we can guarantee there's a color in the palette
            //   (assuming the developer didn't change Palette to an entirely empty array)
            SelectedTile = Palette[0, 0];


            int numOfProps = _mainForm.Props.Count;

            numOfRows = (int)Math.Ceiling((float)numOfProps / 2);
            PropPalette = new Prop[numOfRows, 2];

            for (int i = 0; i < numOfProps; i++)
            {
                int y = (int)Math.Floor((float)i / 2);
                int x = i % 2;

                PropPalette[y, x] = _mainForm.Props.ElementAt(i);
            }

            // Creates all of the buttons in the palette
            CreatePalettePropBoxes();

            // Sets the selected color to some sort of reasonable default -- Palette[0, 0] is the only place we can guarantee there's a color in the palette
            //   (assuming the developer didn't change Palette to an entirely empty array)
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
            Room = new(name, width, height, mainForm.Tiles.ElementAt(0));
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
            InitializeMap(Room);
        }

        #region Create Palettes

        /// <summary>
        /// Creates the color selection buttons corresponding with the palette. Should only be run once!
        /// </summary>
        private void CreatePaletteTileBoxes()
        {
            // Shortcut variables for the width and height
            int height = Palette.GetLength(0);
            int width = Palette.GetLength(1);

            // 5-unit padding on each side and 20-unit padding on the top just happens to look nice
            Rectangle paletteBounds = PadRectInwards(tabPageTiles.ClientRectangle, 5, 5, 20, 5);

            // Give it 5 units of padding between each button just because it happens to look nice
            TileBox[,] swatches = GenerateGrid<TileBox>(width, height, paletteBounds, 5, tabPageTiles);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // We're going to store the color associated with each button in its back color, and then read the back color
                    //   when the button is clicked. Ideally we would track a color index so we could dramatically reduce the
                    //   file size of the saved maps, but again that's hard and not necessary
                    swatches[y, x].Tile = Palette[y, x];
                    swatches[y, x].Click += Swatch_Click;

                    swatches[y, x].SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }
        /// <summary>
        /// Creates the prop selection buttons corresponding with the &*%*@^(^#_. Should only be run once!
        /// </summary>
        private void CreatePalettePropBoxes() //this will be difficult to implement until I have tile info
        {
            // Shortcut variables for the width and height
            int height = PropPalette.GetLength(0);
            int width = PropPalette.GetLength(1);

            // 5-unit padding on each side and 20-unit padding on the top just happens to look nice
            Rectangle paletteBounds = PadRectInwards(tabPageProps.ClientRectangle, 5, 5, 20, 5);

            // Give it 5 units of padding between each button just because it happens to look nice
            PropBox[,] toolBox = GenerateGrid<PropBox>(width, height, paletteBounds, 5, tabPageProps);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // We're going to store the color associated with each button in its back color, and then read the back color
                    //   when the button is clicked. Ideally we would track a color index so we could dramatically reduce the
                    //   file size of the saved maps, but again that's hard and not necessary
                    toolBox[y, x].Prop = PropPalette[y, x];
                    toolBox[y, x].Click += PropSwatch_Click;

                    toolBox[y, x].SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        #endregion

        /// <summary>
        /// Immediately after the form finishes loading.
        /// </summary>
        private void EditorForm_Load(object sender, EventArgs e)
        {
            // Left... semi-intentionally blank? This function is completely unnecessary
            //   but deleting it would mean I'd have to fix the designer file and I
            //   don't wanna have to deal with that hassle. Maybe it'll have a use!
        }

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
        /// When a prop button is clicked, select its color.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Button sender.</exception>
        private void PropSwatch_Click(object? sender, EventArgs e)
        {
            if (sender is not PropBox prop) throw new Exception();

            SelectedProp = prop.Prop;
        }

        #endregion

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
                }
            }
        }

        #region Tile Events

        /// <summary>
        /// When a tile is clicked, either change its color or select its color (for left and right click, respectively).
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Control sender.</exception>
        private void Tile_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not TileBox tile) throw new Exception();

            if (_tabState == TabState.Tiles)
            {
                // We have to disable this control capturing the mouse so that future MouseMove events can be fired on *other* tiles
                //   (to make clicking and dragging work)
                tile.Capture = false;

                (int y, int x) = TileGrid.IndexesOf(tile);

                if (e.Button == MouseButtons.Left)
                {
                    tile.Tile = SelectedTile;

                    Room.Tiles[y, x] = SelectedTile;
                }

                // Right click picks color! Because that's convenient and I wanted it to be a feature!
                if (e.Button == MouseButtons.Right)
                    SelectedTile = tile.Tile;
            }
            else if (_tabState == TabState.Props)
            {
                // We have to disable this control capturing the mouse so that future MouseMove events can be fired on *other* tiles
                //   (to make clicking and dragging work)
                //tile.Capture = false;


                (int y, int x) = TileGrid.IndexesOf(tile);



                if (e.Button == MouseButtons.Left)
                {
                    PropBox proppy = new PropBox()
                    {
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Parent = tile,
                        Location = new Point(0,0),
                        Size = tile.Size,
                        BackColor = Color.Transparent
                    };

                    tile.Controls.Add(proppy);

                    proppy.Prop = SelectedProp;
                    proppy.BringToFront();
                    proppy.MouseDown += PropBox_MouseDown;
                    Room.Props.Add(SelectedProp);
                    _propBoxesInRoom.Add(proppy);
                }
            }
        }
        private void PropBox_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not PropBox proppy) throw new Exception();

            if (e.Button == MouseButtons.Right)
            {
                proppy.Parent?.Controls.Remove(proppy);
                _propBoxesInRoom.Remove(proppy);
                proppy.Dispose();
            }
        }

        /// <summary>
        /// When the mouse moves over top of a tile, if left click is pressed, change its color.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Control sender.</exception>
        private void Tile_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not TileBox tile) throw new Exception();
            if (_tabState == TabState.Tiles)
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
        #region Helpers

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

            int controlWidth = rect.Width / width;
            int controlHeight = rect.Height / height;

            // The grid is generated in columns, from left to right and top to bottom
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    T control = new();
                    Controls.Add(control);

                    // Doesn't matter that the parent argument might be null here -- control.Parent is nullable already!
                    control.Parent = parent;

                    control.SetBounds(x * controlWidth + rect.Left + padding,
                        y * controlHeight + rect.Top + padding,
                        controlWidth - padding,
                        controlHeight - padding);

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

        private void tabControlTilesProps_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (sender is not TabControl tabby) throw new Exception();

            TabState newState = (TabState)tabby.SelectedIndex;

            switch (newState)
            {
                case TabState.Tiles:
                    foreach (PropBox proppy in _propBoxesInRoom)
                    {
                        proppy.Enabled = false;
                    }
                    break;
                case TabState.Props:
                    foreach (PropBox proppy in _propBoxesInRoom)
                    {
                        proppy.Enabled = true;
                    }
                    break;
                default:
                    break;
            }

            _tabState = newState;
        }
    }
}
