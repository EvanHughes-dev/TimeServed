using System.Diagnostics;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GameplayManager _gameplayManager;

        public Point ScreenSize
        {
            get
            {
                return new Point(
                    GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height             
                );
            }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true; // Enable user resizing
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetManager.LoadContent(Content);
            // Gameplay manager must be called after all content is loaded
            //TOFO figure out a new way to access screen size
            _gameplayManager = new GameplayManager();
            MapUtils.Initialize(this, _gameplayManager);
        }

        protected override void Update(GameTime gameTime)
        {
            if (
                GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)
            )
                Exit();

            InputManager.Update();
            _gameplayManager.Update(gameTime);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            System.Diagnostics.Debug.WriteLine(GraphicsDevice.Viewport.Width);

            System.Diagnostics.Debug.WriteLine(ScreenSize);
            _spriteBatch.Begin();
            _gameplayManager.Draw(_spriteBatch);
            // TODO: Add your drawing code here
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void CheckKeyboardInput() { }

        private void DisplayPauseMenu(SpriteBatch sb) { }

        private void DisplayMainMenu(SpriteBatch sb) { }


    }
}
