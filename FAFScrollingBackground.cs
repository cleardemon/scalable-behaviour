using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace FAF
{
    public class FAFScrollingBackground
    {
        public enum ScrollDirection
        {
            Left,
            Right
        }

        readonly List<FAFSprite> spriteBackground;
        FAFSprite spriteRightMost, spriteLeftMost;
        readonly Viewport viewPort;

        public FAFScrollingBackground(Viewport vp)
        {
            spriteBackground = new List<FAFSprite>();
            viewPort = vp;
        }

        public void LoadContent(GraphicsDevice gd)
        {
            spriteLeftMost = spriteRightMost = null;

            float width = 0;
            foreach (var sp in spriteBackground)
            {
                sp.LoadContent(gd);
                sp.Scale = new Vector2(viewPort.Height / sp.Size.Height);

                if (spriteRightMost == null)
                {
                    sp.Position = new Vector2(viewPort.X, viewPort.Y);
                    spriteLeftMost = sp;
                }
                else 
                {
                    sp.Position = new Vector2(spriteRightMost.Position.X + spriteRightMost.Size.Width, viewPort.Y);
                }

                spriteRightMost = sp;
                width += sp.Size.Width;
            }

            int i = 0;
            // fill the viewport with sprites
            if (spriteBackground.Count > 0 && width < viewPort.Width * 2)
            {
                do
                {
                    var sp = new FAFSprite(spriteBackground[i].AssetName);
                    sp.LoadContent(gd);
                    sp.Scale = new Vector2(viewPort.Height / sp.Size.Height);
                    sp.Position = new Vector2(spriteRightMost.Position.X + spriteRightMost.Size.Width, viewPort.Y);
                    spriteBackground.Add(sp);
                    spriteRightMost = sp;

                    width += sp.Size.Width;

                    i++;
                    if (i > spriteBackground.Count - 1)
                        i = 0;
                } while (width < viewPort.Width * 2);
            }
        }

        public void AddBackground(string assetName)
        {
            spriteBackground.Add(new FAFSprite(assetName));
        }

        public void Update(GameTime gt, int speed, ScrollDirection dir)
        {
            var vDirection = Vector2.Zero;

            switch (dir)
            {
                case ScrollDirection.Left:
                    foreach (var sp in spriteBackground)
                    {
                        if (sp.Position.X < viewPort.X - sp.Size.Width)
                        {
                            sp.Position = new Vector2(spriteRightMost.Position.X + spriteRightMost.Size.Width, viewPort.Y);
                            spriteRightMost = sp;
                        }
                    }
                    vDirection.X = -1;
                    break;
                case ScrollDirection.Right:
                    foreach (var sp in spriteBackground)
                    {
                        if (sp.Position.X > viewPort.X + viewPort.Width)
                        {
                            sp.Position = new Vector2(spriteLeftMost.Position.X - spriteLeftMost.Size.Width, viewPort.Y);
                            spriteLeftMost = sp;
                        }
                    }
                    vDirection.X = 1;
                    break;
            }

            // update positions of sprites
            foreach (var sp in spriteBackground)
                sp.Update(gt, new Vector2(speed, 0), vDirection);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var sp in spriteBackground)
            {
                sp.Draw(sb);
            }
        }
    }
}
