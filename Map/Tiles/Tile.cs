using TimeServed.GameObjects.Props;
using TimeServed.GameObjects.Triggers;
using TimeServed.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TimeServed.Map.Tiles
{
    /// <summary>
    /// Called when a box changes state in this tile
    /// </summary>
    delegate void BoxChange();

    /// <summary>
    /// Called when the player enters a tile
    /// </summary>
    /// <param name="location">Location the player entered</param>
    delegate void PlayerEnter(Point location);

    /// <summary>
    /// Hold data about whether a tile can be stood on
    /// and what sprite it should be
    /// </summary>
    internal class Tile
    {
        /// <summary>
        /// Called when a box enters or leaves this tile
        /// </summary>
        public event BoxChange BoxChange;

        /// <summary>
        /// Called when the player has entered a tile
        /// </summary>
        public event PlayerEnter PlayerEnteredTile;

        /// <summary>
        /// Get any prop held by this tile
        /// </summary>
        public Prop PropHeld { get; set; }

        /// <summary>
        /// Get any trigger held by this tile
        /// </summary>
        public Trigger TriggerHeld { get; set; }

        /// <summary>
        /// Get if the tile is walkable
        /// </summary>
        public bool IsWalkable { get; private set; }

        /// <summary>
        /// Get if a camera can look past this. 
        /// </summary>
        public bool IsBlockingCamera { get =>!IsWalkable || PropHeld != null && PropHeld is Box; }

        /// <summary>
        /// Get then index that corresponds to this tile's sprite
        /// </summary>
        public int SpriteIndex { get; private set; }

        /// <summary>
        /// Get the location of this tile in world space
        /// </summary>
        public Point Location { get=>_location; }
        private Point _location;

        private int _watcherCount;

        /// <summary>
        /// Create an instance of a tile with the needed data
        /// </summary>
        /// <param name="isWalkable">If the tile can be walked on</param>
        /// <param name="spriteIndex">The sprite index of the tile</param>
        public Tile(bool isWalkable, int spriteIndex, Point location)
        {
            IsWalkable = isWalkable;
            SpriteIndex = spriteIndex;
            _location = location;
            PropHeld = null;
            TriggerHeld = null;
            _watcherCount = 0;
        }

        /// <summary>
        /// Draw this tile and any prop that may be in it
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public void Draw(SpriteBatch sb)
        {
            // TODO there is a good bit of optimization I can do with screenPos.
            // Rather than calculate the screen pos for every tile, only calculate it 
            // for the first one on the screen (top left) then pass it into each following
            // tile to draw. This way, we only do the really time expensive operation once.
            // Could also pass that value into the prop.
            sb.Draw(AssetManager.TileMap[SpriteIndex],
             new Rectangle(MapUtils.TileToScreen(_location), AssetManager.TileSize), Color.White);

            PropHeld?.Draw(sb);

            if (_watcherCount > 0)
                sb.Draw(AssetManager.CameraSight, new Rectangle(MapUtils.TileToScreen(_location), AssetManager.TileSize), Color.White);

        }

        /// <summary>
        /// Draw this tile in debug mode. Display if it is walkable, if something can be interacted with, and if 
        /// there is a trigger on the tile. Display a different debug image for a checkpoint vs a win trigger.
        /// Also, if it is a checkpoint, display the index for the checkpoint.
        /// </summary>
        /// <param name="sb">Sprite batch to draw the tile with</param>
        public void DebugDraw(SpriteBatch sb)
        {
            // TODO To whomever reviews this pr or to my future self when I'm looking back through this code, remember to remove this block comment

            /*
            * This section of code aims to draw this instance of a tile in debug mode, but we have some options for how we draw this.
            */
        }

        /// <summary>
        /// Add a watcher camera to this tile
        /// </summary>
        public void AddWatcher()
        {
            _watcherCount++;
        }

        /// <summary>
        /// Remove a watcher camera to this tile
        /// </summary>
        public void RemoveWatcher()
        {
            _watcherCount--;
        }

        /// <summary>
        /// A box is either entering or leaving this tile. Either way,
        /// evoke the event to let the camera know
        /// </summary>
        public void BoxMoved(Box box)
        {
            // TODO fix an optimization problem with this. If i move a box inside a vision cone, 
            // it'll call invoke this twice for the same camera
            if (box == PropHeld)
                PropHeld = null;
            else
                PropHeld = box;

            BoxChange?.Invoke();
            if (GameplayManager.PlayerObject.Location == _location)
                PlayerEnter();

        }

        /// <summary>
        /// Called when the player enters this tile
        /// </summary>
        public void PlayerEnter()
        {
            if (_watcherCount > 0)
                PlayerEnteredTile?.Invoke(_location);
        }


        #region Operators

        public static implicit operator Point(Tile t) => t.Location;
        public static implicit operator Vector2(Tile t) => new Vector2(t.Location.X, t.Location.Y);

        #endregion
    }
}
