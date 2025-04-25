// Leah Crain
// 2/9/25
// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on

using LevelEditor.Classes;
using LevelEditor.Classes.Props;
using LevelEditor.Classes.Triggers;
using LevelEditor.Controls;
using LevelEditor.Extensions;
using LevelEditor.Forms.Prompts;

namespace LevelEditor
{
    /// <summary>
    /// Keeps track of tab state
    /// </summary>
    internal enum TabState
    {
        Tiles,
        Props,
        Triggers
    };

    /// <summary>
    /// The EditorForm, with a palette, selected color, save and load buttons, and a grid of PictureBoxes the user can draw on.
    /// </summary>
    public partial class EditorForm : Form
    {
        #region Fields and Properties
        // A reference to the MainForm that created this EditorForm
        private readonly MainForm _mainForm;


        /// <summary>
        /// The Room this editor is editing. Stored within roomRenderer to avoid duplicating state.
        /// </summary>
        private Room Room
        {
            get => roomRenderer.Room;
            set => roomRenderer.Room = value;
        }

        /// <summary>
        /// Which tab is currently open within the EditorForm
        /// </summary>
        private TabState TabState { get; set; }

        /// <summary>
        /// Gets or sets the color the user currently has selected from the palette.
        /// </summary>
        private Tile SelectedTile { get; set; }

        /// <summary>
        /// Gets or sets the prop the user currently has selected from the palette.
        /// </summary>
        private Prop SelectedProp { get; set; }
        /// <summary>
        /// Gets or sets the trigger the user currently has selected from the palette.
        /// </summary>
        private Trigger SelectedTrigger { get; set; }
        /// <summary>
        /// Gets the palette of props the user may select from
        /// </summary>
        private Prop[,] PropPalette { get; }

        /// <summary>
        /// Holds the tile box of the tile that is currently selected on the palette.
        /// </summary>
        private TileBox _currentlySelectedTileBox;
        /// <summary>
        /// Holds the prop box of the prop that is currently selected on the palette.
        /// </summary>
        private PropBox _currentlySelectedPropBox;
        /// <summary>
        /// Holds the trigger box of the trigger that is currently selected on the palette.
        /// </summary>
        private TriggerBox _currentlySelectedTriggerBox;

        private Point? _triggerStartPoint;

        private string _formName;
        private readonly string _formNameBase;

        private bool _keyDownInputRead;

        #endregion
        #region Constructors
        /// <summary>
        /// Creates a new EditorForm with a reference to the MainForm.
        /// </summary>
        private EditorForm(MainForm mainForm)
        {
            InitializeComponent();
            _formName = "Room Editor";
            _formNameBase = "Room Editor";

            _keyDownInputRead = false;

            _mainForm = mainForm;

            Room = null!;
            _currentlySelectedTileBox = null!;
            _currentlySelectedPropBox = null!;

            TabState = TabState.Tiles;

            // Defines how large each swatch should be on each palette. Users of the level editor, change this as is convenient for you!
            const int SwatchSize = 75;

            // Create both palettes
            CreatePalette<Tile, TileBox>(_mainForm.Tiles, SwatchSize, flowLayoutPanelTiles, (swatch, tile) =>
            {
                swatch.Tile = tile;
                swatch.Click += Swatch_Click;

                swatch.BorderColor = Color.Blue;

                swatch.SizeMode = PictureBoxSizeMode.Zoom;
            });

            CreatePalette<Prop, PropBox>(_mainForm.Props, SwatchSize, flowLayoutPanelProps, (swatch, prop) =>
            {
                swatch.Prop = prop;
                swatch.Click += PropSwatch_Click;

                swatch.BorderColor = Color.Blue;

                swatch.SizeMode = PictureBoxSizeMode.Zoom;
            });

            SelectedTile = _mainForm.Tiles.ElementAt(0);
            SelectedProp = _mainForm.Props.ElementAt(0);
            SelectedTrigger = _mainForm.Triggers.ElementAt(0);

            //setup keyboard capturing
            this.KeyPreview = true;
            this.KeyDown += RoomEditor_KeyDown;
            this.KeyUp += RoomEditor_KeyUp;

            this.FormClosing += RoomEditor_FormClosing;
            // Whenever the splitter of the splitContainer moves, we have to reformat the whole EditorForm
            splitContainer.SplitterMoved += (object? _, SplitterEventArgs _) => Reformat();
        }

        /// <summary>
        /// Creates a new EditorForm with an existing Room.
        /// </summary>
        /// <param name="mainForm">A reference to the MainForm.</param>
        /// <param name="room">An existing Room to edit.</param>
        public EditorForm(MainForm mainForm, Room room)
            : this(mainForm)
        {
            Room = room;

            Reformat();
            _formName = $"Room Editor - {Room.Name} {(Room.SavedState == SavedState.Unsaved ? "*" : "")}";
            _formNameBase = $"Room Editor - {Room.Name}";

            this.Text = _formName;
        }

        /// <summary>
        /// Creates a new EditorForm by creating a new room.
        /// </summary>
        /// <param name="mainForm">A reference to the MainForm.</param>
        /// <param name="name">The name of the new room.</param>
        /// <param name="width">The width of the new room.</param>
        /// <param name="height">The height of the new room.</param>
        public EditorForm(MainForm mainForm, string name, int width, int height)
            : this(mainForm, new Room(name, width, height, mainForm.Tiles.ElementAt(0)))
        {
            _mainForm.AddNewRoom(Room);
        }

        #endregion
        #region Form Events
        /// <summary>
        /// Reformats the form.
        /// </summary>
        private void EditorForm_Resize(object sender, EventArgs e)
        {
            Reformat();
        }

        #endregion
        #region Room Events
        /// <summary>
        /// Places a tile or adds/removes/edits a prop depending on the tile clicked and the tab state.
        /// </summary>
        private void roomRenderer_TileMouseDown(object? sender, TileEventArgs e)
        {
            if (TabState == TabState.Tiles)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // Handle setting tiles to the save value
                    if (Room[e.Tile].Sprite == SelectedTile.Sprite)
                        return;

                    Room[e.Tile] = SelectedTile;

                    if (Room.SavedState == SavedState.Saved)
                    {
                        Room.SavedState = SavedState.Unsaved;
                        UpdateFormName($"{_formNameBase} *");
                    }
                }

                // Right click picks tile! Because that's convenient and I wanted it to be a feature!
                if (e.Button == MouseButtons.Right)
                    SelectedTile = Room[e.Tile];
            }
            else if (TabState == TabState.Props)
            {
                // Left click should either place the selected prop if the clicked tile has no prop on it, or it should simply edit the existing prop there
                if (e.Button == MouseButtons.Left)
                {
                    if (e.Prop == null)
                    {
                        Prop createdProp = null!;

                        // Different props have to be created in different ways
                        switch (SelectedProp.PropType)
                        {
                            case ObjectType.Item: // Items, boxes, and cameras are created the same
                            case ObjectType.Box:
                            case ObjectType.Camera:
                                createdProp = SelectedProp.Instantiate(e.Tile);
                                break;

                            case ObjectType.Door:
                                Door door = (Door)SelectedProp;

                                Room room = RoomSelectForm.Prompt(_mainForm.GetAllRooms());
                                if (room == null) return;

                                Point? destination = PositionSelectForm.Prompt(room);
                                if (destination == null) return;

                                createdProp = door.Instantiate(e.Tile, (Point)destination, room.Id);
                                break;
                        }

                        Room.AddProp(createdProp);

                        // Cameras require some post-creation setup
                        switch (createdProp.PropType)
                        {
                            case ObjectType.Camera:
                                CameraForm.Prompt(Room, (Camera)createdProp);
                                break;
                        }

                        // Should this be reworked to make it so *all* props are created the same way, and door editing happens post-placement? Probably!
                    }
                    else
                    {
                        // Edit the clicked prop
                        switch (e.Prop.PropType)
                        {
                            case ObjectType.Item:
                            case ObjectType.Box:
                                // Boxes and items have no data, so they cannot be edited
                                break;

                            case ObjectType.Camera:
                                CameraForm.Prompt(Room, (Camera)e.Prop);
                                break;

                            case ObjectType.Door:
                                // Re-prompt for all of the door's data
                                Door door = (Door)e.Prop;

                                Room room = RoomSelectForm.Prompt(_mainForm.GetAllRooms());
                                if (room == null) return;

                                Point? destination = PositionSelectForm.Prompt(room);
                                if (destination == null) return;

                                door.ConnectedTo = room.Id;
                                door.Destination = destination;
                                break;
                        }
                    }

                    if (Room.SavedState == SavedState.Saved)
                    {
                        Room.SavedState = SavedState.Unsaved;
                        UpdateFormName($"{_formNameBase} *");
                    }
                }
                else if (e.Button == MouseButtons.Right) //Right click will...
                {
                    // Remove! That! Prop!
                    bool removalSuccessful = Room.RemovePropAt(e.Tile);

                    if (removalSuccessful && Room.SavedState == SavedState.Saved)
                    {
                        Room.SavedState = SavedState.Unsaved;
                        UpdateFormName($"{_formNameBase} *");
                    }
                }
            }
            else if (TabState == TabState.Triggers)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _triggerStartPoint = e.Tile; // saves the start tile of a trigger for creating it later

                    if (Room.SavedState == SavedState.Saved)
                    {
                        Room.SavedState = SavedState.Unsaved;
                        UpdateFormName($"{_formNameBase} *");
                    }
                }

                // Right click removes the trigger that contains the mouse (one at a time)
                if (e.Button == MouseButtons.Right)
                {
                    bool removed = Room.RemoveTriggerAt(e.Tile);

                    if (removed && Room.SavedState == SavedState.Saved)
                    {
                        Room.SavedState = SavedState.Unsaved;
                        UpdateFormName($"{_formNameBase} *");
                    }
                }
            }
        }
        /// <summary>
        /// Tracks the mouseup input for placing triggers when the lwft mouse button is released
        /// </summary>
        private void roomrenderer_TileMouseUp(object? sender, TileEventArgs e)
        {
            if (TabState == TabState.Triggers)
            {
                if (_triggerStartPoint != null)
                {
                    Rectangle startRect = new Rectangle(_triggerStartPoint!.Value, new Size(1, 1));

                    Rectangle endRect = new Rectangle(e.Tile, new Size(1, 1));

                    Trigger trigger = SelectedTrigger.Instantiate(Rectangle.Union(startRect, endRect));

                    if (trigger is Checkpoint checkpoint) //if what we are making is a checkpoint
                    {
                        int? checkpointIndex = IntInputForm.Prompt();

                        if (checkpointIndex == null) return;

                        checkpoint.Index = (int)checkpointIndex; // give it an index
                    }

                    Room.AddTrigger(trigger);

                    _triggerStartPoint = null;
                }

            }
        }
        /// <summary>
        /// If left click is held, draws tiles.
        /// </summary>
        private void roomRenderer_TileMouseMove(object? sender, TileEventArgs e)
        {
            if (TabState == TabState.Tiles)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Room[e.Tile] = SelectedTile;

                    if (Room[e.Tile].Sprite != SelectedTile.Sprite && Room.SavedState == SavedState.Saved)
                    {
                        Room.SavedState = SavedState.Unsaved;
                        UpdateFormName($"{_formNameBase} *");
                    }
                }
            }
        }

        #endregion
        #region Swatch Events
        /// <summary>
        /// When a "swatch" button is clicked, select its color.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Button sender.</exception>
        private void Swatch_Click(object? sender, EventArgs e)
        {
            if (sender is not TileBox swatch) throw new Exception();

            if (_currentlySelectedTileBox != null) _currentlySelectedTileBox.BorderWidth = 0;
            swatch.BorderWidth = 5;
            _currentlySelectedTileBox = swatch;
            SelectedTile = swatch.Tile;
        }
        /// <summary>
        /// When a prop button is clicked, select its object.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Button sender.</exception>
        private void PropSwatch_Click(object? sender, EventArgs e)
        {
            if (sender is not PropBox prop) throw new Exception("Invalid call to PropSwatch_Click");

            if (_currentlySelectedPropBox != null) _currentlySelectedPropBox.BorderWidth = 0;
            prop.BorderWidth = 5;
            _currentlySelectedPropBox = prop;
            SelectedProp = prop.Prop;
        }
        /// <summary>
        /// When a trigger button is clicked, select its trigger type.
        /// </summary>
        /// <exception cref="Exception">Thrown when this method is called with a non-Button sender.</exception>
        private void TriggerSwatch_Click(object? sender, EventArgs e)
        {
            if (sender is not TriggerBox trigger) throw new Exception("Invalid call to TriggerSwatch_Click");

            if (_currentlySelectedPropBox != null) _currentlySelectedPropBox.BorderWidth = 0;
            trigger.BorderWidth = 5;
            _currentlySelectedTriggerBox = trigger;
            SelectedTrigger = trigger.TriggerType;
        }
        #endregion

        #region Tab Events
        /// <summary>
        /// Changes the state based on the current tab selected and decides what to do with that info
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void tabControlTilesProps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not TabControl tabby) throw new Exception("Invalid sender to tabControlTilesProps_SelectedIndexChanged");

            TabState newState = (TabState)tabby.SelectedIndex;

            TabState = newState;

            // Switching tabs requires a reformat for reasons kind of beyond me but if we don't do it it doesn't look right
            Reformat();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Auto-formats the form.
        /// </summary>
        private void Reformat()
        {
            // The split container IS the form -- it should take up all the space it can
            splitContainer.Bounds = ClientRectangle;

            // Each of the split container panels has its own container that should completely fill it
            groupBoxMap.Bounds = splitContainer.Panel2.ClientRectangle;

            // Makes the tile and prop palettes fill their parents
            flowLayoutPanelTiles.Bounds = flowLayoutPanelTiles.Parent!.ClientRectangle;
            flowLayoutPanelProps.Bounds = flowLayoutPanelProps.Parent!.ClientRectangle;

            if (roomRenderer != null)
            {
                roomRenderer.Bounds = groupBoxMap.ClientRectangle.NudgeSides(-20, -5, -5, -5);
            }
        }

        /// <summary>
        /// Creates the controls necessary for a palette.
        /// </summary>
        /// <typeparam name="TValue">The type of value the palette will be used to select.</typeparam>
        /// <typeparam name="TControl">The type of control the palette will be made out of.</typeparam>
        /// <param name="elements">The elements to store in the palette.</param>
        /// <param name="size">How large each control in the palette should be, in pixels.</param>
        /// <param name="parent">The FlowLayoutPanel this palette should be contained in.</param>
        /// <param name="setupCallback">The callback to invoke to setup each of the palette controls.</param>
        private void CreatePalette<TValue, TControl>(IEnumerable<TValue> elements, int size, FlowLayoutPanel parent, Action<TControl, TValue> setupCallback)
            where TControl : Control, new()
        {
            foreach (TValue item in elements)
            {
                TControl control = new TControl()
                {
                    Size = new Size(size, size),
                    Parent = parent,
                };

                parent.Controls.Add(control);

                setupCallback(control, item);
            }
        }


        /// <summary>
        /// Update the name of the form
        /// </summary>
        /// <param name="newName">New name for the form</param>
        private void UpdateFormName(string newName)
        {
            _formName = newName;
            this.Text = _formName;
        }

        /// <summary>
        /// Prevent the user from leaving the form with unsaved changes without confirming they 
        /// wish to do so
        /// </summary>
        private void RoomEditor_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (Room.SavedState == SavedState.Unsaved)
            {
                switch (MessageBox.Show("Unsaved changes to room. Do you want to save before exiting?", "Warning",
                                 MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        Room.SavedState = SavedState.Saved;
                        _mainForm.SaveLevel();
                        break;
                    case DialogResult.No:
                        e.Cancel = false;
                        break;
                }
            }
        }
        #endregion

        #region Keyboard Input

        /// <summary>
        /// Read keyboard input and take the needed actions based off which
        /// keys are pressed at the same time
        /// </summary>
        private void RoomEditor_KeyDown(object? sender, KeyEventArgs e)
        {
            // prevent multiple inputs from being read at the same time
            if (_keyDownInputRead)
                return;

            if (e.Control && e.KeyCode == Keys.S)
            {

                e.SuppressKeyPress = true;
                _mainForm.SaveLevel();
                _keyDownInputRead = true;

                Room.SavedState = SavedState.Saved;
                UpdateFormName($"{_formNameBase}");
            }

        }

        /// <summary>
        /// On the key up event, allow key input to be read again
        /// </summary>
        private void RoomEditor_KeyUp(object? sender, KeyEventArgs e)
        {
            _keyDownInputRead = false;
        }

        #endregion

        /// <summary>
        /// Saves the level.
        /// </summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            _mainForm.SaveLevel();
        }

        /// <summary>
        /// Prompts the user to resize the room.
        /// </summary>
        private void buttonResize_Click(object sender, EventArgs e)
        {
            ResizeForm.Prompt(Room, _mainForm.Tiles.ElementAt(0));
        }
    }
}
