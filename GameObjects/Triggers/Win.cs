using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeEveryDayRecount.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    /// <summary>
    /// Trigger that allows the player to win the current level if they step onto it with the correct item
    /// </summary>
    internal class Win : Trigger
    {
        /// <summary>
        /// Sprite index of the item required in the inventory to active this trigger
        /// </summary>
        public int ItemIndex { get; private set; }

        /// <summary>
        /// If true, the player has won the level.
        /// </summary>
        public bool Winning { get; private set; }

        /// <summary>
        /// Constructs a Win trigger
        /// </summary>
        /// <param name="location">Location of the top left-most tile</param>
        /// <param name="itemIndex">Sprite index of the item required to activate the trigger</param>
        /// <param name="width">Width, in tiles</param>
        /// <param name="height">Height, in tiles</param>
        public Win(Point location, int itemIndex, int width, int height) : 
            base(location, width, height)
        {
            ItemIndex = itemIndex;
        }

        /// <summary>
        /// Transitions the player to the cutscene state if they have the correct item
        /// </summary>
        /// <param name="player">The player object</param>
        public override void Activate(Player player)
        {
            if (player.ContainsItem(ItemIndex))
                Winning = true;
        }
    }
}
