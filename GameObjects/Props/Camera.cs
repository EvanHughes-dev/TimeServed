using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.Managers;
using System.Linq;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Camera : Prop
    {
        //If the camera is on or off
        private bool _active;
        //The direction in which the camera's sprite is facing
        //This isn't currently used but if we add more features it migth come in handy so I'm keeping it
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
        readonly Point _drawOrigin;

        //The rays don't project from inside of the wall, where the camera is technically drawn
        //All the rays come out from the point on the floor right "in front of" the camera
        private Point _rayBase;


        private Point[] _visionKite;
        private List<Point> _watchedTiles;
        private List<Point> _previousBoxes;
        //This is going to need a reference to the room that created it in order to check collision
        /// <summary>
        /// Get the room this camera is in
        /// </summary>
        public Room CameraRoom { get; private set; }
        //It also needs a reference to the player to know if they step into the vision kite
        private Player _player = GameplayManager.PlayerObject;

        /// <summary>
        /// The wire box this is connected to
        /// </summary>
        public WireBox WireBox { get; private set; }

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
            _active = SpriteIndex == 0;
            CameraRoom = containingRoom;
            _drawOrigin = new Point(AssetManager.TileSize.X / 2, AssetManager.TileSize.Y / 2);
            _centerPoint = centerPoint;
            _spread = spread;

            // If the sprite inedx is 1 then it is an inactive camera
            if (!_active)
                return;
            #region Raybase Check
            //Check which way the ray for the camera is pointing
            Vector2 centerRay = new Vector2(_centerPoint.X - location.X, _centerPoint.Y - location.Y); //base of this ray is at location


            //Please do not put the centerpoint on top of the camera it breaks everything
            if (Math.Abs(centerRay.X) > Math.Abs(centerRay.Y)) //The ray is more horizontal
            {
                if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y), false)) //cam is pointing left
                {
                    _rayBase = new Point(location.X - 1, location.Y);
                    _direction = 0.5f;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X + 1, location.Y), false))//Cam is pointing right
                {
                    _rayBase = new Point(location.X + 1, location.Y);
                    _direction = 1.5f;
                }
                //If both the left and right tiles aren't walkable, try to set the raybase either up or down
                else
                {
                    if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(Location.X, Location.Y - 1), false))
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
                if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(location.X, location.Y - 1), false)) //camera is pointing up
                {
                    _rayBase = new Point(Location.X, location.Y - 1);
                    _direction = 1f;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X, location.Y + 1), false)) //camera is pointing down
                {
                    _rayBase = new Point(Location.X, location.Y + 1);
                    _direction = 0f;
                }
                //If both the up and down tiles aren't walkable, try to set the raybase either left or right
                else
                {
                    if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y), false))
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
            //The rotation has to be in radians, so we multiply the direction by pi after we determine it
            _direction = _direction * MathF.PI;
            //I intentionally wrote the above to prioritize up/down over left/right
            #endregion

            #region Vision Kite
            //---Find the endpoints for the corners of the kite---
            //Create a vector for the center from the raybase to the centerpoint
            centerRay = new Vector2(_centerPoint.X - _rayBase.X, _centerPoint.Y - location.Y); //Base of the ray is now at raybase
            //Rotate that vector by spread in both directions
            Vector2 clockwiseRay = Vector2.Transform(centerRay, Matrix.CreateRotationZ(spread));
            Vector2 counterclockwiseRay = Vector2.Transform(centerRay, Matrix.CreateRotationZ(-spread));
            //^These built-in methods are *chef kiss*

            //Turn these rotated vectors back into points to get the corners of the kite
            Point clockwisePoint = new Point((int)MathF.Round(_rayBase.X + clockwiseRay.X), (int)MathF.Round(_rayBase.Y + clockwiseRay.Y));
            Point counterclockwisePoint = new Point((int)MathF.Round(_rayBase.X + counterclockwiseRay.X), (int)MathF.Round(_rayBase.Y + counterclockwiseRay.Y));
            //Gang why does mathF still return a float when you round to the nearest integer. This is highly unserious

            //Rasterize between the corners and the centerpoint to get all the points we need to send out a ray to
            List<Point> clockwisePoints = Rasterize(clockwisePoint, _centerPoint);
            List<Point> counterclockwisePoints = Rasterize(_centerPoint, counterclockwisePoint);

            List<Point> endPoints = new List<Point>();
            //Sort the two lists of endpoints so that it goes from the counter-clockwise corner to the center to the clockwise corner
            if (counterclockwisePoints[0] == counterclockwisePoint)
            {
                //Then just add all the points in order
                for (int i = 0; i < counterclockwisePoints.Count; i++)
                {
                    endPoints.Add(counterclockwisePoints[i]);
                }
            }
            else
            {
                //Otherwise they must be in backwards order so we need to add them backwards
                for (int i = counterclockwisePoints.Count - 1; i >= 0; i--)
                {
                    endPoints.Add(counterclockwisePoints[i]);
                }
            }
            //Remove centerpoint because both the lists contain the centerpoint and we don't want it in there twice
            //Centerpoint should currently be the last item in the list
            endPoints.RemoveAt(endPoints.Count - 1);
            //Now do all the stuff we did above for the clockwise points
            if (clockwisePoints[0] == clockwisePoint)
            {
                for (int i = clockwisePoints.Count - 1; i >= 0; i--) endPoints.Add(clockwisePoints[i]);
            }
            else
            {
                for (int i = 0; i < clockwisePoints.Count; i++) endPoints.Add(clockwisePoints[i]);
            }
            //Now save this into the field of the class
            _endPoints = endPoints.ToArray();

            //Create a rectangle that bounds the entire kite
            //TODO: Could finding the corners be done more efficiently?
            Point[] corners = { _rayBase, _centerPoint, clockwisePoint, counterclockwisePoint };
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

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {

                    //For each point in that rectangle, create a vector from the raybase to it
                    Vector2 candidateVector = new Vector2(x - _rayBase.X, y - _rayBase.Y);
                    //Figure out if that vector is between the two edge vectors. If it is, then it should be inside of the vision kite
                    //NOTE: Chris suggested using the dot product for this but I think cross product is better because it doesn't require trigonometry
                    //But maybe if we normalized the vectors and then did dot product? I won't worry about it for now
                    //I got the formula for this from StackOverflow (Andy G)
                    //Where A and C are the edge vectors, and B is the candidate vector, and the three vectors are pointing out from the same point
                    //if (AxB * AxC >= 0 && CxB * CxA >= 0) then B is between A and C
                    if ((counterclockwiseRay.Y * candidateVector.X - counterclockwiseRay.X * candidateVector.Y) *
                        (counterclockwiseRay.Y * clockwiseRay.X - counterclockwiseRay.X * clockwiseRay.Y) >= 0
                        &&
                        (clockwiseRay.Y * candidateVector.X - clockwiseRay.X * candidateVector.Y) *
                        (clockwiseRay.Y * counterclockwiseRay.X - clockwiseRay.X * counterclockwiseRay.Y) >= 0
                        && candidateVector.Length() <= centerRay.Length()) //TODO: Is there a better way to keep it from going over the end?
                    {
                        //Make sure we're not including walls in the vision tile
                        Point candidatePoint = new Point(x, y);
                        if (CameraRoom.VerifyWalkable(candidatePoint)) //If it's walkable, add it
                        {
                            _watchedTiles.Add(candidatePoint);
                        }
                        else //if it's not walkable, but it is a prop (a box), still add it
                        {
                            foreach (Prop prop in CameraRoom.ItemsInRoom)
                            {
                                if (prop.Location == candidatePoint)
                                {
                                    _watchedTiles.Add(candidatePoint);
                                    break;
                                }
                            }
                        }
                        //if it's a wall instead of a prop, don't add it
                    }
                }
            }
            //Permanantely save this into an array that won't be changed and indicates the full kite with no obstructions
            _visionKite = _watchedTiles.ToArray();
            #endregion
            //Note that the length of _endPoints may be even or odd depending on rounding
        }

        /// <summary>
        /// Makes a security camera that watches a certain vision kite to see if the player is inside it, and has an electrical box which the player can interact with to deactivate it
        /// </summary>
        /// <param name="location">The location of the camera, from which the vision kite is projected</param>
        /// <param name="spriteArray">The array of the camera's sprites</param>
        /// <param name="spriteIndex">The index of this camera's sprite</param>
        /// <param name="containingRoom">The room containing the camera. Used to check if tiles are walkable</param>
        /// <param name="centerPoint">The point that forms the center of the camera's vision kite</param>
        /// <param name="spread">The arc from the center of the vision kite to the edge, in radians</param>
        /// <param name="boxLocation">The location of the camera's electrical box</param>
        public Camera(Point location, Texture2D[] spriteArray, int spriteIndex, Room containingRoom, Point centerPoint, float spread, Point boxLocation)
            : this(location, spriteArray, spriteIndex, containingRoom, centerPoint, spread)
        {



            //Add the wire box to the list of props in the room so that it gets drawn
            //I think we have to add it this way because we can't call the methods directly on this property. I might be wrong tho
            //It's like copy-alter-replace
            WireBox = new WireBox(boxLocation, AssetManager.CameraTextures, this, 2);
        }

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
                    if (!CameraRoom.VerifyWalkable(box, true))
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
                        for (int i = 0; i < _endPoints.Length; i++)
                        {
                            List<Point> tilesInRay = Rasterize(_rayBase, _endPoints[i]);
                            foreach (Point box in boxes)
                            {

                                //Debug.WriteLine($"Checking {_rayBase} with {endpoint}");
                                //foreach (Point point in Rasterize(_rayBase, endpoint)) Debug.Write(point + " ");
                                //Debug.WriteLine(null);
                                if (Rasterize(_rayBase, _endPoints[i]).Contains(box)) //A ray is blocked by the box
                                {
                                    //Debug.WriteLine($"Endpoint {endpoint.X}, {endpoint.Y} is blocked");
                                    //Remove all the tiles between the box and the end of the ray
                                    foreach (Point blockedTile in Rasterize(box, _endPoints[i])) _watchedTiles.Remove(blockedTile);
                                    //Now check the rays on either side of the ray you just found and treat them as blocked also
                                    //TODO: There is probably a more efficent way to do this, I'm just trying it out
                                    if (i > 0) foreach (Point blockedTile in Rasterize(box, _endPoints[i - 1])) _watchedTiles.Remove(blockedTile);
                                    if (i < _endPoints.Length - 1) foreach (Point blockedTile in Rasterize(box, _endPoints[i + 1])) _watchedTiles.Remove(blockedTile);
                                }
                            }
                        }
                    }

                    //Now do one more check of the watched tiles. If any of them is isolated, remove it
                    //NOTE: If there's lag or bugs in this area, this might be the cause. This is doing a lot of looping 
                    List<Point> isolatedTiles = new List<Point>();
                    foreach (Point watchedTile in _watchedTiles)
                    {
                        //Check the tiles above, below, and to either side of this tile.
                        //TODO: This may also be inefficent. Using a dictionary instead of a list of points might help with this, but I'm not sure yet
                        //In this case, unwalkable tiles also count as watched tiles basically
                        //Let's see if this causes problems

                        //TODO: Only check if the tile is further from the camera than the box is
                        //But what if there are multiple boxes?
                        int watchedNeighbors = 0;
                        Point watchedNeighbor = watchedTile;
                        //Left
                        watchedNeighbor.X -= 1;
                        if (_watchedTiles.Contains(watchedNeighbor) || !CameraRoom.VerifyWalkable(watchedNeighbor)) watchedNeighbors++;
                        //Right
                        watchedNeighbor.X += 2;
                        if (_watchedTiles.Contains(watchedNeighbor) || !CameraRoom.VerifyWalkable(watchedNeighbor)) watchedNeighbors++;
                        //Above
                        watchedNeighbor.X -= 1;
                        watchedNeighbor.Y -= 1;
                        if (_watchedTiles.Contains(watchedNeighbor) || !CameraRoom.VerifyWalkable(watchedNeighbor)) watchedNeighbors++;
                        //Below
                        watchedNeighbor.Y += 2;
                        if (_watchedTiles.Contains(watchedNeighbor) || !CameraRoom.VerifyWalkable(watchedNeighbor)) watchedNeighbors++;

                        //If the tile has one watched neighbor or less, then remove it
                        if (watchedNeighbors <= 1) isolatedTiles.Add(watchedTile);
                    }
                    //Now remove them all at once
                    foreach (Point p in isolatedTiles) _watchedTiles.Remove(p);

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

        //int endpointnumber = 0;
        public override void Draw(SpriteBatch sb, Point worldToScreen, Point pixelOffset)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToWorld(Location) - worldToScreen + pixelOffset, AssetManager.TileSize), null, //no source rectangle
                Color.White, _direction, Vector2.Zero, SpriteEffects.None, 0f); //Layer depth is not used
            WireBox.Draw(sb, worldToScreen, pixelOffset);
            if (_active)
            {
                foreach (Point tile in _watchedTiles)
                {
                    sb.Draw(AssetManager.CameraSight, new Rectangle(MapUtils.TileToWorld(tile) - worldToScreen + pixelOffset, AssetManager.TileSize), Color.White);
                }
            }
        }

        private List<Point> Rasterize(Point p1, Point p2)
        {
            bool rotated = false;

            // Swap X and Y if the line is steep
            if (Math.Abs(p2.Y - p1.Y) > Math.Abs(p2.X - p1.X))
            {
                rotated = true;
                (p1.X, p1.Y) = (p1.Y, p1.X);
                (p2.X, p2.Y) = (p2.Y, p2.X);
            }

            // Ensure left-to-right drawing
            if (p1.X > p2.X)
            {
                (p1, p2) = (p2, p1);
            }

            int dx = p2.X - p1.X;
            int dy = Math.Abs(p2.Y - p1.Y);
            int yStep = p2.Y > p1.Y ? 1 : -1; //If the line is going down, this will examine one tile down instead of one tile above

            int decisionParam = 2 * dy - dx;
            int y = p1.Y;

            List<Point> returnPoints = new();

            for (int x = p1.X; x <= p2.X; x++)
            {
                //Add the current point
                if (rotated) returnPoints.Add(new Point(y, x));
                else returnPoints.Add(new Point(x, y));

                //Calculate the next point - should Y change?
                if (decisionParam > 0)
                {
                    y += yStep; //either 1 or -1
                    decisionParam -= 2 * dx;
                }
                decisionParam += 2 * dy;
            }

            return returnPoints;
        }

        public override void Interact(Player player)
        {
            //The cameras can't be interacted with directly so this function doesn't do anything
            //TODO: I could change the inheritance to GameObject to take this function out of this class
            //But I think this is fine for now. This function will be called but won't do anything
        }

        /// <summary>
        /// Deacrivate this camera and stop running all detection logic
        /// </summary>
        public void Deactivate()
        {
            _active = false;
            SpriteIndex = 1;
            Sprite = AssetManager.CameraTextures[SpriteIndex];
        }
    }
}
