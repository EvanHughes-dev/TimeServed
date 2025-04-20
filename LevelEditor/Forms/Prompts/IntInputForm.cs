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
        private static int? _input;

        private string _lastValidInput;

        /// <summary>
        /// Asks for an int input from the user
        /// </summary>
        private IntInputForm()
        {
            InitializeComponent();

            _lastValidInput = "";
        }

        /// <summary>
        /// Updates the camera's RadianSpread property based on the input to the text box.
        /// </summary>
        private void UpdateInputFromText()
        {
            bool validInput = int.TryParse(textBoxInput.Text, out int parsed);

            if (validInput) // keep valid inputs
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
        /// <summary>
        /// The submission button of this form
        /// </summary>
        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            UpdateInputFromText();
            Close();
        }
        /// <summary>
        /// makes an instance of this form
        /// </summary>
        /// <returns>The inputted integer</returns>
        public static int? Prompt()
        {
            IntInputForm form = new IntInputForm();

            form.ShowDialog();

            return _input;
        }
    }
}
