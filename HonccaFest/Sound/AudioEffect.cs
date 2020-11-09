using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.Sound
{
    class AudioEffect
    {
        private readonly SoundEffect sound;

        public AudioEffect(string _sound)
        {
            sound = AudioHandler.SoundEffects[_sound];
        }

        public void Play(float volume, Vector2 playPosition)
        {
            float screenDistance = playPosition.X / Globals.ScreenSize.X;

            float pan = MathHelper.Clamp(screenDistance, -1, 1);

            Console.WriteLine($"{pan}, {playPosition.X}, {Globals.ScreenSize.X}");

            sound.Play(volume, 0, pan);
        }

        public void Stop()
        {
            sound.Dispose();
        }
    }
}
