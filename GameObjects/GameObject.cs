using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
namespace MakeEveryDayRecount.GameObjects
{
    internal abstract class GameObject
    {
        public Point _location;
        public Texture2D _sprite;
        

        public GameObject(Point location, Texture2D sprite)
        {
            _location = location;
            _sprite = sprite;
            
        }

        //Methods
        public abstract void Update(float deltaTimeS);
        public void Draw(SpriteBatch sb) {
            throw new NotImplementedException("Draw not been created yet in GameObject");
        }

    }
}
