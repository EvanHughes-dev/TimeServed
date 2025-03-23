
using MakeEveryDayRecount.GameObjects.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MakeEveryDayRecount
{
    delegate void OnInventoryChange(InventorySpace inventorySpace);

    /// <summary>
    /// Track the current inventory section and display its contents
    /// </summary>
    internal class InventorySpace : Button
    {
        /// <summary>
        /// Get if this current inventory space has an item
        /// </summary>
        public bool ContainItem { get { return CurrentItem == null; } }

        /// <summary>
        /// Current item in the inventory
        /// </summary>
        public Item CurrentItem { get; private set; }

        private Texture2D _selectedSpaceTexture;
        private bool _spaceSelected;
        public event OnInventoryChange OnInventorySpaceSelected;

        /// <summary>
        /// Initialize the player without an item in the inventory
        /// </summary>
        /// <param name="image">Base image of the button</param>
        /// <param name="hoverImage">Image when the image is hovered</param>
        /// <param name="rectangle">Rectangle to display the base image in</param>
        /// <param name="active">If the button is active</param>
        /// <param name="selectedTile">Tile to overlay if this is selected</param>
        public InventorySpace(Texture2D image, Texture2D hoverImage, Rectangle rectangle, bool active, Texture2D selectedTile)
        : base(image, hoverImage, rectangle, active)
        {
            _selectedSpaceTexture = selectedTile;
            _spaceSelected = false;
        }

        /// <summary>
        /// Initialize the player with an item in the inventory
        /// </summary>
        /// <param name="image">Base image of the button</param>
        /// <param name="hoverImage">Image when the image is hovered</param>
        /// <param name="rectangle">Rectangle to display the base image in</param>
        /// <param name="active">If the button is active</param>
        /// <param name="currentItem">Item that is in current in this slot</param>
        /// <param name="selectedTile">Tile to overlay if this is selected</param>
        public InventorySpace(Texture2D image, Texture2D hoverImage, Rectangle rectangle, bool active, Texture2D selectedTile, Item currentItem)
        : this(image, hoverImage, rectangle, active, selectedTile)
        {
            CurrentItem = currentItem;
        }

        /// <summary>
        /// Replace the current object in the inventory panel
        /// </summary>
        /// <param name="newItem">Item to replace</param>
        public void ReplaceItem(Item newItem)
        {
            CurrentItem = newItem;
        }

        /// <summary>
        /// If this item is selected, set this item to the current selected item in the inventory
        /// </summary>
        public override void Update()
        {
            if (Hovered && InputManager.GetMousePress(MouseButtonState.Left))
            {
                OnInventorySpaceSelected?.Invoke(this);
                _spaceSelected = true;
            }
        }

        /// <summary>
        /// Draw the inventory section with the item that is in it
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (CurrentItem != null)
                sb.Draw(CurrentItem.Sprite, Rectangle, Color.White);
            if (_spaceSelected)
                sb.Draw(_selectedSpaceTexture, Rectangle, Color.White);

        }

        /// <summary>
        /// Deselect this space
        /// </summary>
        public void DeselectItem()
        {
            _spaceSelected = false;
        }
    }
}