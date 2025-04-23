using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// A prompt that can be used to have the user select a room.
    /// </summary>
    public partial class RoomSelectForm : Form
    {
        private static Room _selected = null!;

        /// <summary>
        /// Creates a new RoomSelectForm based on the given collection of rooms.
        /// </summary>
        /// <param name="rooms">The rooms the user should be allowed to select.</param>
        private RoomSelectForm(ReadOnlyCollection<Room> rooms)
        {
            InitializeComponent();

            // Make all the buttons so the user can actually select the given rooms
            foreach (Room room in rooms)
            {
                CreateRoomButton(room);
            }
        }

        /// <summary>
        /// Creates a new button in the UI that selects the given room when clicked.
        /// </summary>
        /// <param name="room">The Room to create the button for.</param>
        private void CreateRoomButton(Room room)
        {
            // Copy+pasted and modified from MainForm!
            RoomButton roomButton = new()
            {
                Size = new(140, 80),
                Room = room,
                Parent = flowLayoutPanelRooms
            };

            roomButton.Click += MakeClickCallback(room);
        }

        /// <summary>
        /// Creates a callback function to be used when the user clicks a button associated with a given room.
        /// </summary>
        /// <param name="room">The room the button should select.</param>
        /// <returns>The created callback function.</returns>
        private EventHandler MakeClickCallback(Room room)
        {
            return (object? sender, EventArgs e) =>
            {
                _selected = room;

                // Once the user makes a selection, the form should close itself so control flow is returned to the static Prompt() method
                Close();
            };
        }

        /// <summary>
        /// Opens a RoomSelectForm with the given rooms.
        /// </summary>
        /// <param name="rooms">The rooms the user should be able to select.</param>
        /// <returns>The room the user selected, or null if they closed the form manually.</returns>
        public static Room Prompt(ReadOnlyCollection<Room> rooms)
        {
            _selected = null!;
            RoomSelectForm form = new RoomSelectForm(rooms);

            form.ShowDialog();

            return _selected;
        }
    }
}
