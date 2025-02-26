using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;


namespace MakeEveryDayRecount.GameObjects
{
    internal class Item : GameObject, IInteractable
    {

        public Item(Point location, Texture2D sprite): base(location, sprite)
        {
         
        }
        public void Interact(Player player)
        {
            throw new NotImplementedException("Interact has not been created yet in Item");

        }

        public override void Update(float deltaTimeS)
        {
            throw new NotImplementedException("Update has not been created yet in Item");
        }
    }
}
