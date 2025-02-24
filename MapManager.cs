using MakeEveryDayRecount.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace MakeEveryDayRecount
{
    /// <summary>
    /// Keep track of the room the player is currently 
    /// in and load the needed map
    /// </summary>
    internal class MapManager
    {
        /// <summary>
        /// Keep track of the room the player is currently in
        /// </summary>
        public enum Rooms
        {
            Cell = 0,
            Cafiteria = 1,
            Matinence = 2,
            // add rest of rooms
        }

        private Rooms _currentRoom;

        private Tile[,] _currentRoomTiles;
        private Tile[][,] _allRoomTiles;

        private Texture2D[] _tileSprites;

        public MapManager(Texture2D[] tileSprites)
        {
            _tileSprites = tileSprites;
            // Assume the player always starts in their cell
            // May change later in different levels
            _currentRoom = Rooms.Cell;

            // Retrieve data from the files and establish the 
            // current room to display on first call
            _allRoomTiles = LoadMapData();
            _currentRoomTiles = _allRoomTiles[(int)_currentRoom];
        }

        /// <summary>
        /// Draw the current room to the screen
        /// </summary>
        /// <param name="batch">Batch of sprites to add to</param>
        public void Draw(SpriteBatch batch)
        {
            throw new NotImplementedException("Draw has not been created yet in MapManager");
        }

        /// <summary>
        /// Check if a player is allowed to move to a certain tile
        /// </summary>
        /// <param name="playerDest">Tile player wants to move to</param>
        /// <returns>If the player is allowed to move there</returns>
        public bool CheckPlayerCollision(Point playerDest)
        {
          throw new NotImplementedException("CheckPlayerCollisionhas not been created yet in MapManager");
        }

        /// <summary>
        /// Load all the needed data relating to each room 
        /// from the corresponding files and format them
        /// </summary>
        /// <returns>Formated data loaded from files</returns>
        private Tile[][,] LoadMapData()
        {

            /*
             * Form of data files
             * 
             * int tileArrayLength
             * int tileArrayHeight
             * 
             * Tiles:
             *      Bit
             *      0 -> Door/vent
             *      1 -> Other (Floor, Wall, Bench)
             *      
             *      If 0
             *          int source
             */
            throw new NotImplementedException("LoadMapData has not been created yet in MapManager");
        }



    }
}
