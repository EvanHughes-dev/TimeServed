using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Classes.Props;

namespace LevelEditor.Controls
{
    /// <summary>
    /// An extension of BorderBox specialized to store a Prop.
    /// </summary>
    internal class PropBox : BorderBox
    {
        private Prop _prop;

        /// <summary>
        /// Gets or sets the Prop this PropBox should display.
        /// </summary>
        public Prop Prop
        {
            get => _prop;
            set
            {
                // When the tile is uploaded, also update the displayed sprite.
                Image = value?.Sprite;
                _prop = value!;
            }
        }
    }
}
