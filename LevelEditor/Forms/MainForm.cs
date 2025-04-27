using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using LevelEditor.Classes.Triggers;
using LevelEditor.Helpers;
using System.Collections.ObjectModel;

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

        /// <summary>
        /// The gospel determining what Triggers have what corresponding indexes.
        /// </summary>
        public IReadOnlyCollection<Trigger> Triggers { get; private set; }

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

        private string _formName;
        private bool _keyDownInputRead;


        /// <summary>
        /// Creates a new MainForm.
        /// </summary>
        public MainForm()
        {
            FileIOHelpers.CopyFolder("../../.././Sprites", "./Sprites");

            InitializeComponent();

            _level = null!;
            Tiles = null!;
            Props = null!;
            Folder = null!;
            OnNewLevelLoaded = null!;

            _formName = "Level Selector";
            this.Text = _formName;

            this.FormClosing += LevelEditor_FormClosing;
            //setup keyboard capturing
            this.KeyPreview = true;
            this.KeyDown += LevelEditor_KeyDown;
            this.KeyUp += LevelEditor_KeyUp;

        }

        /// <summary>
        /// Loads the tile files upon the form finishing loading.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadTiles();
            LoadProps();

            Triggers = new Trigger[]
            {
                new Checkpoint(-1, null)
            };

            // In case LoadTiles creating a MessageBox, activating this window will ensure it still becomes
            //   the center of attention afterwards
            Activate();
        }

        # region Level Loading and Creation

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

                    FileIOHelpers.LoadTile("tile_wall_corner_inner_tr.png", false),
                    FileIOHelpers.LoadTile("tile_wall_corner_inner_tl.png", false),
                    FileIOHelpers.LoadTile("tile_wall_corner_inner_br.png", false),
                    FileIOHelpers.LoadTile("tile_wall_corner_inner_bl.png", false),

                    FileIOHelpers.LoadTile("tile_wall_corner_outer_tr.png", false),
                    FileIOHelpers.LoadTile("tile_wall_corner_outer_tl.png", false),
                    FileIOHelpers.LoadTile("tile_wall_corner_outer_br.png", false),
                    FileIOHelpers.LoadTile("tile_wall_corner_outer_bl.png", false),

                    FileIOHelpers.LoadTile("tile_wall0_top.png", false),
                    FileIOHelpers.LoadTile("tile_wall1_top.png", false),
                    FileIOHelpers.LoadTile("tile_wall2_top.png", false),
                    FileIOHelpers.LoadTile("tile_wall3_top.png", false),

                    FileIOHelpers.LoadTile("tile_wall0_right.png", false),
                    FileIOHelpers.LoadTile("tile_wall1_right.png", false),
                    FileIOHelpers.LoadTile("tile_wall2_right.png", false),
                    FileIOHelpers.LoadTile("tile_wall3_right.png", false),

                    FileIOHelpers.LoadTile("tile_wall0_bottom.png", false),
                    FileIOHelpers.LoadTile("tile_wall1_bottom.png", false),
                    FileIOHelpers.LoadTile("tile_wall2_bottom.png", false),
                    FileIOHelpers.LoadTile("tile_wall3_bottom.png", false),

                    FileIOHelpers.LoadTile("tile_wall0_left.png", false),
                    FileIOHelpers.LoadTile("tile_wall1_left.png", false),
                    FileIOHelpers.LoadTile("tile_wall2_left.png", false),
                    FileIOHelpers.LoadTile("tile_wall3_left.png", false),

                    FileIOHelpers.LoadTile("tile_walkable0.png", true),
                    FileIOHelpers.LoadTile("tile_walkable1.png", true),
                    FileIOHelpers.LoadTile("tile_walkable2.png", true),
                    FileIOHelpers.LoadTile("tile_walkable3.png", true),
                    FileIOHelpers.LoadTile("tile_walkable4.png", true),
                    ];

                Tiles = tiles.AsReadOnly();
            }
            catch (Exception ex)
            {
                Tiles = [];

                DialogResult response = MessageBox.Show($"Could not load tiles: {ex}",
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
                /*
                * Rules for adding new props to this list
                *
                * Order:
                *   All items
                *   All Boxes
                *   All doors
                *   All Cameras
                * 
                * This is an arbitrary order, but it shouldn't be changed
                * 
                * Inertial order needs to be the same as the order the sprites are in the corresponding array in AssetManager
                * Whenever you add new props, make sure you update any index that is affected in FilIOHelpers.LoadRoom
                */
                Prop[] props = [
                        FileIOHelpers.LoadItem("idCard.png",0, KeyType.KeyCard),
                        FileIOHelpers.LoadItem("screwdriver.png",1, KeyType.Screwdriver),
                        FileIOHelpers.LoadItem("wireCutters.png",2, KeyType.None),
                        FileIOHelpers.LoadItem("hook.png",3, KeyType.None),
                        FileIOHelpers.LoadItem("hookAndRope.png",4, KeyType.None),
                        FileIOHelpers.LoadBox("Box.png", 0),
                        FileIOHelpers.LoadDoor("prop_door_locked_top.png", 0, KeyType.KeyCard),
                        FileIOHelpers.LoadDoor("prop_door_locked_right.png", 1, KeyType.KeyCard),
                        FileIOHelpers.LoadDoor("prop_door_locked_bottom.png", 2, KeyType.KeyCard),
                        FileIOHelpers.LoadDoor("prop_door_locked_left.png", 3, KeyType.KeyCard),
                        FileIOHelpers.LoadDoor("prop_vent_top.png", 4, KeyType.Screwdriver),
                        FileIOHelpers.LoadDoor("prop_vent_right.png", 5, KeyType.Screwdriver),
                        FileIOHelpers.LoadDoor("prop_vent_bottom.png", 6, KeyType.Screwdriver),
                        FileIOHelpers.LoadDoor("prop_vent_left.png", 7, KeyType.Screwdriver),
                        FileIOHelpers.LoadDoor("Door-Top.png", 8, KeyType.None),
                        FileIOHelpers.LoadDoor("Door-Right.png", 9, KeyType.None),
                        FileIOHelpers.LoadDoor("Door-Bottom.png", 10, KeyType.None),
                        FileIOHelpers.LoadDoor("Door-Left.png", 11, KeyType.None),
                        FileIOHelpers.LoadCamera("prop_cameraOn.png", 0)
                    ];

                Props = props.AsReadOnly();
            }
            catch (Exception ex)
            {
                Props = [];

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

                        SaveLevel();
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
                            Level = FileIOHelpers.LoadLevel(fullPath, Tiles, Props, Triggers);

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
            RoomButton roomButton = new()
            {
                Size = new(140, 80),
                Room = room,
                Parent = flowLayoutPanelRooms
            };

            roomButton.Click += (object? _, EventArgs _) =>
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
            SaveLevel();
        }

        #endregion

        /// <summary>
        /// Gets all rooms contained within the level currently being edited, or null if no level is open.
        /// </summary>
        /// <returns>A read only collection of the rooms in the level currently being edited, or null if no level is open.</returns>
        public ReadOnlyCollection<Room> GetAllRooms()
        {
            return Level?.Rooms.AsReadOnly()!;
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
            SaveLevel();
        }

        /// <summary>
        /// Loads the props when clicked.
        /// </summary>
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadProps();
        }

        /// <summary>
        /// Prevent the user from leaving the form with unsaved changes without confirming they 
        /// wish to do so
        /// </summary>
        private void LevelEditor_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (Level == null || Level.Rooms == null)
                return;
            List<Room> unsavedRooms = Level.Rooms.Where(room => room.SavedState == SavedState.Unsaved).ToList();
            if (unsavedRooms.Count != 0)
            {
                switch (MessageBox.Show($"Unsaved changes to {unsavedRooms.Count} room{(unsavedRooms.Count != 1 ? "s" : "")}. Do you want to save before exiting?", "Warning",
                                 MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        foreach (Room room in unsavedRooms) room.SavedState = SavedState.Saved;
                        SaveLevel();
                        break;
                    case DialogResult.No:
                        e.Cancel = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Save the current level
        /// </summary>
        public void SaveLevel()
        {
            if (Folder == null)
                return;
            FileIOHelpers.SaveLevel(Level, Folder, Tiles);
            FileIOHelpers.UpdateContentMGCB();

            if (Level != null && Level.Rooms != null)
                foreach (Room room in Level.Rooms) room.SavedState = SavedState.Saved;
        }

        #region Keyboard Input

        /// <summary>
        /// Read keyboard input and take the needed actions based off which
        /// keys are pressed at the same time
        /// </summary>
        private void LevelEditor_KeyDown(object? sender, KeyEventArgs e)
        {
            // prevent multiple inputs from being read at the same time
            if (_keyDownInputRead)
                return;

            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                SaveLevel();
                _keyDownInputRead = true;

            }

        }

        /// <summary>
        /// On the key up event, allow key input to be read again
        /// </summary>
        private void LevelEditor_KeyUp(object? sender, KeyEventArgs e)
        {
            _keyDownInputRead = false;
        }

        #endregion
    }
}
