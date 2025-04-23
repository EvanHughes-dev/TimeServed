using LevelEditor.Classes;
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

namespace LevelEditor.Controls
{
    /// <summary>
    /// A specialized button that displays a room the user can click on.
    /// </summary>
    public partial class RoomButton : UserControl
    {
        /// <summary>
        /// Gets or sets the room associated with this RoomButton.
        /// </summary>
        public Room Room
        {
            // We store the room directly inside of roomRenderer to avoid duplicating state
            get => roomRenderer.Room;
            set
            {
                labelRoomName.Text = value?.Name ?? "";
                roomRenderer.Room = value!;
            }
        }

        public RoomButton()
        {
            InitializeComponent();
        }
    }
}
