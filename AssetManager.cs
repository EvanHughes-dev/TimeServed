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
        /// <summary>
        /// An array corresponding to all tiles for the map's environment
        /// </summary>
        public static Texture2D[] MapTiles { get; private set; }

        public static void LoadContent(ContentManager content)
        {
            throw new NotImplementedException(
                "LoadContent has not been implemented in AssetManager"
            );
        }
    }
}
