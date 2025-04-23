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
        /// <returns>True if successfully activated, false otherwise</returns>
        public override bool Activate(Player player)
        {
            return player.ContainsItem(ItemIndex);
        }

        //JTODO: Can't open the content file so I'm leaving this as a note to self
        //In order to test out the levels I want to stick them in the a folder in levels called "Level2"
        //I can make them with the level editor, but I think they should already be in the content file
        //Wish I could open it up to look around but c'est la vie
    }
}
