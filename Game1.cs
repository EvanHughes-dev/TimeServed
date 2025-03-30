using MakeEveryDayRecount.DebugModes;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.UI;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// The current overall game state
    /// </summary>
    public enum GameState
    {
        Menu,
        Pause,
        Level,
        Cutscene,
        Playback
    }

    public class Game1 : Game
    {
        private enum DebugState
        {
            None,
            Global,
            Player,
            Room
        }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Buttons
        private List<Button> pauseButtons;
        private List<Button> menuButtons;
        private Texture2D defaultButtonTexture;

        private GameState _state;

        private GameplayManager _gameplayManager;


        /// <summary>
        /// Access the current size of the screen in pixels
        /// </summary>
        public Point ScreenSize
        {
            get { return new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height); }
        }

        private DebugState _debugState;

        private BaseDebug[] _debugModes;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true; // Enable user resizing
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // Set default window size to half the screen size
            _graphics.PreferredBackBufferWidth =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            _graphics.PreferredBackBufferHeight =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            _graphics.ApplyChanges();

            _debugState = DebugState.None;

            _debugModes = new BaseDebug[2];

            //Initialize button lists
            pauseButtons = new List<Button>();
            menuButtons = new List<Button>();
            InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.MainMenu;
            InterfaceManager.gameStateChange += SwitchState;
            InterfaceManager.exitGame += ExitGame;
            //Set initial GameState
            _state = GameState.Menu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.LoadContent(Content);
            SoundManager.LoadContent(Content);

            // Initialize all items that need assets to be loaded 

            _gameplayManager = new GameplayManager(ScreenSize);

            MapUtils.Initialize(this, _gameplayManager);

            GlobalDebug.Initialize();

            _debugModes[0] = new PlayerDebug(_gameplayManager);
            _debugModes[1] = new MapDebug(_gameplayManager);

            InterfaceManager.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();

            switch (_state)
            {
                case GameState.Menu:
                    break;

                case GameState.Pause:

                    if (InputManager.GetKeyPress(Keys.Escape))
                    {
                        _state = GameState.Level;
                        SoundManager.ResumeBGM();
                        InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.Level;
                    }
                    break;

                case GameState.Level:
                    //TODO: On level end
                    //_state = GameState.Cutscene;
                    _gameplayManager.Update(gameTime);

                    if (InputManager.GetKeyPress(Keys.Escape))
                    {
                        _state = GameState.Pause;
                        SoundManager.PauseBGM();
                        InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.PauseMenu;
                    }

                    break;

                case GameState.Cutscene:
                    //When the cutscene is over
                    //_state = GameState.Level;

                    //If this is the last cutscene
                    //_state = GameState.Playback;
                    break;

                case GameState.Playback:
                    //When playback is finished
                    //_state = GameState.Menu;
                    break;

                default:
                    break;
            }

            CheckKeyboardInput();

            InterfaceManager.Update();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // "Point" sampling means that our chunky pixels won't get blurred
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            switch (_state)
            {
                case GameState.Menu:
                    // DrawMenu(_spriteBatch);
                    break;
                case GameState.Pause:
                    _gameplayManager.Draw(_spriteBatch, ScreenSize);
                    DisplayDebug();
                    break;
                case GameState.Level:
                    CheckKeyboardInput();
                    //TODO: Blur the gameplay in the background.
                    _gameplayManager.Draw(_spriteBatch, ScreenSize);
                    DisplayDebug();
                    break;
                case GameState.Cutscene:
                    break;
                case GameState.Playback:
                    break;
                default:
                    break;
            }

            InterfaceManager.Draw(_spriteBatch);

            //End the sprite batch
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw the current debug display to the screen
        /// </summary>
        private void DisplayDebug()
        {
            switch (_debugState)
            {
                case DebugState.Global:
                    GlobalDebug.Draw(_spriteBatch);
                    break;
                case DebugState.Player:
                    _debugModes[0].Draw(_spriteBatch);
                    break;
                case DebugState.Room:
                    _debugModes[1].Draw(_spriteBatch);
                    break;
            }
        }

        /// <summary>
        /// Check for user input that affects the game state or debug
        /// modes of the overall game
        /// </summary>
        private void CheckKeyboardInput()
        {
            // Use F1-F4 to control the Debug Modes
            // We don't need to check if the function
            // key is pressed or if the computer is in
            // function lock since they have separate
            // key codes and are sent to the OS separately
            if (InputManager.GetKeyStatus(Keys.F1))
                _debugState = DebugState.None;
            else if (InputManager.GetKeyStatus(Keys.F2))
                _debugState = DebugState.Global;
            else if (InputManager.GetKeyStatus(Keys.F3))
                _debugState = DebugState.Player;
            else if (InputManager.GetKeyStatus(Keys.F4))
                _debugState = DebugState.Room;
        }

        /// <summary>
        /// Exit the game when button is clicked
        /// </summary>
        private void ExitGame()
        {
            Exit();
        }

        /// <summary>
        /// Creates an Action delegate to set the state to the given GameState. DOES NOT CHANGE THE STATE ITSELF!
        /// </summary>
        /// <param name="state">The state that should be applied when the delegate is called.</param>
        /// <returns>An Action that sets the game state to the provided state when the ACTION is called.</returns>
        public void SwitchState(GameState state)
        {
            if (state == GameState.Level)
            {
                if (SoundManager.PlayingMusic)
                    SoundManager.ResumeBGM();
                else
                    SoundManager.PlayBGM(_gameplayManager.Level);
            }
            _state = state;
        }
    }
}
