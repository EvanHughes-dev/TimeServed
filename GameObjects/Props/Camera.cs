using System;
using System.Collections.Generic;
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
        private Vector2[] _rays;

        private List<Point> _watchedTiles;

        public Camera(Point location, Texture2D sprite)
            : base(location, sprite)
        {
            _watchedTiles = new List<Point>();
            //All cams start active
            _active = true;
        }
        //TODO: add an alternative constructor that takes a point as the center of the vision cone and constructs a vector from that

        public void Update(float deltaTime)
        {
            //Add a check to see if there's a box directly in front of the camera, and if so don't check any of the rays
        }

        private void CheckRay(Vector2 ray)
        {
            //create a parametric line using the point and the vector
            //Step along that line in small intervals, checking what tile you're over at every step
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
