using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.Managers;
using System.Linq;
using System.Diagnostics;

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

        //Specify a point for the center of it's vision and an angle in radians for the width of it's field of vision

        //The point at the center of the vision cone
        private Point _centerPoint;
        private float _spread;
        //The camera will get a certain set of endpoints to look towards when it's created, and those points will never change, even if they get blocked
        private Point[] _endPoints;
        //This float tells the camera which way it's facing. By default, the camera faces down
        private float _direction;

        //The rays don't project from inside of the wall, where the camera is technically drawn
        //All the rays come out from the point on the floor right "in front of" the camera
        private Point _rayBase;

        private Point[] _visionKite;
        private List<Point> _watchedTiles;
        private List<Point> _previousBoxes;
        //This is going to need a reference to the room that created it in order to check collision
        private Room _room;
        //It also needs a reference to the player to know if they step into the vision kite
        private Player _player = GameplayManager.PlayerObject;

        //TODO: Check in with Daniel to make sure these properties are ok to have
        public Point CenterPoint
        {
            get { return _centerPoint; }
        }

        public float Spread
        {
            get { return _spread; }
        }

        /// <summary>
        /// Makes a security camera that watches a certain vision kite to see if the player is inside it
        /// </summary>
        /// <param name="location">The location of the camera, from which the vision kite is projected</param>
        /// <param name="spriteArray">The array of the camera's sprites</param>
        /// <param name="spriteIndex">The index of this camera's sprite</param>
        /// <param name="containingRoom">The room containing the camera. Used to check if tiles are walkable</param>
        /// <param name="centerPoint">The point that forms the center of the camera's vision kite</param>
        /// <param name="spread">The arc from the center of the vision kite to the edge, in radians</param>
        public Camera(Point location, Texture2D[] spriteArray, int spriteIndex, Room containingRoom, Point centerPoint, float spread)
            : base(location, spriteArray, spriteIndex)
        {
            _watchedTiles = new List<Point>(); //This ends up being the same as visionkite by the end of this function
            _previousBoxes = new List<Point>();
            //All cams start active
            _active = true;
            _room = containingRoom;
            //_player = GameplayManager.PlayerObject;
            _centerPoint = centerPoint;
            _spread = spread;

            #region Raybase Check
            //Check which way the ray for the camera is pointing
            Vector2 centerRay = new Vector2(_centerPoint.X-location.X, _centerPoint.Y-location.Y); //base of this ray is at location

            //TODO: Let the raybase be caty-corners from the camera location
            //Please do not put the centerpoint on top of the camera it breaks everything
            if (Math.Abs(centerRay.X) > Math.Abs(centerRay.Y)) //The ray is more horizontal
            {
                if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y))) //cam is pointing left
                {
                    _rayBase = new Point(location.X - 1, location.Y);
                    _direction = 0.5f;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X + 1, location.Y)))//Cam is pointing right
                {
                    _rayBase = new Point(location.X + 1, location.Y);
                    _direction = 1.5f;
                }
                //If both the left and right tiles aren't walkable, try to set the raybase either up or down
                else
                {
                    if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(Location.X, Location.Y - 1)))
                    {
                        _rayBase = new Point(location.X, Location.Y - 1);
                        _direction = 1f;
                    }
                    else //This will be fine unless the camera is deep in the wall or cameraRay is zero vector
                    {
                        _rayBase = new Point(location.X, Location.Y + 1);
                        _direction = 0f;
                    }
                }
            }

            else //the ray is more vertical
            {
                if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(location.X, location.Y - 1))) //camera is pointing up
                {
                    _rayBase = new Point(Location.X, location.Y - 1);
                    _direction = 1f;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X, location.Y + 1))) //camera is pointing down
                {
                    _rayBase = new Point(Location.X, location.Y + 1);
                    _direction = 0f;
                }
                //If both the up and down tiles aren't walkable, try to set the raybase either left or right
                else
                {
                    if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y)))
                    {
                        _rayBase = new Point(Location.X - 1, location.Y);
                        _direction = 0.5f;
                    }
                    else //This will be fine unless the camera is deep in the wall or cameraRay is zero vector
                    {
                        _rayBase = new Point(Location.X + 1, Location.Y);
                        _direction = 1.5f;
                    }
                }
            }
            //I intentionally wrote the above to prioritize up/down over left/right
            #endregion
            //TODO: Allow the raybase to be catycorners from the camera's location
            //Debug.WriteLine($"Raybase is {_rayBase.X}, {_rayBase.Y}");

            //TESTING - show me the center ray
            //_watchedTiles.AddRange(Rasterize(_rayBase, _centerPoint));

            #region Vision Kite
            //---Find the endpoints for the corners of the kite---
            //Create a vector for the center from the raybase to the centerpoint
            centerRay = new Vector2(_centerPoint.X - _rayBase.X, _centerPoint.Y - location.Y); //Base of the ray is now at raybase
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
            
            //TESTING - Tell me all the endpoints for each half
            //Debug.Write("Clockwise points are: ");
            //foreach (Point debugPoint in clockwisePoints) Debug.Write(debugPoint + " ");
            //Debug.WriteLine(null);

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
                //TODO: This shit is NOT efficent. HEWLP
                //It's not that bad because this only runs when the cam is created, but it's O(n^2)
                Point yPoint = Point.Zero;
                foreach (Point endpoint in _endPoints)
                {
                    if (endpoint.Y == y)
                    {
                        yPoint = endpoint;
                    }
                }
                if (yPoint != Point.Zero)
                {
                    maxX = yPoint.X;
                }
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
            //Permanantely save this into an array that won't be changed and indicates the full kite with no obstructions
            _visionKite = _watchedTiles.ToArray();

            #endregion
            //Note that the length of _endPoints may be even or odd depending on rounding
        }

        //TODO: add an alternative constructor that creates an electrical box connected to this camera

        public void Update(float deltaTime)
        {
            if (_active)
            {
                #region Update the watched tiles and check for the player
                //Check for boxes before we look for the player
                List<Point> boxes = new List<Point>();
                //Used to check if there's a box on the raybase
                bool raybaseBlocked = false;
                //Check the entire vision kite for boxes in order to figure out what tiles are currently watched
                foreach (Point box in _visionKite)
                {
                    //Check if there's a box. Any un-walkable tile is treated like a box
                    if (!_room.VerifyWalkable(box))
                    {
                        //Debug.WriteLine($"Found a box at {box.X}, {box.Y}");
                        boxes.Add(box);
                        if (box == _rayBase) raybaseBlocked = true;
                    }
                }

                //If the raybase is blocked, we can't see anything and can't detect the player
                if (raybaseBlocked) _watchedTiles.Clear();
                else
                {
                    //Now check to see if any boxes have moved since the last frame
                    if (_previousBoxes.Count != boxes.Count || !boxes.All(_previousBoxes.Contains))
                    {
                        //We've found a new box in the way. Check all the rays fellas!
                        _watchedTiles = _visionKite.ToList();
                        foreach (Point box in boxes)
                        {
                            foreach (Point endpoint in _endPoints)
                            {
                                //Debug.WriteLine($"Checking {_rayBase} with {endpoint}");
                                //foreach (Point point in Rasterize(_rayBase, endpoint)) Debug.Write(point + " ");
                                //Debug.WriteLine(null);
                                if (Rasterize(_rayBase, endpoint).Contains(box)) //A ray is blocked by the box
                                {
                                    //Debug.WriteLine($"Endpoint {endpoint.X}, {endpoint.Y} is blocked");
                                    //Remove all the tiles between the box and the end of the ray
                                    foreach (Point blockedPoint in Rasterize(box, endpoint))
                                    {
                                        _watchedTiles.Remove(blockedPoint);
                                        Debug.WriteLine($"Removed: {blockedPoint}");
                                    }
                                }
                            }
                        }
                    }

                    //Now check for the player :)
                    foreach (Point watchedTile in _watchedTiles)
                    {
                        if (_player.Location == watchedTile)
                        {
                            //WE FOUND THE PLAYER! GET HIM BOYS!
                            _player.Detected();
                        }
                    }
                }
                #endregion

                _previousBoxes = boxes;
            }

        }

        public override void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - worldToScreen + pixelOffset, AssetManager.TileSize), null, //no source rectangle
                Color.White, _direction, Vector2.Zero, SpriteEffects.None, 0f); //Layer depth is not used
            foreach (Point tile in _watchedTiles)
            {
                sb.Draw(AssetManager.CameraSight, new Rectangle(MapUtils.TileToWorld(tile) - worldToScreen + pixelOffset, AssetManager.TileSize), Color.White);
            }
        }

        private List<Point> Rasterize(Point p1, Point p2)
        {
            #region Bresenham's Line Algorithm
            bool rotated = false;
            //If dy > dx, switch the X and Y of the endpoints
            if (Math.Abs(p2.Y-p1.Y) > Math.Abs(p2.X - p1.X))
            {
                //Debug.WriteLine("Swapped X and Y");
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

            //If the line is going down, we reflect it over the x-axis so that it goes up instead
            bool reflected = false;
            if (p2.Y - p1.Y < 0)
            {
                //Debug.WriteLine("Reflected over x-axis");
                //Indicated that we reflected this stuff for later
                reflected = true;
                //Reflect the line over it's x-center in order to fix this, and then flip it back?
                //So basically switch the Y of the start and end points
                int swapY = p1.Y;
                p1.Y = p2.Y;
                p2.Y = swapY;
            }

            //If the line is going from right to left, we switch the start and end point so it can be drawn left to right
            if (p1.X > p2.X)
            {
                //Debug.WriteLine("Switched start and end");
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

            //If we reflected everything, fix it before we rotate it back
            if (reflected)
            {
                //Debug.WriteLine("Un-reflecting");
                for (int i = 0; i < returnPoints.Count/2; i++)
                {
                    //Debug.WriteLine($"Swapped {returnPoints[i]} and {returnPoints[^(i + 1)]}");
                    int swapY = returnPoints[i].Y;
                    returnPoints[i] = new Point(returnPoints[i].X, returnPoints[^(i + 1)].Y);
                    returnPoints[^(i + 1)] = new Point(returnPoints[^(i + 1)].X, swapY);
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
            #endregion
            //Return the points intersected by the line
            return returnPoints;
        }

        public override void Interact(Player player)
        {
            throw new NotImplementedException("Interact has not been created yet in Camera");
        }
    }
}
