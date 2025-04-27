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

        /// <summary>
        /// Sound effect for when the wire cutters cut wire
        /// </summary>
        public static SoundEffect WirecutterSound { get; private set; }

        /// <summary>
        /// Sound effect for cameras being deactivated
        /// </summary>
        public static SoundEffect PowerDownSound { get; private set; }

        private static Random rng;

        /// <summary>
        /// Loads all music and sound effects.
        /// </summary>
        /// <param name="content">Content manager used to load all music and sound effects.</param>
        public static void LoadContent(ContentManager content)
        {

            //Sound effects
            PlayerStepSound = content.Load<SoundEffect>("Audio/Sound Effects/Player Step");
            KeycardSwipeSound = content.Load<SoundEffect>("Audio/Sound Effects/Keycard Swipe");
            WoodenDoorOpenSound = content.Load<SoundEffect>("Audio/Sound Effects/Wooden Door Open");
            WirecutterSound = content.Load<SoundEffect>("Audio/Sound Effects/Wirecutter");
            PowerDownSound = content.Load<SoundEffect>("Audio/Sound Effects/Power Down");
            VentUnscrewSound = content.Load<SoundEffect>("Audio/Sound Effects/Unscrew");

            rng = new Random();
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
