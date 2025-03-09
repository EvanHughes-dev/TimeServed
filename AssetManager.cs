using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace MakeEveryDayRecount
{
    public static class AssetManager
    {
        /// <summary>
        /// Array with every type of tile asset we have in the game
        /// </summary>
        public static Texture2D[] TileMap { get; private set; }
        /// <summary>
        /// Spritesheet for the player
        /// </summary>
        public static Texture2D PlayerTexture { get; private set; }
        /// <summary>
        /// Texture for the door
        /// </summary>
        public static Texture2D DoorTexture { get; private set; }
        /// <summary>
        /// Array of the assets for the props in the game
        /// </summary>
        public static Texture2D[] PropTextures { get; private set; }
        /// <summary>
        /// The size of all tiles
        /// </summary>
        public static Point TileSize { get; private set; }
        public static void LoadContent(ContentManager content)
        {
            PropTextures = new Texture2D[]
            {
                content.Load<Texture2D>("item_idCard"),
                content.Load<Texture2D>("item_screwdriver"),
                content.Load<Texture2D>("item_wireCutters"),
                content.Load<Texture2D>("item_hook"),
                content.Load<Texture2D>("item_hookAndRope")
            };
            TileMap = new Texture2D[]
            {
                content.Load<Texture2D>("tile_testWall"),
                content.Load<Texture2D>("tile_testWalkable")
            };
            PlayerTexture = content.Load<Texture2D>("player");

            TileSize = new Point(PlayerTexture.Width, PlayerTexture.Height);
        }
    }
}
