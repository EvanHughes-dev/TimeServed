namespace LevelEditor.Forms.Prompts
{
    partial class ResizeForm
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
            roomRenderer1 = new Controls.RoomRenderer();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // roomRenderer1
            // 
            roomRenderer1.BackColor = Color.Black;
            roomRenderer1.Location = new Point(152, 106);
            roomRenderer1.Name = "roomRenderer1";
            roomRenderer1.Room = null;
            roomRenderer1.ShowProps = true;
            roomRenderer1.ShowTiles = true;
            roomRenderer1.ShowTriggers = true;
            roomRenderer1.Size = new Size(251, 223);
            roomRenderer1.TabIndex = 0;
            roomRenderer1.Text = "roomRenderer1";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(224, 47);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 1;
            // 
            // ResizeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(568, 450);
            Controls.Add(textBox1);
            Controls.Add(roomRenderer1);
            Name = "ResizeForm";
            Text = "Resize";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Controls.RoomRenderer roomRenderer1;
        private TextBox textBox1;
    }
}