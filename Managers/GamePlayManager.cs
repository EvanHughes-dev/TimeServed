using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.Managers
{
    /// <summary>
    /// Called when the player object is updated in memory
    /// </summary>
    /// <param name="player">New player object</param>
    delegate void OnPlayerUpdate(Player player);

    /// <summary>
    /// Manager of Player and the Map Manager.
    /// </summary>
    internal static class GameplayManager
    {
        /// <summary>
        /// The current level being played
        /// </summary>
        public static int Level { get; private set; }

        /// <summary>
        /// Access the reference to the Player
        /// </summary>
        public static Player PlayerObject { get; private set; }

        public static OnPlayerUpdate OnPlayerUpdate;

        public static void Initialize(Point screenSize)
        {
            Level = 1;
            PlayerObject = new Player(new Point(5, 5), AssetManager.PlayerTexture, screenSize);
            OnPlayerUpdate?.Invoke(PlayerObject);
            MapManager.Initialize();
        }
        public static void Update(GameTime gameTime)
        {
            //Update Player
            PlayerObject.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Draws the map and the player.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        public static void Draw(SpriteBatch sb)
        {
            //Draw the map
            MapManager.Draw(sb);

            //Draw the player
            PlayerObject.Draw(sb);
        }

        /// <summary>
        /// Enter replay mode
        /// </summary>
        public static void ReplayMode()
        {
            Level = 1;
            MapManager.ChangeLevel();
            PlayerObject.ChangeRoom(new Point(5, 5));
            PlayerObject.ClearStates();
        }
    }
}
