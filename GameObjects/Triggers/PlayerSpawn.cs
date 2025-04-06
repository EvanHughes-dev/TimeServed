using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using Microsoft.Xna.Framework.Content;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    internal class PlayerSpawn : Trigger
    {
        public PlayerSpawn(Point location, Texture2D sprite)
            : base(location, sprite, 1, 1) { }

        /// <summary>
        /// The player spawn is only used to spawn the player, it can't be activated.
        /// Thus, this activate method does nothing!
        /// </summary>
        /// <param name="player"></param>
        public override void Activate(Player player) { }
    }
}
