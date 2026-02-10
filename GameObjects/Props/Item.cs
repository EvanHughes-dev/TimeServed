using TimeServed.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeServed.Players;

namespace TimeServed.GameObjects.Props
{

    /// <summary>
    /// An item the player can pick up and add to their inventory
    /// </summary>
    internal class Item : Prop
    {
        public event ItemPickup OnItemPickup;

        /// <summary>
        /// The key type for this item. Tells it what doors it can open
        /// </summary>
        public Door.DoorKeyType ItemKeyType { get; private set; }

        /// <summary>
        /// Create an item object
        /// </summary>
        /// <param name="location">Location in its room</param>
        /// <param name="spriteArray">Array of sprites containing this GameObject's sprite</param>
        /// <param name="spriteIndex">Index of this GameObject's sprite in its spriteArray</param>
        /// <param name="keyType">Type of Door that this item unlocks (type None if it doesn't unlock a door)</param>
        public Item(Point location, Texture2D[] spriteArray, int spriteIndex, Door.DoorKeyType keyType)
            : base(location, spriteArray, spriteIndex)
        {
            ItemKeyType = keyType;
        }

        /// <summary>
        /// When the player interacts with the item, they pick it up and it's removed from the room
        /// </summary>
        /// <param name="player">Player object interacting</param>
        public override void Interact(Player player)
        {
            //This tells the map manager to remove the item from the room
            OnItemPickup?.Invoke(this);
            //Since it can't return itself because then it wouldn't be an override of GameObject.Interact, this instead calls a method in the player which adds it to the player's inventory
            player.PickUpItem(this);
        }
    }
}
