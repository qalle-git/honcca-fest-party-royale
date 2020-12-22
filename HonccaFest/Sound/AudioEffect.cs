// AudioEffect.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using HonccaFest.MainClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace HonccaFest.Sound
{
    class AudioEffect
    {
        private readonly SoundEffect sound;

        public AudioEffect(string _sound)
        {
            // AudioEffect doesn't exist, change to default.
            if (!AudioHandler.SoundEffects.ContainsKey(_sound))
                _sound = "quack_sound";
            // AudioEffect isn't loaded, change to default.
            if (AudioHandler.SoundEffects[_sound] == null)
                _sound = "quack_sound";

            sound = AudioHandler.SoundEffects[_sound];
        }

        public void Play(float volume, Vector2 playPosition)
        {
            float newPlayPositionX = (playPosition.X / Globals.GameSize.X * 2) - 1;

            sound.Play(volume, 0f, newPlayPositionX);
        }

        public void Stop()
        {
            sound.Dispose();
        }
    }
}
