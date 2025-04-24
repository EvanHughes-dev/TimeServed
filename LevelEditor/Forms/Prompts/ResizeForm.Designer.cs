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
            roomRenderer = new LevelEditor.Controls.RoomRenderer();
            label1 = new Label();
            intInputBoxTop = new LevelEditor.Controls.IntInputBox();
            intInputBoxRight = new LevelEditor.Controls.IntInputBox();
            intInputBoxBottom = new LevelEditor.Controls.IntInputBox();
            intInputBoxLeft = new LevelEditor.Controls.IntInputBox();
            buttonDone = new Button();
            SuspendLayout();
            // 
            // roomRenderer
            // 
            roomRenderer.Location = new Point(152, 106);
            roomRenderer.Name = "roomRenderer";
            roomRenderer.Room = null;
            roomRenderer.ShowProps = true;
            roomRenderer.ShowTiles = true;
            roomRenderer.ShowTriggers = true;
            roomRenderer.Size = new Size(251, 223);
            roomRenderer.TabIndex = 0;
            roomRenderer.Text = "roomRenderer1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(61, 9);
            label1.Name = "label1";
            label1.Size = new Size(448, 15);
            label1.TabIndex = 2;
            label1.Text = "Positive numbers push that wall outwards, negative numbers trim that wall inwards.";
            // 
            // intInputBoxTop
            // 
            intInputBoxTop.Input = 0;
            intInputBoxTop.Location = new Point(252, 54);
            intInputBoxTop.Name = "intInputBoxTop";
            intInputBoxTop.Size = new Size(49, 23);
            intInputBoxTop.TabIndex = 3;
            // 
            // intInputBoxRight
            // 
            intInputBoxRight.Input = 0;
            intInputBoxRight.Location = new Point(460, 213);
            intInputBoxRight.Name = "intInputBoxRight";
            intInputBoxRight.Size = new Size(49, 23);
            intInputBoxRight.TabIndex = 3;
            // 
            // intInputBoxBottom
            // 
            intInputBoxBottom.Input = 0;
            intInputBoxBottom.Location = new Point(252, 376);
            intInputBoxBottom.Name = "intInputBoxBottom";
            intInputBoxBottom.Size = new Size(49, 23);
            intInputBoxBottom.TabIndex = 3;
            // 
            // intInputBoxLeft
            // 
            intInputBoxLeft.Input = 0;
            intInputBoxLeft.Location = new Point(49, 213);
            intInputBoxLeft.Name = "intInputBoxLeft";
            intInputBoxLeft.Size = new Size(49, 23);
            intInputBoxLeft.TabIndex = 3;
            // 
            // buttonDone
            // 
            buttonDone.Location = new Point(460, 419);
            buttonDone.Name = "buttonDone";
            buttonDone.Size = new Size(100, 26);
            buttonDone.TabIndex = 4;
            buttonDone.Text = "Done";
            buttonDone.UseVisualStyleBackColor = true;
            buttonDone.Click += buttonDone_Click;
            // 
            // ResizeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(568, 450);
            Controls.Add(buttonDone);
            Controls.Add(intInputBoxLeft);
            Controls.Add(intInputBoxBottom);
            Controls.Add(intInputBoxRight);
            Controls.Add(intInputBoxTop);
            Controls.Add(label1);
            Controls.Add(roomRenderer);
            Name = "ResizeForm";
            Text = "Resize";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Controls.RoomRenderer roomRenderer;
        private Label label1;
        private Controls.IntInputBox intInputBoxTop;
        private Controls.IntInputBox intInputBoxRight;
        private Controls.IntInputBox intInputBoxBottom;
        private Controls.IntInputBox intInputBoxLeft;
        private Button buttonDone;
    }
}