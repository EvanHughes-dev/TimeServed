// Leah Crain
// 2/9/25
// The MainForm, a small window with a "load file" button, height and width text boxes, and a "create new" button. Creates a new EditorForm after either button is clicked

namespace LevelEditor
{
    /// <summary>
    /// The MainForm, a small window with a "load file" button, height and width text boxes, and a "create new" button. Creates a new EditorForm after either button is clicked.
    /// </summary>
    public partial class NewRoomForm : Form
    {
        public NewRoomForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On click, loads a file.
        /// </summary>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        /// <summary>
        /// On click, creates a new map with the user's input.
        /// </summary>
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            CreateNewMap();
        }

        /// <summary>
        /// Prompts the user to open a file, then creates a new EditorForm with the selected file path.
        /// </summary>
        private void LoadFile()
        {
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                EditorForm editor = new(openFileDialog.FileName);
                editor.ShowDialog();
            }
        }

        /// <summary>
        /// Validates the inputs in the width and height boxes and then creates a new EditorForm with the given size.
        /// </summary>
        private void CreateNewMap()
        {
            /* Validate user inputs */

            bool widthIsParsable = int.TryParse(textBoxWidth.Text, out int width);
            bool heightIsParsable = int.TryParse(textBoxHeight.Text, out int height);

            // Check for as many errors as possible to try and show the user several errors at once, if they exist
            List<string> errors = [];

            // Both the height and width must be integers between 10 and 30 (inclusive)
            if (!widthIsParsable)
                errors.Add("Cannot parse width.");
            else
            {
                if (width > 30)
                    errors.Add("Width too big. Must be at most 30.");
                else if (width < 10)
                    errors.Add("Width too small. Must be at least 10.");
            }

            if (!heightIsParsable)
                errors.Add("Cannot parse height.");
            else
            {
                if (height > 30)
                    errors.Add("Height too big. Must be at most 30.");
                else if (height < 10)
                    errors.Add("Height too small. Must be at least 10.");
            }

            if (errors.Count > 0)
            {
                string message = "Errors:";
                foreach (string error in errors)
                {
                    message += $"\n - {error}";
                }

                MessageBox.Show(message, "Error Creating Map", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Very important return statement!
                return;
            }

            /* Create new EditorForm with given dimensions */

            EditorForm editor = new(width, height);
            editor.ShowDialog();
        }
    }
}
