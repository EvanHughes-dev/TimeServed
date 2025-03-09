using Microsoft.Xna.Framework;

namespace MakeEveryDayRecount.Map
{
    /// <summary>
    /// This class serves to handle any calculations needed related 
    /// to the current map and provides utility to the entire project 
    /// relating to converting between tile space, screen space,and game space
    /// </summary>
    internal static class MapUtils
    {
        private static Room _currentRoom;
        private static Player _currentPlayer;

        public static Point ScreenSize;

        private const int TileSize = 128;

        #region Utility Properties

        /// <summary>
        /// Get the center point of the screen in pixels
        /// </summary>
        public static Point ScreenCenter {
            get {  return new Point(ScreenSize.X/2, ScreenSize.Y/2); }
        }

        /// <summary>
        /// The first tile that map will start shifting
        /// </summary>
        public static Point MinDisplayableTile
        {
            get
            {
                return
                    new Point(
                    (int)ScreenSize.X / 2 / 128,
                    (int)ScreenSize.Y / 2 / 128
                    );
            }
        }

        /// <summary>
        /// The last tile the map will move with the player
        /// </summary>
        public static Point MaxDisplayableTile
        {
            get
            {
                return
                    new Point(
                    (int)(MapSizePixels.X- ScreenSize.X / 2) / 128,
                    (int)(MapSizePixels.Y - ScreenSize.Y / 2) / 128
                    );
            }
        }

        
        /// <summary>
        /// Get the size of the current map in tiles
        /// </summary>
        public static Point MapSizeTiles
        {
            get { return _currentRoom.MapSize; }
        }

        /// <summary>
        /// Get the size of the current map in pixels
        /// </summary>
        public static Point MapSizePixels
        {
            get { return new Point(MapSizeTiles.X * 128, MapSizeTiles.Y * 128); }
        }

        #endregion

        #region Utility Functions
        /// <summary>
        /// Convert an a position from tile space to world space
        /// </summary>
        /// <param name="tilePosition">Position in tile space</param>
        /// <returns>Position in world space</returns>
        public static Point TileToWorld(Point tilePosition)
        {
            return TileToWorld(tilePosition.X, tilePosition.Y);
        }

        /// <summary>
        /// Convert an a position from tile space to world space
        /// </summary>
        /// <param name="xPos">X position in tile space</param>
        /// <param name="yPos">Y position in tile space</param>
        /// <returns>Position in world space</returns>
        public static Point TileToWorld(int xPos, int yPos)
        {
            return new Point(xPos * TileSize, yPos * TileSize);
        }

        /// <summary>
        /// Get the point to offset between screen and world positions
        /// </summary>
        /// <returns>Point that corresponds to the distance between world and screen pos</returns>
        public static Point WorldToScreen()
        {
            Point worldToScreen = MapUtils.TileToWorld(_currentPlayer.Location) - ScreenCenter;
            // Clamp the screen's position so only tiles will be displayed without any empty space
            return new Point(
                MathHelper.Clamp(worldToScreen.X, 0, MapSizePixels.X - ScreenSize.X),
                MathHelper.Clamp(worldToScreen.Y, 0, MapSizePixels.Y - ScreenSize.Y)
            );
          
        }

        #endregion

        /// <summary>
        /// Change the room that is being displayed
        /// </summary>
        /// <param name="newRoom">The new room being displayed</param>
        public static void SetCurrentRoom(Room newRoom)
        {
            _currentRoom = newRoom;
        }

        /// <summary>
        /// Change the current player 
        /// </summary>
        /// <param name="player">New player to set</param>
        public static void SetCurrentPlayer(Player player)
        {
            _currentPlayer = player;
        }
    }
}
