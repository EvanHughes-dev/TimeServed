using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class WireBox : Prop
    {
        //Boxes need a direction just like the camera 
        private float _direction;
        private Camera _camera;
        private Point _drawOrigin;

        public WireBox(Point location, Texture2D[] sprite, float direction, Camera connectedCamera, int spriteIndex)
            : base(location, sprite, spriteIndex)
        {
            _camera = connectedCamera;
            _direction = direction;
            _drawOrigin = new Point(AssetManager.TileSize.X / 2, AssetManager.TileSize.Y / 2);
        }

        public override void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - worldToScreen + pixelOffset + _drawOrigin, AssetManager.TileSize), null, //no source rectangle
                Color.White, _direction, _drawOrigin.ToVector2(), SpriteEffects.None, 0f);
        }

        public override void Interact(Player player)
        {
            _camera.Deactivate();
            //TODO: This should make itself un-interactable after this function is called once
        }
    }
}
