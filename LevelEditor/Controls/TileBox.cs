using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Classes;

namespace LevelEditor.Controls
{
    /// <summary>
    /// An extension of PixelBox specialized to store a Tile.
    /// </summary>
    internal class TileBox : PixelBox
    {
        private Tile _tile;

        /// <summary>
        /// Gets or sets the Tile this TileBox should display.
        /// </summary>
        public Tile Tile
        {
            get => _tile;
            set
            {
                // When the tile is uploaded, also update the displayed sprite.
                Image = value.Sprite;
                _tile = value;
            }
        }
    }
}
