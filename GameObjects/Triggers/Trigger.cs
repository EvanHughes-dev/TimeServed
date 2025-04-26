using Microsoft.Xna.Framework;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.GameObjects.Triggers
{
    /// <summary>
    /// Extension of GameObject that can "trigger" an event if the player steps onto it
    /// </summary>
    internal abstract class Trigger : GameObject
    {
        //Fields
        protected int _width;
        protected int _height;

        //Properties

        /// <summary>
        /// Width of the trigger, in tiles
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Height of the trigger, in tiles
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Creates a trigger object
        /// </summary>
        /// <param name="location">Location of the top left-most tile</param>
        /// <param name="width">Width of the trigger, in tiles</param>
        /// <param name="height">Height of the trigger in tiles</param>
        protected Trigger(Point location, int width, int height)
            : base(location, null)
        {
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Allow the player to interact with this trigger
        /// </summary>
        /// <param name="player">Player that triggered the trigger</param>
        /// <returns>If the trigger action was successful</returns>
        public abstract bool Activate(Player player);
    }
}
