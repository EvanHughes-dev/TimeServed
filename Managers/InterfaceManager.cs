using System.Collections.Generic;
using MakeEveryDayRecount.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace MakeEveryDayRecount.Managers
{
    delegate void GameStateChange(GameState gameState);
    delegate void ExitGame();
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

        /// <summary>
        /// Get and set the current menu mode
        /// </summary>
        public static MenuModes CurrentMenu { get; set; }

        private static Menu _mainMenu;
        private static Menu _pauseMenu;
        private static Menu _levelMenu;
        public static event GameStateChange gameStateChange;
        public static event ExitGame exitGame;
        private static UserMouse _mouse;


        /// <summary>
        /// Initialize all needed menus for the game
        /// </summary>
        public static void Initialize()
        {

            _mouse = new UserMouse();

            {
                Rectangle menuPlayRect = new Rectangle(400, 200, 300, 100);
                Button menuPlay = new Button(menuPlayRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Play", AssetManager.Arial20);
                menuPlay.OnClick += GameStateChange(GameState.Level);
                menuPlay.OnClick += MenuChange(MenuModes.Level);

                Rectangle menuQuitRect = new Rectangle(400, 340, 100, 40);
                Button menuQuit = new Button(menuQuitRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Exit", AssetManager.Arial20);
                menuQuit.OnClick += ExitGame();

                List<Button> buttons = new List<Button>
                {
                    menuPlay,
                    menuQuit
                };

                _mainMenu = new Menu(null, buttons);
            }
            {
                Rectangle pauseContinueRect = new Rectangle(200, 200, 400, 100);
                Button pauseContinue = new Button(pauseContinueRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Resume", AssetManager.Arial20);
                pauseContinue.OnClick += GameStateChange(GameState.Level);
                pauseContinue.OnClick += MenuChange(MenuModes.Level);

                Rectangle pauseQuitRect = new Rectangle(200, 440, 400, 100);
                Button pauseQuit = new Button(pauseQuitRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Exit", AssetManager.Arial20);
                pauseQuit.OnClick += GameStateChange(GameState.Menu);
                pauseQuit.OnClick += MenuChange(MenuModes.MainMenu);

                List<Button> buttons = new List<Button>
                {
                    pauseContinue,
                    pauseQuit
                };

                _pauseMenu = new Menu(null, buttons);
            }


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
                case MenuModes.Level:
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
                case MenuModes.Level:
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
        /// Called when an exit game button is pressed
        /// </summary>
        /// <returns>Action to exit game</returns>
        private static Action ExitGame()
        {
            return () =>
            {
                exitGame?.Invoke();
            };
        }

        /// <summary>
        /// Update the hover state of the mouse
        /// </summary>
        /// <param name="newState">New hover state of the mouse</param>
        public static void HoverModeChange(UserMouse.MouseHover newState)
        {
            _mouse.UpdateHoverState(newState);
        }
    }

}