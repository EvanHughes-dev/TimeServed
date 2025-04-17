using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using LevelEditor.Extensions;
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
    /// <summary>
    /// A prompt that can be used to have a user select a position within a room.
    /// </summary>
    public partial class PositionSelectForm : Form
    {
        private static Point? _selected;

        /// <summary>
        /// Creates a new PositionSelectForm with the given room.
        /// </summary>
        /// <param name="room">The room to have the user select a position of.</param>
        private PositionSelectForm(Room room)
        {
            InitializeComponent();

            roomRenderer.Room = room;

            // Auto-formatting! Huzzah!
            Reformat();
        }

        /// <summary>
        /// Auto-formats the form.
        /// </summary>
        private void Reformat()
        {
            // These padding values just happen to look nice
            groupBoxMap.Bounds = ClientRectangle.NudgeSides(-5, -5, -5, -5);
            roomRenderer.Bounds = groupBoxMap.ClientRectangle.NudgeSides(-20, -5, -5, -5);
        }

        /// <summary>
        /// Selects the clicked tile and closes the form.
        /// </summary>
        private void RoomRenderer_TileMouseClick(object? sender, TileEventArgs e)
        {
            _selected = e.Tile;

            Close();
        }
        /// <summary>
        /// Reformats the form.
        /// </summary>
        private void PositionSelectForm_Resize(object sender, EventArgs e)
        {
            Reformat();
        }

        /// <summary>
        /// Opens a PositionSelectForm with the given room.
        /// </summary>
        /// <param name="room">The room to have the user select a position within.</param>
        /// <returns>The user's selected position, or null if they closed the form manually.</returns>
        public static Point? Prompt(Room room)
        {
            _selected = null!;

            PositionSelectForm form = new PositionSelectForm(room);
            form.ShowDialog();

            return _selected;
        }

    }
}
