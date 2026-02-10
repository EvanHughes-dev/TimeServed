using System.Collections.Generic;
using TimeServed.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using TimeServed.Map;
namespace TimeServed.Managers
{
    /// <summary>
    /// Called to update the game state in Game1
    /// </summary>
    /// <param name="gameState">New game state</param>
    delegate void GameStateChange(GameState gameState);

    /// <summary>
    /// Called to exit the game in Game1
    /// </summary>
    delegate void ExitGame();

    /// <summary>
    /// Called to change the replay speed in Game1
    /// </summary>
    /// <param name="change">Change to value</param>
    /// <returns>New replay speed</returns>
    delegate int ReplaySpeedChange(int change);

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

        /// <summary>
        /// Index of the font to use
        /// </summary>
        public static int FontIndex { get; private set; }

        /// <summary>
        /// Called to update the game state in Gam1
        /// </summary>
        public static event GameStateChange GameStateChange;

        /// <summary>
        /// Called to exit the game
        /// </summary>
        public static event ExitGame ExitGame;

        /// <summary>
        /// Called to change the replay speed in Game1
        /// </summary>
        public static event ReplaySpeedChange ReplaySpeedChange;

        private static Menu _mainMenu;
        private static Menu _pauseMenu;
        private static Menu _replayMenu;

        private static UserMouse _mouse;
        private static int _replaySpeed;

        private static readonly Point BaseScreenSize = new Point(640, 360);

        /// <summary>
        /// Initialize the scaled values
        /// </summary>
        /// <param name="screenSize">Size of the screen</param>
        public static void InitializeScaling(Point screenSize)
        {
            _scaleFactorX = screenSize.X / (float)BaseScreenSize.X;
            _scaleFactorY = screenSize.Y / (float)BaseScreenSize.Y;
            _replaySpeed = 2;
        }

        /// <summary>
        /// Initialize all needed menus for the game. Base screen size is 640x360 pixels. All menus are generated when the
        /// game starts and displayed only when their mode is activated    
        /// </summary>
        /// <param name="screenSize">Size of the screen</param>
        public static void InitializeMenus(Point screenSize)
        {

            _mouse = new UserMouse(screenSize.X / 64);

            Point buttonSize = ScalePoint(new Point(100, 25));
            FontIndex = ((int)ScaleFactorX) > AssetManager.ArialFonts.Length ? AssetManager.ArialFonts.Length - 1 : (int)ScaleFactorX;
            SpriteFont font = AssetManager.ArialFonts[FontIndex];

            int buttonSpacing = buttonSize.Y / 4;

            {
                int numberOfButtons = 3;// used to center on the screen

                Point drawPoint = FindFirstPoint(buttonSize, screenSize, numberOfButtons, buttonSpacing);

                List<Button> buttons = new List<Button> { };
                Point menuSize = ScalePointUniform(new Point(AssetManager.MenuButtons[3].Width * 4, AssetManager.MenuButtons[3].Height * 4));
                Image title = new Image(new Point(screenSize.X/2- menuSize.X/2, screenSize.Y/5),
                    menuSize, AssetManager.MenuButtons[3]);

                Rectangle menuPlayRect = new Rectangle(drawPoint, buttonSize);
                Button menuPlay = new Button(menuPlayRect, AssetManager.MenuButtons[0], AssetManager.MenuButtons[0], true);
                menuPlay.OnClick += GameplayManager.ClearSavedData;
                menuPlay.OnClick += ReplayManager.ClearSavedData;
                menuPlay.OnClick += GameplayManager.NextLevel;
                menuPlay.OnClick += () => GameStateChange.Invoke(GameState.Level);
                menuPlay.OnClick += () => CurrentMenu = MenuModes.Level;
                buttons.Add(menuPlay);

                Rectangle menuCheckPointRect = new Rectangle(IncrementScreenPos(drawPoint, 1, buttonSize.Y, buttonSpacing), buttonSize);
                Button menuCheckPoint = new Button(menuCheckPointRect, AssetManager.MenuButtons[1], AssetManager.MenuButtons[1], true);
                menuCheckPoint.OnClick += () =>
                {
                    if (TriggerManager.LoadCheckpoint())
                    {
                        CurrentMenu = MenuModes.Level;
                        GameStateChange.Invoke(GameState.Level);
                    }

                };
                buttons.Add(menuCheckPoint);


                Rectangle menuQuitRect = new Rectangle(IncrementScreenPos(drawPoint, 2, buttonSize.Y, buttonSpacing), buttonSize);
                Button menuQuit = new Button(menuQuitRect, AssetManager.MenuButtons[2], AssetManager.MenuButtons[2], true);
                menuQuit.OnClick += () => ExitGame.Invoke();
                buttons.Add(menuQuit);

                _mainMenu = new Menu(null, new List<Image> { title }, buttons );
            }
            {
                int numberOfButtons = 2;
                Point drawPoint = FindFirstPoint(buttonSize, screenSize, numberOfButtons, buttonSpacing);

                Rectangle pauseContinueRect = new Rectangle(drawPoint, buttonSize);
                Button pauseContinue = new Button(pauseContinueRect, AssetManager.MenuButtons[0], AssetManager.MenuButtons[0], true);
                pauseContinue.OnClick += () => GameStateChange.Invoke(GameState.Level);
                pauseContinue.OnClick += () => CurrentMenu = MenuModes.Level;

                Rectangle pauseQuitRect = new Rectangle(IncrementScreenPos(drawPoint, 1, buttonSize.Y, buttonSpacing), buttonSize);
                Button pauseQuit = new Button(pauseQuitRect, AssetManager.MenuButtons[2], AssetManager.MenuButtons[2], true);
                pauseQuit.OnClick += () => GameStateChange.Invoke(GameState.Menu);
                pauseQuit.OnClick += () => CurrentMenu = MenuModes.MainMenu;

                List<Button> buttons = new List<Button>
                {
                    pauseContinue,
                    pauseQuit
                };

                _pauseMenu = new Menu(null, buttons);
            }
            {
                int numberOfButtons = 1;
                Point drawPoint = FindFirstPoint(buttonSize, screenSize + new Point(0, MapUtils.ScreenCenter.Y), numberOfButtons, buttonSpacing);

                Rectangle replayQuitRect = new Rectangle(IncrementScreenPos(drawPoint, 1, buttonSize.Y, buttonSpacing), buttonSize);
                Button replayQuit = new Button(replayQuitRect, AssetManager.MenuButtons[2], AssetManager.MenuButtons[2], true);
                replayQuit.OnClick += () => GameStateChange.Invoke(GameState.Menu);


                Point bottomRightStartPoint = new Point(9 * screenSize.X / 10, 9 * screenSize.Y / 10);
                Point replayButtonSize = ScalePointUniform(new Point(10, 10));
                // Display the change in replay speed in the bottom right
                // Decrease button left, current speed center, increase button right
                Rectangle replaySpeedDecreaseRect = new Rectangle(bottomRightStartPoint, replayButtonSize);
                Button replaySpeedDecrease = new Button(replaySpeedDecreaseRect, AssetManager.MenuButtons[5], AssetManager.MenuButtons[5], true);
                replaySpeedDecrease.OnClick += () => _replaySpeed = ReplaySpeedChange.Invoke(-1);


                bottomRightStartPoint += new Point(replayButtonSize.X, 0);

                Text text = new Text(bottomRightStartPoint, _replaySpeed.ToString(), font, true);

                bottomRightStartPoint += new Point(replayButtonSize.X, 0);
                Rectangle replaySpeedIncreaseRect = new Rectangle(bottomRightStartPoint, replayButtonSize);
                Button replaySpeedIncrease = new Button(replaySpeedIncreaseRect, AssetManager.MenuButtons[4], AssetManager.MenuButtons[4], true);
                replaySpeedIncrease.OnClick += () => _replaySpeed = ReplaySpeedChange.Invoke(1);

                List<Button> buttons = new List<Button>
                {
                   replayQuit,
                   replaySpeedDecrease,
                   replaySpeedIncrease,
                };


                List<Text> texts = new List<Text>{
                    text
                };

                _replayMenu = new Menu(null, buttons, texts);
            }


        }

        /// <summary>
        /// Update the elements of the interface. For the sake of the ui, we don't need delta time
        /// </summary>
        public static void Update()
        {
            _mouse.Update();
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
                case MenuModes.ReplayMode:
                    _replayMenu.TextToDisplay[0].UpdateText(_replaySpeed.ToString());
                    _replayMenu.Update();
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
                case MenuModes.ReplayMode:
                    _replayMenu.Draw(sb);
                    break;
            }

            _mouse.Draw(sb);
        }


        /// <summary>
        /// Initialize replay mode
        /// </summary>
        public static void ReplayMode()
        {
            CurrentMenu = MenuModes.ReplayMode;
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