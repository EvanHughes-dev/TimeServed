using Microsoft.Xna.Framework;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;
using System.IO;


namespace MakeEveryDayRecount.GameObjects.Triggers
{
    /// <summary>
    /// Trigger that saves the player's progress when they trigger it
    /// </summary>
    internal class Checkpoint : Trigger
    {
        //Properties

        /// <summary>
        /// Index of this checkpoint
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Whether or not this checkpoint can be activated
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Index of the room this checkpoint is housed in
        /// </summary>
        public int RoomIndex { get; private set; }

        public static readonly string BaseFolder = "./CheckpointData";

        /// <summary>
        /// Constructs a checkpoint
        /// </summary>
        /// <param name="location">Location of the top left-most point of the checkpoint in its room</param>
        /// <param name="index">Index of the checkpoint</param>
        /// <param name="width">Width of this checkpoint</param>
        /// <param name="height">Height of this checkpoint</param>
        /// <param name="active">Whether or not this checkpoint can be activated</param>
        public Checkpoint(Point location, int index, int width, int height, bool active)
            : base(location, width, height)
        {
            Index = index;
            Active = active;
        }


        /// <summary>
        /// Saves the map and player data
        /// </summary>
        /// <param name="player">The player object</param>
        /// <returns>True if the checkpoint successfully activated, false otherwise</returns>
        public override bool Activate(Player player)
        {
            // Don't use checkpoints if in replay mode.
            // This would lead to overwriting keyboard data
            if (ReplayManager.PlayingReplay)
                return false;
            //Makes sure the prior checkpoint has been activated
            //unless this is the first checkpoint, in which case it's always good to activate
            if (Index != 0)
                if (TriggerManager.Checkpoints[Index - 1].Active)
                    return false;
            if (!Active)
                return false;


            //Deactivate the checkpoint, need to before it has saved the data
            Active = false;

            //Always want to overwrite the folder if it exists
            if (Directory.Exists(BaseFolder))
                RecursiveDelete(BaseFolder);

            //(Re)make the folder
            Directory.CreateDirectory(BaseFolder);

            //Save map and player data
            MapManager.SaveMap(BaseFolder);
            player.Save(BaseFolder);

            //Save the room this checkpoint is located in
            RoomIndex = MapManager.CurrentRoom.RoomIndex;

            // Save the level in case the user wants to quit and come back later
            using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite($"{BaseFolder}/level.data")))
            {
                binaryWriter.Write(GameplayManager.Level);
                binaryWriter.Write(RoomIndex);
                binaryWriter.Write(Index);
            }

            //Update the current checkpoint
            TriggerManager.CurrentCheckpoint = this;

            //Call the replay manager function
            ReplayManager.SaveData(GameplayManager.Level, Index);
            return true;
        }

        /// <summary>
        /// Recursively delete all contents of a folder
        /// </summary>
        /// <param name="folderPath">Folder to delete</param>
        private void RecursiveDelete(string folderPath)
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
        /// Load this checkpoint's data
        /// </summary>
        /// <param name="player">Player that triggered this checkpoint</param>
        public void LoadCheckpoint(Player player)
        {
            int level;
            using (BinaryReader binaryReader = new BinaryReader(File.OpenRead($"{BaseFolder}/level.data")))
                level = binaryReader.ReadInt32();

            MapManager.LoadCheckpoint(BaseFolder, RoomIndex, level);
            player.Load(BaseFolder);
        }

    }
}
