namespace MakeEveryDayRecount.Map.Tiles
{
    /// <summary>
    /// Hold data about whether a tile can be stood on
    /// and what sprite it should be
    /// </summary>
    internal class Tile
    {
        /// <summary>
        /// Get if the tile is walkable
        /// </summary>
        public bool IsWalkable { get; private set; }

        /// <summary>
        /// Get then index that corresponds to this tile's sprite
        /// </summary>
        public int SpriteIndex { get; private set; }

        /// <summary>
        /// Create an instance of a tile with the needed data
        /// </summary>
        /// <param name="isWalkable">If the tile can be walked on</param>
        /// <param name="spriteIndex">The sprite index of the tile</param>
        public Tile(bool isWalkable, int spriteIndex)
        {
            IsWalkable = isWalkable;
            SpriteIndex = spriteIndex;
        }
    }
}
