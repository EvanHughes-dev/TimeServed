using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Triggers
{
    /// <summary>
    /// Represents a single PlayerSpawn, marking where to create the player in a room if it is the first room of a level.
    /// </summary>
    public class PlayerSpawn : Trigger
    {
        /// <summary>
        /// Creates a new PlayerSpawn.
        /// </summary>
        /// <param name="location">The location of the new PlayerSpawn, or null if the PlayerSpawn is not in a room.</param>
        public PlayerSpawn(Point? location)
            : base( // The bounds of the checkpoint should be 1x1 at the given location
                  location != null
                  ? new((Point)location, new(1, 1))
                  : null
                  ) 
        {
            // Nothing else is necessary!
        }

        /// <summary>
        /// Creates a copy of this PlayerSpawn given the provided bounds. 
        ///   Note that the size of the provided bounds will be set to 1x1!
        /// </summary>
        /// <param name="bounds">The bounds of the copy.</param>
        /// <returns>A copy of this PlayerSpawn.</returns>
        public override PlayerSpawn Instantiate(Rectangle bounds)
        {
            return new(bounds.Location);
        }
    }
}
