// MainMenu.cs
// Author Carl Åberg
// LBS Kreativa Gymnasiet

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HonccaFest.Files
{
    public class GraphicsHandler
    {
        public Dictionary<string, Sprite> Graphics = new Dictionary<string, Sprite>()
        {
            {
                "HonccaLogo",

                new Sprite()
                {
                    FileName = "Sprites/honccaLogo"
                }
            },
            {
                "TileSheet",

                new Sprite()
                {
                    FileName = "Tiles/tileSet"
                }
            },
            {
                "OutlineRectangle",

                new Sprite()
                {
                    FileName = "Sprites/outlineRectangle"
                }
            },
            {
                "TransparentRectangle",

                new Sprite()
                {
                    FileName = "Sprites/transparentRectangle"
                }
            },
            {
                "FilledRectangle",

                new Sprite()
                {
                    FileName = "Sprites/filledRectangle"
                }
            },
            {
                "TaggerArrow",

                new Sprite()
                {
                    FileName = "Sprites/taggerPointer"
                }
            },
            {
                "CharacterSelectionArrow",

                new Sprite()
                {
                    FileName = "Sprites/characterSelectionArrow"
                }
            },
            {
                "CheckMark",

                new Sprite()
                {
                    FileName = "Sprites/checkmark"
                }
            },
            {
                "JoystickButtons",

                new Sprite()
                {
                    FileName = "Sprites/joystick"
                }
            },
            {
                "DuckyRoadHome",

                new Sprite()
                {
                    FileName = "Sprites/duckyRoadBase"
                }
            },

            {
                "PlayerOneSprite",

                new Sprite()
                {
                    FileName = "SpriteSheets/playerSpritesheet"
                }
            },
            {
                "FireballSprite",

                new Sprite()
                {
                    FileName = "SpriteSheets/fireballSpritesheet"
                }
            },
            {
                "CarSprite",

                new Sprite()
                {
                    FileName = "SpriteSheets/carSpritesheet"
                }
            },
            {
                "CoinSprite",

                new Sprite()
                {
                    FileName = "SpriteSheets/coinSpritesheet"
                }
            },
            {
                "CannonSprite",

                new Sprite()
                {
                    FileName = "SpriteSheets/cannonSprite"
                }
            },
            {
                "CannonBallSprite",

                new Sprite()
                {
                    FileName = "Sprites/cannonBallSprite"
                }
            },
            {
                "LoadingCircleSprite",

                new Sprite()
                {
                    FileName = "SpriteSheets/loadingCircle"
                }
            }
        };

        public GraphicsHandler()
        {
            Dictionary<string, Sprite> newGraphics = Graphics;

            foreach (var graphic in newGraphics)
                Graphics[graphic.Key].LoadTexture();
        }

        public Texture2D GetSprite(string spriteName)
        {
            if (Graphics.ContainsKey(spriteName))
            {
                if (Graphics[spriteName].Texture == null)
                    throw new Exception($"{spriteName} doesn't exist in the Content folder.");

                return Graphics[spriteName].Texture;
            }

            throw new Exception($"{spriteName} doesn't exist in the dictionary.");
        }
    }

    public class Sprite
    {
        public string FileName;

        public Texture2D Texture;

        public void LoadTexture()
        {
            Texture = Main.Instance.Content.Load<Texture2D>(FileName);
        }
    }
}
