// AudioHandler.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace HonccaFest.Sound
{
    public class AudioHandler
    {
        public static Dictionary<string, SoundEffect> SoundEffects = new Dictionary<string, SoundEffect>()
        {
            {
                "quack_sound",
                null
            },
            {
                "coin_sound",
                null
            },
            {
                "cannon_sound",
                null
            },
            {
                "finish_sound",
                null
            }
        };

        public AudioHandler()
        {
            Dictionary<string, SoundEffect> newSoundEffects = new Dictionary<string, SoundEffect>();

            foreach (var dict in SoundEffects)
                newSoundEffects[dict.Key] = Main.Instance.Content.Load<SoundEffect>($"Sounds/{dict.Key}");

            SoundEffects = newSoundEffects;
        }
    }
}
