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
        public IReadOnlyCollection<Image> Tiles { get; private set; }
        private Level _level;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadTiles();

            OpenLevel();
        }

        private void LoadTiles()
        {
            const string tilePath = "./Tiles";

            try
            {
                Image[] tiles = FileIOHelpers.LoadImages(tilePath);
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

        private void OpenLevel()
        {
            DialogResult response = folderBrowserDialog.ShowDialog();

            switch (response)
            {
                case DialogResult.OK:
                    string folderPath = folderBrowserDialog.SelectedPath;

                    string levelFilePath = folderPath + "/level.level";
                    if (File.Exists(levelFilePath))
                    {
                        try
                        {
                            FileIOHelpers.LoadLevel(folderPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Could not load level: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        }
                    } else
                    {
                        _level = new();
                    }

                    break;

                case DialogResult.Cancel:
                    _level = null!;
                    break;
            }
        }

        private void reloadTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTiles();
        }

        private void buttonAddNewRoom_Click(object sender, EventArgs e)
        {
            NewRoomForm newRoomForm = new(this);
        }
    }
}
