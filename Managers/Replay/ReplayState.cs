using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace MakeEveryDayRecount.Managers.Replay
{

    /// <summary>
    /// Track just the needed keys pressed by the player. Acts as
    /// the KeyboardState and MouseState when rendering the replay
    /// </summary>
    internal class ReplayState
    {
        /// <summary>
        /// Get the current state of the left button
        /// </summary>
        public ButtonState LeftButton { get => _mouseState[MouseButtonState.Left]; }
        /// <summary>
        /// Get the current state of the middle button
        /// </summary>
        public ButtonState MiddleButton { get => _mouseState[MouseButtonState.Middle]; }
        /// <summary>
        /// Get the current state of the right button
        /// </summary>
        public ButtonState RightButton { get => _mouseState[MouseButtonState.Right]; }

        /// <summary>
        /// Get the current position of the mouse
        /// </summary>
        public Point CurrentMousePosition { get; private set; }

        /// <summary>
        /// Time since the previous frame
        /// </summary>
        public float DeltaTime { get; private set; }

        private Dictionary<Keys, bool> _keyStates;
        private Dictionary<MouseButtonState, ButtonState> _mouseState;

        private static readonly List<Keys> WantedKeys = new List<Keys> { Keys.W, Keys.A, Keys.S, Keys.D, Keys.E,
                                                                         Keys.D1, Keys.D2, Keys.D3, Keys.D4 };

        private static readonly List<MouseButtonState> WantedState = new List<MouseButtonState> { MouseButtonState.Left, MouseButtonState.Middle,
                                                                                                  MouseButtonState.Right };

        /// <summary>
        /// Create a replay state from a KeyboardState and MouseState
        /// </summary>
        /// <param name="kb">Current KeyboardState</param>
        /// <param name="ms">Current MouseState</param>
        /// <param name="deltaTime">Time since last frame</param>
        public ReplayState(KeyboardState kb, MouseState ms, float deltaTime)
        {
            _keyStates = new Dictionary<Keys, bool> { };
            foreach (Keys key in WantedKeys)
            {
                _keyStates.Add(key, kb.IsKeyDown(key));
            }

            DeltaTime = deltaTime;

            CurrentMousePosition = new Point(ms.X, ms.Y);

            _mouseState = new Dictionary<MouseButtonState, ButtonState>{
                {MouseButtonState.Left, ms.LeftButton},
                {MouseButtonState.Middle, ms.MiddleButton},
                {MouseButtonState.Right, ms.RightButton},
            };

        }

        /// <summary>
        /// Create a ReplayState from a file
        /// </summary>
        /// <param name="binaryReader">BinaryReader to read from</param>
        public ReplayState(BinaryReader binaryReader)
        {
            _keyStates = new Dictionary<Keys, bool> { };
            _mouseState = new Dictionary<MouseButtonState, ButtonState> { };
            LoadData(binaryReader);
        }

        /// <summary>
        /// Write data from this class to the file in binary
        /// </summary>
        /// <param name="binaryWriter">Writer to use</param>
        public void SaveToFile(BinaryWriter binaryWriter)
        {
            foreach (Keys key in WantedKeys)
            {
                binaryWriter.Write(_keyStates[key]);
            }
            foreach (MouseButtonState state in WantedState)
            {
                binaryWriter.Write(_mouseState[state] == ButtonState.Pressed);
            }
            binaryWriter.Write(CurrentMousePosition.X);
            binaryWriter.Write(CurrentMousePosition.Y);
            binaryWriter.Write(DeltaTime);
        }
        /// <summary>
        /// Read data from an incoming file
        /// </summary>
        /// <param name="binaryReader">Read binary data</param>
        public void LoadData(BinaryReader binaryReader)
        {

            foreach (Keys key in WantedKeys)
            {
                _keyStates.Add(key, binaryReader.ReadBoolean());
            }
            foreach (MouseButtonState state in WantedState)
            {
                _mouseState.Add(state, binaryReader.ReadBoolean() ? ButtonState.Pressed : ButtonState.Released);
            }
            CurrentMousePosition = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
            DeltaTime = binaryReader.ReadSingle();
        }

        /// <summary>
        /// Get if a key is currently down. If the key is not one record, returns false
        /// </summary>
        /// <param name="isKeyDown">Key to look for</param>
        /// <returns>If the key is down. True -> Key is down</returns>
        public bool IsKeyDown(Keys isKeyDown)
        {
            if (_keyStates.TryGetValue(isKeyDown, out bool keyState))
                return keyState;

            return false;
        }


        /// <summary>
        /// Get if a key is currently up. If the key is not one record, returns false
        /// </summary>
        /// <param name="isKeyDown">Key to look for</param>
        /// <returns>If the key is up. True -> Key is up</returns>
        public bool IsKeyUp(Keys isKeyUp)
        {
            if (_keyStates.TryGetValue(isKeyUp, out bool keyState))
                return !keyState; // keyState is false if key is up

            return false;
        }

        /// <summary>
        /// Get the current state of the mouse
        /// </summary>
        /// <param name="bs">Button to check</param>
        /// <returns>If the button is down</returns>
        public ButtonState GetMouseState(MouseButtonState bs)
        {
            return _mouseState[bs];
        }


    }
}