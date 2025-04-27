using LevelEditor.Classes.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Triggers
{
    /// <summary>
    /// Represents a single win trigger that the player can collide with.
    /// </summary>
    class WinTrigger : Trigger
    {
        /// <summary>
        /// Holds the win condition item
        /// </summary>
        public Item RequiredItem { get; set; }
        /// <summary>
        /// The color representing this trigger
        /// </summary>
        public override Color Color => Color.Yellow;
        /// <summary>
        /// Creates a new win trigger with the given index.
        /// </summary>
        /// <param name="index">The index of the checkpoint.</param>
        /// <param name="bounds">The bounds of the checkpoint, or null if it has none.</param>
        public WinTrigger(Item requiredItem, Rectangle? bounds = null)
            : base(bounds)
        {
            RequiredItem = requiredItem;
        }

        /// <summary>
        /// Creates a copy of this win trigger with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the copy.</param>
        /// <returns>A copy of this Checkpoint.</returns>
        public override WinTrigger Instantiate(Rectangle bounds)
        {
            return new(RequiredItem, bounds);
        }
    }
}
