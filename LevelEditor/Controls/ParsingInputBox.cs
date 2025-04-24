using LevelEditor.Classes.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Controls
{
    /// <summary>
    /// A child of TextBox that parses the input to a given type.
    /// </summary>
    /// <typeparam name="T">The type of data this ParsingInputBox should parse to.</typeparam>
    public class ParsingInputBox<T> : TextBox where T : IParsable<T>
    {
        private T? _input;
        /// <summary>
        /// Gets or sets the parsed input from this ParsingInputBox.
        /// </summary>
        public T? Input
        {
            get => _input;
            private set
            {
                _input = value;

                InputChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called whenever this ParsingInputBox changes its input.
        /// </summary>
        public EventHandler? InputChanged;

        private string _lastValidInput = "";

        /// <summary>
        /// Updates the camera's RadianSpread property based on the input to the text box.
        /// </summary>
        private void UpdateInputFromText()
        {
            bool validInput = T.TryParse(Text, null, out T? parsed);

            if (validInput)
            {
                Input = parsed;

                _lastValidInput = Text;
            }
            else
            {
                // If the user typed something that can't be parsed to a float, we want to undo that work
                Text = _lastValidInput;
            }
        }

        /// <summary>
        /// Updates the parsed input from the new "submitted" text.
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            UpdateInputFromText();
        }

        /// <summary>
        /// If enter was pressed, updates the parsed input from the new "submitted" text.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter)
            {
                UpdateInputFromText();
            }
        }
    }
}
