using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount
{
    public static class AssetManager
    {
        /*
         * Place public read-only properties for assets here!
         */
        public static Texture2D[] TileMap { get; private set; }
        public static Texture2D[] PropTextures { get; private set; }

        public static void LoadContent(ContentManager content)
        {
            throw new NotImplementedException(
                "LoadContent has not been implemented in AssetManager"
            );
        }
    }
}
