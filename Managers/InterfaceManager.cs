using System.Collections.Generic;
using MakeEveryDayRecount.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace MakeEveryDayRecount.Managers
{
    delegate void GameStateChange(GameState gameState);
    delegate void HoverChange(UserMouse.MouseHover newHover);


    /// <summary>
    /// A manager that controls all UI features and creates all UI screen
    /// </summary>
    internal static class InterfaceManager
    {
        public enum MenuModes
        {
            None,
            MainMenu,
            CheckpointMenu,
            Level,
            PauseMenu,
            SettingsMenu,
        }

        public static MenuModes CurrentMenu { get; set; }

        private static Menu _mainMenu;
        private static Menu _pauseMenu;
        private static Menu _levelMenu;
        public static event GameStateChange gameStateChange;
        private static UserMouse _mouse;

        public static void Initialize()
        {

            _mouse = new UserMouse();
            Rectangle menuPlayRect = new Rectangle(400, 200, 300, 100);
            Button menuPlay = new Button(menuPlayRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true);
            menuPlay.OnClick += GameStateChange(GameState.Level);
            menuPlay.OnClick += MenuChange(MenuModes.Level);
            List<Button> buttons = new List<Button>
            {
                menuPlay
            };

            foreach (Button btn in buttons)
                btn.OnHoverChange += HoverModeChange;
            _mainMenu = new Menu(null, buttons);
        }

        /// <summary>
        /// Update the elements of the interface
        /// </summary>
        public static void Update()
        {
            switch (CurrentMenu)
            {
                case MenuModes.None:
                    break;
                case MenuModes.MainMenu:
                    _mainMenu.Update();
                    break;
                case MenuModes.CheckpointMenu:
                    break;
                case MenuModes.PauseMenu:
                    _pauseMenu.Update();
                    break;
                case MenuModes.SettingsMenu:
                    break;
            }
        }

        /// <summary>
        /// Draw any needed UI elements to the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw with </param>
        public static void Draw(SpriteBatch sb)
        {

            switch (CurrentMenu)
            {
                case MenuModes.None:
                    break;
                case MenuModes.MainMenu:
                    _mainMenu.Draw(sb);
                    break;
                case MenuModes.CheckpointMenu:
                    break;
                case MenuModes.PauseMenu:
                    _pauseMenu.Draw(sb);
                    break;
                case MenuModes.SettingsMenu:
                    break;
            }

            _mouse.Draw(sb);
        }

        /// <summary>
        /// Update the game state from a button
        /// </summary>
        /// <param name="gameState">New game state</param>
        /// <returns>Action to update state</returns>
        private static Action GameStateChange(GameState gameState)
        {
            return () =>
            {
                gameStateChange?.Invoke(gameState);
            };
        }

        /// <summary>
        /// Update the menu currently selected
        /// </summary>
        /// <param name="mode">New menu mode</param>
        /// <returns>Action to update</returns>
        private static Action MenuChange(MenuModes mode)
        {
            return () =>
            {
                CurrentMenu = mode;
            };
        }

        /// <summary>
        /// Update the hover state of the mouse
        /// </summary>
        /// <param name="newState">New hover state of the mouse</param>
        private static void HoverModeChange(UserMouse.MouseHover newState)
        {
            _mouse.UpdateHoverState(newState);
        }
    }

}