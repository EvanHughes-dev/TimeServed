using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LevelEditor.Classes;
using LevelEditor.Classes.Props;

namespace LevelEditor
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The gospel determining what Tiles have what corresponding indexes.
        /// </summary>
        public IReadOnlyCollection<Tile> Tiles { get; private set; }

        /// <summary>
        /// The gospel determining what Props have what corresponding indexes.
        /// </summary>
        public IReadOnlyCollection<Prop> Props { get; private set; }

        // The level currently being edited
        private Level _level;

        /// <summary>
        /// Gets or sets the Level being edited currently.
        /// </summary>
        private Level Level
        {
            get => _level; set
            {
                if (value == null)
                {
                    labelNoLevelOpen.Visible = true;
                    flowLayoutPanelRooms.Visible = false;
                }
                else
                {
                    labelNoLevelOpen.Visible = false;
                    flowLayoutPanelRooms.Visible = true;
                }

                _level = value!;
            }
        }

        /// <summary>
        /// Called whenever a new level is loaded or created.
        /// </summary>
        private event Action OnNewLevelLoaded;

        /// <summary>
        /// The folder containing the .level file currently being edited.
        /// </summary>
        public string Folder { get; private set; }

        /// <summary>
        /// Creates a new MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _level = null!;
            Tiles = null!;

            Folder = null!;
            FileIOHelpers.CopyFolder("../../.././Sprites", ".bin/Debug/net8.0-windows");
        }

        /// <summary>
        /// Loads the tile files upon the form finishing loading.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadTiles();
            LoadProps();

            // In case LoadTiles creating a MessageBox, activating this window will ensure it still becomes
            //   the center of attention afterwards
            Activate();
        }

        /// <summary>
        /// Loads all of the tile sprites and initializes the tile array, displaying a MessageBox
        /// if loading the tiles fails.
        /// </summary>
        private void LoadTiles()
        {
            try
            {
                Tile[] tiles = [
                    FileIOHelpers.LoadTile("void.png", false),
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

        /// <summary>
        /// Loads all of the prop sprites and initializes the prop array, displaying a MessageBox
        /// if loading the props fails.
        /// </summary>
        private void LoadProps()
        {
            try
            {
                Prop[] props = [
                        FileIOHelpers.LoadBox("Box.png", 0),
                        FileIOHelpers.LoadItem("hook.png",0, KeyType.None),
                        FileIOHelpers.LoadItem("hookAndRope.png",0, KeyType.None),
                        FileIOHelpers.LoadItem("idCard.png",0, KeyType.KeyCard),
                        FileIOHelpers.LoadItem("screwdriver.png",0, KeyType.Screwdriver),
                        FileIOHelpers.LoadItem("wireCutters.png",0, KeyType.None),
                        FileIOHelpers.LoadDoor("Door-Top.png", 0, KeyType.KeyCard, Orientation.North),
                        FileIOHelpers.LoadDoor("Door-Right.png", 0, KeyType.KeyCard, Orientation.East),
                        FileIOHelpers.LoadDoor("Door-Bottom.png", 0, KeyType.KeyCard, Orientation.South),
                        FileIOHelpers.LoadDoor("Door-Left.png", 0, KeyType.KeyCard, Orientation.West),

                    ];

                Props = props.AsReadOnly();
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
                        LoadProps();
                        break;

                    case DialogResult.Cancel:
                    case DialogResult.Ignore:
                        // Do nothing! Assume that it's fine that nothing could be loaded!
                        break;
                }
            }
        }

        /// <summary>
        /// Creates a new level and saves it to a user-chosen location.
        /// </summary>
        private void CreateNewLevel()
        {
            DialogResult response = saveFileDialog.ShowDialog();

            switch (response)
            {
                case DialogResult.OK:
                    // Only actually do anything if the user hits OK! Otherwise we should do nothing at all
                    Level newLevel = new();

                    string fullPath = saveFileDialog.FileName;
                    try
                    {
                        Folder = Path.GetDirectoryName(fullPath)!;

                        FileIOHelpers.SaveLevel(newLevel, Folder, Tiles);
                        FileIOHelpers.UpdateContentMGCB();
                        Level = newLevel;

                        OnNewLevelLoaded?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        DialogResult errResponse = MessageBox.Show($"Could not create the new level: {ex.Message}",
                            "Error",
                            MessageBoxButtons.RetryCancel,
                            MessageBoxIcon.Error);

                        switch (errResponse)
                        {
                            case DialogResult.Cancel:
                                // Do nothing!
                                break;

                            case DialogResult.Retry:
                                // This will re-prompt the user to select the save location... but that's fine
                                CreateNewLevel();
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Opens an existing level file, as chosen by the user.
        /// </summary>
        private void OpenLevel()
        {
            DialogResult response = openFileDialog.ShowDialog();

            switch (response)
            {
                case DialogResult.OK:
                    // Only do anything if the user hits OK!
                    string fullPath = openFileDialog.FileName;
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            Level = FileIOHelpers.LoadLevel(fullPath, Tiles);

                            Folder = Path.GetDirectoryName(fullPath)!;

                            OnNewLevelLoaded?.Invoke();

                            foreach (Room room in Level.Rooms)
                            {
                                CreateRoomButton(room);
                            }
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

        /// <summary>
        /// Creates a new button in the UI that edits the given room when clicked.
        /// </summary>
        /// <param name="room">The Room to create the button for.</param>
        private void CreateRoomButton(Room room)
        {
            Button roomButton = new()
            {
                Size = new(140, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = room.Name,
                Parent = flowLayoutPanelRooms
            };

            roomButton.Click += (object? sender, EventArgs e) =>
            {
                EditorForm editor = new(this, room);
                editor.Show();
            };

            // When a new level is loaded, we have to delete all of the old buttons
            //   There's probably a cleaner way to do this than an event... but this works
            OnNewLevelLoaded += roomButton.Dispose;
        }

        /// <summary>
        /// Adds a new room to the level's list of rooms and creates its corresponding button
        /// on the MainForm.
        /// </summary>
        /// <param name="room">The room to add.</param>
        public void AddNewRoom(Room room)
        {
            Level.Rooms.Add(room);

            CreateRoomButton(room);
        }

        /// <summary>
        /// Loads the tiles when clicked.
        /// </summary>
        private void reloadTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTiles();
        }

        /// <summary>
        /// Opens a NewRoomForm when clicked.
        /// </summary>
        private void buttonAddNewRoom_Click(object sender, EventArgs e)
        {
            new NewRoomForm(this).Show();
        }

        /// <summary>
        /// Creates a new level when clicked.
        /// </summary>
        private void newLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewLevel();
        }

        /// <summary>
        /// Opens an existing level when clicked.
        /// </summary>
        private void loadLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenLevel();
        }

        /// <summary>
        /// Saves the level when clicked.
        /// </summary>
        private void saveLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileIOHelpers.SaveLevel(Level, Folder, Tiles);
            FileIOHelpers.UpdateContentMGCB();
        }

        /// <summary>
        /// Loads the props when clicked.
        /// </summary>
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadProps();
        }
    }
}
