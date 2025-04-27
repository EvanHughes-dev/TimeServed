using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.GameObjects.Props
{
    /// <summary>
    /// Wire box that can deactivate a camera
    /// </summary>
    internal class WireBox : Prop
    {
        //Boxes need a direction just like the camera 
        private float _direction;
        private Camera _camera;
        private Point _drawOrigin;

        /// <summary>
        /// Create a wire box that is linked to a camera 
        /// </summary>
        /// <param name="location">Location of creation</param>
        /// <param name="sprite">Sprite array to get the sprite from</param>
        /// <param name="connectedCamera">Camera this is connected to</param>
        /// <param name="spriteIndex">Index for the sprite</param>
        public WireBox(Point location, Texture2D[] sprite, Camera connectedCamera, int spriteIndex)
            : base(location, sprite, spriteIndex)
        {

            switch (connectedCamera.CameraRoom.GetTileSpriteName(location).Split("_")[2])
            {
                case "right":
                    _direction = MathHelper.PiOver2;
                    break;
                case "bottom":
                    _direction = 0f;
                    break;
                case "left":
                    _direction = MathHelper.PiOver2 * 3;
                    break;
                case "top":
                    _direction = MathHelper.Pi;
                    break;
                default:
                    _direction = 0f;
                    System.Diagnostics.Debug.Write($"Unknown direction in wireboxes: {connectedCamera.CameraRoom.GetTileSpriteName(location).Split("_")[2]}");
                    break;

            }
            _camera = connectedCamera;
            _drawOrigin = new Point(Sprite.Width / 2, Sprite.Height / 2);
        }

        /// <summary>
        /// Draw this wire box
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        /// <param name="worldToScreen">Offset for the world position and screen position</param>
        /// <param name="pixelOffset">Number of pixels to offset for map offset</param>
        public override void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {

            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - worldToScreen + pixelOffset + AssetManager.HalfTileSize, AssetManager.TileSize), null, //no source rectangle
                Color.White, _direction, _drawOrigin.ToVector2(), SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Allow the player to interact with this item
        /// </summary>
        /// <param name="player">Player that interacted</param>
        public override void Interact(Player player)
        {
            if (_camera.SpriteIndex == 0 && player.SelectedItem.SpriteIndex == 2)
            {
                _camera.Deactivate();
                SpriteIndex = 3;
                Sprite = AssetManager.CameraTextures[SpriteIndex];
                SoundManager.PlaySFX(SoundManager.WirecutterSound, -40, 40);
            }
        }
    }
}
