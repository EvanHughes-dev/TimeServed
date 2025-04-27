
/********************************************************\
 * Leah Crain, Avigail Daniels, James Young, Evan Hughes
 * 
 * 4/27/2025
 * 
 * Create a game titled "Time Served" in which the player
 * plays as a prisoner trying to escape from prison. 
 * The player goes backwards through the days, gathering
 * the tools and items they had in the previous level, 
 * finally watching their fall escape in a replay
\********************************************************/

using MakeEveryDayRecount.DebugModes;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MakeEveryDayRecount.Managers;

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

        private int _replaySpeed;
        private const int MaxReplaySpeed = 5;
        private int _currentAnimationIndex;
        private const float SecondsPerAnimationFrame = .2f;
        private float _currentElapsedTimeAnimation;
        private Texture2D[] _currentAnimation;


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
            InterfaceManager.GameStateChange += SwitchState;
            InterfaceManager.ExitGame += ExitGame;
            InterfaceManager.ReplaySpeedChange += ChangeReplaySpeed;

            ReplayManager.Initialize();
            TriggerManager.Initialize();
            //Set initial GameState
            _state = GameState.Menu;
            _replaySpeed = 2;

            _currentAnimation = null;
            _currentAnimationIndex = 0;
            _currentElapsedTimeAnimation = 0f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            AssetManager.LoadContent(Content);
            SoundManager.LoadContent(Content);

            GlobalDebug.Initialize();
            GameplayManager.Initialize(ScreenSize);
            GameplayManager.WinCondition += () => SwitchState(GameState.Cutscene);

            MapUtils.Initialize(this);

            // Initialize all items that need assets to be loaded 
            InterfaceManager.InitializeMenus(ScreenSize);

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
                    if (IsMouseVisible)
                        IsMouseVisible = false;
                    break;

                case GameState.Pause:

                    if (InputManager.GetKeyPress(Keys.Escape))
                    {
                        _state = GameState.Level;
                        InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.Level;
                    }
                    break;

                case GameState.Level:
                    ReplayManager.SaveState((float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (InputManager.GetKeyPress(Keys.Escape))
                    {
                        _state = GameState.Pause;
                        InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.PauseMenu;
                        // Don't call anything after the game has paused
                        break;
                    }
                    GameplayManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                    // Save the current state of the keyboard

                    break;

                case GameState.Cutscene:
                    _currentElapsedTimeAnimation += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_currentElapsedTimeAnimation >= SecondsPerAnimationFrame)
                    {
                        _currentAnimationIndex++;
                        _currentElapsedTimeAnimation -= SecondsPerAnimationFrame;
                        if (_currentAnimationIndex >= _currentAnimation.Length)
                        {
                            GameplayManager.NextLevel();
                            SwitchState(GameState.Level);
                        }
                    }
                    break;

                case GameState.Playback:
                    // If the replay has not been started yet, start the replay
                    // This will auto assign the first frame of input and allow the
                    // input manager to work. For rest of the frames, call the next 
                    // frame of the ReplayManager and check if you have read all frames
                    for (int i = 0; i < _replaySpeed; i++)
                    {
                        if (!ReplayManager.PlayingReplay)
                        {
                            ReplayManager.BeginReplay();
                            GameplayManager.ReplayMode();
                            InterfaceManager.ReplayMode();
                            IsMouseVisible = true;
                        }
                        else if (!ReplayManager.NextFrame())
                        {
                            if (GameplayManager.Level == 1)
                            {
                                EndReplay();
                                break;
                            }
                        }

                        InputManager.ReplayUpdate();
                        GameplayManager.Update(ReplayManager.CurrentReplayState.DeltaTime);
                    }

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
                    //If statement in order to display tutorial text
                    if (GameplayManager.Level == 1)
                    {
                        if (MapManager.CurrentRoom.RoomName == "JRoom0")
                        {
                            //Press E or Mouse to interact, use Moue to select items from inventory
                            _spriteBatch.DrawString(AssetManager.Arial20, "Press E or click to Interact.", new Point(ScreenSize.X / 5, ScreenSize.Y / 4).ToVector2(), Color.White);
                            _spriteBatch.DrawString(AssetManager.Arial20, "Use the mouse to select items \nfrom the inventory.", new Point(ScreenSize.X / 5, ScreenSize.Y / 2).ToVector2(), Color.White);
                        }
                    }
                    break;
                case GameState.Pause:
                    GameplayManager.Draw(_spriteBatch);
                    DisplayDebug();
                    break;
                case GameState.Cutscene:
                    _spriteBatch.Draw(_currentAnimation[_currentAnimationIndex], new Rectangle(Point.Zero, ScreenSize), Color.White);
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

            if (_state == GameState.Playback && state == GameState.Menu)
            {
                EndReplay();
            }
            else if (state == GameState.Menu)
            {
                GameplayManager.Initialize(ScreenSize);

            }

            if (state == GameState.Cutscene)
            {
                if (AssetManager.LevelChanges.Length <= GameplayManager.Level)
                {
                    state = GameState.Playback;
                }
                else
                {
                    _currentElapsedTimeAnimation = 0f;
                    _currentAnimationIndex = 0;
                    _currentAnimation = AssetManager.LevelChanges[GameplayManager.Level - 1];
                }
            }
            _state = state;
        }

        /// <summary>
        /// Change the speed of the replay
        /// </summary>
        /// <param name="changeInSpeed">Value to change the replay by</param>
        /// <returns>Speed that has been changed to</returns>
        public int ChangeReplaySpeed(int changeInSpeed)
        {
            _replaySpeed += changeInSpeed;
            if (_replaySpeed < 0)
                _replaySpeed = 0;
            else if (_replaySpeed > MaxReplaySpeed)
                _replaySpeed = MaxReplaySpeed;

            return _replaySpeed;
        }

        public void EndReplay()
        {
            _state = GameState.Menu;
            ReplayManager.ClearSavedData();
            GameplayManager.ClearSavedData();
            MapManager.ClearMap();
            InterfaceManager.CurrentMenu = InterfaceManager.MenuModes.MainMenu;
            ReplayManager.EndReplay();

            GameplayManager.Initialize(ScreenSize);
        }
    }
}
