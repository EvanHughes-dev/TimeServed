using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal abstract class Prop : GameObject
    {
        protected Prop(Point location, Texture2D[] spriteArray, int spriteIndex)
            : base(location, spriteArray, spriteIndex) { }

        public abstract void Interact(Player player);
    }
}
