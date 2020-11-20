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
        private readonly SoundEffectInstance sound;

        public AudioEffect(string _sound)
        {
            sound = AudioHandler.SoundEffects[_sound].CreateInstance();
        }

        public void Play(float volume, Vector2 playPosition)
        {
            AudioEmitter emitter = new AudioEmitter();
            AudioListener listener = new AudioListener();

            listener.Position = new Vector3(new Vector2(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y / 2), 0);
            emitter.Position = new Vector3(playPosition, 0);

            sound.Volume = volume;

            sound.Apply3D(listener, emitter);
            sound.Play();
        }

        public void Stop()
        {
            sound.Dispose();
        }
    }
}
