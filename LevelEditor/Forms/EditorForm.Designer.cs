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
            roomRenderer = new Controls.RoomRenderer();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            tabControlPalettes = new TabControl();
            tabPageTiles = new TabPage();
            flowLayoutPanelTiles = new FlowLayoutPanel();
            tabPageProps = new TabPage();
            flowLayoutPanelProps = new FlowLayoutPanel();
            tabPageTriggers = new TabPage();
            splitContainer = new SplitContainer();
            groupBoxMap.SuspendLayout();
            tabControlPalettes.SuspendLayout();
            tabPageTiles.SuspendLayout();
            tabPageProps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxMap
            // 
            groupBoxMap.Controls.Add(roomRenderer);
            groupBoxMap.Location = new Point(3, 12);
            groupBoxMap.Name = "groupBoxMap";
            groupBoxMap.Size = new Size(441, 494);
            groupBoxMap.TabIndex = 2;
            groupBoxMap.TabStop = false;
            groupBoxMap.Text = "Map";
            // 
            // roomRenderer
            // 
            roomRenderer.Location = new Point(6, 22);
            roomRenderer.Name = "roomRenderer";
            roomRenderer.Room = null;
            roomRenderer.ShowProps = true;
            roomRenderer.ShowTiles = true;
            roomRenderer.ShowTriggers = true;
            roomRenderer.Size = new Size(429, 472);
            roomRenderer.TabIndex = 0;
            roomRenderer.Text = "roomRenderer";
            roomRenderer.TileMouseDown += roomRenderer_TileMouseDown;
            roomRenderer.TileMouseMove += roomRenderer_TileMouseMove;
            roomRenderer.TileMouseUp += roomrenderer_TileMouseUp;
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
            // tabControlPalettes
            // 
            tabControlPalettes.Controls.Add(tabPageTiles);
            tabControlPalettes.Controls.Add(tabPageProps);
            tabControlPalettes.Controls.Add(tabPageTriggers);
            tabControlPalettes.Location = new Point(12, 12);
            tabControlPalettes.Name = "tabControlPalettes";
            tabControlPalettes.SelectedIndex = 0;
            tabControlPalettes.Size = new Size(215, 494);
            tabControlPalettes.TabIndex = 0;
            tabControlPalettes.SelectedIndexChanged += tabControlTilesProps_SelectedIndexChanged;
            // 
            // tabPageTiles
            // 
            tabPageTiles.Controls.Add(flowLayoutPanelTiles);
            tabPageTiles.Location = new Point(4, 24);
            tabPageTiles.Name = "tabPageTiles";
            tabPageTiles.Size = new Size(207, 466);
            tabPageTiles.TabIndex = 0;
            tabPageTiles.Text = "Tiles";
            tabPageTiles.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelTiles
            // 
            flowLayoutPanelTiles.AutoScroll = true;
            flowLayoutPanelTiles.Location = new Point(3, 3);
            flowLayoutPanelTiles.Name = "flowLayoutPanelTiles";
            flowLayoutPanelTiles.Size = new Size(201, 460);
            flowLayoutPanelTiles.TabIndex = 0;
            // 
            // tabPageProps
            // 
            tabPageProps.Controls.Add(flowLayoutPanelProps);
            tabPageProps.Location = new Point(4, 24);
            tabPageProps.Name = "tabPageProps";
            tabPageProps.Size = new Size(207, 466);
            tabPageProps.TabIndex = 0;
            tabPageProps.Text = "Props";
            tabPageProps.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelProps
            // 
            flowLayoutPanelProps.Location = new Point(3, 3);
            flowLayoutPanelProps.Name = "flowLayoutPanelProps";
            flowLayoutPanelProps.Size = new Size(201, 460);
            flowLayoutPanelProps.TabIndex = 0;
            // 
            // tabPageTriggers
            // 
            tabPageTriggers.Location = new Point(4, 24);
            tabPageTriggers.Name = "tabPageTriggers";
            tabPageTriggers.Size = new Size(207, 466);
            tabPageTriggers.TabIndex = 1;
            tabPageTriggers.Text = "Triggers";
            tabPageTriggers.UseVisualStyleBackColor = true;
            // 
            // splitContainer
            // 
            splitContainer.Location = new Point(0, 0);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(tabControlPalettes);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(groupBoxMap);
            splitContainer.Size = new Size(690, 514);
            splitContainer.SplitterDistance = 230;
            splitContainer.TabIndex = 3;
            // 
            // EditorForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(690, 514);
            Controls.Add(splitContainer);
            Name = "EditorForm";
            Text = "Level Editor";
            Resize += EditorForm_Resize;
            groupBoxMap.ResumeLayout(false);
            tabControlPalettes.ResumeLayout(false);
            tabPageTiles.ResumeLayout(false);
            tabPageProps.ResumeLayout(false);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBoxMap;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private TabControl tabControlPalettes;
        private TabPage tabPageTiles;
        private TabPage tabPageProps;
        private SplitContainer splitContainer;
        private FlowLayoutPanel flowLayoutPanelTiles;
        private FlowLayoutPanel flowLayoutPanelProps;
        private TabPage tabPageTriggers;
        private Controls.RoomRenderer roomRenderer;
    }
}