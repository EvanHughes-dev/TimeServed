using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Classes.Props;

namespace LevelEditor.Classes
{
    /// <summary>
    /// A Room, with a name, ID, and grid of tiles.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// The room's name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The room's randomly-generated ID.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The grid of tiles the room is made of.
        /// </summary>
        public Tile[,] Tiles { get; }

        /// <summary>
        /// The props that have been placed in this room.
        /// </summary>
        public List<Prop> Props { get; set; }

        /// <summary>
        /// Creates a new Room with a name, dimensions, and optional Tile to fill the grid with.
        /// </summary>
        /// <param name="name">The room's name.</param>
        /// <param name="width">The room's width, in tiles.</param>
        /// <param name="height">The room's height, in tiles.</param>
        /// <param name="bg">
        /// If provided, every tile in the Room will be set to this tile.
        /// Should only be excluded if you're planning to immediately set every tile manually!
        /// </param>
        public Room(string name, int width, int height, Tile? bg = null)
        {
            Name = name;

            Tiles = new Tile[height, width];

            if (bg != null)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Tiles[y, x] = (Tile)bg;
                    }
                }
            }

            Id = Program.Random.Next();
        }

        // TODO: Add Resize(north, south, east, west) method
        //   Allows for resizing of room after initial creation by adding or removing rows or columns
        //   on any of the four edges, referred to by cardinal directions
    }
}
