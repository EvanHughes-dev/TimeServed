using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public class Level
    {
        /// <summary>
        /// The Rooms contained within this Level.
        /// </summary>
        public List<Room> Rooms { get; }

        /// <summary>
        /// Creates a new Level with the given Rooms in it.
        /// </summary>
        /// <param name="rooms">The rooms in the level.</param>
        public Level(List<Room> rooms)
        {
            Rooms = rooms;
        }

        /// <summary>
        /// Creates a new Level with no rooms in it.
        /// </summary>
        public Level()
          : this([]) { }
    }
}
