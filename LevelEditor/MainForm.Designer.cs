namespace LevelEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            flowLayoutPanelRooms = new FlowLayoutPanel();
            buttonAddNewRoom = new Button();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newLevelToolStripMenuItem = new ToolStripMenuItem();
            loadLevelToolStripMenuItem = new ToolStripMenuItem();
            saveLevelToolStripMenuItem = new ToolStripMenuItem();
            saveLevelAsToolStripMenuItem = new ToolStripMenuItem();
            tilesToolStripMenuItem = new ToolStripMenuItem();
            reloadTilesToolStripMenuItem = new ToolStripMenuItem();
            folderBrowserDialog = new FolderBrowserDialog();
            labelNoLevelOpen = new Label();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            flowLayoutPanelRooms.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanelRooms
            // 
            flowLayoutPanelRooms.Controls.Add(buttonAddNewRoom);
            flowLayoutPanelRooms.Location = new Point(12, 30);
            flowLayoutPanelRooms.Name = "flowLayoutPanelRooms";
            flowLayoutPanelRooms.Size = new Size(776, 408);
            flowLayoutPanelRooms.TabIndex = 0;
            flowLayoutPanelRooms.Visible = false;
            // 
            // buttonAddNewRoom
            // 
            buttonAddNewRoom.Location = new Point(3, 3);
            buttonAddNewRoom.Name = "buttonAddNewRoom";
            buttonAddNewRoom.Size = new Size(139, 80);
            buttonAddNewRoom.TabIndex = 0;
            buttonAddNewRoom.Text = "Add New Room";
            buttonAddNewRoom.UseVisualStyleBackColor = true;
            buttonAddNewRoom.Click += buttonAddNewRoom_Click;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, tilesToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(800, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newLevelToolStripMenuItem, loadLevelToolStripMenuItem, saveLevelToolStripMenuItem, saveLevelAsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 20);
            fileToolStripMenuItem.Text = "Level";
            // 
            // newLevelToolStripMenuItem
            // 
            newLevelToolStripMenuItem.Name = "newLevelToolStripMenuItem";
            newLevelToolStripMenuItem.Size = new Size(123, 22);
            newLevelToolStripMenuItem.Text = "New";
            newLevelToolStripMenuItem.Click += newLevelToolStripMenuItem_Click;
            // 
            // loadLevelToolStripMenuItem
            // 
            loadLevelToolStripMenuItem.Name = "loadLevelToolStripMenuItem";
            loadLevelToolStripMenuItem.Size = new Size(123, 22);
            loadLevelToolStripMenuItem.Text = "Open";
            loadLevelToolStripMenuItem.Click += loadLevelToolStripMenuItem_Click;
            // 
            // saveLevelToolStripMenuItem
            // 
            saveLevelToolStripMenuItem.Name = "saveLevelToolStripMenuItem";
            saveLevelToolStripMenuItem.Size = new Size(123, 22);
            saveLevelToolStripMenuItem.Text = "Save";
            // 
            // saveLevelAsToolStripMenuItem
            // 
            saveLevelAsToolStripMenuItem.Name = "saveLevelAsToolStripMenuItem";
            saveLevelAsToolStripMenuItem.Size = new Size(123, 22);
            saveLevelAsToolStripMenuItem.Text = "Save As...";
            // 
            // tilesToolStripMenuItem
            // 
            tilesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { reloadTilesToolStripMenuItem });
            tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
            tilesToolStripMenuItem.Size = new Size(42, 20);
            tilesToolStripMenuItem.Text = "Tiles";
            // 
            // reloadTilesToolStripMenuItem
            // 
            reloadTilesToolStripMenuItem.Name = "reloadTilesToolStripMenuItem";
            reloadTilesToolStripMenuItem.Size = new Size(110, 22);
            reloadTilesToolStripMenuItem.Text = "Reload";
            reloadTilesToolStripMenuItem.Click += reloadTilesToolStripMenuItem_Click;
            // 
            // folderBrowserDialog
            // 
            folderBrowserDialog.Description = "Select a Level Folder";
            folderBrowserDialog.UseDescriptionForTitle = true;
            // 
            // labelNoLevelOpen
            // 
            labelNoLevelOpen.AutoSize = true;
            labelNoLevelOpen.Location = new Point(180, 195);
            labelNoLevelOpen.Name = "labelNoLevelOpen";
            labelNoLevelOpen.Size = new Size(415, 15);
            labelNoLevelOpen.TabIndex = 1;
            labelNoLevelOpen.Text = "No level open. Use the Level tab to create a new level or open an existing one!";
            // 
            // openFileDialog
            // 
            openFileDialog.DefaultExt = "level";
            openFileDialog.FileName = "level.level";
            openFileDialog.Filter = "Level files|level.level";
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "level";
            saveFileDialog.FileName = "level.level";
            saveFileDialog.Filter = "Level files|level.level";
            saveFileDialog.Title = "Save Level";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(flowLayoutPanelRooms);
            Controls.Add(labelNoLevelOpen);
            Controls.Add(menuStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            Text = "Recount Level Editor";
            Load += MainForm_Load;
            flowLayoutPanelRooms.ResumeLayout(false);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanelRooms;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newLevelToolStripMenuItem;
        private ToolStripMenuItem loadLevelToolStripMenuItem;
        private ToolStripMenuItem saveLevelToolStripMenuItem;
        private ToolStripMenuItem saveLevelAsToolStripMenuItem;
        private ToolStripMenuItem tilesToolStripMenuItem;
        private ToolStripMenuItem reloadTilesToolStripMenuItem;
        private Button buttonAddNewRoom;
        private FolderBrowserDialog folderBrowserDialog;
        private Label labelNoLevelOpen;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
    }
}