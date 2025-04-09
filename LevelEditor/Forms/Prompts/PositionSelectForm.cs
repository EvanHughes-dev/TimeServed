using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor.Forms.Prompts
{
    public partial class PositionSelectForm : Form
    {
        private Point? _selected;

        public PositionSelectForm(Room room)
        {
            InitializeComponent();

            _selected = null;

            InitializeMap(room);
        }

        public Point? Prompt()
        {
            ShowDialog();

            return _selected;
        }

        private EventHandler MakeClickCallback(Point point)
        {
            return (object? _, EventArgs _) =>
            {
                _selected = point;
            };
        }

        #region Helpers
        /// <summary>
        /// Initializes the grid of TileBoxes and sets their display to facilitate editing of a given Room.
        /// </summary>
        /// <param name="room">The room to be edited.</param>
        private void InitializeMap(Room room)
        {
            int height = room.Tiles.GetLength(0);
            int width = room.Tiles.GetLength(1);

            // We need the overall form window to adapt to the size of the map, so if we measure how much the groupBoxMap changes
            //   width then we can apply that same width change to the form window
            int priorWidth = groupBoxMap.Width;

            groupBoxMap.Width = groupBoxMap.Height / height * width;

            int widthChange = groupBoxMap.Width - priorWidth;
            Width += widthChange;

            // Padding of 5 units on each side and 20 units on the top just happens to look good
            Rectangle mapBounds = PadRectInwards(groupBoxMap.ClientRectangle, 5, 5, 20, 5);
            TileBox[,] tileGrid = GenerateGrid<TileBox>(width, height, mapBounds, parent: groupBoxMap);

            // Now, lastly, we set up every individual TileBox to show the proper tile
            //   and respond properly when clicked on or dragged over
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileBox tileBox = tileGrid[y, x];
                    tileBox.Tile = room.Tiles[y, x];
                    tileBox.Click += MakeClickCallback(new Point(x, y));
                }
            }

            foreach (Prop prop in room.Props)
            {
                if (prop.Position.HasValue)
                {
                    Point propPosition = new Point(prop.Position.Value.X, prop.Position.Value.Y);
                    TileBox tile = tileGrid[propPosition.Y, propPosition.X];
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

                    proppy.Enabled = false;
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
    }
}
