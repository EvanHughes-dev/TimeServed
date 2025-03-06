using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    internal struct Tile
    {
        int TileIndex { get; set; }
        bool IsWalkable { get; set; }

        public Tile()
        {
            TileIndex = -1;
            IsWalkable = true;
        }
    }
}
