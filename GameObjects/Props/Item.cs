using System;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Item : Prop
    {
        public event ItemPickup ItemPickedUp;

        public Item(Point location, Texture2D sprite)
            : base(location, sprite) { }

        /// <summary>
        /// Interaction with the item
        /// </summary>
        /// <param name="player">Player object interacting</param>
        public override void Interact(Player player)
        {
            ItemPickedUp(this);
        }
    }
}
