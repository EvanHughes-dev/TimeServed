namespace LevelEditor.Forms.Prompts
{
    partial class CameraForm
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
            roomRenderer = new LevelEditor.Controls.RoomRenderer();
            trackBarSpread = new TrackBar();
            textBoxSpread = new TextBox();
            labelSpread = new Label();
            buttonDone = new Button();
            splitContainer = new SplitContainer();
            groupBoxMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarSpread).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxMap
            // 
            groupBoxMap.Controls.Add(roomRenderer);
            groupBoxMap.Location = new Point(5, 3);
            groupBoxMap.Name = "groupBoxMap";
            groupBoxMap.Size = new Size(565, 264);
            groupBoxMap.TabIndex = 3;
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
            roomRenderer.ShowTriggers = false;
            roomRenderer.Size = new Size(533, 404);
            roomRenderer.TabIndex = 0;
            roomRenderer.Text = "roomRenderer1";
            roomRenderer.TileMouseClick += RoomRenderer_TileMouseClick;
            // 
            // trackBarSpread
            // 
            trackBarSpread.LargeChange = 30;
            trackBarSpread.Location = new Point(73, 3);
            trackBarSpread.Maximum = 180;
            trackBarSpread.Name = "trackBarSpread";
            trackBarSpread.Size = new Size(216, 45);
            trackBarSpread.TabIndex = 4;
            trackBarSpread.TickFrequency = 30;
            trackBarSpread.Value = 30;
            trackBarSpread.Scroll += trackBarSpread_Scroll;
            // 
            // textBoxSpread
            // 
            textBoxSpread.Location = new Point(5, 3);
            textBoxSpread.Name = "textBoxSpread";
            textBoxSpread.Size = new Size(62, 23);
            textBoxSpread.TabIndex = 5;
            textBoxSpread.KeyPress += textBoxSpread_KeyPress;
            // 
            // labelSpread
            // 
            labelSpread.AutoSize = true;
            labelSpread.Location = new Point(11, 28);
            labelSpread.Name = "labelSpread";
            labelSpread.Size = new Size(48, 15);
            labelSpread.TabIndex = 6;
            labelSpread.Text = "Spread°";
            // 
            // buttonDone
            // 
            buttonDone.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonDone.Location = new Point(485, 22);
            buttonDone.Name = "buttonDone";
            buttonDone.Size = new Size(85, 26);
            buttonDone.TabIndex = 7;
            buttonDone.Text = "Done";
            buttonDone.UseVisualStyleBackColor = true;
            buttonDone.Click += buttonDone_Click;
            // 
            // splitContainer
            // 
            splitContainer.Location = new Point(18, 12);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(groupBoxMap);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(buttonDone);
            splitContainer.Panel2.Controls.Add(trackBarSpread);
            splitContainer.Panel2.Controls.Add(labelSpread);
            splitContainer.Panel2.Controls.Add(textBoxSpread);
            splitContainer.Size = new Size(573, 488);
            splitContainer.SplitterDistance = 432;
            splitContainer.TabIndex = 8;
            splitContainer.SplitterMoved += splitContainer_SplitterMoved;
            // 
            // CameraForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(609, 514);
            Controls.Add(splitContainer);
            Name = "CameraForm";
            Text = "Camera Editor";
            Resize += CameraForm_Resize;
            groupBoxMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)trackBarSpread).EndInit();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxMap;
        private TrackBar trackBarSpread;
        private TextBox textBoxSpread;
        private Label labelSpread;
        private Button buttonDone;
        private Controls.RoomRenderer roomRenderer;
        private SplitContainer splitContainer;
    }
}