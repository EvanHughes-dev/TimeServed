using System.Collections.Generic;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        private Point _sizeOffset;

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
        /// <param name="spriteArray">Array of sprites containing this GameObject's sprite</param>
        /// <param name="spriteIndex">Index of this GameObject's sprite in its spriteArray</param>
        public Box(Point location, Texture2D[] spriteArray, int spriteIndex) : base(location, spriteArray, spriteIndex)
        {
            AttachmentPoint = Point.Zero;
            _worldPos = MapUtils.TileToWorld(Location);
            _sizeOffset = Point.Zero;
        }

        /// <summary>
        /// Update the position of the box
        /// </summary>
        /// <param name="newPosition">New position the box has been moved to</param>
        public void UpdatePosition(Point newPosition)
        {
            if (newPosition + AttachmentPoint == Location)
                return;
            MapManager.CurrentRoom.GetTile(newPosition + AttachmentPoint).BoxMoved(this);
            MapManager.CurrentRoom.GetTile(Location).BoxMoved(this);

            Location = newPosition + AttachmentPoint;
            _worldPos = MapUtils.TileToWorld(Location);
        }

        /// <summary>
        /// Have the box become "attached" to the player
        /// </summary>
        /// <param name="player">Player to attach to</param>
        public override void Interact(Player player)
        {
            AttachmentPoint = Location - player.Location;
            player.PickupBox(this);
            _sizeOffset = new Point(AssetManager.TileSize.X / 4, AssetManager.TileSize.Y / 4);
        }

        /// <summary>
        /// This box has been dropped. Remove attachment point and reset screen location to a point
        /// </summary>
        public void DropBox()
        {
            AttachmentPoint = Point.Zero;
            _sizeOffset = Point.Zero;
        }

        /// <summary>
        /// Draw this box to the screen
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToScreen(Location) +
            new Point(_sizeOffset.X / 2, _sizeOffset.Y / 2), AssetManager.TileSize - _sizeOffset), Color.White);
        }
    }
}