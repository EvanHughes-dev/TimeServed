using System;
using System.Collections.Generic;
using System.Diagnostics;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Camera : Prop
    {
        //Specify a line for the center of it's vision and the length of its vision, an angle in radians for the width of it's field of vision
        private bool _active;

        //All the lines share the same parametric point (the camera's location), so only the vectors for each line are different
        //The ray at the center of the vision cone
        private Vector2 _centerRay;
        //The camera will get a certain set of rays when it's created, and those rays will never change, even if they get blocked
        //TODO: Maybe these should be changed into points?
        private Vector2[] _rays;

        private float _spread;

        private List<Point> _watchedTiles;

        /// <summary>
        /// Makes a security camera that watches a certain vision cone to see if the player is inside it
        /// </summary>
        /// <param name="location">The location of the camera, from which the vision cone is projected</param>
        /// <param name="sprite">The camera's sprite</param>
        /// <param name="centerRay">The ray that forms the center of the camera's vision cone</param>
        /// <param name="spread">The arc from the center of the vision cone to the edge, in radians</param>
        public Camera(Point location, Texture2D sprite, Vector2 centerRay, float spread)
            : base(location, sprite)
        {
            _watchedTiles = new List<Point>();
            //All cams start active
            _active = true;
            _centerRay = centerRay;
            _spread = spread;
        }
        //TODO: add an alternative constructor that takes a point as the center of the vision cone and constructs a vector from that

        public void Update(float deltaTime)
        {
            //Add a check to see if there's a box directly in front of the camera, and if so don't check any of the rays
            CheckRay(new Vector2(5, 4));
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - MapUtils.WorldToScreen() + MapUtils.PixelOffset(), new Point(128, 128)), Color.White);
            foreach (Point tile in _watchedTiles)
            {
                sb.Draw(AssetManager.PropTextures[3], new Rectangle(MapUtils.TileToWorld(tile) - MapUtils.WorldToScreen() + MapUtils.PixelOffset(), new Point(128, 128)), Color.White);
            }
        }

        private void CheckRay(Vector2 ray)
        {
            //create a parametric line using the point and the vector
            //Step along that line in small intervals, checking what tile you're over at every step
            float dx = ray.X;
            float dy = ray.Y;
            float error = 0;
            int y = Location.Y;

            for (int x = Location.X; x < Location.X + ray.X; x++)
            {
                _watchedTiles.Add(new Point(x, Location.Y + (int)(dy*((x-Location.X)/dx))));
                Debug.WriteLine("Added " + x + ", " + Location.Y + (int)(dy * (x / dx)));

                //if (error > 1)
                //{
                //    y = y + 1;
                //    error = error - 1;
                //}
                //if (error == error + 2 * dy)
                //{
                //    break;
                //}
            }
        }

        private void LookForPlayer()
        {
            throw new NotImplementedException();
            //This can be optimized by making it only check the tiles on the outside edges of the "vision cone"
            //Meaning the two unblocked rays on either side with the furthest angle from the center ray, and the most extreme point of each ray
        }

        public override void Interact(Player player)
        {
            throw new NotImplementedException("Interact has not been created yet in Camera");
        }
    }
}
