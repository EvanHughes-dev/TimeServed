using LevelEditor.Classes.Props;
using LevelEditor.Controls;
using System.Collections.Generic;

namespace LevelEditor.Forms.Prompts
{
    public partial class ItemInputForm : Form
    {
        private static Item? _input;

        /// <summary>
        /// Asks the suer to select an item
        /// </summary>
        /// <param name="items">the list of items to select from</param>
        private ItemInputForm(IReadOnlyCollection<Prop> props)
        {
            InitializeComponent();

            CreatePalette<Prop, PropBox>(props, 80, flowLayoutPanelItems, (swatch, prop) =>
            {
                swatch.Prop = prop;
                swatch.Click += MakeClickCallback(prop);

                swatch.BorderColor = Color.Blue;

                swatch.SizeMode = PictureBoxSizeMode.Zoom;
            });
        }
        /// <summary>
        /// Creates the controls necessary for a palette.
        /// </summary>
        /// <typeparam name="TValue">The type of value the palette will be used to select.</typeparam>
        /// <typeparam name="TControl">The type of control the palette will be made out of.</typeparam>
        /// <param name="elements">The elements to store in the palette.</param>
        /// <param name="size">How large each control in the palette should be, in pixels.</param>
        /// <param name="parent">The FlowLayoutPanel this palette should be contained in.</param>
        /// <param name="setupCallback">The callback to invoke to setup each of the palette controls.</param>
        private void CreatePalette<TValue, TControl>(IEnumerable<TValue> elements, int size, FlowLayoutPanel parent, Action<TControl, TValue> setupCallback)
            where TControl : Control, new()
        {
            foreach (TValue prop in elements)
            {
                if (prop is Item item)
                {
                    TControl control = new TControl()
                    {
                        Size = new Size(size, size),
                        Parent = parent,
                    };

                    parent.Controls.Add(control);

                    setupCallback(control, prop);
                }
            }
        }

        /// <summary>
        /// Creates a callback function to be used when the user clicks a button associated with a given item.
        /// </summary>
        /// <param name="item">The item the button should select.</param>
        /// <returns>The created callback function.</returns>
        private EventHandler MakeClickCallback(Prop prop)
        {
            return (object? sender, EventArgs e) =>
            {
                if(prop is Item item) _input = item;

                // Once the user makes a selection, the form should close itself so control flow is returned to the static Prompt() method
                Close();
            };
        }

        /// <summary>
        /// makes an instance of this form
        /// </summary>
        /// <returns>The inputted integer</returns>
        public static Item? Prompt(IReadOnlyCollection<Prop> items)
        {
            _input = null!;

            ItemInputForm form = new ItemInputForm(items);

            form.ShowDialog();

            return _input;
        }
    }
}
