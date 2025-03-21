using MakeEveryDayRecount.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeEveryDayRecount.GameObjects.Props;

namespace MakeEveryDayRecount
{
    internal class Inventory
    {

        /// <summary>
        /// Array of buttons used to display the inventory
        /// </summary>
        private Button[] _inventoryUI = new Button[4];

        /// <summary>
        /// A list of the items the inventory currently holds
        /// </summary>
        private List<Item> _contents;

        /// <summary>
        /// Returns the list of the items currently in the inventory
        /// </summary>
        public List<Item> Contents { get { return _contents; } }

        public Inventory()
        {
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                //create a button and offset the x
                _inventoryUI[i] = new Button(AssetManager.InventoryBoxes[0], AssetManager.InventoryBoxes[1],
                    new Rectangle(0 + (AssetManager.InventoryBoxes[0].Width * i), 0,
                    AssetManager.InventoryBoxes[0].Width, AssetManager.InventoryBoxes[0].Height), true);
            }
        }
        public void Draw(SpriteBatch sb) 
        {
            foreach (Button button in _inventoryUI)
            {
                button.Draw(sb);
            }

        }
    }
}
