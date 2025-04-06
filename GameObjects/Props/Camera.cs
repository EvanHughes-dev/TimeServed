using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MakeEveryDayRecount.Players;

namespace MakeEveryDayRecount.GameObjects.Props
{
    internal class Camera : Prop
    {
        public Camera(Point location, Texture2D[] spriteArray, int spriteIndex)
            : base(location, spriteArray, spriteIndex) { }

        public override void Interact(Player player)
        {
            throw new NotImplementedException("Interact has not been created yet in Camera");
        }
    }
}
