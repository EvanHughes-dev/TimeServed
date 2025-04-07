using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Managers;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Camera : Prop
    {
        //If the camera is on or off
        private bool _active;
        //The direction in which the camera's sprite is facing
        private enum CameraDirections
        {
            Up,
            Down,
            Left,
            Right
        }

        //Specify a line for the center of it's vision and the length of its vision, an angle in radians for the width of it's field of vision
        //All the lines share the same parametric point (the camera's location), so only the vectors for each line are different
        //The point at the center of the vision cone
        private Point _centerPoint;
        private float _spread;
        //The camera will get a certain set of rays when it's created, and those rays will never change, even if they get blocked
        //TODO: Maybe these should be changed into points?
        private Point[] _endPoints;
        //To tell the cam which way to face
        private CameraDirections _direction;

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
        /// <param name="centerPoint">The point that forms the center of the camera's vision kite</param>
        /// <param name="spread">The arc from the center of the vision kite to the edge, in radians</param>
        public Camera(Point location, Texture2D sprite, Room containingRoom, Point centerPoint, float spread)
            : base(location, sprite)
        {
            _watchedTiles = new List<Point>();
            //All cams start active
            _active = true;
            _room = containingRoom;
            //_player = GameplayManager.PlayerObject;
            _centerPoint = centerPoint;
            _spread = spread;

            #region Raybase Check
            //Check which way the ray for the camera is pointing
            Vector2 centerRay = new Vector2(_centerPoint.X-location.X, _centerPoint.Y-location.Y); //base of this ray is at location

            //Please do not put the centerpoint on top of the camera it breaks everything
            if (Math.Abs(centerRay.X) > Math.Abs(centerRay.Y)) //The ray is more horizontal
            {
                if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y))) //cam is pointing left
                {
                    _rayBase = new Point(location.X - 1, location.Y);
                    _direction = CameraDirections.Left;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X + 1, location.Y)))//Cam is pointing right
                {
                    _rayBase = new Point(location.X + 1, location.Y);
                    _direction = CameraDirections.Right;
                }
                //If both the left and right tiles aren't walkable, try to set the raybase either up or down
                else
                {
                    if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(Location.X, Location.Y - 1)))
                    {
                        _rayBase = new Point(location.X, Location.Y - 1);
                        _direction = CameraDirections.Up;
                    }
                    else //This will be fine unless the camera is deep in the wall or cameraRay is zero vector
                    {
                        _rayBase = new Point(location.X, Location.Y + 1);
                        _direction = CameraDirections.Down;
                    }
                }
            }

            else //the ray is more vertical
            {
                if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(location.X, location.Y - 1))) //camera is pointing up
                {
                    _rayBase = new Point(Location.X, location.Y - 1);
                    _direction = CameraDirections.Up;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X, location.Y + 1))) //camera is pointing down
                {
                    _rayBase = new Point(Location.X, location.Y + 1);
                    _direction = CameraDirections.Down;
                }
                //If both the up and down tiles aren't walkable, try to set the raybase either left or right
                else
                {
                    if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y)))
                    {
                        _rayBase = new Point(Location.X - 1, location.Y);
                        _direction = CameraDirections.Left;
                    }
                    else //This will be fine unless the camera is deep in the wall or cameraRay is zero vector
                    {
                        _rayBase = new Point(Location.X + 1, Location.Y);
                        _direction = CameraDirections.Right;
                    }
                }
            }
            //I intentionally wrote the above to prioritize up/down over left/right
            //I don't think we need a special case for if camera ray X and Y are equal because that can only have an effect if the camera is on an outside corner 
            #endregion
            //Debug.WriteLine($"Raybase is {_rayBase.X}, {_rayBase.Y}");

            //TESTING - show me the center ray
            //_watchedTiles.AddRange(Rasterize(_rayBase, _centerPoint));

            #region Vision Kite
            //---Find the endpoints for the corners of the kite---
            //Create a vector for the center from the raybase to the centerpoint
            centerRay = new Vector2(_centerPoint.X - _rayBase.X, _centerPoint.Y - location.Y); //Base of the ray is now at raybase
            //TODO: The edge rays are not being calculated correctly
            //Rotate that vector by spread in both directions
            Vector2 clockwiseRay = Vector2.Transform(centerRay, Matrix.CreateRotationZ(spread));
            Vector2 counterclockwiseRay = Vector2.Transform(centerRay, Matrix.CreateRotationZ(-spread));
            //^These built-in methods are *chef kiss*

            //TESTING - tell me the rays
            //Debug.WriteLine("Center ray is " + centerRay.ToString());
            //Debug.WriteLine("Counter-clockwise ray is: " + counterclockwiseRay.ToString());
            //Debug.WriteLine("Clockwise ray is : " + clockwiseRay.ToString());

            //Turn these rotated vectors back into points to get the corners of the kite
            Point clockwisePoint = new Point((int)MathF.Round(_rayBase.X + clockwiseRay.X), (int)MathF.Round(_rayBase.Y + clockwiseRay.Y));
            Point counterclockwisePoint = new Point((int)MathF.Round(_rayBase.X + counterclockwiseRay.X), (int)MathF.Round(_rayBase.Y + counterclockwiseRay.Y));
            //Gang why does mathF still return a float when you round to the nearest integer. This is highly unserious

            //TESTING - tell me where the corners are
            //Debug.WriteLine("Center point is " + _centerPoint.ToString());
            //Debug.WriteLine("Counter-clockwise point is: " + counterclockwisePoint.ToString());
            //Debug.WriteLine("Clockwise point is: " + clockwisePoint.ToString());

            //Rasterize between the corners and the centerpoint to get all the points we need to send out a ray to
            List<Point> clockwisePoints = Rasterize(clockwisePoint, _centerPoint);
            List<Point> counterclockwisePoints = Rasterize(_centerPoint, counterclockwisePoint);
            //TODO: Maybe sort the lists so that the _endpoints array ends up in a centain order?
            //The endpoints array starts from the counterclockwise corner, goes to the center, and ends at the clockwise corner
            //Basically sweeping from left to right, from the camera's point of view
            //Although maybe if the camera "sweeps" from the center outwards, it would somehow help us avoid duplicates more?
            //Because rays in the center are more likely to have tiles that overlap with the rays at the edges. The closer to the center you are the more overlap there is.
            //Arbitrarily remove the centerpoint from the clockwise list so we don't add it to endpoints twice
            //We can't use RemoveAt for this because the centerpoint could be at either end of this list
            counterclockwisePoints.Remove(_centerPoint);
            //Now combine the two lists into one and turn that into an array
            List<Point> endPoints = clockwisePoints;
            endPoints.AddRange(counterclockwisePoints);
            _endPoints = endPoints.ToArray();

            //Create a rectangle that bounds the entire kite
            //TODO: Could finding the corners be done more efficiently?
            Point[] corners = {_rayBase, _centerPoint, clockwisePoint, counterclockwisePoint};
            //Find the minimum/maximum X and Y of the 4 bounding points (the edges of the rectangle basically)
            int minX = _rayBase.X;
            int maxX = _rayBase.X;
            int minY = _rayBase.Y;
            int maxY = _rayBase.Y;
            foreach (Point corner in corners)
            {
                if (corner.X < minX) minX = corner.X;
                if (corner.X > maxX) maxX = corner.X;
                if (corner.Y < minY) minY = corner.Y;
                if (corner.Y > maxY) maxY = corner.Y;
            }
            //Nested for loops to go from left to right and then top to bottom
            for (int y = minY; y <= maxY; y++)
            {
                //If there's an endpoint in this column, and y is further from the camera than that endpoint is
                //Continue
                for (int x = minX; x <= maxX; x++)
                {
                    //If there's an endpoint in this row, and x is further from the camera than that endpoint is
                    //Then break

                    //For each point in that rectangle, create a vector from the raybase to it
                    Vector2 candidateVector = new Vector2(x - _rayBase.X, y - _rayBase.Y);
                    //Figure out if that vector is between the two edge vectors. If it is, then it should be inside of the vision kite
                    //I got the formula for this from StackOverflow (Andy G)
                    //Where A and C are the edge vectors, and B is the candidate vector, and the three vectors are pointing out from the same point
                    //if (AxB * AxC >= 0 && CxB * CxA >= 0) then B is between A and C
                    if ((counterclockwiseRay.Y * candidateVector.X - counterclockwiseRay.X * candidateVector.Y) *
                        (counterclockwiseRay.Y * clockwiseRay.X - counterclockwiseRay.X * clockwiseRay.Y) >= 0
                        &&
                        (clockwiseRay.Y * candidateVector.X - clockwiseRay.X * candidateVector.Y) *
                        (clockwiseRay.Y * counterclockwiseRay.X - clockwiseRay.X * counterclockwiseRay.Y) >= 0)
                    {
                        _watchedTiles.Add(new Point(x, y));
                    }
                }
            }

            #endregion
            //Note that the length of _endPoints may be even or odd depending on rounding

        }
        //TODO: add an alternative constructor that takes a point as the center of the vision cone and constructs a vector from that

        public void Update(float deltaTime)
        {
            //TimeSpan startTime = DateTime.Now.TimeOfDay;
            //TODO: Add a check to see if there's a box directly in front of the camera, and if so don't check any of the rays
            //TODO: Only check rays if we saw a box in the watchedtiles during the previous frame??

            
            //Debug.WriteLine(DateTime.Now.TimeOfDay - startTime);
        }

        public override void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - worldToScreen + pixelOffset, AssetManager.TileSize), Color.White);
            //TODO: Draw the camera differently depending on its _direction
            foreach (Point tile in _watchedTiles)
            {
                sb.Draw(AssetManager.CameraSight, new Rectangle(MapUtils.TileToWorld(tile) - worldToScreen + pixelOffset, AssetManager.TileSize), Color.White);
            }
        }

        private List<Point> Rasterize(Point p1, Point p2)
        {
            bool rotated = false;
            //If dy > dx, switch the X and Y of the endpoints
            if (Math.Abs(p2.Y-p1.Y) > Math.Abs(p2.X - p1.X))
            {
                //Indicate that we changed this stuff for later
                rotated = true;
                //Swap the X and Y of p1
                int swapX = p1.X;
                p1.X = p1.Y;
                p1.Y = swapX;
                //Then do the same to p2
                swapX = p2.X;
                p2.X = p2.Y;
                p2.Y = swapX;
            }

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
                //Debug.WriteLine($"Added point {x}, {y}");
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

            //If we flipped the X and Y of everything, flip it back here
            if (rotated)
            {
                for (int i = 0; i < returnPoints.Count; i++)
                {
                    returnPoints[i] = new Point(returnPoints[i].Y, returnPoints[i].X);
                }
            }

            //Return the list of points
            return returnPoints;
        }

        //TODO: Create a version of Rasterize that always returns from the raybase first
        //I think this will allow us to more easily check if a box is blocking a ray

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
