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
        /// <summary>
        /// Creates an inventory with a button array of size 4
        /// </summary>
        /// <param name="screenSize">The size of the screen</param>
        public Inventory(Point screenSize)
        {
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                //create a button and offset the x
                _inventoryUI[i] = new Button(AssetManager.InventoryBoxes[0], AssetManager.InventoryBoxes[1],
                    new Rectangle(screenSize.X/2 - (_boxSize * 2) + (_boxSize * i), 
                    screenSize.Y - (_boxSize + 30),
                    _boxSize, _boxSize), true);
                int copyOfIndex = i;
                _inventoryUI[i].OnClick += () => Select(copyOfIndex);
            }
        }
        /// <summary>
        /// Updates the inventory array
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                _inventoryUI[i].Update();
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
        /// <summary>
        /// Selects and deselects the inventory box when it is pressed
        /// </summary>
        /// <param name="index"></param>
        public void Select(int index) {
            if (indexOfSelected == index)
            {
                indexOfSelected = -1;
                _inventoryUI[index].Image = AssetManager.InventoryBoxes[0];
                _inventoryUI[index].Active = true;
            }
            else
            {
                //if currectly a box is already selected
                if (indexOfSelected != -1)
                {
                    //deselect the box
                    _inventoryUI[indexOfSelected].Image = AssetManager.InventoryBoxes[0];
                    _inventoryUI[indexOfSelected].Active = true;
                }
                //update the selected index
                indexOfSelected = index;

                //select the new one
                _inventoryUI[indexOfSelected].Image = AssetManager.InventoryBoxes[2];
                _inventoryUI[indexOfSelected].Active = false;
            }
        }
    }
}
