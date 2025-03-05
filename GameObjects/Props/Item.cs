using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Item : Prop
    {
        public Item(Point location, Texture2D sprite)
            : base(location, sprite) { }

        public override void Interact(Player player)
        {
            throw new NotImplementedException("Interact has not been created yet in Item");
        }

        public override void Update(float deltaTimeS)
        {
            throw new NotImplementedException("Update has not been created yet in Item");
        }
    }
}
