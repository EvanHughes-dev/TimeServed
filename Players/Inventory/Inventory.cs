using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.UI;

namespace MakeEveryDayRecount.Players.InventoryFiles
{
    internal class Inventory
    {
        int _boxSize = AssetManager.InventoryBoxes[0].Width / 2;

        /// <summary>
        /// Array of buttons used to display the inventory
        /// </summary>
        private InventorySpace[] _inventoryUI;

        /// <summary>
        /// A list of the items the inventory currently holds
        /// </summary>
        private List<Item> _contents;

        /// <summary>
        /// Get the current item selected in the inventory
        /// </summary>
        public Item SelectedItem { get; private set; }

        private InventorySpace _selectedSpace;

        /// <summary>
        /// Creates an inventory with a button array of size 4
        /// </summary>
        /// <param name="screenSize">The size of the screen</param>
        public Inventory(Point screenSize)
        {
            _selectedSpace = null;
            _inventoryUI = new InventorySpace[4];
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                //create a button and offset the x
                _inventoryUI[i] = new InventorySpace(AssetManager.InventoryBoxes[0], AssetManager.InventoryBoxes[1],
                    new Rectangle(screenSize.X / 2 - (_boxSize * 2) + (_boxSize * i),
                    screenSize.Y - (_boxSize + 30),
                    _boxSize, _boxSize), true, AssetManager.InventoryBoxes[2]);
                _inventoryUI[i].OnInventorySpaceSelected += SwitchActiveInventorySpace;
            }

            _contents = new List<Item> { };
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
        /// <param name="sb">SpriteBatch to draw with</param>
        public void Draw(SpriteBatch sb, Point screenSize)
        {
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                _inventoryUI[i].ChangeOrigin(new Point(screenSize.X / 2 - (_boxSize * 2) + (_boxSize * i), screenSize.Y - (_boxSize + 30)));
                _inventoryUI[i].Draw(sb);
            }

        }

        /// <summary>
        /// Add an item to the player's inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItemToInventory(Item item)
        {
            _contents.Add(item);
            _inventoryUI[_contents.Count - 1].ReplaceItem(item);
        }

        /// <summary>
        /// Update which tile is selected
        /// </summary>
        /// <param name="inventorySpace">New inventory space to select</param>
        public void SwitchActiveInventorySpace(InventorySpace inventorySpace)
        {
            SelectedItem = inventorySpace.CurrentItem;
            _selectedSpace?.DeselectItem();
            _selectedSpace = inventorySpace;
        }
    }
}
