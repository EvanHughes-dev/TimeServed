using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.Managers
{
    public static class AssetManager
    {
        #region Player Assets

        /// <summary>
        /// Sprite sheet for the player
        /// </summary>
        public static Texture2D PlayerTexture { get; private set; }

        /// <summary>
        /// Sprite sheet for the player when disguised
        /// </summary>
        public static Texture2D PlayerDisguisedTexture { get; private set; }

        #endregion

        #region Map Assets

        /// <summary>
        /// Array with every type of tile asset we have in the game
        /// </summary>
        public static Texture2D[] TileMap { get; private set; }

        /// <summary>
        /// Texture for the doors. Indexed in order top, right, bottom, left
        /// </summary>
        public static Texture2D[] DoorTexture { get; private set; }

        #endregion

        #region Debug Assets

        /// <summary>
        /// Texture for the debug tile for walkable tiles
        /// </summary>
        public static Texture2D DebugWalkableTile { get; private set; }

        /// <summary>
        /// Texture for the debug tile for not walkable tiles
        /// </summary>
        public static Texture2D DebugNotWalkableTile { get; private set; }

        #endregion

        #region UI Assets
        /// <summary>
        /// Default texture for all buttons
        /// </summary>
        public static Texture2D DefaultButton { get; private set; }

        /// <summary>
        /// Array of the textures for an inventory box in different states
        /// </summary>
        public static Texture2D[] InventoryBoxes { get; private set; }

        /// <summary>
        /// UI display for tiles the camera can see
        /// </summary>
        public static Texture2D CameraSight { get; private set; }

        /// <summary>
        /// Holds the textures of the cursor when idle (0) and hovering (1)
        /// </summary>
        public static Texture2D[] CursorStates { get; private set; }

        #endregion

        #region Prop Assets

        /// <summary>
        /// Array of the assets for the props in the game
        /// </summary>
        public static Texture2D[] PropTextures { get; private set; }

        /// <summary>
        /// Array of camera assets (off = 0, on = 1)
        /// </summary>
        public static Texture2D[] CameraTextures { get; private set; }

        /// <summary>
        /// An array of all box textures
        /// </summary>
        public static Texture2D[] Boxes { get; private set; }

        #endregion

        #region Font Assets

        /// <summary>
        /// Default font for debugging
        /// </summary>
        public static SpriteFont Arial20 { get; private set; }
        public static SpriteFont[] ArialFonts;
        #endregion

        /// <summary>
        /// The size of each tile, in pixels
        /// </summary>
        public static Point TileSize { get; private set; }

        public static void LoadContent(ContentManager content)
        {

            //LOAD PLAYER ASSETS

            PlayerTexture = content.Load<Texture2D>("Player/player");
            PlayerDisguisedTexture = content.Load<Texture2D>("Player/player_disguised");

            //LOAD MAP ASSETS

            TileMap = new Texture2D[]
            {
                content.Load<Texture2D>("Tiles/void"),

                content.Load<Texture2D>("Tiles/tile_wall0_top"),
                content.Load<Texture2D>("Tiles/tile_wall1_top"),
                content.Load<Texture2D>("Tiles/tile_wall2_top"),
                content.Load<Texture2D>("Tiles/tile_wall3_top"),

                content.Load<Texture2D>("Tiles/tile_wall0_right"),
                content.Load<Texture2D>("Tiles/tile_wall1_right"),
                content.Load<Texture2D>("Tiles/tile_wall2_right"),
                content.Load<Texture2D>("Tiles/tile_wall3_right"),

                content.Load<Texture2D>("Tiles/tile_wall0_bottom"),
                content.Load<Texture2D>("Tiles/tile_wall1_bottom"),
                content.Load<Texture2D>("Tiles/tile_wall2_bottom"),
                content.Load<Texture2D>("Tiles/tile_wall3_bottom"),

                content.Load<Texture2D>("Tiles/tile_wall0_left"),
                content.Load<Texture2D>("Tiles/tile_wall1_left"),
                content.Load<Texture2D>("Tiles/tile_wall2_left"),
                content.Load<Texture2D>("Tiles/tile_wall3_left"),

                content.Load<Texture2D>("Tiles/tile_walkable0"),
                content.Load<Texture2D>("Tiles/tile_walkable1"),
                content.Load<Texture2D>("Tiles/tile_walkable2"),
                content.Load<Texture2D>("Tiles/tile_walkable3"),
                content.Load<Texture2D>("Tiles/tile_walkable4"),
            };
            DoorTexture = new Texture2D[]
            {
                content.Load<Texture2D>("Doors/Door-Top"),
                content.Load<Texture2D>("Doors/Door-Right"),
                content.Load<Texture2D>("Doors/Door-Bottom"),
                content.Load<Texture2D>("Doors/Door-Left")
            };
            TileSize = InterfaceManager.ScalePointUniform(new Point(TileMap[0].Width / 4, TileMap[0].Height / 4));

            //LOAD DEBUG ASSETS
            DebugWalkableTile = content.Load<Texture2D>("DebugAssets/WALKABLE");
            DebugNotWalkableTile = content.Load<Texture2D>("DebugAssets/NOT_WALKABLE");


            //LOAD UI ASSETS

            InventoryBoxes = new Texture2D[]
             {
                content.Load<Texture2D>("Player/Inventory/InventoryBox_idle"),
                content.Load<Texture2D>("Player/Inventory/InventoryBox_hover"),
                content.Load<Texture2D>("Player/Inventory/InventoryBox_selected")
             };

            DefaultButton = content.Load<Texture2D>("UI/DefaultButton");
            CameraSight = content.Load<Texture2D>("UI/UI_cameraSight");

            CursorStates = new Texture2D[]
            {
                content.Load<Texture2D>("UI/ui_cursor"),
                content.Load<Texture2D>("UI/ui_cursorHover")
            };

            //LOAD PROP ASSETS

            PropTextures = new Texture2D[]
            {
                content.Load<Texture2D>("Items/idCard"),
                content.Load<Texture2D>("Items/screwdriver"),
                content.Load<Texture2D>("Items/wireCutters"),
                content.Load<Texture2D>("Items/hook"),
                content.Load<Texture2D>("Items/hookAndRope"),

            };
            CameraTextures = new Texture2D[]
            {
                content.Load<Texture2D>("Items/prop_cameraOff"),
                content.Load<Texture2D>("Items/prop_cameraOn")
            };
            Boxes = new Texture2D[]{
                content.Load<Texture2D>("Items/Box")
            };

            //FONT STUFF
            Arial20 = content.Load<SpriteFont>("Fonts/Arial20");

            ArialFonts = new SpriteFont[]{
                content.Load<SpriteFont>("Fonts/Arial5"),
                content.Load<SpriteFont>("Fonts/Arial10"),
                content.Load<SpriteFont>("Fonts/Arial20"),
                content.Load<SpriteFont>("Fonts/Arial30"),
            };

        }
    }
}
