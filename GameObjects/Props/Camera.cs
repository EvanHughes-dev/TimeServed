using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Camera : Prop
    {
        //If the camera is on or off
        private bool _active;
        //The direction in which the camera's sprite is facing
        private enum CameraDirection
        {
            up,
            down,
            left,
            right
        }

        //Specify a line for the center of it's vision and the length of its vision, an angle in radians for the width of it's field of vision
        //All the lines share the same parametric point (the camera's location), so only the vectors for each line are different
        //The point at the center of the vision cone
        private Point _centerPoint;
        private float _spread;
        //The camera will get a certain set of rays when it's created, and those rays will never change, even if they get blocked
        //TODO: Maybe these should be changed into points?
        private Point[] _endPoints;

        //The rays don't project from inside of the wall, where the camera is technically drawn
        //All the rays come out from the point on the floor right "in front of" the camera
        private Point _rayBase;
        
        private List<Point> _watchedTiles;
        //This is going to need a reference to the room that created it in order to check collision
        private Room _room;
        //It also needs a reference to the player to know if they step into the vision kite
        private Player _player;

        /// <summary>
        /// Makes a security camera that watches a certain vision kite to see if the player is inside it
        /// </summary>
        /// <param name="location">The location of the camera, from which the vision kite is projected</param>
        /// <param name="sprite">The camera's sprite</param>
        /// <param name="containingRoom">The room containing the camera. Used to check if tiles are walkable</param>
        /// <param name="searchingPlayer">The player the camera is meant to search for</param>
        /// <param name="centerPoint">The point that forms the center of the camera's vision kite</param>
        /// <param name="spread">The arc from the center of the vision kite to the edge, in radians</param>
        public Camera(Point location, Texture2D sprite, Room containingRoom, Player searchingPlayer, Point centerPoint, float spread)
            : base(location, sprite)
        {
            _watchedTiles = new List<Point>();
            //All cams start active
            _active = true;
            _room = containingRoom;
            _player = searchingPlayer;
            _centerPoint = centerPoint;
            _spread = spread;
            //TODO: Write a check to figure out the raybase here
            //Check the tiles on either side of the camera

            //NOTE: for testing the camera is in the top wall, so the raybase is one down from the camera's location
            _rayBase = new Point(Location.X, Location.Y - 1);

            //-----Create the vision kite and figure out all the rays-----
            //---Find the endpoints for the corners of the kite---
            //Create a vector for the center from the raybase to the centerpoint

            //Rotate that vector by spread in both directions

            //Round the components of each vector

            //Add the vectors to the raybase to find the corner endpoints

            //Add those corners to the list of endpoints

            //Rasterize between the corners and the centerpoint to get all the points we need to send out a ray to

        }
        //TODO: add an alternative constructor that takes a point as the center of the vision cone and constructs a vector from that

        public void Update(float deltaTime)
        {
            //TimeSpan startTime = DateTime.Now.TimeOfDay;
            //Add a check to see if there's a box directly in front of the camera, and if so don't check any of the rays
            CheckRay(new Vector2(3, -4));
            //Debug.WriteLine(DateTime.Now.TimeOfDay - startTime);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - MapUtils.WorldToScreen() + MapUtils.PixelOffset(), new Point(128, 128)), Color.White);
            foreach (Point tile in _watchedTiles)
            {
                sb.Draw(AssetManager.PropTextures[3], new Rectangle(MapUtils.TileToWorld(tile) - MapUtils.WorldToScreen() + MapUtils.PixelOffset(), new Point(128, 128)), Color.White);
            }
        }

        private List<Point> Rasterize(Point p1, Point p2)
        {
            //If the line is going from right to left, we switch the start and end point so it can be drawn left to right
            if (p1.X > p2.X)
            {
                //Store this so that we don't forget a point
                Point swapPoint = p1;
                p1 = p2;
                p2 = swapPoint;
            }
            
            int dx = p2.X - p1.X;
            int dy = p2.Y - p1.Y;
            int p = (2 * dy) - dx;
            int x = p1.X;
            int y = p1.Y;

            //The array of points that will be returned. Length = change in the independent variable + 1
            List<Point> returnPoints = new List<Point>();

            for (; x <= p2.X; x++) //using x as loop variable
            {
                returnPoints.Add(new Point(x, y));
                if (p < 0)
                {
                    p = p + (2 * dy);
                }
                else //If p >= 0
                {
                    p = p + (2 * dy) - (2 * dx);
                    y = y + 1;
                }
            }
            //Return the list of points
            return returnPoints;
        }

        /**
        private void ExperimentalCheckRay(Vector2 ray)
        {

            //create a parametric line using the point and the vector
            //Step along that line in small intervals, checking what tile you're over at every step
            float dx = ray.X;
            float dy = ray.Y;
            //error is the slope error. We assume it starts exactly right
            float error = 0;
            int y = 0;

            //All this is old.Take this out before I make the pull request
            for (int x = 0; x <= ray.X; x++)
            {
                //Add the current point to the line
                //This first time this runs, this will always be the raybase
                _watchedTiles.Add(new Point(x + _rayBase.X, y + _rayBase.Y));
                //Debug.WriteLine("Added " + (x + _rayBase.X) + ", " + (y + _rayBase.Y));

                //Then check error for the next point along
                //Debug.WriteLine("Evaluated: " + ((_rayBase.Y * ray.X) + ((x) * ray.Y) - ((y + 0.5) * ray.X)));
                error = error + (ray.Y / ray.X) * (x - _rayBase.X) + _rayBase.Y  - y;

                //And figure out if it should go up or not
                //Debug.WriteLine("Error is: " + error);
                if (error >= 0.5)
                {
                    error = error - 1;
                    y = y + 1;
                }

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
        **/

        private List<Point> CheckRay(Vector2 ray)
        {
            //None of the rays check the base, because all of the rays come from the base
            //If the rays are a float, we're in the soup
            int dx = (int)ray.X;
            int dy = (int)ray.Y;
            //Create a local raybase for this particular vector so we can move it for this ray if necessarry
            //Without messing up the other rays
            Point rayBase = _rayBase;
            //This is the array of points this will return
            List<Point> intersectededPoints = new List<Point>();

            if (Math.Abs(dy) > Math.Abs(dx))
            {
                //X is the dependant variable
                int x = 0;

                //If the line is going from down to up, switch the start and end points so we can draw it from top to bottom
                if (dy < 0)
                {
                    rayBase.Y = rayBase.Y + dy;
                    rayBase.X = rayBase.X + dx;

                    dy = dy * -1;
                    dx = dx * -1;
                }

                //The length of the return array will be the change in the independent variable

                for (int y = rayBase.Y; y <= rayBase.Y + dy; y++)
                {
                    //get this, x = my + b
                    double xValue = (ray.X / ray.Y) * (y - rayBase.Y) + rayBase.X;
                    int xCoord;
                    xCoord = (int)Math.Round(xValue);
                    intersectededPoints.Add(new Point(xCoord, y));
                    Debug.WriteLine($"Added {xCoord}, {y}");
                }
            }
            else
            {
                //Y is the dependant variable
                int y = 0;

                //If the line is going from right to left, we switch the start and end point so it can be drawn left to right
                if (dx < 0)
                {
                    rayBase.X = rayBase.X + (int)dx;
                    rayBase.Y = rayBase.Y + (int)dy;

                    dx = dx * -1;
                    dy = dy * -1;
                    //_watchedTiles.Add(_rayBase);
                    //Debug.WriteLine($"Okay, raybase is at {rayBase.X}, {rayBase.Y} --- DX is {dx} and DY is {dy}");
                }

                for (int x = rayBase.X; x <= rayBase.X + dx; x++)
                {
                    //y = mx + b
                    double yValue = (ray.Y / ray.X) * (x - rayBase.X) + rayBase.Y;
                    int yCoord;
                    yCoord = (int)Math.Round(yValue);
                    intersectededPoints.Add(new Point(x, yCoord));
                    Debug.WriteLine($"Added {x}, {yCoord}");
                }
            }
            return intersectededPoints;
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
