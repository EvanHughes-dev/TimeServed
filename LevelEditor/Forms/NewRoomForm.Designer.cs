namespace LevelEditor
{
    partial class NewRoomForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBoxNew = new GroupBox();
            buttonCreate = new Button();
            textBoxHeight = new TextBox();
            textBoxName = new TextBox();
            textBoxWidth = new TextBox();
            labelName = new Label();
            labelHeight = new Label();
            labelWidth = new Label();
            buttonLoad = new Button();
            openFileDialog = new OpenFileDialog();
            groupBoxNew.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxNew
            // 
            groupBoxNew.Controls.Add(buttonCreate);
            groupBoxNew.Controls.Add(textBoxHeight);
            groupBoxNew.Controls.Add(textBoxName);
            groupBoxNew.Controls.Add(textBoxWidth);
            groupBoxNew.Controls.Add(labelName);
            groupBoxNew.Controls.Add(labelHeight);
            groupBoxNew.Controls.Add(labelWidth);
            groupBoxNew.Location = new Point(12, 93);
            groupBoxNew.Name = "groupBoxNew";
            groupBoxNew.Size = new Size(277, 230);
            groupBoxNew.TabIndex = 1;
            groupBoxNew.TabStop = false;
            groupBoxNew.Text = "Create New Room";
            // 
            // buttonCreate
            // 
            buttonCreate.Location = new Point(23, 149);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(229, 75);
            buttonCreate.TabIndex = 4;
            buttonCreate.Text = "Create New Room";
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += buttonCreate_Click;
            // 
            // textBoxHeight
            // 
            textBoxHeight.Location = new Point(100, 110);
            textBoxHeight.Name = "textBoxHeight";
            textBoxHeight.Size = new Size(171, 47);
            textBoxHeight.TabIndex = 3;
            // 
            // textBoxName
            // 
            textBoxName.Location = new Point(100, 32);
            textBoxName.Name = "textBoxName";
            textBoxName.Size = new Size(171, 47);
            textBoxName.TabIndex = 1;
            // 
            // textBoxWidth
            // 
            textBoxWidth.Location = new Point(100, 70);
            textBoxWidth.Name = "textBoxWidth";
            textBoxWidth.Size = new Size(171, 47);
            textBoxWidth.TabIndex = 1;
            // 
            // labelName
            // 
            labelName.AutoSize = true;
            labelName.Location = new Point(55, 35);
            labelName.Name = "labelName";
            labelName.Size = new Size(97, 41);
            labelName.TabIndex = 0;
            labelName.Text = "Name";
            labelName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelHeight
            // 
            labelHeight.AutoSize = true;
            labelHeight.Location = new Point(6, 113);
            labelHeight.Name = "labelHeight";
            labelHeight.Size = new Size(218, 41);
            labelHeight.TabIndex = 2;
            labelHeight.Text = "Height (in tiles)";
            // 
            // labelWidth
            // 
            labelWidth.AutoSize = true;
            labelWidth.Location = new Point(10, 73);
            labelWidth.Name = "labelWidth";
            labelWidth.Size = new Size(209, 41);
            labelWidth.TabIndex = 0;
            labelWidth.Text = "Width (in tiles)";
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(35, 12);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(229, 75);
            buttonLoad.TabIndex = 0;
            buttonLoad.Text = "Import Existing Room";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += buttonLoad_Click;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "myRoom";
            openFileDialog.Filter = "Room Files|*.room";
            openFileDialog.Title = "Open a level file";
            // 
            // NewRoomForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(301, 335);
            Controls.Add(buttonLoad);
            Controls.Add(groupBoxNew);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "NewRoomForm";
            Text = "Level Editor";
            groupBoxNew.ResumeLayout(false);
            groupBoxNew.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxNew;
        private Label labelHeight;
        private Label labelWidth;
        private TextBox textBoxHeight;
        private TextBox textBoxWidth;
        private Button buttonCreate;
        private Button buttonLoad;
        private OpenFileDialog openFileDialog;
        private TextBox textBoxName;
        private Label labelName;
    }
}
