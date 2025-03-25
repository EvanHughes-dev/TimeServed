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
            groupBoxMap = new GroupBox();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            tabControlTilesProps = new TabControl();
            tabPageTiles = new TabPage();
            tabPageProps = new TabPage();
            tabControlTilesProps.SuspendLayout();
            SuspendLayout();
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
            tabControlTilesProps.Size = new Size(178, 490);
            tabControlTilesProps.TabIndex = 0;
            // 
            // tabPageTiles
            // 
            tabPageTiles.Location = new Point(4, 24);
            tabPageTiles.Name = "tabPageTiles";
            tabPageTiles.Size = new Size(170, 462);
            tabPageTiles.TabIndex = 0;
            tabPageTiles.Text = "Tiles";
            tabPageTiles.UseVisualStyleBackColor = true;
            // 
            // tabPageProps
            // 
            tabPageProps.Location = new Point(4, 24);
            tabPageProps.Name = "tabPageProps";
            tabPageProps.Size = new Size(170, 462);
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "EditorForm";
            Text = "Level Editor";
            Load += EditorForm_Load;
            tabControlTilesProps.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBoxMap;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private TabControl tabControlTilesProps;
        private TabPage tabPageTiles;
        private TabPage tabPageProps;
    }
}