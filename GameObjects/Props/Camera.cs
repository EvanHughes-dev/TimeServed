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

        //The rays don't project from inside of the wall, where the camera is technically drawn
        //All the rays come out from the point on the floor right "in front of" the camera
        private Point _rayBase;

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
            //TODO: Write a check to figure out the raybase here
            //NOTE: for testing the camera is in the top wall, so the raybase is one down from the camera's location
            _rayBase = new Point(Location.X, Location.Y + 1);
        }
        //TODO: add an alternative constructor that takes a point as the center of the vision cone and constructs a vector from that

        public void Update(float deltaTime)
        {
            //Add a check to see if there's a box directly in front of the camera, and if so don't check any of the rays
            CheckRay(new Vector2(5, 2));
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - MapUtils.WorldToScreen() + MapUtils.PixelOffset(), new Point(128, 128)), Color.White);
            foreach (Point tile in _watchedTiles)
            {
                sb.Draw(AssetManager.PropTextures[3], new Rectangle(MapUtils.TileToWorld(tile) - MapUtils.WorldToScreen() + MapUtils.PixelOffset(), new Point(128, 128)), Color.White);
            }
        }

        private void CheckRayPoint(Point p2)
        {
            int dx = p2.X - _rayBase.X;
            int dy = p2.Y - _rayBase.Y;
            int p ;
            int x;
            int y;


        }



        private void CheckRay(Vector2 ray)
        {

            //create a parametric line using the point and the vector
            //Step along that line in small intervals, checking what tile you're over at every step
            float dx = ray.X;
            float dy = ray.Y;
            //This is a constant added to all the calculations. We find it at the beginning of the loop
            float c;
            //error is the slope error. We assume it starts exactly right
            float error = 0;
            int y = 0;

            
            //If the line is in an up or down octet
            if (Math.Abs(dy) > Math.Abs(dx))
            {
                throw new NotImplementedException("We can't handle up/down lines yet");
            }
            else
            {
                //If the line is going from right to left, we switch the start and end point so it can be drawn left to right
                if (dx < 0)
                {
                    _rayBase.X = _rayBase.X + (int)dx;
                    _rayBase.Y = _rayBase.Y + (int)dy;

                    dx = dx * -1;
                    dy = dy * -1;
                }
                //If Y is decreasing
                if (dy < 0)
                {
                    throw new NotImplementedException("We can't handle decreasing Y yet");
                }

                //Now that we're ready, we can calculate the constant that gets added to every equation
                c = (2 * dy) - dx;
                //pretend that X and Y both start at 0 while we're drawing the line
                //We add offsets later to correct this
                for (int x = 0; x <= dx; x++)
                {
                    //Add the current point to the line
                    _watchedTiles.Add(new Point(x + _rayBase.X, y + _rayBase.Y));
                    Debug.WriteLine("Added " + (x + _rayBase.X) + ", " + (y + _rayBase.Y));

                    Debug.WriteLine($"Calculated {2 * dy * (x )} - {2 * dx * y} + {c} = {(2 * dy * (x )) - (2 * dx * y) + c}");
                    //Now look at the next point along and see if it should go up or not
                    if ((2*dy*(x)) - (2*dx*y) + c < 0)
                    {
                        y += 1;
                    }
                }
            }

            //All this is old. Take this out before I make the pull request
            //for (int x = 0; x <= ray.X; x++)
            //{
            //    //Add the current point to the line
            //    //This first time this runs, this will always be the raybase
            //    _watchedTiles.Add(new Point(x + _rayBase.X, y + _rayBase.Y));
            //    Debug.WriteLine("Added " + (x + _rayBase.X) + ", " + (y + _rayBase.Y) );

            //    //Then check error for the next point along
            //    //Debug.WriteLine("Evaluated: " + ((_rayBase.Y * ray.X) + ((x) * ray.Y) - ((y + 0.5) * ray.X)));
            //    error = error + (y * ray.X) + ((x + 1) * ray.Y) - (y * ray.X);

            //    //And figure out if it should go up or not
            //    Debug.WriteLine("Error is: " + error);
            //    if (error >= 0.5)
            //    {
            //        error = error - 1;
            //        y = y + 1;
            //    }

            //    //if (error > 1)
            //    //{
            //    //    y = y + 1;
            //    //    error = error - 1;
            //    //}
            //    //if (error == error + 2 * dy)
            //    //{
            //    //    break;
            //    //}
            //}
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
