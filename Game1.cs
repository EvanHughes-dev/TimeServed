using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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

        //Buttons
        private List<Button> pauseButtons;
        private List<Button> menuButtons;
        private Texture2D defaultButtonTexture;

        private GameState _state;

        private GameplayManager _gameplayManager;

        /// <summary>
        /// Access the current size of the screen on pixels
        /// </summary>
        public Point ScreenSize
        {
            get { return new Point(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height); }
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
            // Set default window size to half the screen size
            _graphics.PreferredBackBufferWidth =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;
            _graphics.PreferredBackBufferHeight =
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;
            _graphics.ApplyChanges();

            //Initialize button lists
            pauseButtons = new List<Button>();
            menuButtons = new List<Button>();

            //Set initial GameState
            _state = GameState.Menu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            AssetManager.LoadContent(Content);
            // Gameplay manager must be called after all content is loaded
            _gameplayManager = new GameplayManager();
            MapUtils.Initialize(this, _gameplayManager);

            //Load buttons
            LoadButtons();
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();

            switch (_state)
            {
                case GameState.Menu:
                    CheckButtonClicks(menuButtons);
                    break;

                case GameState.Pause:
                    CheckButtonClicks(pauseButtons);
                    break;

                case GameState.Level:
                    //On level end
                    //_state = GameState.Cutscene;
                    _gameplayManager.Update(gameTime);

                    //Pause button can change, figured escape makes the most sense
                    if (InputManager.GetKeyPress(Keys.Escape))
                        _state = GameState.Pause;
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

            InputManager.Update();
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            // TODO: Add your drawing code here
            switch (_state)
            {
                case GameState.Menu:
                    DrawMenu(_spriteBatch);
                    break;
                case GameState.Pause:
                    _gameplayManager.Draw(_spriteBatch);
                    DrawPause(_spriteBatch);
                    break;
                case GameState.Level:
                    _gameplayManager.Draw(_spriteBatch);
                    break;
                case GameState.Cutscene:
                    break;
                case GameState.Playback:
                    break;
                default:
                    break;
            }

            //End the sprite batch

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CheckKeyboardInput() { }

        private void DisplayPauseMenu(SpriteBatch sb) { }

        private void DisplayMainMenu(SpriteBatch sb) { }

        /// <summary>
        /// Creates all buttons and fills their respective lists them with them.
        /// </summary>
        private void LoadButtons()
        {
            //Load button textures
            defaultButtonTexture = Content.Load<Texture2D>("Default Button");
            //... and more, when we have them

            //Fill pause buttons list with buttons
            Rectangle pauseContinueRect = new Rectangle(200, 200, 400, 100);
            Button pauseContinue = new Button(defaultButtonTexture, defaultButtonTexture, pauseContinueRect, true);
            pauseContinue.OnClick += MakeSwitchStateAction(GameState.Level);
            pauseButtons.Add(pauseContinue);

            Rectangle pauseLastCheckpointRect = new Rectangle(200, 320, 400, 100);
            Button pauseLastCheckpoint = new Button(defaultButtonTexture, defaultButtonTexture, pauseLastCheckpointRect, true);
            //pauseLastCheckpoint += the method that brings you back to your last checkpoint
            pauseButtons.Add(pauseLastCheckpoint);

            Rectangle pauseQuitRect = new Rectangle(200, 440, 400, 100);
            Button pauseQuit = new Button(defaultButtonTexture, defaultButtonTexture, pauseQuitRect, true);
            pauseQuit.OnClick += MakeSwitchStateAction(GameState.Menu);
            pauseButtons.Add(pauseQuit);

            //Fill menu buttons list with buttons
            Rectangle menuPlayRect = new Rectangle(400, 200, 300, 100);
            Button menuPlay = new Button(defaultButtonTexture, defaultButtonTexture, menuPlayRect, true);
            menuPlay.OnClick += MakeSwitchStateAction(GameState.Level);
            menuButtons.Add(menuPlay);

            Rectangle menuQuitRect = new Rectangle(400, 340, 100, 40);
            Button menuQuit = new Button(defaultButtonTexture, defaultButtonTexture, menuQuitRect, true);
            //menuQuit.OnClick += the method that closes the game
            menuButtons.Add(menuQuit);

        }

        private void CheckButtonClicks(List<Button> list)
        {

            for (int i = 0; i < list.Count; i++)
            {
                //Invoke the button's on click effect if it has been clicked
                list[i].Click();
            }
        }

        /// <summary>
        /// Draws the main menu.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        private void DrawMenu(SpriteBatch sb)
        {
            //Draw the buttons
            for (int i = 0; i < menuButtons.Count; i++)
            {
                menuButtons[i].Draw(sb);
            }

        }

        /// <summary>
        /// Draws the pause menu.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        private void DrawPause(SpriteBatch sb)
        {
            //Draw the buttons
            for (int i = 0; i < pauseButtons.Count; i++)
            {
                pauseButtons[i].Draw(sb);
            }
        }

        /// <summary>
        /// Draws the cutscene.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        private void DrawCutscene(SpriteBatch sb)
        {

        }

        /// <summary>
        /// Draws the playback.
        /// </summary>
        /// <param name="sb">sprite batch used to draw</param>
        private void DrawPlayback(SpriteBatch sb)
        {

        }

        /// <summary>
        /// Creates an Action delegate to set the state to the given GameState. DOES NOT CHANGE THE STATE ITSELF!
        /// </summary>
        /// <param name="state">The state that should be applied when the delegate is called.</param>
        /// <returns>An Action that sets the game state to the provided state when the ACTION is called.</returns>
        public Action MakeSwitchStateAction(GameState state)
        {
            return () =>
            {
                _state = state;
            };
        }
    }
}
