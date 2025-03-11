using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.DebugModes
{
    internal class MapDebug : BaseDebug
    {
        private MapManager _map;
        private Room _currentRoom;

        /// <summary>
        /// Initialize debug mode map
        /// </summary>
        /// <param name="sf">Sprite font to draw debug text</param>
        /// <param name="gameplayManager">Gameplay manager to pull data from</param>
        public MapDebug(SpriteFont sf, GameplayManager gameplayManager)
            : base(sf, gameplayManager)
        {
            _map = gameplayManager.Map;
        }

        /// <summary>
        /// Draw all debug tiles before the text
        /// </summary>
        /// <param name="sb">Sprite batch to draw to</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb, "Map");
        }

        /// <summary>
        /// Update the room debug data is drawn from
        /// </summary>
        /// <param name="newRoom">New room to have selected</param>
        private void UpdateCurrentRoom(Room newRoom)
        {
            _currentRoom = newRoom;
        }
    }
}
