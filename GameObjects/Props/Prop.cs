using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TimeServed.Players;
using TimeServed.Managers;
using TimeServed.Map;

namespace TimeServed.GameObjects.Props
{
    /// <summary>
    /// Prop is a GameObject that can be interacted with and has its own draw function
    /// </summary>
    internal abstract class Prop : GameObject
    {
        /// <summary>
        /// Create an interactable GameObject
        /// </summary>
        /// <param name="location">Location of prop</param>
        /// <param name="spriteArray">Array containing the prop's image</param>
        /// <param name="spriteIndex">Index of the prop's image in its array</param>
        protected Prop(Point location, Texture2D[] spriteArray, int spriteIndex)
            : base(location, spriteArray, spriteIndex) { }

        /// <summary>
        /// Allow the player to interact with objects
        /// </summary>
        /// <param name="player">Player interacting</param>
        public abstract void Interact(Player player);

        /// <summary>
        /// Allow props to draw themselves
        /// </summary>
        /// <param name="sb">Spritebatch to draw with</param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToScreen(Location), AssetManager.TileSize), Color.White);
        }
    }
}
