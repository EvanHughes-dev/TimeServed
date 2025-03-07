using System;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects.Props
{
    /// <summary>
    /// An item the player cabn pick up and add to their inventory
    /// </summary>
    internal class Item : Prop
    {
        public event ItemPickup OnItemPickup;

        /// <summary>
        /// Get-only. The name of the item
        /// </summary>
        public string ItemName { get; private set; }
        /// <summary>
        /// The key type for this item. Tells it what doors it can open
        /// </summary>
        public Door.DoorKeyType ItemKeyType { get; private set; }

        public Item(Point location, Texture2D sprite, string name, Door.DoorKeyType keyType)
            : base(location, sprite)
        {
            ItemName = name;
            ItemKeyType = keyType;
        }

        /// <summary>
        /// When the player interacts with the item, they pick it up and it's removed from the room
        /// </summary>
        /// <param name="player">Player object interacting</param>
        public override void Interact(Player player)
        {
            OnItemPickup(this);
            //Since it can't return itself because then it wouldn't be an override of GameObject.Interact, this instead calls a method in the player which adds it to the player's inventory
            player.PickUpItem(this);
        }
    }
}
