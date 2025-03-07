using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    internal class Level
    {
        public List<Room> Rooms { get; }

        public Level(List<Room> rooms)
        {
            Rooms = rooms;
        }

        public Level()
          : this([]) { }
    }
}
