namespace LevelEditor.Forms.Prompts
{
    partial class IntInputForm
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
            label1 = new Label();
            textBoxInput = new TextBox();
            buttonSubmit = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(32, 19);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 0;
            label1.Text = "int please";
            // 
            // textBoxInput
            // 
            textBoxInput.Location = new Point(12, 49);
            textBoxInput.Name = "textBoxInput";
            textBoxInput.Size = new Size(100, 23);
            textBoxInput.TabIndex = 1;
            // 
            // buttonSubmit
            // 
            buttonSubmit.Location = new Point(25, 90);
            buttonSubmit.Name = "buttonSubmit";
            buttonSubmit.Size = new Size(75, 23);
            buttonSubmit.TabIndex = 2;
            buttonSubmit.Text = "Yeah";
            buttonSubmit.UseVisualStyleBackColor = true;
            buttonSubmit.Click += buttonSubmit_Click;
            // 
            // IntInputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(127, 131);
            Controls.Add(buttonSubmit);
            Controls.Add(textBoxInput);
            Controls.Add(label1);
            Name = "IntInputForm";
            Text = "Input an Integer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBoxInput;
        private Button buttonSubmit;
    }
}