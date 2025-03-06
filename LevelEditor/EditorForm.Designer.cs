namespace HW2_LevelEditor
{
    partial class EditorForm
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
            groupBoxPalette = new GroupBox();
            vScrollBar1 = new VScrollBar();
            buttonSave = new Button();
            buttonLoad = new Button();
            groupBoxMap = new GroupBox();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            groupBoxPalette.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxPalette
            // 
            groupBoxPalette.Controls.Add(vScrollBar1);
            groupBoxPalette.Location = new Point(10, 8);
            groupBoxPalette.Name = "groupBoxPalette";
            groupBoxPalette.Size = new Size(180, 414);
            groupBoxPalette.TabIndex = 0;
            groupBoxPalette.TabStop = false;
            groupBoxPalette.Text = "Tile Selector";
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new Point(152, 12);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new Size(28, 399);
            vScrollBar1.TabIndex = 0;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(17, 428);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(90, 33);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Save File";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(17, 467);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(90, 35);
            buttonLoad.TabIndex = 1;
            buttonLoad.Text = "Load File";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += buttonLoad_Click;
            // 
            // groupBoxMap
            // 
            groupBoxMap.Location = new Point(196, 8);
            groupBoxMap.Name = "groupBoxMap";
            groupBoxMap.Size = new Size(284, 494);
            groupBoxMap.TabIndex = 2;
            groupBoxMap.TabStop = false;
            groupBoxMap.Text = "Map";
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "myLevel";
            openFileDialog.Filter = "Level Files|*.level";
            openFileDialog.Title = "Open a level file";
            // 
            // saveFileDialog
            // 
            saveFileDialog.FileName = "myLevel";
            saveFileDialog.Filter = "Level Files|*.level";
            // 
            // EditorForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(492, 514);
            Controls.Add(groupBoxMap);
            Controls.Add(buttonLoad);
            Controls.Add(buttonSave);
            Controls.Add(groupBoxPalette);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "EditorForm";
            Text = "Level Editor";
            groupBoxPalette.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxPalette;
        private Button buttonSave;
        private Button buttonLoad;
        private GroupBox groupBoxMap;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private VScrollBar vScrollBar1;
    }
}