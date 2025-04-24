using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using LevelEditor.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
    public partial class CameraForm : Form
    {
        private readonly Camera _camera;

        // The last valid input to the textBoxSpread
        private string _lastValidInput;

        /// <summary>
        /// Creates a new CameraForm with the given room.
        /// </summary>
        /// <param name="room">The room to have the user select a position of.</param>
        private CameraForm(Room room, Camera camera)
        {
            InitializeComponent();

            // Save params
            _camera = camera;
            roomRenderer.Room = room;

            // For some reason you can't subscribe to this event in the designer, so just do it here
            textBoxSpread.LostFocus += TextBoxSpread_LostFocus;

            // Set the text box and track bar to show the camera's existing RadianSpread
            float degreeSpread = float.RadiansToDegrees(camera.RadianSpread);
            textBoxSpread.Text = degreeSpread.ToString();
            trackBarSpread.Value = int.Clamp((int)degreeSpread, trackBarSpread.Minimum, trackBarSpread.Maximum);

            _lastValidInput = textBoxSpread.Text;

            // Automatically size all of this form's components
            Reformat();
        }

        /// <summary>
        /// Updates the auto-resizing components of this form.
        /// </summary>
        private void Reformat()
        {
            // The splitContainer should just cover the entire form
            splitContainer.Bounds = ClientRectangle;

            // The map should take up the entirety of the top panel of the split container
            groupBoxMap.Bounds = splitContainer.Panel1.ClientRectangle;
            roomRenderer.Bounds = groupBoxMap.ClientRectangle.NudgeSides(-20, -5, -5, -5); // This padding looks nice
        }

        /// <summary>
        /// Updates the camera's RadianSpread property based on the input to the text box.
        /// </summary>
        private void UpdateSpreadFromText()
        {
            bool validInput = float.TryParse(textBoxSpread.Text, out float degSpread);

            if (validInput)
            {
                _camera.RadianSpread = float.DegreesToRadians(degSpread);

                trackBarSpread.Value = int.Clamp((int)float.Round(degSpread), trackBarSpread.Minimum, trackBarSpread.Maximum);

                _lastValidInput = textBoxSpread.Text;
            }
            else
            {
                // If the user typed something that can't be parsed to a float, we want to undo that work
                textBoxSpread.Text = _lastValidInput;
            }
        }

        #region Event Handlers
        /// <summary>
        /// Reformats the form.
        /// </summary>
        private void CameraForm_Resize(object sender, EventArgs e)
        {
            Reformat();
        }

        /// <summary>
        /// Reformats the form.
        /// </summary>
        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Reformat();
        }

        /// <summary>
        /// Closes the form.
        /// </summary>
        private void buttonDone_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Updates the text box and the camera's radian spread.
        /// </summary>
        private void trackBarSpread_Scroll(object sender, EventArgs e)
        {
            if (sender is not TrackBar bar) throw new Exception();

            textBoxSpread.Text = bar.Value.ToString();

            _camera.RadianSpread = float.DegreesToRadians(bar.Value);
        }

        /// <summary>
        /// Updates the camera's spread when enter is pressed.
        /// </summary>
        private void textBoxSpread_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                UpdateSpreadFromText();
            }
        }

        /// <summary>
        /// Updates the camera's spread.
        /// </summary>
        private void TextBoxSpread_LostFocus(object? sender, EventArgs e)
        {
            UpdateSpreadFromText();
        }

        /// <summary>
        /// Sets the camera's target to the clicked tile.
        /// </summary>
        private void RoomRenderer_TileMouseClick(object? sender, TileEventArgs e)
        {
            _camera.Target = e.Tile;
        }

        /// <summary>
        /// Prompts the user for a position and then sets the camera's wire box to that position.
        /// </summary>
        private void buttonAddWireBox_Click(object sender, EventArgs e)
        {
            Point? newWireBox = PositionSelectForm.Prompt(roomRenderer.Room);

            _camera.WireBoxPosition = newWireBox;
        }

        /// <summary>
        /// Sets the camera's wire box position to null.
        /// </summary>
        private void buttonRemoveWireBox_Click(object sender, EventArgs e)
        {
            _camera.WireBoxPosition = null;
        }
        #endregion

        /// <summary>
        /// Opens a CameraForm with the given room and camera.
        /// </summary>
        /// <param name="room">The room currently being edited.</param>
        /// <param name="camera">The camera to have the user edit.</param>
        public static void Prompt(Room room, Camera camera)
        {
            CameraForm form = new CameraForm(room, camera);
            form.ShowDialog();
        }
    }
}
