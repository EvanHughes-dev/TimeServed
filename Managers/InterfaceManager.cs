using System.Collections.Generic;
using MakeEveryDayRecount.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using MakeEveryDayRecount.GameObjects.Triggers;

namespace MakeEveryDayRecount.Managers
{
    delegate void GameStateChange(GameState gameState);
    delegate void ExitGame();
    /// <summary>
    /// A manager that controls all UI features and creates all UI screen. All menus 
    /// are designed for a screen size of 640x360. Display this size, uncomment the two lines
    /// in Game1.Initialize. To make screen scaling easy, do all location calculating relative
    /// to the screen size, not just using a value
    /// </summary>
    internal static class InterfaceManager
    {
        /// <summary>
        /// The menu options
        /// </summary>
        public enum MenuModes
        {
            None,
            MainMenu,
            Level,
            PauseMenu,
            ReplayMode
        }

        /// <summary>
        /// Get and set the current menu mode
        /// </summary>
        public static MenuModes CurrentMenu { get; set; }

        private static float _scaleFactorX;
        private static float _scaleFactorY;

        /// <summary>
        /// Get the scale factor on the x axis
        /// </summary>
        public static float ScaleFactorX { get => _scaleFactorX; }
        /// <summary>
        /// Get the scale factor on the y axis
        /// </summary>
        public static float ScaleFactorY { get => _scaleFactorY; }

        private static Menu _mainMenu;
        private static Menu _pauseMenu;
        private static Menu _levelMenu;
        public static event GameStateChange gameStateChange;
        public static event ExitGame exitGame;
        private static UserMouse _mouse;

        private static readonly Point BaseScreenSize = new Point(640, 360);

        /// <summary>
        /// Initialize the scaled values
        /// </summary>
        /// <param name="screenSize">Size of the screen</param>
        public static void InitializeScaling(Point screenSize)
        {
            _scaleFactorX = screenSize.X / (float)BaseScreenSize.X;
            _scaleFactorY = screenSize.Y / (float)BaseScreenSize.Y;
        }

        /// <summary>
        /// Initialize all needed menus for the game
        /// </summary>
        /// <param name="screenSize">Size of the screen</param>
        public static void InitializeMenus(Point screenSize)
        {

            _mouse = new UserMouse(screenSize.X / 64);

            Point buttonSize = ScalePoint(new Point(100, 25));
            int fontIndex = ((int)ScaleFactorX) > AssetManager.ArialFonts.Length ? AssetManager.ArialFonts.Length - 1 : (int)ScaleFactorX;
            SpriteFont font = AssetManager.ArialFonts[fontIndex];

            int buttonSpacing = buttonSize.Y / 4;

            {
                int numberOfButtons = 3;// used to center on the screen

                Point drawPoint = FindFirstPoint(buttonSize, screenSize, numberOfButtons, buttonSpacing);

                List<Button> buttons = new List<Button> { };

                Rectangle menuPlayRect = new Rectangle(drawPoint, buttonSize);
                Button menuPlay = new Button(menuPlayRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Play", font);
                menuPlay.OnClick += GameplayManager.ClearSavedData;
                menuPlay.OnClick += GameStateChange(GameState.Level);
                menuPlay.OnClick += MenuChange(MenuModes.Level);
                buttons.Add(menuPlay);


                Rectangle menuCheckPointRect = new Rectangle(IncrementScreenPos(drawPoint, 1, buttonSize.Y, buttonSpacing), buttonSize);
                Button menuCheckPoint = new Button(menuCheckPointRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Checkpoints", font);
                menuPlay.OnClick += TriggerManager.LoadCheckpoint;
                buttons.Add(menuCheckPoint);


                Rectangle menuQuitRect = new Rectangle(IncrementScreenPos(drawPoint, 2, buttonSize.Y, buttonSpacing), buttonSize);
                Button menuQuit = new Button(menuQuitRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Exit", font);
                menuQuit.OnClick += ExitGame();
                buttons.Add(menuQuit);

                _mainMenu = new Menu(null, buttons);
            }
            {
                int numberOfButtons = 2;
                Point drawPoint = FindFirstPoint(buttonSize, screenSize, numberOfButtons, buttonSpacing);

                Rectangle pauseContinueRect = new Rectangle(drawPoint, buttonSize);
                Button pauseContinue = new Button(pauseContinueRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Resume", font);
                pauseContinue.OnClick += GameStateChange(GameState.Level);
                pauseContinue.OnClick += MenuChange(MenuModes.Level);

                Rectangle pauseQuitRect = new Rectangle(IncrementScreenPos(drawPoint, 1, buttonSize.Y, buttonSpacing), buttonSize);
                Button pauseQuit = new Button(pauseQuitRect, AssetManager.DefaultButton, AssetManager.DefaultButton, true, "Exit", font);
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
                case MenuModes.PauseMenu:
                    _pauseMenu.Update();
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
                case MenuModes.PauseMenu:
                    _pauseMenu.Draw(sb);
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


        /// <summary>
        /// Increment a button position
        /// </summary>
        /// <param name="startingPoint">Starting position</param>
        /// <param name="offsetNumber">Number offset in the list</param>
        /// <param name="buttonHeight">Height of a button</param>
        /// <param name="offset">Additional offset value</param>
        /// <returns>Point with value updated</returns>
        private static Point IncrementScreenPos(Point startingPoint, int offsetNumber, int buttonHeight, int offset)
        {
            return new Point(startingPoint.X, startingPoint.Y + offsetNumber * (buttonHeight + offset));
        }

        /// <summary>
        /// Find the first point to draw at
        /// </summary>
        /// <param name="buttonSize">Size of a button</param>
        /// <param name="screenSize">Size of the screen</param>
        /// <param name="buttonCount">Number of buttons</param>
        /// <param name="buttonSpacing">Number of pixels between buttons</param>
        /// <returns>Point to draw the first button</returns>
        private static Point FindFirstPoint(Point buttonSize, Point screenSize, int buttonCount, int buttonSpacing)
        {
            return new Point(screenSize.X / 2 - buttonSize.X / 2, screenSize.Y / 2 - buttonCount * (buttonSize.Y / 2) - buttonSpacing * buttonCount % 2);
        }

        /// <summary>
        /// Scale to a new size based on the screen 
        /// </summary>
        /// <param name="point">Point to scale</param>
        /// <returns>Scaled point</returns>
        public static Point ScalePoint(Point point)
        {
            return new Point((int)(point.X * ScaleFactorX), (int)(point.Y * ScaleFactorY));
        }

        /// <summary>
        /// Get the uniform scaling float value
        /// </summary>
        /// <returns>Uniform scale value</returns>
        private static float UniformScale()
        {
            return Math.Min(ScaleFactorX, ScaleFactorY);
        }

        /// <summary>
        /// Get a uniformly scaled point
        /// </summary>
        /// <param name="point">Point to scale</param>
        /// <returns>Uniformly scaled point</returns>
        public static Point ScalePointUniform(Point point)
        {
            float scale = UniformScale();
            return new Point((int)(point.X * scale), (int)(point.Y * scale));
        }

    }
}