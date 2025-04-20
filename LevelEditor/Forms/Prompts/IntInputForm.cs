using LevelEditor.Classes.Props;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor.Forms.Prompts
{
    public partial class IntInputForm : Form
    {
        private int? _input;

        private string _lastValidInput;

        private IntInputForm()
        {
            InitializeComponent();

            _lastValidInput = "";
        }

        /// <summary>
        /// Updates the camera's RadianSpread property based on the input to the text box.
        /// </summary>
        private void UpdateSpreadFromText()
        {
            bool validInput = int.TryParse(textBoxInput.Text, out int parsed);

            if (validInput)
            {
                _input = parsed;

                _lastValidInput = textBoxInput.Text;
            }
            else
            {
                // If the user typed something that can't be parsed to a float, we want to undo that work
                textBoxInput.Text = _lastValidInput;
            }
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public int? Prompt()
        {
            IntInputForm form = new IntInputForm();

            form.ShowDialog();

            return _input;
        }
    }
}
