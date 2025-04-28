using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace MakeEveryDayRecount.Managers
{

    /// <summary>
    /// Take care of all non-sound asset loading here. Create a central location for 
    /// all gameobjects, tiles, players, and text to get their needed attributes from
    /// </summary>
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

        /// <summary>
        /// Get all the buttons for the ui
        /// </summary>
        public static Texture2D[] MenuButtons { get; private set; }

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

        /// <summary>
        /// An array of all the level change animations. Indexed around the current level - 1
        /// </summary>
        public static Texture2D[][] LevelChanges { get; private set; }

        #region Font Assets

        /// <summary>
        /// Default font for debugging
        /// </summary>
        public static SpriteFont Arial20 { get; private set; }

        /// <summary>
        /// Get scalable arial fonts
        /// </summary>
        public static SpriteFont[] ArialFonts { get; private set; }

        /// <summary>
        /// Get the font for the title
        /// </summary>
        public static SpriteFont TitleFont { get; private set; }
        #endregion

        /// <summary>
        /// The size of each tile, in pixels
        /// </summary>
        public static Point TileSize { get; private set; }

        /// <summary>
        /// The size of each tile divided by 2
        /// </summary>
        public static Point HalfTileSize => new Point(TileSize.X / 2, TileSize.Y / 2);

        /// <summary>
        /// Load all content needed for the game at once
        /// </summary>
        /// <param name="content">ContentManager to load with</param>
        public static void LoadContent(ContentManager content)
        {

            //LOAD PLAYER ASSETS

            PlayerTexture = content.Load<Texture2D>("Player/player");
            PlayerDisguisedTexture = content.Load<Texture2D>("Player/player_disguised");

            //LOAD MAP ASSETS

            TileMap = new Texture2D[]
            {
                content.Load<Texture2D>("Tiles/void"),

                content.Load<Texture2D>("Tiles/tile_wall_corner_inner_tr"),
                content.Load<Texture2D>("Tiles/tile_wall_corner_inner_tl"),
                content.Load<Texture2D>("Tiles/tile_wall_corner_inner_br"),
                content.Load<Texture2D>("Tiles/tile_wall_corner_inner_bl"),

                content.Load<Texture2D>("Tiles/tile_wall_corner_outer_tr"),
                content.Load<Texture2D>("Tiles/tile_wall_corner_outer_tl"),
                content.Load<Texture2D>("Tiles/tile_wall_corner_outer_br"),
                content.Load<Texture2D>("Tiles/tile_wall_corner_outer_bl"),

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
                content.Load<Texture2D>("Doors/prop_door_locked-top"),
                content.Load<Texture2D>("Doors/prop_door_locked-right"),
                content.Load<Texture2D>("Doors/prop_door_locked-bottom"),
                content.Load<Texture2D>("Doors/prop_door_locked-left"),

                content.Load<Texture2D>("Doors/prop_vent-top"),
                content.Load<Texture2D>("Doors/prop_vent-right"),
                content.Load<Texture2D>("Doors/prop_vent-bottom"),
                content.Load<Texture2D>("Doors/prop_vent-left"),

                content.Load<Texture2D>("Doors/Door-Top"),
                content.Load<Texture2D>("Doors/Door-Right"),
                content.Load<Texture2D>("Doors/Door-Bottom"),
                content.Load<Texture2D>("Doors/Door-Left")
            };
            TileSize = InterfaceManager.ScalePointUniform(new Point(TileMap[0].Width / 3, TileMap[0].Height / 3));

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
                content.Load<Texture2D>("UI/ui_cursor "), //DO NOT REMOVE THE SPACE
                content.Load<Texture2D>("UI/ui_cursorHover")
            };

            MenuButtons = new Texture2D[]
            {
                content.Load<Texture2D>("UI/ui_button_play"), 
                content.Load<Texture2D>("UI/ui_button_load"), 
                content.Load<Texture2D>("UI/ui_button_exit"), 
                content.Load<Texture2D>("UI/ui_title"), 
                content.Load<Texture2D>("UI/ui_button_arrow"), 
                content.Load<Texture2D>("UI/ui_button_arrow-left") 
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
                content.Load<Texture2D>("Items/prop_cameraOn"),
                content.Load<Texture2D>("Items/prop_wireBox"),
                content.Load<Texture2D>("Items/prop_wireBox_snipped")
            };
            Boxes = new Texture2D[]{
                content.Load<Texture2D>("Items/prop_box")
            };

            // LOAD INTERMEDIARY ANIMATIONS
            LevelChanges = new Texture2D[2][];
            LevelChanges[0] = LoadLevelAnimation(1, content);
            LevelChanges[1] = LoadLevelAnimation(2, content);

            //FONT STUFF
            Arial20 = content.Load<SpriteFont>("Fonts/Arial20");

            ArialFonts = new SpriteFont[]{
                content.Load<SpriteFont>("Fonts/Arial5"),
                content.Load<SpriteFont>("Fonts/Arial10"),
                content.Load<SpriteFont>("Fonts/Arial20"),
                content.Load<SpriteFont>("Fonts/Arial30"),
            };

            TitleFont = content.Load<SpriteFont>("Fonts/Arial50");
        }

        /// <summary>
        /// Find the images for the animation from one level to the next
        /// </summary>
        /// <param name="startingLevel">Level the animation is coming from</param>
        /// <param name="content">ContentManager to load from</param>
        /// <returns>Images ordered in ascending order</returns>
        private static Texture2D[] LoadLevelAnimation(int startingLevel, ContentManager content)
        {
            int startingDay = 6 - startingLevel;
            int destDay = startingDay - 1;


            // the files are in the form a-b-c.png where a-b are the same as the starting day
            // and destDay and c is the order of the image in the overall animation. 
            // This code parses the name and orders based on the c value in ascending order
            string[] fileNames = Directory
                .GetFiles($"./Content/DayChange/{startingDay}-{destDay}")
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(name =>
                {
                    string withoutExtension = Path.GetFileNameWithoutExtension(name);
                    string[] parts = withoutExtension.Split('-');
                    return int.Parse(parts[2]);
                })
                .ToArray();

            Texture2D[] textures = new Texture2D[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
                textures[i] = content.Load<Texture2D>($"DayChange/{startingDay}-{destDay}/{fileNames[i]}");

            return textures;

        }
    }
}
