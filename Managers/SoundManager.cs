using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;

namespace MakeEveryDayRecount.Managers
{
    /// <summary>
    /// Manages all sounds in the game (music, sound effects).
    /// </summary>
    public static class SoundManager
    {
        //Fields
        static Random rng = new Random();

        /// <summary>
        /// Array of all background tracks in the game
        /// </summary>
        public static Song[] BackgroundMusic { get; private set; }



        /// <summary>
        /// Sound effect used for when the player moves.
        /// </summary>
        public static SoundEffect PlayerStepSound { get; private set; }

        /// <summary>
        /// Sound effect used for unlocking a door with the Keycard
        /// </summary>
        public static SoundEffect KeycardSwipeSound { get; private set; }

        /// <summary>
        /// Sound effect used for opening a door that doesn't require a key
        /// </summary>
        public static SoundEffect WoodenDoorOpenSound { get; private set; }

        /// <summary>
        /// Sound effect for unscrewing a vent
        /// </summary>
        public static SoundEffect VentUnscrewSound { get; private set; }

        //TODO: use the wirecutter sound and camera sound

        /// <summary>
        /// Sound effect for when the wirecutters cut wire
        /// </summary>
        public static SoundEffect WirecutterSound { get; private set; }

        /// <summary>
        /// Sound effect for cameras being deactivated
        /// </summary>
        public static SoundEffect PowerDownSound { get; private set; }

        /// <summary>
        /// If the background music has a track
        /// </summary>
        public static bool PlayingMusic { get; set; }

        //Methods

        /// <summary>
        /// Loads all music and sound effects.
        /// </summary>
        /// <param name="content">Content manager used to load all music and sound effects.</param>
        public static void LoadContent(ContentManager content)
        {
            //Background Music
            BackgroundMusic = new Song[]
            {
                content.Load<Song>("Audio/Music/Sneaky Snitch")
            };

            //Sound effects
            PlayerStepSound = content.Load<SoundEffect>("Audio/Sound Effects/Player Step");
            KeycardSwipeSound = content.Load<SoundEffect>("Audio/Sound Effects/Keycard Swipe");
            WoodenDoorOpenSound = content.Load<SoundEffect>("Audio/Sound Effects/Wooden Door Open");
            WirecutterSound = content.Load<SoundEffect>("Audio/Sound Effects/Wirecutter");
            PowerDownSound = content.Load<SoundEffect>("Audio/Sound Effects/Power Down");
            VentUnscrewSound = content.Load<SoundEffect>("Audio/Sound Effects/Unscrew");

            PlayingMusic = false;
        }

        /// <summary>
        /// Plays background music for the requested level
        /// </summary>
        /// <param name="level">Level for which background music is being played</param>
        public static void PlayBGM(int level)
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Stop();
            //TODO: Lower the volume
            //TODO: find better music
            //MediaPlayer.Play(BackgroundMusic[level - 1]);
            PlayingMusic = true;
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
        /// Stop the current bgm
        /// </summary>
        public static void StopBGM()
        {
            MediaPlayer.Stop();
            PlayingMusic = false;
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="soundEffect">Sound effect to be played</param>
        public static void PlaySFX(SoundEffect soundEffect)
        {
            soundEffect.Play();
        }

        /// <summary>
        /// Plays a sound effect with randomly modulated pitch.
        /// </summary>
        /// <param name="soundEffect">Sound effect to be played</param>
        /// <param name="minPitch">Minimum pitch modulation for the sound, cannot be lower than -100</param>
        /// <param name="maxPitch">Maximum pitch modulation for the sound, cannot be higher than 100</param>
        public static void PlaySFX(SoundEffect soundEffect, int minPitch, int maxPitch)
        {
            soundEffect.Play(1, 0.01f * rng.Next(minPitch, maxPitch), 0);
        }
    }
}
