namespace LevelEditor.Controls
{
    partial class RoomButton
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            roomRenderer = new RoomRenderer();
            labelRoomName = new Label();
            SuspendLayout();
            // 
            // roomRenderer
            // 
            roomRenderer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            roomRenderer.Enabled = false;
            roomRenderer.Location = new Point(0, 0);
            roomRenderer.Name = "roomRenderer";
            roomRenderer.Room = null;
            roomRenderer.ShowProps = true;
            roomRenderer.ShowTiles = true;
            roomRenderer.ShowTriggers = false;
            roomRenderer.Size = new Size(150, 122);
            roomRenderer.TabIndex = 0;
            roomRenderer.Text = "roomRenderer1";
            // 
            // labelRoomName
            // 
            labelRoomName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelRoomName.Location = new Point(0, 125);
            labelRoomName.Name = "labelRoomName";
            labelRoomName.Size = new Size(150, 15);
            labelRoomName.TabIndex = 1;
            labelRoomName.Text = "RoomName";
            labelRoomName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // RoomButton
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(labelRoomName);
            Controls.Add(roomRenderer);
            Name = "RoomButton";
            Size = new Size(150, 140);
            ResumeLayout(false);
        }

        #endregion

        private RoomRenderer roomRenderer;
        private Label labelRoomName;
    }
}
