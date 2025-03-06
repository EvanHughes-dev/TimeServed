namespace LevelEditor
{
    partial class EditorForm
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
            vScrollBarTiles = new VScrollBar();
            buttonSave = new Button();
            buttonLoad = new Button();
            groupBoxMap = new GroupBox();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            tabControlTilesProps = new TabControl();
            tabPageTiles = new TabPage();
            tabPageProps = new TabPage();
            tabControlTilesProps.SuspendLayout();
            tabPageTiles.SuspendLayout();
            SuspendLayout();
            // 
            // vScrollBarTiles
            // 
            vScrollBarTiles.Location = new Point(154, 0);
            vScrollBarTiles.Name = "vScrollBarTiles";
            vScrollBarTiles.Size = new Size(16, 423);
            vScrollBarTiles.TabIndex = 0;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(16, 469);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(80, 33);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Save File";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(106, 469);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(80, 33);
            buttonLoad.TabIndex = 1;
            buttonLoad.Text = "Load File";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += buttonLoad_Click;
            // 
            // groupBoxMap
            // 
            groupBoxMap.Location = new Point(196, 8);
            groupBoxMap.Name = "groupBoxMap";
            groupBoxMap.Size = new Size(284, 494);
            groupBoxMap.TabIndex = 2;
            groupBoxMap.TabStop = false;
            groupBoxMap.Text = "Map";
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "myLevel";
            openFileDialog.Filter = "Level Files|*.level";
            openFileDialog.Title = "Open a level file";
            // 
            // saveFileDialog
            // 
            saveFileDialog.FileName = "myLevel";
            saveFileDialog.Filter = "Level Files|*.level";
            // 
            // tabControlTilesProps
            // 
            tabControlTilesProps.Controls.Add(tabPageTiles);
            tabControlTilesProps.Controls.Add(tabPageProps);
            tabControlTilesProps.Location = new Point(12, 12);
            tabControlTilesProps.Name = "tabControlTilesProps";
            tabControlTilesProps.SelectedIndex = 0;
            tabControlTilesProps.Size = new Size(178, 451);
            tabControlTilesProps.TabIndex = 0;
            // 
            // tabPageTiles
            // 
            tabPageTiles.Controls.Add(vScrollBarTiles);
            tabPageTiles.Location = new Point(4, 24);
            tabPageTiles.Name = "tabPageTiles";
            tabPageTiles.Size = new Size(170, 423);
            tabPageTiles.TabIndex = 0;
            tabPageTiles.Text = "Tiles";
            tabPageTiles.UseVisualStyleBackColor = true;
            // 
            // tabPageProps
            // 
            tabPageProps.Location = new Point(4, 24);
            tabPageProps.Name = "tabPageProps";
            tabPageProps.Size = new Size(170, 423);
            tabPageProps.TabIndex = 0;
            tabPageProps.Text = "Props";
            tabPageProps.UseVisualStyleBackColor = true;
            // 
            // EditorForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(492, 514);
            Controls.Add(tabControlTilesProps);
            Controls.Add(groupBoxMap);
            Controls.Add(buttonLoad);
            Controls.Add(buttonSave);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "EditorForm";
            Text = "Level Editor";
            tabControlTilesProps.ResumeLayout(false);
            tabPageTiles.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button buttonSave;
        private Button buttonLoad;
        private GroupBox groupBoxMap;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private VScrollBar vScrollBarTiles;
        private TabControl tabControlTilesProps;
        private TabPage tabPageTiles;
        private TabPage tabPageProps;
    }
}