using System;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Item : Prop
    {
        public event ItemPickup OnItemPickup;

        public Item(Point location, Texture2D sprite)
            : base(location, sprite) { }

        /// <summary>
        /// Interaction with the item
        /// </summary>
        /// <param name="player">Player object interacting</param>
        public override void Interact(Player player)
        {
            OnItemPickup?.Invoke(this);
        }
    }
}
