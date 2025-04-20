using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Props
{
    /// <summary>
    /// Represents a single security camera that can spot the player.
    /// </summary>
    public class Camera : Prop
    {
        /// <summary>
        /// This camera's integer position in the room, in tile space.
        ///   (0, 0) is the top-left corner of the room.
        ///   Triggers a view frustum update.
        /// </summary>
        public override Point? Position {
            get => base.Position;
            set 
            {
                base.Position = value;
                ViewFrustumUpdate?.Invoke(this);
            } 
        }

        /// <summary>
        /// The point, in tilespace, this camera is pointed at.
        ///   Triggers a view frustum update.
        /// </summary>
        public Point? Target
        {
            get => _target;
            set
            {
                _target = value;
                ViewFrustumUpdate?.Invoke(this);
            }
        }
        private Point? _target;
        /// <summary>
        /// The spread of this camera, in radians. Equal to the radian distance between the center ray and either side ray.
        ///   Triggers a view frustum update.
        /// </summary>
        public float RadianSpread
        {
            get => _radianSpread;
            set
            {
                _radianSpread = value;
                ViewFrustumUpdate?.Invoke(this);
            }
        }
        private float _radianSpread;

        /// <summary>
        /// Invoked whenever a property of this Camera changes that would require a re-rendering of the view frustum.
        ///   Passes in a reference to this camera when invoked.
        /// </summary>
        public event Action<Camera>? ViewFrustumUpdate;

        /// <summary>
        /// Creates a new Camera with the given sprite, position, target, and spread.
        /// </summary>
        /// <param name="sprite">The sprite this Box displays with.</param>
        /// <param name="imageIndex">Index of this image within the list of camera sprites.</param>
        /// <param name="position">The position of this Camera.</param>
        /// <param name="target">The tile this Camera should point to.</param>
        /// <param name="radianSpread">The angle of spread the camera should have to each side of its center ray, in radians.</param>
        public Camera(Image sprite, int imageIndex, Point? position = null, Point? target = null, float radianSpread = float.NaN)
            : base(sprite, imageIndex, ObjectType.Camera, position)
        {
            Target = target;
            RadianSpread = radianSpread;
        }

        /// <summary>
        /// Creates a copy of this Camera at the given position.
        /// </summary>
        /// <param name="position">The position to copy this Camera to.</param>
        /// <returns>A copy of this Camera at the given position.</returns>
        public override Camera Instantiate(Point position)
        {
            return new Camera(Sprite, ImageIndex, position, Target, RadianSpread);
        }

        /// <summary>
        /// Creates a copy of this Camera at the given position with the given target and spread.
        /// </summary>
        /// <param name="position">The position to copy this Camera to.</param>
        /// <param name="target">The tile this Camera should point to.</param>
        /// <param name="radianSpread">The angle of spread the camera should have to each side of its center ray, in radians.</param>
        /// <returns>The created copy.</returns>
        public Camera Instantiate(Point position, Point target, float radianSpread)
        {
            return new Camera(Sprite, ImageIndex, position, target, radianSpread);
        }
    }
}
