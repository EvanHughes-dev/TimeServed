using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.DebugModes
{
    /// <summary>
    /// Debug class for the player, displaying relevant position data,
    /// walkable tiles, and intractable elements in the world.
    /// </summary>
    internal class PlayerDebug : BaseDebug
    {
        private readonly Texture2D _walkableTileDebug;
        private readonly Texture2D _notWalkableTileDebug;
        private readonly Player _player;

        private readonly Point[] _playerMovementDirections;

        /// <summary>
        /// Initializes the player debug system.
        /// This must be called before drawing debug information.
        /// </summary>
        public PlayerDebug()
            : base()
        {
            _player = GameplayManager.PlayerObject;
            AddPlayerDebugInfo();
            _walkableTileDebug = AssetManager.DebugWalkableTile;
            _notWalkableTileDebug = AssetManager.DebugNotWalkableTile;
            _playerMovementDirections = new Point[]
            {
                new Point(-1, 0),
                new Point(1, 0),
                new Point(0, -1),
                new Point(0, 1)
            };
        }

        /// <summary>
        /// Adds relevant player data to the debug display.
        /// </summary>
        private void AddPlayerDebugInfo()
        {
            _objectsToDisplay.Add("Tile Position", () => _player.Location);
            _objectsToDisplay.Add("Screen Position", () => _player.PlayerScreenPosition);
            _objectsToDisplay.Add("World Position", () => MapUtils.TileToWorld(_player.Location));
            _objectsToDisplay.Add("State", () => _player.CurrentPlayerState);
            _objectsToDisplay.Add("Direction", () => _player.PlayerCurrentDirection);
        }

        /// <summary>
        /// Draws debug tiles and player-specific debug information.
        /// </summary>
        /// <param name="sb">Sprite batch used for rendering.</param>
        public override void Draw(SpriteBatch sb)
        {
            DrawTileDebug(sb);
            Draw(sb, "Player");
        }

        /// <summary>
        /// Displays debug tiles indicating whether the player can move to adjacent positions.
        /// </summary>
        /// <param name="sb">Sprite batch used for rendering.</param>
        private void DrawTileDebug(SpriteBatch sb)
        {
            var map = GameplayManager.Map;
            var playerTilePos = _player.Location;

            foreach (var direction in _playerMovementDirections)
            {
                var playerDest = playerTilePos + direction;
                var displayTile = MapManager.CheckPlayerCollision(playerDest)
                    ? _walkableTileDebug
                    : _notWalkableTileDebug;

                var screenPos = MapUtils.TileToWorld(playerDest) - MapUtils.WorldToScreen();
                sb.Draw(displayTile, new Rectangle(screenPos, AssetManager.TileSize), Color.White);
            }
        }
    }
}
