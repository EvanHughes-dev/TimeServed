using Microsoft.Xna.Framework;
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
    class SoundManager
    {
        //Fields


        //Properties

        //Methods

        /// <summary>
        /// Plays background music for the requested level
        /// </summary>
        /// <param name="level">Level for which background music is being played</param>
        public void PlayBGM(int level)
        {
            MediaPlayer.Play(AssetManager.BackgroundMusic[level - 1]);
        }

        /// <summary>
        /// Pauses the background music.
        /// </summary>
        public void PauseBGM()
        {
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Resumes the background music.
        /// </summary>
        public void ResumeBGM()
        {
            MediaPlayer.Resume();
        }
    }
}
