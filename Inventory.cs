using MakeEveryDayRecount.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount
{
    internal class Inventory
    {
        private Inventory() 
        { 
            //for (int i = 0; i < inventory.Length; i++)
            //{
            //    create a button and offset the x
            //}
        }

        private static Inventory instance = null;
        public static Inventory Instance
        {
            get 
            { 
                if (instance == null) { instance = new Inventory(); }

                return instance;
            }
        }
        //inventory will always be 4 buttons
        //Button[] inventoryUI = new Button[4];
        private List<Item> _contents;
        public List<Item> Contents { get { return _contents; } }
    }
}
