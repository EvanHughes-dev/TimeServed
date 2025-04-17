using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.GameObjects.Props;
using System;
using System.IO;

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

        /// <summary>
        /// Initialize GameplayManager to create the player and map
        /// </summary>
        public static void Initialize(Point screenSize)
        {
            Level = 1;
            PlayerObject = new Player(new Point(4, 5), AssetManager.PlayerTexture, screenSize);
            OnPlayerUpdate?.Invoke(PlayerObject);
            MapManager.Initialize();
        }

        public static void Update(GameTime gameTime)
        {
            float floatGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Update Player
            PlayerObject.Update(floatGameTime);

            //Update all the cameras in the current room
            foreach (Camera cam in MapManager.CurrentRoom.Cameras)
            {
                cam.Update(floatGameTime);
            }
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

        /// <summary>
        /// Called to reset the level to the starting state
        /// </summary>
        public static void LevelReset(){
            // TODO: This never gets called, I think its safe to delete?
            Level = 1;
            PlayerObject = new Player(new Point(5, 5), AssetManager.PlayerTexture, MapUtils.ScreenSize);
            MapManager.ChangeLevel();
            OnPlayerUpdate?.Invoke(PlayerObject);
            ReplayManager.ClearData();
        }

        /// <summary>
        /// Clears checkpoint data and player data
        /// </summary>
        public static void ClearSavedData()
        {
            //Delete saved data
            if (Directory.Exists("./CheckpointData"))
                RecursiveDelete("./CheckpointData");
            if (Directory.Exists("./PlayerData"))
                RecursiveDelete("./PlayerData");

            //Save the current map data to the initial checkpoint (the "player spawn")
            TriggerManager.PlayerSpawn.Activate(PlayerObject);
        }

        /// <summary>
        /// Recursively delete all contents of a folder
        /// </summary>
        /// <param name="folderPath">Folder to delete</param>
        private static void RecursiveDelete(string folderPath)
        {

            foreach (string file in Directory.GetFiles(folderPath))
                File.Delete(file);

            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                RecursiveDelete(folder);
            }

            Directory.Delete(folderPath);
        }
    }
}
