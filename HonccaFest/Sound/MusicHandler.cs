// MusicHandler.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace HonccaFest.Sound
{
    public class MusicHandler
    {
        public static Dictionary<string, Song> Music = new Dictionary<string, Song>()
        {
            {
                "new_eyes",
                null
            },
            {
                "engines_revved",
                null
            },
            {
                "loading",
                null
            },
            {
                "duck_tag",
                null
            },
            {
                "lose_stinger",
                null
            },
            {
                "wandering_maze",
                null
            }
        };

        public MusicHandler()
        {
            Dictionary<string, Song> newSoundEffects = new Dictionary<string, Song>();

            foreach (var dict in Music)
                newSoundEffects[dict.Key] = Main.Instance.Content.Load<Song>($"Songs/{dict.Key}");

            Music = newSoundEffects;
        }

        public void Play(string musicName, bool loop = true)
        {
            // Music doesn't exist, quit.
            if (!Music.ContainsKey(musicName))
                return;
            // Music isn't loaded properly, quit.
            if (Music[musicName] == null)
                return;

            MediaPlayer.Play(Music[musicName]);

            MediaPlayer.Volume = 0.05f;
            MediaPlayer.IsRepeating = loop;
        }
    }
}
