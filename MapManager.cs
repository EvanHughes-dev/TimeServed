using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using MakeEveryDayRecount.GameObjects;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// Keep track of the room the player is currently 
    /// in and load the needed map
    /// </summary>
    internal class MapManager
    {
        private Room _currentRoom;
        private Room[] _rooms;
        private Texture2D[] _tileSprites;

        public MapManager(Texture2D[] tileSprites) { 
            _tileSprites = tileSprites;
            _rooms = LoadMapData();
        }



        public void TransitionRoom(Door transDoor) {
            throw new NotImplementedException("TransitionRoom not been created yet in MapManager");
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
          throw new NotImplementedException("CheckPlayerCollision has not been created yet in MapManager");
        }

        /// <summary>
        /// Load all the needed data relating to each room 
        /// from the corresponding files and format them
        /// </summary>
        /// <returns>Formated data loaded from files</returns>
        private Room[] LoadMapData()
        {

            // only read binary data here
            // each room is in charge of parsing itself
            throw new NotImplementedException("LoadMapData has not been created yet in MapManager");
        }



    }
}
