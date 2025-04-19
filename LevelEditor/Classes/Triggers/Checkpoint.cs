using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Triggers
{
    /// <summary>
    /// Represents a single Checkpoint that the player can collide with.
    /// </summary>
    public class Checkpoint : Trigger
    {
        /// <summary>
        /// Gets or sets the checkpoint's index. Checkpoints should be collected (on a per-room basis)
        ///   from lowest index to highest index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Creates a new Checkpoint with the given index.
        /// </summary>
        /// <param name="index">The index of the checkpoint.</param>
        /// <param name="bounds">The bounds of the checkpoint, or null if it has none.</param>
        public Checkpoint(int index, Rectangle? bounds = null)
            : base(bounds)
        {
            Index = index;
        }

        /// <summary>
        /// Creates a copy of this Checkpoint with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the copy.</param>
        /// <returns>A copy of this Checkpoint.</returns>
        public override Checkpoint Instantiate(Rectangle bounds)
        {
            return new(Index, bounds);
        }
    }
}
