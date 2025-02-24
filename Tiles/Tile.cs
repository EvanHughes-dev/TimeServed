using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount.Tiles
{
    /// <summary>
    /// Hold the data relating to a single tile in the map
    /// </summary>
    internal class Tile
    {
        public bool _isWalkable;
        public int _spriteIndex;

        /// <summary>
        /// Create an instance of a tile with the needed data
        /// </summary>
        /// <param name="isWalkable">If the tile can be walked on</param>
        /// <param name="spriteIndex">The sprite index of the tile</param>
        public Tile(bool isWalkable, int spriteIndex)
        {
            _isWalkable = isWalkable;
            _spriteIndex = spriteIndex;
        }
    }
}
