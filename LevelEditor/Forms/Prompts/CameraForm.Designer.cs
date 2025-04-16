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
            trackBarSpread = new TrackBar();
            textBoxSpread = new TextBox();
            labelSpread = new Label();
            buttonDone = new Button();
            ((System.ComponentModel.ISupportInitialize)trackBarSpread).BeginInit();
            SuspendLayout();
            // 
            // groupBoxMap
            // 
            groupBoxMap.Location = new Point(313, 12);
            groupBoxMap.Name = "groupBoxMap";
            groupBoxMap.Size = new Size(284, 494);
            groupBoxMap.TabIndex = 3;
            groupBoxMap.TabStop = false;
            groupBoxMap.Text = "Map";
            // 
            // trackBarSpread
            // 
            trackBarSpread.LargeChange = 30;
            trackBarSpread.Location = new Point(80, 219);
            trackBarSpread.Maximum = 180;
            trackBarSpread.Name = "trackBarSpread";
            trackBarSpread.Size = new Size(216, 45);
            trackBarSpread.TabIndex = 4;
            trackBarSpread.TickFrequency = 30;
            trackBarSpread.Value = 30;
            // 
            // textBoxSpread
            // 
            textBoxSpread.Location = new Point(12, 219);
            textBoxSpread.Name = "textBoxSpread";
            textBoxSpread.Size = new Size(62, 23);
            textBoxSpread.TabIndex = 5;
            // 
            // labelSpread
            // 
            labelSpread.AutoSize = true;
            labelSpread.Location = new Point(18, 244);
            labelSpread.Name = "labelSpread";
            labelSpread.Size = new Size(48, 15);
            labelSpread.TabIndex = 6;
            labelSpread.Text = "Spread°";
            // 
            // buttonDone
            // 
            buttonDone.Location = new Point(222, 476);
            buttonDone.Name = "buttonDone";
            buttonDone.Size = new Size(85, 26);
            buttonDone.TabIndex = 7;
            buttonDone.Text = "Done";
            buttonDone.UseVisualStyleBackColor = true;
            // 
            // CameraForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(609, 514);
            Controls.Add(buttonDone);
            Controls.Add(labelSpread);
            Controls.Add(textBoxSpread);
            Controls.Add(trackBarSpread);
            Controls.Add(groupBoxMap);
            Name = "CameraForm";
            Text = "Camera Editor";
            ((System.ComponentModel.ISupportInitialize)trackBarSpread).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBoxMap;
        private TrackBar trackBarSpread;
        private TextBox textBoxSpread;
        private Label labelSpread;
        private Button buttonDone;
    }
}