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
        private Point _centerpoint;

        public WireBox(Point location, Texture2D sprite, Camera connectedCamera)
            : base (location, sprite)
        {
            _camera = connectedCamera;
            //Add a direction check (copy/paste from the camera) here
            //Don't forget to also calculate the centerpoint just like you do for the cameras
        }

        public override void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - worldToScreen + pixelOffset + _centerpoint, AssetManager.TileSize), null, //no source rectangle
                Color.White, _direction, _centerpoint.ToVector2(), SpriteEffects.None, 0f);
        }

        public override void Interact(Player player)
        {
            _camera.Deactivate();
            //TODO: This should make itself un-interactable after this function is called once
        }
    }
}
