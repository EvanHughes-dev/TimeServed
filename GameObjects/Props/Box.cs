using System.Collections.Generic;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Net.Mail;

namespace MakeEveryDayRecount.GameObjects.Props
{
    /// <summary>
    /// A box that the player can push and pull around the map.
    /// Completely moveable by the player that can block the view
    /// of cameras and guards.
    /// </summary>
    internal class Box : Prop
    {

        /// <summary>
        /// Represents the box's attached position relative to the player. Direction.Up
        /// means the box is in the square that has the position of player.Location.Y-1 and so on
        /// </summary>
        public Direction AttachmentDirection
        {
            get { return _attachmentDirectionFinder[AttachmentPoint]; }
        }

        /// <summary>
        /// The direction of the location of the box relative to the player
        /// </summary>
        public Point AttachmentPoint { get; private set; }

        private Point _worldPos;

        private static readonly Dictionary<Point, Direction> _attachmentDirectionFinder =
            new Dictionary<Point, Direction>{
                { Point.Zero, Direction.None },
                { new Point(0, -1), Direction.Up },
                { new Point(1, 0), Direction.Right },
                { new Point(0, 1), Direction.Down },
                { new Point(-1, 0), Direction.Left }
            };

        /// <summary>
        /// Construct a new box with a location and sprite to display
        /// </summary>
        /// <param name="location">Location to display at</param>
        /// <param name="sprite">Sprite to display as</param>
        public Box(Point location, Texture2D sprite) : base(location, sprite)
        {
            AttachmentPoint = Point.Zero;
            _worldPos = MapUtils.TileToWorld(Location);
        }

        /// <summary>
        /// Update the position of the box
        /// </summary>
        /// <param name="newPosition">New position the box has been moved to</param>
        public void UpdatePosition(Point newPosition)
        {
            Location = newPosition + AttachmentPoint;
            _worldPos = MapUtils.TileToWorld(Location);
        }

        /// <summary>
        /// Update the box's position in the world without the grid
        /// </summary>
        /// <param name="drawpoint">Point to draw</param>
        public void UpdateDrawPoint(Point drawpoint)
        {
            _worldPos = drawpoint + new Point(AssetManager.TileSize.X * AttachmentPoint.X, AssetManager.TileSize.Y * AttachmentPoint.Y);
        }

        /// <summary>
        /// Have the box become "attached" to the player
        /// </summary>
        /// <param name="player">Player to attach to</param>
        public override void Interact(Player player)
        {
            AttachmentPoint = Location - player.Location;
            player.PickupBox(this);
        }

        /// <summary>
        /// This box has been dropped
        /// </summary>
        public void DropBox()
        {
            AttachmentPoint = Point.Zero;
        }

        public override void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {
            sb.Draw(Sprite, new Rectangle(_worldPos - worldToScreen + pixelOffset, AssetManager.TileSize), Color.White);

        }
    }
}