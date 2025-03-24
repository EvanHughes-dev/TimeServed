using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    /// <summary>
    /// A single Tile, with a sprite and whether or not it can be walked on.
    /// </summary>
    public readonly struct Tile
    {
        /// <summary>
        /// The tile's sprite.
        /// </summary>
        public Image Sprite { get; }
        /// <summary>
        /// Whether this tile can be walked on.
        /// </summary>
        public bool IsWalkable { get; }

        /// <summary>
        /// Creates a new Tile with the given sprite and walkability.
        /// </summary>
        /// <param name="sprite">The tile's sprite.</param>
        /// <param name="isWalkable">Whether this tile can be walked on.</param>
        public Tile(Image sprite, bool isWalkable)
        {
            Sprite = sprite;
            IsWalkable = isWalkable;
        }
    }
}
