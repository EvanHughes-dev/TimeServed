using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount
{
    /// <summary>
    /// Manages all sounds in the game (music, sound effects).
    /// </summary>
    public static class SoundManager
    {
        //Fields


        //Properties

        /// <summary>
        /// Array of all background tracks in the game
        /// </summary>
        public static Song[] BackgroundMusic { get; private set; }

        /// <summary>
        /// Sound effect used for when the player moves.
        /// </summary>
        public static SoundEffect PlayerStepSound { get; private set; }

        //Methods

        /// <summary>
        /// Louds all music and sound effects.
        /// </summary>
        /// <param name="content">Content manager used to load all music and sound effects.</param>
        public static void LoadContent(ContentManager content)
        {
            BackgroundMusic = new Song[]
            {
                content.Load<Song>("Audio/Music/Sneaky Snitch")
            };

            PlayerStepSound = content.Load<SoundEffect>("Audio/Sound Effects/Player Step");
        }

        /// <summary>
        /// Plays background music for the requested level
        /// </summary>
        /// <param name="level">Level for which background music is being played</param>
        public static void PlayBGM(int level)
        {
            MediaPlayer.Play(BackgroundMusic[level - 1]);
        }

        /// <summary>
        /// Pauses the background music.
        /// </summary>
        public static void PauseBGM()
        {
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Resumes the background music.
        /// </summary>
        public static void ResumeBGM()
        {
            MediaPlayer.Resume();
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="soundEffect">Sound effect to be played</param>
        public static void PlaySFX(SoundEffect soundEffect)
        {
            soundEffect.Play();
        }
    }
}
