using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeEveryDayRecount.GameObjects;
using MakeEveryDayRecount.Map.Tiles;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.Map
{
    public enum RoomType
    {
        Cell=0,
        Cafiteria=1,
        Matinence=2
    }
    internal class Room
    {
        private Tile[,] _map;
        private List<GameObject> _props;
        private List<Door> _doors;
        private RoomType _roomType;

        public Room(BinaryReader binaryData)
        {

        }

        public void Draw(SpriteBatch sb) {
            throw new NotImplementedException("Draw not been created yet in Room");
        }
        private void ParseData(BinaryReader binaryData) {
            throw new NotImplementedException("ParseData not been created yet in Room");

        }
    }
}
