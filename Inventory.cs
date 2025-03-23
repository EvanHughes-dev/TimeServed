using MakeEveryDayRecount.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects.Props;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Map;

namespace MakeEveryDayRecount
{
    internal class Inventory
    {
        int _boxSize = AssetManager.InventoryBoxes[0].Width / 2;
        /// <summary>
        /// The index of the current selected item
        /// </summary>
        private int indexOfSelected = -1;
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

        public Inventory(Point screenSize)
        {
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                //create a button and offset the x
                _inventoryUI[i] = new Button(AssetManager.InventoryBoxes[0], AssetManager.InventoryBoxes[1],
                    new Rectangle(screenSize.X/2 - (_boxSize * 2) + (_boxSize * i), 
                    screenSize.Y - (_boxSize + 30),
                    _boxSize, _boxSize), true);
                _inventoryUI[i].OnClick += () => Select(i);
            }
        }
        /// <summary>
        /// Draws the inventory's UI using a button array 
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb, Point screenSize) 
        {
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                _inventoryUI[i].Rectangle= new Rectangle(screenSize.X / 2 - (_boxSize * 2) + (_boxSize * i), screenSize.Y - (_boxSize + 30), _boxSize, _boxSize);
                _inventoryUI[i].Draw(sb);
            }

        }
        public void Select(int index) {
            if (indexOfSelected == index) 
            {
                indexOfSelected = -1;
                _inventoryUI[index].Image = AssetManager.InventoryBoxes[0];
                _inventoryUI[index].Active = true;
            }

            indexOfSelected = index;
            _inventoryUI[index].Image = AssetManager.InventoryBoxes[2];
            _inventoryUI[index].Active = false;
        }
    }
}
