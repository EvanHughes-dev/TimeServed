using LevelEditor.Classes;
using LevelEditor.Classes.Props;
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
    public partial class RoomSelectForm : Form
    {
        private Room _selected;

        public RoomSelectForm(ReadOnlyCollection<Room> rooms)
        {
            InitializeComponent();

            _selected = null!;

            foreach (Room room in rooms)
            {
                Button roomButton = new()
                {
                    Size = new(140, 80),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = room.Name,
                    Parent = flowLayoutPanelRooms
                };

                roomButton.Click += MakeClickCallback(room);

                Controls.Add(roomButton);
            }
        }

        private EventHandler MakeClickCallback(Room room)
        {
            return (object? sender, EventArgs e) =>
            {
                _selected = room;
            };
        }

        public Room Prompt()
        {
            ShowDialog();

            return _selected;
        }
    }
}
