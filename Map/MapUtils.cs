using Microsoft.Xna.Framework;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.Players;

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

        private static Game1 game1Instance;

        #region Utility Properties

        /// <summary>
        /// Get the size of the screen in pixels
        /// </summary>
        public static Point ScreenSize
        {
            get => game1Instance.ScreenSize;
        }

        /// <summary>
        /// Get the center point of the screen in pixels
        /// </summary>
        public static Point ScreenCenter
        {
            get { return new Point(ScreenSize.X / 2, ScreenSize.Y / 2); }
        }

        /// <summary>
        /// The first tile that map will start shifting with the player
        /// </summary>
        public static Point MinDisplayableTile
        {
            get
            {
                return new Point(
                    ScreenCenter.X / AssetManager.TileSize.X,
                    ScreenCenter.Y / AssetManager.TileSize.Y
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
                return new Point(
                    (MapSizePixels.X - ScreenCenter.X) / AssetManager.TileSize.X,
                    (MapSizePixels.Y - ScreenCenter.Y) / AssetManager.TileSize.Y
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
            get
            {
                return new Point(
                    MapSizeTiles.X * AssetManager.TileSize.X,
                    MapSizeTiles.Y * AssetManager.TileSize.Y
                );
            }
        }

        /// <summary>
        /// Get the point to offset between screen and world positions assuming the map
        /// is larger than the size of the screen. To compensate for any needed offset,
        /// add the PixelOffset value. This value should be subtracted from the target's 
        /// world position
        /// </summary>
        public static Point WorldToScreen
        {
            get; private set;
        }

        /// <summary>
        /// Get the number of pixel the map needs to be offset by due to the map
        /// being smaller than the size of the screen
        /// </summary>
        public static Point PixelOffset
        {
            get; private set;
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
            return new Point(xPos * AssetManager.TileSize.X, yPos * AssetManager.TileSize.Y);
        }

        /// <summary>
        /// Calculate the location of a tile in screen space
        /// </summary>
        /// <param name="tileLocation">Tile location to translate</param>
        /// <returns>Point of the tile in screen space</returns>
        public static Point TileToScreen(Point tileLocation)
        {
            return TileToWorld(tileLocation) - WorldToScreen + PixelOffset;
        }

        /// <summary>
        /// Calculate the location of a tile in screen space
        /// </summary>
        /// <param name="xPos">X position in tile space</param>
        /// <param name="yPos">Y position in tile space</param>
        /// <returns>Point of the tile in screen space</returns>
        public static Point TileToScreen(int xPos, int yPos)
        {
            return TileToWorld(xPos, yPos) - WorldToScreen + PixelOffset;
        }

        /// <summary>
        /// Get the point to offset between screen and world positions assuming the map
        /// is larger than the size of the screen. To compensate for any needed offset,
        /// add the PixelOffset value
        /// </summary>
        private static void CalculateWorldToScreen()
        {
            Point worldToScreen = TileToWorld(_currentPlayer.Location) - ScreenCenter;
            // Clamp the screen's position so only tiles will be displayed without any empty space
            WorldToScreen = new Point(
                 MathHelper.Clamp(worldToScreen.X, 0, MapSizePixels.X - ScreenSize.X),
                 MathHelper.Clamp(worldToScreen.Y, 0, MapSizePixels.Y - ScreenSize.Y)
             );
        }

        /// <summary>
        /// Calculate the difference between the size of the map and size of the screen
        /// </summary>
        private static void CalculatePixelOffset()
        {
            Point offset = Point.Zero;
            if (ScreenSize.X > MapSizePixels.X)
                offset.X = (ScreenSize.X - MapSizePixels.X) / 2;

            if (ScreenSize.Y > MapSizePixels.Y)
                offset.Y = (ScreenSize.Y - MapSizePixels.Y) / 2;
            PixelOffset = offset;
        }

        /// <summary>
        /// Take in a location on the screen in pixels and return the position in the world in tile
        /// </summary>
        /// <param name="screenPos">Position on the screen</param>
        /// <returns>Position in tile space</returns>
        public static Point ScreenToTile(Point screenPos)
        {
            if (MapManager.CurrentRoom == null)
                return Point.Zero;
            Point screenToWorld = new Point(screenPos.X + WorldToScreen.X, screenPos.Y + WorldToScreen.Y) - PixelOffset;

            return new Point(screenToWorld.X / AssetManager.TileSize.X, screenToWorld.Y / AssetManager.TileSize.Y);
        }
        #endregion

        /// <summary>
        /// Called when the room the player is in changes by MapManager
        /// </summary>
        /// <param name="newRoom">The new room being displayed</param>
        private static void SetCurrentRoom(Room newRoom)
        {
            _currentRoom = newRoom;
        }

        /// <summary>
        /// Called when the current player is changed by GameplayManager
        /// </summary>
        /// <param name="player">New player to set</param>
        private static void SetCurrentPlayer(Player player)
        {
            _currentPlayer = player;
        }

        /// <summary>
        /// Initialize the ScreenSize and events to update the components
        /// of this file that will change during run time
        /// </summary>
        /// <param name="game1">Reference to this instance of the game</param>
        /// <param name="gameplayManager">Reference to the overall GameplayManager</param>
        public static void Initialize(Game1 game1)
        {
            game1Instance = game1;
            GameplayManager.OnPlayerUpdate += SetCurrentPlayer;
            MapManager.OnRoomUpdate += SetCurrentRoom;
            // Despite the delegate event systems, the object have already been initialized in
            // memory before the function is added to the event, so get the initial value manually
            _currentPlayer = GameplayManager.PlayerObject;
            _currentRoom = MapManager.CurrentRoom;
        }

        /// <summary>
        /// Update all the values that will not change during the draw phase. Call
        /// this function last in the update function to ensure accurate calculations
        /// </summary>
        public static void UpdateValues()
        {
            CalculateWorldToScreen();
            CalculatePixelOffset();
        }
    }
}
