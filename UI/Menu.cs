using System.Collections.Generic;
using TimeServed.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace TimeServed.UI
{
    /// <summary>
    /// Represent a single menu to display when selected
    /// </summary>
    internal class Menu
    {
        private Image _backgroundImage;
        private List<Image> _imagesToDisplay;
        private List<Button> _buttonsToDisplay;

        /// <summary>
        /// Get the text to display
        /// </summary>
        public List<Text> TextToDisplay { get; private set; }

        /// <summary>
        /// Create a menu with all images, buttons, and text
        /// </summary>
        /// <param name="backgroundImage">Image to render behind all others</param>
        /// <param name="images">Image to display</param>
        /// <param name="buttons">Buttons to render</param>
        /// <param name="text">Text to display</param>
        public Menu(Image backgroundImage, List<Image> images, List<Button> buttons, List<Text> text)
        {
            _backgroundImage = backgroundImage;
            _imagesToDisplay = images;
            _buttonsToDisplay = buttons;
            TextToDisplay = text;
        }

        /// <summary>
        /// Create a menu with just a background image
        /// </summary>
        /// <param name="backgroundImage">Image to render behind all others</param>
        public Menu(Image backgroundImage) : this(backgroundImage, new List<Image> { }, new List<Button> { }, new List<Text> { }) { }

        /// <summary>
        /// Create a menu with just a background image and list of images to display
        /// </summary>
        /// <param name="backgroundImage">Image to render behind all others</param>
        /// <param name="images">Image to display</param>
        public Menu(Image backgroundImage, List<Image> images) : this(backgroundImage, images, new List<Button> { }, new List<Text> { }) { }

        /// <summary>
        /// Create a menu with a background image, list of images, and buttons
        /// </summary>
        /// <param name="backgroundImage">Image to render behind all others</param>
        /// <param name="images">Image to display</param>
        /// <param name="buttons">Buttons to render</param>
        public Menu(Image backgroundImage, List<Image> images, List<Button> buttons) : this(backgroundImage, images, buttons, new List<Text> { }) { }

        /// <summary>
        /// Create a menu with a background image, list of images, and text
        /// </summary>
        /// <param name="backgroundImage">Image to render behind all others</param>
        /// <param name="images">Image to display</param>
        /// <param name="text">Text to display</param>
        public Menu(Image backgroundImage, List<Image> images, List<Text> text) : this(backgroundImage, images, new List<Button> { }, text) { }

        /// <summary>
        /// Create a menu with a background image, list of buttons, and text
        /// </summary>
        /// <param name="backgroundImage">Image to render behind all others</param>
        /// <param name="buttons">Buttons to render</param>
        /// <param name="text">Text to display</param>
        public Menu(Image backgroundImage, List<Button> buttons, List<Text> text) : this(backgroundImage, new List<Image> { }, buttons, text) { }

        /// <summary>
        /// Create a menu with a background image and a list of buttons
        /// </summary>
        /// <param name="backgroundImage">Image to render behind all others</param>
        /// <param name="buttons">Buttons to render</param>
        public Menu(Image backgroundImage, List<Button> buttons) : this(backgroundImage, new List<Image> { }, buttons, new List<Text> { }) { }


        /// <summary>
        /// Update the buttons on the screen
        /// </summary>
        public void Update()
        {
            foreach (Button button in _buttonsToDisplay)
            {
                if (ReplayManager.PlayingReplay)
                    button.ReplayUpdate();
                button.Update();
            }
        }

        /// <summary>
        /// Draw this menu
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with</param>
        public void Draw(SpriteBatch sb)
        {
            _backgroundImage?.Draw(sb);
            foreach (Image image in _imagesToDisplay)
                image.Draw(sb);
            foreach (Button button in _buttonsToDisplay)
                button.Draw(sb);
            foreach (Text text in TextToDisplay)
                text.Draw(sb);
        }
    }
}