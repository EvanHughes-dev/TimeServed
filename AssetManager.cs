using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount
{
    public static class AssetManager
    {
        public static Texture2D[] TileMap { get; private set; }
        public static Texture2D PlayerTexture { get; private set; }
        public static Texture2D DoorTexture { get; private set; }
        public static Texture2D[] PropTextures { get; private set; }


        public static void LoadContent(ContentManager content)
        {
            PropTextures = new Texture2D[] {
                content.Load<Texture2D>("item_idCard"),
                content.Load<Texture2D>("item_screwdriver"),
                content.Load<Texture2D>("item_wireCutters"),
                content.Load<Texture2D>("item_hook"),
                content.Load<Texture2D>("item_hookAndRope")
            };
            TileMap = new Texture2D[] {
                content.Load<Texture2D>("tile_testWall"),
                content.Load<Texture2D>("tile_testWalkable")
            };

        }
    }
}
