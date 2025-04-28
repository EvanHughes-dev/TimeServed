using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.GameObjects.Props;
using System.IO;
using System.Reflection.Metadata;
using MakeEveryDayRecount.Players.InventoryFiles;

namespace MakeEveryDayRecount.Managers
{
    /// <summary>
    /// Called when the player object is updated in memory
    /// </summary>
    /// <param name="player">New player object</param>
    delegate void OnPlayerUpdate(Player player);

    /// <summary>
    /// Called when the player triggers a win condition trigger
    /// </summary>
    delegate void OnWinCondition();

    /// <summary>
    /// Manager of Player and the Map Manager.
    /// </summary>
    internal static class GameplayManager
    {
        /// <summary>
        /// The current level being played
        /// </summary>
        public static int Level { get; private set; }

        private readonly static int _highestLevel = 3;

        /// <summary>
        /// Get the highest level for the game
        /// </summary>
        public static int HighestLevel { get => _highestLevel; }

        /// <summary>
        /// Access the reference to the Player
        /// </summary>
        public static Player PlayerObject { get; private set; }

        public static OnPlayerUpdate OnPlayerUpdate;
        public static OnWinCondition WinCondition;

        /// <summary>
        /// Initialize GameplayManager to create the player and map
        /// </summary>
        public static void Initialize(Point screenSize)
        {
            Level = 2;
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
            MapManager.ChangeRoom(TriggerManager.PlayerSpawn.RoomIndex);
            PlayerObject.ClearStates();
            GiveItems(Level);
            // Don't activate triggers if replay mode is active
            if (!ReplayManager.PlayingReplay)
                TriggerManager.PlayerSpawn.Activate(PlayerObject);
        }

        /// <summary>
        /// Assign the level from a saved checkpoint and update player
        /// </summary>
        /// <param name="level">Level to update to</param>
        public static void LoadLevelFromCheckpoint(int level)
        {
            Level = level;
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
            Level = _highestLevel;
            MapManager.ChangeLevel(Level);
            PlayerObject.ChangeRoom(TriggerManager.PlayerSpawn.Location);
            MapManager.ChangeRoom(TriggerManager.PlayerSpawn.RoomIndex);
            PlayerObject.ClearStates();
            GiveItems(Level);
        }


        /// <summary>
        /// Clears checkpoint data and player data
        /// </summary>
        public static void ClearSavedData()
        {
            //Delete saved data
            if (Directory.Exists("./CheckpointData"))
                RecursiveDelete("./CheckpointData");
        }

        /// <summary>
        /// When the player completes the win condition for the level
        /// </summary>
        public static void PlayerWinTrigger()
        {
            if (ReplayManager.PlayingReplay)
            {
                if (Level != 1)
                {
                    Level--;
                    ReplayManager.NextLevel();
                    MapManager.ChangeLevel(Level);
                    PlayerObject.ChangeRoom(TriggerManager.PlayerSpawn.Location);
                    MapManager.ChangeRoom(TriggerManager.PlayerSpawn.RoomIndex);
                    PlayerObject.ClearStates();
                    GiveItems(Level);
                }
            }
            else
            {
                WinCondition?.Invoke();
            }
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

        /// <summary>
        /// Gives items to the player based on their current level
        /// </summary>
        /// <param name="level"></param>
        private static void GiveItems(int level)
        {
            PlayerObject.AddItemToInventory(new Item(Point.Zero, AssetManager.PropTextures, 0, "Card", Door.DoorKeyType.Card));
            if (level < 3)
                PlayerObject.AddItemToInventory(new Item(Point.Zero, AssetManager.PropTextures, 2, "Wirecutters", Door.DoorKeyType.None));
            if (level < 2)
                PlayerObject.AddItemToInventory(new Item(Point.Zero, AssetManager.PropTextures, 1, "Screwdriver", Door.DoorKeyType.Screwdriver));
        }
    }
}
