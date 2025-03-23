using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount
{
    public static class AssetManager
    {
        /// <summary>
        /// Array of the assets for the props in the game
        /// </summary>
        public static Texture2D[] PropTextures { get; private set; }
        /// <summary>
        /// Array with every type of tile asset we have in the game
        /// </summary>
        public static Texture2D[] TileMap { get; private set; }
        /// <summary>
        /// Array of the textures for an inventory box in different states
        /// </summary>
        public static Texture2D[] InventoryBoxes { get; private set; }

        /// <summary>
        /// Sprite sheet for the player
        /// </summary>
        public static Texture2D PlayerTexture { get; private set; }

        /// <summary>
        /// Texture for the doors. Indexed in order top, right, bottom, left
        /// </summary>
        public static Texture2D[] DoorTexture { get; private set; }

        /// <summary>
        /// Texture for the debug tile for walkable tiles
        /// </summary>
        public static Texture2D DebugWalkableTile { get; private set; }

        /// <summary>
        /// Texture for the debug tile for not walkable tiles
        /// </summary>
        public static Texture2D DebugNotWalkableTile { get; private set; }

        /// <summary>
        /// Default font for debugging
        /// </summary>
        public static SpriteFont TimesNewRoman20 { get; private set; }

        /// <summary>
        /// The size of each tile, in pixels
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
            InventoryBoxes = new Texture2D[]
            {
                content.Load<Texture2D>("InventoryBox_idle"),
                content.Load<Texture2D>("InventoryBox_hover"),
                content.Load<Texture2D>("InventoryBox_selected")
            };
            PlayerTexture = content.Load<Texture2D>("player");

            DebugWalkableTile = content.Load<Texture2D>("DebugAssets/WALKABLE");
            DebugNotWalkableTile = content.Load<Texture2D>("DebugAssets/NOT_WALKABLE");
            TimesNewRoman20 = content.Load<SpriteFont>("Arial20");

            DoorTexture = new Texture2D[4];

            DoorTexture[0] = content.Load<Texture2D>("Doors/TempDoor-Top");
            DoorTexture[1] = content.Load<Texture2D>("Doors/TempDoor-Right");
            DoorTexture[2] = content.Load<Texture2D>("Doors/TempDoor-Bottom");
            DoorTexture[3] = content.Load<Texture2D>("Doors/TempDoor-Left");
            TileSize = new Point(TileMap[0].Width, TileMap[0].Height);
        }
    }
}
