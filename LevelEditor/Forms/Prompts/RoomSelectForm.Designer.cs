namespace LevelEditor.Forms.Prompts
{
    partial class RoomSelectForm
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
            SuspendLayout();
            // 
            // flowLayoutPanelRooms
            // 
            flowLayoutPanelRooms.Location = new Point(12, 21);
            flowLayoutPanelRooms.Name = "flowLayoutPanelRooms";
            flowLayoutPanelRooms.Size = new Size(776, 408);
            flowLayoutPanelRooms.TabIndex = 1;
            // 
            // RoomSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(flowLayoutPanelRooms);
            Name = "RoomSelectForm";
            Text = "DoorForm";
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanelRooms;
    }
}