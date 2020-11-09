using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonccaFest.Sound
{
    public class AudioHandler
    {
        public static Dictionary<string, SoundEffect> SoundEffects = new Dictionary<string, SoundEffect>()
        {
            {
                "quack_sound",
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
