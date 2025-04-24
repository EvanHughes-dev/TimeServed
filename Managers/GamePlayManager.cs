using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.GameObjects.Props;
using System;
using System.IO;
using MakeEveryDayRecount.GameObjects.Triggers;

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
            Level = 0;
            PlayerObject = new Player(new Point(0, 0), AssetManager.PlayerTexture, screenSize);
            OnPlayerUpdate?.Invoke(PlayerObject);
            MapManager.Initialize();
        }

        /// <summary>
        /// Begin the next level
        /// </summary>
        public static void NextLevel()
        {
            Level++;
            MapManager.ChangeLevel(Level);
            PlayerObject.ChangeRoom(TriggerManager.PlayerSpawn.Location);
            TriggerManager.PlayerSpawn.Activate(PlayerObject);
        }

        public static void Update(float gameTime)
        {
            //Update Player
            PlayerObject.Update(gameTime);

            //Update all the cameras in the current room
            foreach (Camera cam in MapManager.CurrentRoom.Cameras)
            {
                cam.Update(gameTime);
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
            MapManager.ChangeLevel(Level);
            PlayerObject.ChangeRoom(TriggerManager.PlayerSpawn.Location);
            TriggerManager.PlayerSpawn.Activate(PlayerObject);
            PlayerObject.ClearStates();
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
