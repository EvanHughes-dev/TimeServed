using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class MainForm : Form
    {
        public IReadOnlyCollection<Tile> Tiles { get; private set; }

        private Level _level;
        public Level Level { get => _level; private set
            {
                if (value == null)
                {
                    labelNoLevelOpen.Visible = true;
                    flowLayoutPanelRooms.Visible = false;
                } else
                {
                    labelNoLevelOpen.Visible = false;
                    flowLayoutPanelRooms.Visible = true;
                }

                _level = value!;
            } }

        public MainForm()
        {
            InitializeComponent();

            _level = null!;
            Tiles = null!;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadTiles();

            Activate();
        }

        private void LoadTiles()
        {
            const string tilePath = "./Tiles";

            try
            {
                Tile[] tiles = [
                    FileIOHelpers.LoadTile("testWalkable.png", true),
                    FileIOHelpers.LoadTile("testWall.png", false)
                    ];

                Tiles = tiles.AsReadOnly();
            }
            catch (Exception ex)
            {
                Tiles = [];

                DialogResult response = MessageBox.Show($"Could not load tiles: {ex.Message}",
                    "Error",
                    MessageBoxButtons.AbortRetryIgnore,
                    MessageBoxIcon.Error);

                switch (response)
                {
                    case DialogResult.Abort:
                        Close();
                        break;

                    case DialogResult.Retry:
                        LoadTiles();
                        break;

                    case DialogResult.Cancel:
                    case DialogResult.Ignore:
                        // Do nothing! Assume that it's fine that nothing could be loaded!
                        break;
                }
            }
        }

        private void CreateNewLevel()
        {
            DialogResult response = saveFileDialog.ShowDialog();

            switch (response)
            {
                case DialogResult.OK:
                    Level = new();

                    FileIOHelpers.SaveLevel(Level, saveFileDialog.FileName);
                    break;
            }
        }

        private void OpenLevel()
        {
            DialogResult response = openFileDialog.ShowDialog();

            switch (response)
            {
                case DialogResult.OK:
                    if (File.Exists(openFileDialog.FileName))
                    {
                        try
                        {
                            Level = FileIOHelpers.LoadLevel(openFileDialog.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Could not load level: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Could not find a level at location {openFileDialog.FileName}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    break;
            }
        }

        private void OpenNewRoomForm()
        {
            NewRoomForm newRoomForm = new(this);

            newRoomForm.ShowDialog(this);
        }

        private void reloadTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTiles();
        }

        private void buttonAddNewRoom_Click(object sender, EventArgs e)
        {
            OpenNewRoomForm();
        }

        private void newLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewLevel();
        }

        private void loadLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLevel();
        }
    }
}
