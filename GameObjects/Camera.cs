using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;


namespace MakeEveryDayRecount.GameObjects
{
    internal class Camera: GameObject, IInteractable
    {
        public Camera(Point location, Texture2D sprite) : base(location, sprite)
        {

        }
        public void Interact(Player player)
        {
            throw new NotImplementedException("Interact has not been created yet in Camera");

        }

        public override void Update(float deltaTimeS)
        {
            throw new NotImplementedException("Update has not been created yet in Camera");
        }
    }
}
