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
            SuspendLayout();
            // 
            // groupBoxMap
            // 
            groupBoxMap.Location = new Point(12, 12);
            groupBoxMap.Name = "groupBoxMap";
            groupBoxMap.Size = new Size(284, 494);
            groupBoxMap.TabIndex = 3;
            groupBoxMap.TabStop = false;
            groupBoxMap.Text = "Map";
            // 
            // PositionSelectForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(304, 514);
            Controls.Add(groupBoxMap);
            Name = "PositionSelectForm";
            Text = "DoorPositionSelectForm";
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxMap;
    }
}