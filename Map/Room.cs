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
    internal class Room
    {
        private Tile[,] _map;
        private List<GameObject> _props;
        private List<Door> _doors;
        public int RoomIndex { get; private set; }
        public string RoomName { get; private set; }

        public Room(string filePath, string roomName, int roomIndex)
        {
            RoomIndex = roomIndex;
            RoomName = roomName;
            ParseData(filePath);
        }

        public void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException("Draw not been created yet in Room");
        }

        private void ParseData(string filePath)
        {
            /*
            * Form of the room data is as follows
            *
            * int tileMapWidth
            * int tileMapHeight
            *
            * Tiles:
            *   bit isWalkable 0 -> false 1 -> true
            *   int textureIndex
            *
            * int gameObjectCount
            *
            * GameObject:
            *   int propIndex
            *   int positionX
            *   int positionY
            *
            *   bit isDoor 0 -> false 1 -> true
            *   int entranceIndex
            *   int destRoom
            *   int destDoor
            *
            */
            Stream stream = File.OpenRead(filePath);
            BinaryReader binaryReader = new BinaryReader(stream);

            int tileMapWidth = binaryReader.ReadInt32();
            int tileMapHeight = binaryReader.ReadInt32();

            _map = new Tile[tileMapWidth, tileMapHeight];

            for (int tileXIndex = 0; tileXIndex < tileMapWidth; tileXIndex++)
            {
                for (int tileYIndex = 0; tileYIndex < tileMapHeight; tileYIndex++)
                {
                    _map[tileXIndex, tileYIndex] = new Tile(
                        binaryReader.ReadBoolean(),
                        binaryReader.ReadInt32()
                    );
                }
            }

            int numberOfGameObjects = binaryReader.ReadInt32();

            while (numberOfGameObjects > 0)
            {
                //TODO finish

                numberOfGameObjects--;
            }

            binaryReader.Close();
        }
    }
}
