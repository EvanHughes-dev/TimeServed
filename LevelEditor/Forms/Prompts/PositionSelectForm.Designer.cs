namespace LevelEditor.Forms.Prompts
{
    partial class PositionSelectForm
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
            groupBoxMap.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxMap
            // 
            groupBoxMap.Controls.Add(roomRenderer);
            groupBoxMap.Location = new Point(12, 12);
            groupBoxMap.Name = "groupBoxMap";
            groupBoxMap.Size = new Size(562, 494);
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
            roomRenderer.Size = new Size(550, 466);
            roomRenderer.TabIndex = 0;
            roomRenderer.Text = "roomRenderer1";
            roomRenderer.TileMouseClick += RoomRenderer_TileMouseClick;
            // 
            // PositionSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(586, 514);
            Controls.Add(groupBoxMap);
            Name = "PositionSelectForm";
            Text = "Select a Position";
            Resize += PositionSelectForm_Resize;
            groupBoxMap.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxMap;
        private Controls.RoomRenderer roomRenderer;
    }
}