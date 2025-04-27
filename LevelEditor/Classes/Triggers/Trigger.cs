using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Triggers
{
    /// <summary>
    /// Represents a Trigger, an invisible bounding box that potentially has behavior upon collision.
    /// </summary>
    public abstract class Trigger
    {
        /// <summary>
        /// Gets or sets the bounds of the Trigger, declaring both its position and size (in tilespace).
        /// </summary>
        public Rectangle? Bounds { get; set; }
        /// <summary>
        /// returns the color of the associated trigger
        /// </summary>
        public abstract Color Color { get; }

        /// <summary>
        /// Creates a new Trigger.
        /// </summary>
        /// <param name="bounds">The bounds of the Trigger, or null if it is not in a room.</param>
        public Trigger(Rectangle? bounds = null)
        {
            // Save params -- no processing necessary
            Bounds = bounds;
        }

        /// <summary>
        /// Creates a copy of this Trigger with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the copy.</param>
        /// <returns>A copy of this Trigger.</returns>
        public abstract Trigger Instantiate(Rectangle bounds);
    }
}
