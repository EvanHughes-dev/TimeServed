using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    internal class Room
    {
        public int Width { get; }
        public int Height { get; }

        public Tile[,] Tiles { get; }

        public Room(int width, int height) {
            Width = width;
            Height = height;
        }
    }
}
