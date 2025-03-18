using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public class Room
    {
        public string Name { get; set; }
        public int Id { get; }

        public Tile[,] Tiles { get; }

        public Room(int width, int height) {
            Tiles = new Tile[height, width];

            Id = Program.Random.Next();
        }

        // TODO: Add Resize(north, south, east, west) method
        //   Allows for resizing of room after initial creation by adding or removing rows or columns
        //   on any of the four edges, referred to by cardinal directions
    }
}
