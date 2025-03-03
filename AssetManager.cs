using Microsoft.Xna.Framework;
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
        public static Texture2D[] TileMap { get; }
        public static Texture2D PlayerTexture { get; }
        public static Texture2D DoorTexture { get; }
        public static Texture2D[] PropTextures { get; }


        public static void LoadContent(ContentManager content)
        {
            
            throw new NotImplementedException("LoadContent has not been implemented in AssetManager");
        }
    }
}
