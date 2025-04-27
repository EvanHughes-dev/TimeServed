using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MakeEveryDayRecount.GameObjects.Props;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.UI;
using System.Collections.ObjectModel;

namespace MakeEveryDayRecount.Players.InventoryFiles
{
    /// <summary>
    /// Represent what the player is holding in there inventory to the player
    /// </summary>
    internal class Inventory
    {
        private Point _boxSize;

        private InventorySpace[] _inventoryUI;

        private List<Item> _contents;

        /// <summary>
        /// Read only version of the list of items currently in the inventory
        /// </summary>
        public ReadOnlyCollection<Item> Contents
        {
            get { return _contents.AsReadOnly(); }
        }

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
            _boxSize = new Point(screenSize.X / 32, screenSize.X / 32);
            int testValue = (int)(InterfaceManager.ScaleFactorX / 2);
            _inventoryUI = new InventorySpace[4];
            _selectedSpace = null;

            for (int i = 0; i < _inventoryUI.Length; i++)
            {
                Point drawPoint = new Point(screenSize.X / 2 - (_boxSize.X * _inventoryUI.Length / 2 + _boxSize.X / 2 * (_inventoryUI.Length % 2)) + (_boxSize.X * i),
                screenSize.Y - _boxSize.Y);

                //create a button and offset the x
                _inventoryUI[i] = new InventorySpace(AssetManager.InventoryBoxes[0], AssetManager.InventoryBoxes[1],
                    new Rectangle(drawPoint, _boxSize), true, AssetManager.InventoryBoxes[2]);
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
        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < _inventoryUI.Length; i++)
            {
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

        /// <summary>
        /// Clear the player's inventory
        /// </summary>
        public void ClearInventory()
        {
            _contents.Clear();
            foreach (InventorySpace space in _inventoryUI)
            {
                space.ReplaceItem(null);
                space.DeselectItem();
            }

            SelectedItem = null;
        }
    }
}
