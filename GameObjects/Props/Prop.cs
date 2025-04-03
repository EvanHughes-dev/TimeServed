using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.Map;

namespace MakeEveryDayRecount.GameObjects.Props
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
        /// <param name="sprite">Image of the prop</param>
        protected Prop(Point location, Texture2D sprite)
    : base(location, sprite) { }

        /// <summary>
        /// Allow the player to interact with objects
        /// </summary>
        /// <param name="player">Player interacting</param>
        public abstract void Interact(Player player);
        /// <summary>
        /// Allow props to draw themselves
        /// </summary>
        /// <param name="sb">Spritebatch to draw with</param>
        /// <param name="worldToScreen">Pixel offset between the world position and screen position</param>
        /// <param name="pixelOffset">Pixel offset between the map in the world and on the screen</param>
        public virtual void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - worldToScreen + pixelOffset, AssetManager.TileSize), Color.White);

        }
    }
}
