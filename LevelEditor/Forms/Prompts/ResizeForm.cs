using LevelEditor.Classes.Props;
using LevelEditor.Classes;
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
    /// A form used to prompt the user to resize a room.
    /// </summary>
    public partial class ResizeForm : Form
    {
        private Tile _bg;

        /// <summary>
        /// Creates a new ResizeForm.
        /// </summary>
        /// <param name="room">The room to resize.</param>
        /// <param name="bg">The background tile to fill any new tiles with.</param>
        private ResizeForm(Room room, Tile bg)
        {
            InitializeComponent();

            roomRenderer.Room = room;
            _bg = bg;
        }

        /// <summary>
        /// Resizes the room with the inputted new dimensions and closes the form.
        /// </summary>
        private void buttonDone_Click(object sender, EventArgs e)
        {
            int top = intInputBoxTop.Input;
            int bottom = intInputBoxBottom.Input;
            int left = intInputBoxLeft.Input;
            int right = intInputBoxRight.Input;

            roomRenderer.Room.Resize(top, bottom, left, right, _bg);

            Close();
        }

        /// <summary>
        /// Opens a ResizeForm for the given room with the given background tile.
        /// </summary>
        /// <param name="room">The room to resize.</param>
        /// <param name="bg">The tile to set any new tiles to display.</param>
        public static void Prompt(Room room, Tile bg)
        {
            ResizeForm form = new ResizeForm(room, bg);
            form.ShowDialog();
        }
    }
}
