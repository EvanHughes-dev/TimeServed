using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.Managers;
using System.Linq;
using MakeEveryDayRecount.Map.Tiles;

namespace MakeEveryDayRecount.GameObjects.Props
{

    /// <summary>
    /// Create a camera that can watch tiles and detect if the 
    /// player has entered its view. Allow boxes to block its view and
    /// update what tiles are watched
    /// </summary>
    internal class Camera : Prop
    {
        //If the camera is on or off
        private bool _active;

        /// <summary>
        /// The wire location this is connected to
        /// </summary>
        public WireBox WireBox { get; private set; }

        /// <summary>
        /// Center point this camera is watching
        /// </summary>
        public Point CenterPoint
        {
            get { return _centerPoint; }
        }

        /// <summary>
        /// Radian spread of this camera's sight
        /// </summary>
        public float Spread
        {
            get { return _spread; }
        }

        /// <summary>
        /// Get the room this camera is in
        /// </summary>
        public Room CameraRoom { get; private set; }

        //Specify a point for the center of it's vision and an angle in radians for the width of it's field of vision

        //The point at the center of the vision cone
        private Point _centerPoint;
        private float _spread;
        //The camera will get a certain set of endpoints to look towards when it's created, and those points will never change, even if they get blocked
        private List<Tile> _endPoints;
        //This float tells the camera which way it's facing. By default, the camera faces down
        private float _direction;

        //The rays don't project from inside of the wall, where the camera is technically drawn
        //All the rays come out from the point on the floor right "in front of" the camera
        private Tile _rayBase;

        private HashSet<Tile> _visionKite;
        private List<Tile> _watchedTiles;

        //It also needs a reference to the player to know if they step into the vision kite
        private Player _player;

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
            _watchedTiles = new List<Tile>(); //This ends up being the same as visionkite by the end of this function
            _player = GameplayManager.PlayerObject;
            //All cams start active
            _active = SpriteIndex == 0;
            CameraRoom = containingRoom;
            _centerPoint = centerPoint;
            _spread = spread;

            // If the sprite index is 1 then it is an inactive camera
            if (!_active)
                return;
            #region Raybase Check
            //Check which way the ray for the camera is pointing
            Vector2 centerRay = new Vector2(_centerPoint.X - location.X, _centerPoint.Y - location.Y); //base of this ray is at location

            // TODO rewrite this logic for the love of god
            //Please do not put the centerpoint on top of the camera it breaks everything
            if (Math.Abs(centerRay.X) > Math.Abs(centerRay.Y)) //The ray is more horizontal
            {
                if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y), true)) //cam is pointing left
                {
                    _rayBase = CameraRoom.GetTile(new Point(location.X - 1, location.Y));
                    _direction = 0.5f;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X + 1, location.Y), true))//Cam is pointing right
                {
                    _rayBase = CameraRoom.GetTile(new Point(location.X + 1, location.Y));
                    _direction = 1.5f;
                }
                //If both the left and right tiles aren't walkable, try to set the raybase either up or down
                else
                {
                    if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(Location.X, Location.Y - 1), true))
                    {
                        _rayBase = CameraRoom.GetTile(new Point(location.X, Location.Y - 1));
                        _direction = 1f;
                    }
                    else //This will be fine unless the camera is deep in the wall or cameraRay is zero vector
                    {
                        _rayBase = CameraRoom.GetTile(new Point(location.X, Location.Y + 1));
                        _direction = 0f;
                    }
                }
            }
            else //the ray is more vertical
            {
                if (centerRay.Y < 0 && containingRoom.VerifyWalkable(new Point(location.X, location.Y - 1), true)) //camera is pointing up
                {
                    _rayBase = CameraRoom.GetTile(new Point(Location.X, location.Y - 1));
                    _direction = 1f;
                }
                else if (containingRoom.VerifyWalkable(new Point(location.X, location.Y + 1), true)) //camera is pointing down
                {
                    _rayBase = CameraRoom.GetTile(new Point(Location.X, location.Y + 1));
                    _direction = 0f;
                }
                //If both the up and down tiles aren't walkable, try to set the raybase either left or right
                else
                {
                    if (centerRay.X < 0 && containingRoom.VerifyWalkable(new Point(location.X - 1, location.Y), true))
                    {
                        _rayBase = CameraRoom.GetTile(new Point(Location.X - 1, location.Y));
                        _direction = 0.5f;
                    }
                    else //This will be fine unless the camera is deep in the wall or cameraRay is zero vector
                    {
                        _rayBase = CameraRoom.GetTile(new Point(Location.X + 1, Location.Y));
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
            centerRay = new Vector2(_centerPoint.X - _rayBase.Location.X, _centerPoint.Y - location.Y); //Base of the ray is now at raybase
            //Rotate that vector by spread in both directions
            Vector2 clockwiseRay = Vector2.Transform(centerRay, Matrix.CreateRotationZ(spread));
            Vector2 counterclockwiseRay = Vector2.Transform(centerRay, Matrix.CreateRotationZ(-spread));
            //^These built-in methods are *chef kiss*

            //Turn these rotated vectors back into points to get the corners of the kite
            Point clockwisePoint = new Point((int)MathF.Round(_rayBase.Location.X + clockwiseRay.X), (int)MathF.Round(_rayBase.Location.Y + clockwiseRay.Y));
            Point counterclockwisePoint = new Point((int)MathF.Round(_rayBase.Location.X + counterclockwiseRay.X), (int)MathF.Round(_rayBase.Location.Y + counterclockwiseRay.Y));
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
            // TODO finding endpoints is wrong
            _endPoints = endPoints.Select(p => CameraRoom.GetTile(p)).OfType<Tile>().ToList<Tile>();

            //Create a rectangle that bounds the entire kite

            Point[] corners = { _rayBase, _centerPoint, clockwisePoint, counterclockwisePoint };
            //Find the minimum/maximum X and Y of the 4 bounding points (the edges of the rectangle basically)
            int minX = _rayBase.Location.X;
            int maxX = _rayBase.Location.X;
            int minY = _rayBase.Location.Y;
            int maxY = _rayBase.Location.Y;
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
                    Vector2 candidateVector = new Vector2(x - _rayBase.Location.X, y - _rayBase.Location.Y);
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
                        && candidateVector.Length() <= centerRay.Length())
                    {
                        //Make sure we're not including walls in the vision tile
                        Point candidatePoint = new Point(x, y);
                        if (CameraRoom.VerifyWalkable(candidatePoint, true)) //If it's walkable, add it
                        {

                            Tile pointTile = CameraRoom.GetTile(candidatePoint);
                            _watchedTiles.Add(pointTile);
                            
                            pointTile.AddWatcher();
                            pointTile.BoxChange += UpdateTrackedTiles;
                            pointTile.PlayerEnteredTile += DetectPlayer;
                        }

                    }
                }
            }
            //Permanently save this into an array that won't be changed and indicates the full kite with no obstructions
            _visionKite = _watchedTiles.ToHashSet<Tile>();
            #endregion
            //Note that the length of _endPoints may be even or odd depending on rounding
        }

        /// <summary>
        /// Makes a security camera that watches a certain vision kite to see if the player is inside it, and has an electrical location which the player can interact with to deactivate it
        /// </summary>
        /// <param name="location">The location of the camera, from which the vision kite is projected</param>
        /// <param name="spriteArray">The array of the camera's sprites</param>
        /// <param name="spriteIndex">The index of this camera's sprite</param>
        /// <param name="containingRoom">The room containing the camera. Used to check if tiles are walkable</param>
        /// <param name="centerPoint">The point that forms the center of the camera's vision kite</param>
        /// <param name="spread">The arc from the center of the vision kite to the edge, in radians</param>
        /// <param name="boxLocation">The location of the camera's electrical location</param>
        public Camera(Point location, Texture2D[] spriteArray, int spriteIndex, Room containingRoom, Point centerPoint, float spread, Point boxLocation)
            : this(location, spriteArray, spriteIndex, containingRoom, centerPoint, spread)
        {
            //Add the wire location to the list of props in the room so that it gets interacted with
            WireBox = new WireBox(boxLocation, AssetManager.CameraTextures, this, 2);
            CameraRoom.AddProp(WireBox);

        }

        /// <summary>
        /// Update which tiles are watched
        /// </summary>
        private void UpdateTrackedTiles()
        {
            foreach (Tile tile in _watchedTiles)
            {
                UnWatch(tile);
            }

 
            //Used to check if there's a location on the raybase
            bool raybaseBlocked = false;
            //Check the entire vision kite for boxes in order to figure out what tiles are currently watched
            _watchedTiles.Clear();

            // If the raybase itself is blocked, camera sees nothing
            Tile rayBaseTile = CameraRoom.GetTile(_rayBase);
            if (rayBaseTile.IsBlockingCamera)
                return;

            HashSet<Tile> uncheckedTiles = new HashSet<Tile>(_visionKite);

            uncheckedTiles.Remove(rayBaseTile);

            _watchedTiles.Add(rayBaseTile);
            WatchTile(rayBaseTile);



            // This code needs to be optimized
            // This currently runs in O(n^2) time
            // I think I can get this down to O(n) or maybe O(nlog(n)) time
            // We can cache the result of the ray and look for similar points
            // or Rasterize to ends then find any missed points and raterize them

            foreach (Tile target in _endPoints)
            {
                // Cast from start to endpoint
                List<Point> ray = Rasterize(_rayBase, target.Location);

                // Skip raybase (i = 1) 
                for (int i = 1; i < ray.Count; i++)
                {
                    Tile stepTile = CameraRoom.GetTile(ray[i]);
                    uncheckedTiles.Remove(stepTile);

                    // If this tile has a box
                    if (stepTile.IsBlockingCamera)
                    {
                        // Remove all tiles in the ray cast from this tile to the endPoint
                        // These tiles can not be seen by the camera
                        for (int j = i + 1; j < ray.Count; j++)
                        {
                            Tile nextTileNotInView = CameraRoom.GetTile(ray[j]);
                            uncheckedTiles.Remove(nextTileNotInView);
                        }
                        break;
                    }
                    else
                    {
                        WatchTile(stepTile);
                        // Otherwise this tile can be seen
                        _watchedTiles.Add(stepTile);
                    }
                }   


            }

            foreach (Tile target in uncheckedTiles)
            {
                // Check if this tile has a box on it
                if (target.IsBlockingCamera) continue;

                List<Point> ray = Rasterize(_rayBase, target.Location);

                bool blocked = false;
                
                // Skip raybase (i = 1) and stop *before* the target tile
                for (int i = 1; i < ray.Count - 1; i++)
                {
                    Tile stepTile = CameraRoom.GetTile(ray[i]);
                    if (stepTile.IsBlockingCamera)
                    {
                        blocked = true;
                        break;
                    }
                }

                if (!blocked)
                {
                    _watchedTiles.Add(target);
                    WatchTile(target);
                }
            }
        }

        /// <summary>
        /// Let the tile know that it is being watched
        /// </summary>
        /// <param name="watchTile">Tile to watch</param>
        private void WatchTile(Tile watchTile)
        {
            watchTile.AddWatcher();
        }

        /// <summary>
        /// Let the tile know that this camera is no longer watching it
        /// </summary>
        /// <param name="unWatch">Tile to un watch</param>
        private void UnWatch(Tile unWatch)
        {
            unWatch.RemoveWatcher();
        }
        /// <summary>
        /// Draw the camera, wire location and all tiles this camera can see
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Sprite, new Rectangle(MapUtils.TileToScreen(Location) + AssetManager.HalfTileSize, AssetManager.TileSize), null, //no source rectangle
                Color.White, _direction, new Point(Sprite.Width / 2, Sprite.Height / 2).ToVector2(), SpriteEffects.None, 0f); //Layer depth is not used
            WireBox?.Draw(sb);
        }

        /// <summary>
        /// Draw the path of each camera's vision cone
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public void DebugDraw(SpriteBatch sb)
        {
            // TODO find some other way to draw rays and endpoints
            foreach (Point EndPoint in _endPoints)
                foreach (Point endpoint in Rasterize(_rayBase, EndPoint))
                    sb.Draw(AssetManager.PropTextures[3], new Rectangle(MapUtils.TileToScreen(endpoint), AssetManager.TileSize), Color.Red);
            foreach (Point endpoint in _endPoints)
                sb.Draw(AssetManager.PropTextures[3], new Rectangle(MapUtils.TileToScreen(endpoint), AssetManager.TileSize), Color.Green);
        }

        /// <summary>
        /// Cast a ray between two points
        /// </summary>
        /// <param name="point1">Point to cast from</param>
        /// <param name="Point2">Point to cast to</param>
        /// <returns>List of all points between the two points</returns>
        private List<Point> Rasterize(Point point1, Point Point2)
        {
            bool rotated = false;

            // Swap X and Y if the line is steep
            if (Math.Abs(Point2.Y - point1.Y) > Math.Abs(Point2.X - point1.X))
            {
                rotated = true;
                (point1.X, point1.Y) = (point1.Y, point1.X);
                (Point2.X, Point2.Y) = (Point2.Y, Point2.X);
            }

            // Ensure left-to-right drawing
            if (point1.X > Point2.X)
            {
                (point1, Point2) = (Point2, point1);
            }

            int dx = Point2.X - point1.X;
            int dy = Math.Abs(Point2.Y - point1.Y);
            int yStep = Point2.Y > point1.Y ? 1 : -1; //If the line is going down, this will examine one tile down instead of one tile above

            int decisionParam = 2 * dy - dx;
            int y = point1.Y;

            List<Point> returnPoints = new();

            for (int x = point1.X; x <= Point2.X; x++)
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

        /// <summary>
        /// The player can not interact with a camera
        /// </summary>
        /// <param name="player">Player that tried to interact</param>
        public override void Interact(Player player)
        {
            //The cameras can't be interacted with directly so this function doesn't do anything
            //But I think this is fine for now. This function will be called but won't do anything
        }

        /// <summary>
        /// Deactivate this camera and stop running all detection logic
        /// </summary>
        public void Deactivate()
        {
            _active = false;
            SpriteIndex = 1;
            Sprite = AssetManager.CameraTextures[SpriteIndex];
            foreach (Tile tile in _visionKite)
            {
                tile.BoxChange -= UpdateTrackedTiles;
                tile.PlayerEnteredTile -= DetectPlayer;
            }

            foreach (Tile tile in _watchedTiles)
            {
                tile.RemoveWatcher();
            }
        }

        /// <summary>
        /// Check if the location is in the currently watched tiles
        /// </summary>
        /// <param name="location">Location to check</param>
        private void DetectPlayer(Point location)
        {
            //Now check for the player :)
            foreach (Tile watchedTile in _watchedTiles)
            {
                if (location == watchedTile.Location)
                {
                    //WE FOUND THE PLAYER! GET HIM BOYS!
                    _player.Detected();
                    break;
                }
            }
        }
    }
}
