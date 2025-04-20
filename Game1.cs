using MakeEveryDayRecount.DebugModes;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.UI;
using MakeEveryDayRecount.GameObjects.Triggers;

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

        private GameState _state;


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
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // Set default window size to half the screen size
            _graphics.PreferredBackBufferWidth =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //_graphics.PreferredBackBufferWidth = 640;
            //_graphics.PreferredBackBufferHeight = 360;

            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = true; 
            _graphics.ApplyChanges();

            _debugState = DebugState.None;

            _debugModes = new BaseDebug[2];

            //Initialize button lists
            InterfaceManager.InitializeScaling(ScreenSize);
            InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.MainMenu;
            InterfaceManager.gameStateChange += SwitchState;
            InterfaceManager.exitGame += ExitGame;

            ReplayManager.Initialize();
            TriggerManager.Initialize();
            //Set initial GameState
            _state = GameState.Menu;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.LoadContent(Content);
            SoundManager.LoadContent(Content);

            GlobalDebug.Initialize();
            GameplayManager.Initialize(ScreenSize);
            // Initialize all items that need assets to be loaded 
            InterfaceManager.InitializeMenus(ScreenSize);

            MapUtils.Initialize(this);

            

            _debugModes[0] = new PlayerDebug();
            _debugModes[1] = new MapDebug();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_state != GameState.Playback)
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

                    if (InputManager.GetKeyPress(Keys.Escape))
                    {
                        _state = GameState.Pause;
                        SoundManager.PauseBGM();
                        InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.PauseMenu;
                        // Don't call anything after the game has paused
                        break;
                    }
                    GameplayManager.Update(gameTime);
                    // Save the current state of the keyboard
                    ReplayManager.SaveState();
                    if (InputManager.GetKeyPress(Keys.Tab))
                    {
                        ReplayManager.SaveData(1, 1);
                        _state = GameState.Playback;
                    }

                    //DELETE THIS
                    //"kill" the player
                    if (InputManager.GetKeyPress(Keys.P))
                    {
                        GameplayManager.PlayerObject.Detected();
                    }

                    break;

                case GameState.Cutscene:
                    //When the cutscene is over
                    //_state = GameState.Level;

                    //If this is the last cutscene
                    //_state = GameState.Playback;
                    break;

                case GameState.Playback:
                    // If the replay has not been started yet, start the replay
                    // This will auto assign the first frame of input and allow the
                    // input manager to work. For rest of the frames, call the next 
                    // frame of the ReplayManager and check if you have read all frames
                    if (!ReplayManager.PlayingReplay)
                    {
                        //TODO: Is this needed?
                        //ReplayManager.BeginReplay()
                         GameplayManager.ReplayMode();
                    }
                    else if (!ReplayManager.NextFrame())
                    {
                        _state = GameState.Menu;
                        ReplayManager.EndReplay();
                        InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.MainMenu;

                    }

                    InputManager.ReplayUpdate();
                    GameplayManager.Update(gameTime);

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
                case GameState.Level:
                    GameplayManager.Draw(_spriteBatch);
                    DisplayDebug();
                    break;
                case GameState.Pause:
                    //TODO: Blur the gameplay in the background.
                    GameplayManager.Draw(_spriteBatch);
                    DisplayDebug();
                    break;
                case GameState.Cutscene:
                    break;
                case GameState.Playback:
                    GameplayManager.Draw(_spriteBatch);
                    DisplayDebug();
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
        public void SwitchState(GameState state)
        {
            if (state == GameState.Level)
            {
                if (SoundManager.PlayingMusic)
                    SoundManager.ResumeBGM();
                else
                    SoundManager.PlayBGM(GameplayManager.Level);
            }
            _state = state;
        }
    }
}
