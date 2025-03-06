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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Initialize button lists
            pauseButtons = new List<Button>();
            menuButtons = new List<Button>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load buttons
            LoadButtons();


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (_state)
            {
                case GameState.Menu:
                    //If start game button is pressed
                    //_state = GameState.Level;
                    break;

                case GameState.Pause:
                    //If quit button is pressed
                    //_state = GameState.Menu;

                    //If unpaused button is pressed
                    //_state = GameState.Level;
                    break;

                case GameState.Level:
                    //On level end
                    //_state = GameState.Cutscene;

                    //If pause button is pressed
                    //_state = GameState.Pause;
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here


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
            Button pauseContinue = new Button(defaultButtonTexture, defaultButtonTexture, pauseContinueRect);
            pauseContinue.OnClick += MakeSwitchStateAction(GameState.Level);
            pauseButtons.Add(pauseContinue);

            Rectangle pauseLastCheckpointRect = new Rectangle(200, 320, 400, 100);
            Button pauseLastCheckpoint = new Button(defaultButtonTexture, defaultButtonTexture, pauseLastCheckpointRect);
            //pauseLastCheckpoint += the method that brings you back to your last checkpoint
            pauseButtons.Add(pauseLastCheckpoint);

            Rectangle pauseQuitRect = new Rectangle(200, 440, 400, 100);
            Button pauseQuit = new Button(defaultButtonTexture, defaultButtonTexture, pauseQuitRect);
            pauseQuit.OnClick += MakeSwitchStateAction(GameState.Menu);
            pauseButtons.Add(pauseQuit);

            //Fill menu buttons list with buttons
            Rectangle menuPlayRect = new Rectangle(400, 200, 600, 200);
            Button menuPlay = new Button(defaultButtonTexture, defaultButtonTexture, menuPlayRect);
            menuPlay.OnClick += MakeSwitchStateAction(GameState.Level);
            menuButtons.Add(menuPlay);

            Rectangle menuQuitRect = new Rectangle(400, 440, 300, 100);
            Button menuQuit = new Button(defaultButtonTexture, defaultButtonTexture, menuPlayRect);
            //menuQuit.OnClick += the method that closes the game
            menuButtons.Add(menuQuit);

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
