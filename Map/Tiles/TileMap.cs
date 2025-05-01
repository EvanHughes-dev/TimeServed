using Microsoft.Xna.Framework;

namespace MakeEveryDayRecount.Map.Tiles
{
    /// <summary>
    /// This class exists to make accessing the tile map easier,
    /// including taking much of the stress off the Room.cs to 
    /// track and handle when things are interactable
    /// </summary>
    class TileMap
    {
        /// <summary>
        /// Size of the map in tiles
        /// </summary>
        public Point MapSize { get; private set; }

        private Tile[,] _map;

        /// <summary>
        /// Initialize a new map of size xLength x yLength
        /// </summary>
        /// <param name="xLength">Length on the x axis</param>
        /// <param name="yLength">Length on the y axis</param>
        public TileMap(int xLength, int yLength)
        {
            _map = new Tile[yLength, xLength];
            MapSize = new Point(xLength, yLength);
        }

        /// <summary>
        /// Create a map with size 0 x 0
        /// </summary>
        public TileMap() : this(0, 0)
        {

        }

        /// <summary>
        /// Get and set values within the map
        /// </summary>
        /// <param name="yIndex">Y index to access</param>
        /// <param name="xIndex">X index to access</param>
        /// <returns>Tile at the provided location</returns>
        public Tile this[int yIndex, int xIndex]
        {
            get => _map[yIndex, xIndex];
            set => _map[yIndex, xIndex] = value;
        }

        /// <summary>
        /// Get and set values within the map
        /// </summary>
        /// <param name="accessPoint">Point to access in the array</param>
        /// <returns>Tile at the provided location</returns>
        public Tile this[Point accessPoint]
        {
            get => _map[accessPoint.Y, accessPoint.X];
            set => _map[accessPoint.Y, accessPoint.X] = value;
        }

        /// <summary>
        /// Check that the provided indexes are within the bounds of the array
        /// </summary>
        /// <param name="xIndex">X index to check</param>
        /// <param name="yIndex">Y index to check</param>
        /// <returns>True if both indexes are within the bounds. False otherwise</returns>
        public bool WithinBounds(int xIndex, int yIndex)
        {
            return xIndex >= 0 && yIndex >= 0 && xIndex < MapSize.X && yIndex < MapSize.Y;
        }

        /// <summary>
        /// Check that the provided indexes are within the bounds of the array
        /// </summary>
        /// <param name="xIndex">X index to check</param>
        /// <param name="yIndex">Y index to check</param>
        /// <returns>True if both indexes are within the bounds. False otherwise</returns>
        public bool WithinBounds(Point pointToCheck)
        {
            return WithinBounds(pointToCheck.X, pointToCheck.Y);
        }

    }

}