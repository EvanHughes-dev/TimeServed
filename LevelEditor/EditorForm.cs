// Leah Crain
// 2/9/25
// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on

namespace LevelEditor
{
    /// <summary>
    /// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on.
    /// </summary>
    public partial class EditorForm : Form
    {
        /// <summary>
        /// Gets or sets the color the user currently has selected from the palette.
        /// </summary>
        private Color SelectedColor { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of grid tiles the user can draw on.
        /// </summary>
        private PictureBox[,] Tiles { get; set; }
        /// <summary>
        /// Gets the palette of colors the user may select from, hardcoded.
        /// </summary>
        private Color[,] Palette { get; } = { // Literally just colors I picked at random
            { Color.FloralWhite, Color.CornflowerBlue, Color.LightPink },
            { Color.LightSeaGreen, Color.YellowGreen, Color.Tomato },
        };

        /// <summary>
        /// Creates a new EditorForm, doing the work that is common between the two other constructors.
        /// </summary>
        private EditorForm()
        {
            InitializeComponent();

            // Creates all of the buttons in the palette
            CreateTilePalette();

            // Sets the selected color to some sort of reasonable default -- Palette[0, 0] is the only place we can guarantee there's a color in the palette
            //   (assuming the developer didn't change Palette to an entirely empty array)
            SelectedColor = Palette[0, 0];
        }

        /// <summary>
        /// Creates a new EditorForm with a blank canvas.
        /// </summary>
        /// <param name="width">The width of the new canvas.</param>
        /// <param name="height">The height of the new canvas.</param>
        public EditorForm(int width, int height) 
            : this()
        {
            CreateNewMap(width, height);
        }

        /// <summary>
        /// Creates a new EditorForm, loading from the given file path.
        /// </summary>
        /// <param name="filePath">The file path to load from.</param>
        public EditorForm(string filePath) 
            : this()
        {
            LoadRoomFromFile(filePath);
        }

        /// <summary>
        /// Creates the color selection buttons corresponding with the palette. Should only be run once!
        /// </summary>
        private void CreateTilePalette()
        {
            // Uses the dimensions from the hardcoded palette to determine the grid dimensions of the buttons
            // This is just the easiest way for me to program this, I think the ideal version of this would
            //   instead have Palette be a one-dimensional array and then automatically calculate the ideal
            //   grid dimensions to fit all of the colors... but that's hard and not necessary
            int width = Palette.GetLength(0);
            int height = Palette.GetLength(1);

            // 5-unit padding on each side and 20-unit padding on the top just happens to look nice
            Rectangle paletteBounds = PadRectInwards(tabPageTiles.ClientRectangle, 5, 5, 20, 5);

            // Give it 5 units of padding between each button just because it happens to look nice
            Button[,] swatches = GenerateGrid<Button>(width, height, paletteBounds, 5, tabPageTiles);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // We're going to store the color associated with each button in its back color, and then read the back color
                    //   when the button is clicked. Ideally we would track a color index so we could dramatically reduce the
                    //   file size of the saved maps, but again that's hard and not necessary
                    swatches[x, y].BackColor = Palette[x, y];
                    swatches[x, y].Click += Swatch_Click;
                }
            }
        }

        /// <summary>
        /// When a "swatch" button is clicked, select its color.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Control sender.</exception>
        private void Swatch_Click(object? sender, EventArgs e)
        {
            if (sender is not Control swatch) throw new Exception();

            SelectedColor = swatch.BackColor;
        }

        /// <summary>
        /// Creates a new blank map with the given dimensions.
        /// </summary>
        /// <param name="width">The width of the new map.</param>
        /// <param name="height">The height of the new map.</param>
        private void CreateNewMap(int width, int height)
        {
            // If Tiles isn't null, then this function has been called before and we need to delete (or re-use?) all of the prior existing tiles
            if (Tiles != null)
            {
                // It would be best to reuse all of the existing tiles (with like a pool system) but this is easier
                foreach (PictureBox tile in Tiles)
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

            // Padding of 5 units on each side and 20 units on the top happens to look good
            Rectangle mapBounds = PadRectInwards(groupBoxMap.ClientRectangle, 5, 5, 20, 5);
            Tiles = GenerateGrid<PictureBox>(width, height, mapBounds, parent: groupBoxMap);

            foreach (PictureBox tile in Tiles)
            {
                // Palette[0, 0] can be safely assumed to be *a* color at least, so it makes for a good default background color
                tile.BackColor = Palette[0, 0];
                tile.MouseDown += Tile_MouseDown;
                tile.MouseMove += Tile_MouseMove;
            }
        }

        /// <summary>
        /// When a tile is clicked, either change its color or select its color (for left and right click, respectively).
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Control sender.</exception>
        private void Tile_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not Control tile) throw new Exception();

            // We have to disable this control capturing the mouse so that future MouseMove events can be fired on *other* tiles
            //   (to make clicking and dragging work)
            tile.Capture = false;

            if (e.Button == MouseButtons.Left)
                tile.BackColor = SelectedColor;

            // Right click picks color! Because that's convenient and I wanted it to be a feature!
            if (e.Button == MouseButtons.Right)
                SelectedColor = tile.BackColor;
        }

        /// <summary>
        /// When the mouse moves over top of a tile, if left click is pressed, change its color.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Control sender.</exception>
        private void Tile_MouseMove(object? sender, MouseEventArgs e)
        {
            if (sender is not Control tile) throw new Exception();

            if (e.Button == MouseButtons.Left)
            {
                tile.BackColor = SelectedColor;
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
            T[,] controls = new T[width, height];

            int controlWidth = rect.Width / width;
            int controlHeight = rect.Height / height;

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

                    controls[x, y] = control;
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
        /// <returns></returns>
        private static Rectangle PadRectInwards(Rectangle rect, int padLeft, int padRight, int padTop, int padBottom)
        {
            return new Rectangle(rect.Left + padLeft,
                rect.Top + padTop,
                rect.Width - padLeft - padRight,
                rect.Height - padTop - padBottom);
        }

        /// <summary>
        /// Prompts the user with a SaveFileDialog and then saves to that file.
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                SaveRoomToFile(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Prompts the user with an OpenFileDialog and then loads from that file.
        /// </summary>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                LoadRoomFromFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Saves the current map to a given file path.
        /// </summary>
        /// <param name="path">The path to save to.</param>
        private void SaveRoomToFile(string path)
        {
            // File IO is always completely unsafe, so we try-catch everything
            try
            {
                /*
                 * THE .room FILE FORMAT:
                 *  - int width
                 *  - int height
                 *  - Several tiles, with the following format:
                 *    - bit isWalkable (1 == walkable, 0 == not walkable)
                 *    - int tileIndex
                 *  - int numOfProps
                 *  - Several props, with the following format:
                 *    - int propIndex
                 *    - int positionX
                 *    - int positionY
                 *    - bit isDoor (1 == this is a door, 0 == this is not a door)
                 *    - If isDoor is 1, then it also includes the following data:
                 *      - int entranceIndex
                 *      - int destinationRoom
                 *      - int destinationDoor
                 */
                BinaryWriter writer = new(new FileStream(path, FileMode.Create));

                int width = Tiles.GetLength(0);
                int height = Tiles.GetLength(1);

                writer.Write(width);
                writer.Write(height);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        writer.Write(Tiles[x, y].BackColor.ToArgb());
                    }
                }

                writer.Close();

                MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file. {ex.Message}", "Error Saving File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads a map from a file.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        private void LoadRoomFromFile(string path)
        {
            // File IO is always completely unsafe, so we try-catch everything
            try
            {
                // Read the file format from above!
                BinaryReader reader = new(new FileStream(path, FileMode.Open));

                int width = reader.ReadInt32();
                int height = reader.ReadInt32();

                CreateNewMap(width, height);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Tiles[x, y].BackColor = Color.FromArgb(reader.ReadInt32());
                    }
                }

                reader.Close();

                MessageBox.Show("File loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file. {ex.Message}", "Error Loading File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
