using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MakeEveryDayRecount.Map;
using MakeEveryDayRecount.Debug;


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

        private GameplayManager _gameplayManager;

        private readonly Point _screenSize = new Point(1280, 1152);

        private DebugState _debugState;

        private BaseDebug[] _debugModes;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _debugState = DebugState.None;
            DebugMode.Initialize();
            _debugModes = new BaseDebug[2];
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = (int)_screenSize.X;
            _graphics.PreferredBackBufferHeight = (int)_screenSize.Y;
            _graphics.ApplyChanges();
            MapUtils.ScreenSize = _screenSize;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetManager.LoadContent(Content);
            SpriteFont sf = Content.Load<SpriteFont>("Arial20");
            DebugMode.SetFont(sf);

            // Gameplay manager must be called after all content is loaded
            _gameplayManager = new GameplayManager();
            _debugModes[0] = new PlayerDebug(sf, _gameplayManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)
            )
                Exit();

            CheckKeyboardInput();

            InputManager.Update();
            _gameplayManager.Update(gameTime);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            _gameplayManager.Draw(_spriteBatch);

            switch (_debugState)
            {
                case DebugState.Global:
                    DebugMode.Draw(_spriteBatch);
                    break;
                case DebugState.Player:
                    _debugModes[0].Draw(_spriteBatch);
                    break;
                case DebugState.Room:
                    break;
            }

            // TODO: Add your drawing code here
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Check for user input that effect the game state or debug
        /// modes of the overall game
        /// </summary>
        private void CheckKeyboardInput() {

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

        private void DisplayPauseMenu(SpriteBatch sb) { }

        private void DisplayMainMenu(SpriteBatch sb) { }
    }
}
