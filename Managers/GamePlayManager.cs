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
    internal class GameplayManager
    {
        /// <summary>
        /// The current level being played
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Access the reference to the Player
        /// </summary>
        public Player PlayerObject { get; private set; }

        /// <summary>
        /// Access the current MapManager
        /// </summary>
        public MapManager Map { get; private set; }

        public OnPlayerUpdate OnPlayerUpdate;

        /// <summary>
        /// Initialize GameplayManager to create the player and map
        /// </summary>
        public GameplayManager(Point screenSize)
        {
            Level = 1;
            PlayerObject = new Player(new Point(5, 5), AssetManager.PlayerTexture, this, screenSize);
            Map = new MapManager(this);
            OnPlayerUpdate?.Invoke(PlayerObject);
        }

        public void Update(GameTime gameTime)
        {
            //Update Player
            PlayerObject.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            //DELETE this. It's just for testing cams
            Map.CurrentRoom.roomTestCamera.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Draws the map and the player.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        public void Draw(SpriteBatch sb)
        {
            //Draw the map
            Map.Draw(sb);

            //Draw the player
            PlayerObject.Draw(sb);
        }

        /// <summary>
        /// Enter replay mode
        /// </summary>
        public void ReplayMode()
        {
            Level = 1;
            Map.ChangeLevel();
            PlayerObject.ChangeRoom(new Point(5, 5));
            PlayerObject.ClearStates();
        }
    }
}
