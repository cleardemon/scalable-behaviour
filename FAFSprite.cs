using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;

namespace FAF
{
    public class FAFSpriteAnimationFrame
    {
        public Rectangle SourceRectangle { get; set; }
        public TimeSpan FrameDuration { get; set; }
    }

    public class FAFSpriteAnimation
    {
        readonly List<FAFSpriteAnimationFrame> Frames = new List<FAFSpriteAnimationFrame>();
        TimeSpan timeAnimationPosition; // animation position

        public FAFSpriteAnimation(params FAFSpriteAnimationFrame[] frames)
        {
            Frames.AddRange(frames);
        }

        public static FAFSpriteAnimation FromFrameCount(int frameWidth, int frameHeight, int frameCount, int frameYOffset = 0, double frameRate = 0.25)
        {
            var frames = new List<FAFSpriteAnimationFrame>();
            for (var frame = 0; frame < frameCount; frame++)
            {
                frames.Add(new FAFSpriteAnimationFrame
                {
                    SourceRectangle = new Rectangle(frameWidth * frame, frameYOffset, frameWidth, frameHeight),
                    FrameDuration = TimeSpan.FromSeconds(frameRate)
                });
            }
            return new FAFSpriteAnimation(frames.ToArray());
        }

        public TimeSpan Duration
        {
            get
            {
                double secs = 0;
                foreach (var f in Frames)
                {
                    secs += f.FrameDuration.TotalSeconds;
                }

                return TimeSpan.FromSeconds(secs);
            }
        }

        public void AddFrame(Rectangle frameSize, TimeSpan duration)
        {
            Frames.Add(new FAFSpriteAnimationFrame
            {
                SourceRectangle = frameSize,
                FrameDuration = duration
            });
        }

        public void Update(GameTime gt)
        {
            var secsIntoAnimation = timeAnimationPosition.TotalSeconds + gt.ElapsedGameTime.TotalSeconds;
            var remaining = secsIntoAnimation % Duration.TotalSeconds;
            timeAnimationPosition = TimeSpan.FromSeconds(remaining);
        }

        public Rectangle CurrentFrameRectangle
        {
            get
            {
                FAFSpriteAnimationFrame frame = null;

                TimeSpan accumulatedTime = TimeSpan.Zero;
                foreach (var f in Frames)
                {
                    if (accumulatedTime + f.FrameDuration >= timeAnimationPosition)
                    {
                        frame = f;
                        break;
                    }
                    accumulatedTime += f.FrameDuration;
                }

                // If no frame was found, then try the last frame, 
                // just in case timeAnimationPosition somehow exceeds FrameDuration
                if (frame == null)
                {
                    frame = Frames.LastOrDefault();
                }

                // If we found a frame, return its rectangle, otherwise
                // return an empty rectangle (one with no width or height)
                return frame != null ? frame.SourceRectangle : Rectangle.Empty;
            }
        }
    }

    public class FAFSprite
    {
        public Vector2 Position
        {
            get;
            set;
        }

        Texture2D texture;

        public Rectangle Size
        {
            get;
            private set;
        }

        Vector2 scale;
        public Vector2 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                UpdateSize();
            }
        }

        public string AssetName
        {
            get;
        }

        public FAFSpriteAnimation Animation
        {
            get;
            set;
        }

        void UpdateSize()
        {
            Size = texture != null ? new Rectangle(0, 0, (int)(texture.Width * Scale.X), (int)(texture.Height * Scale.Y)) : Rectangle.Empty;
        }

        public void LoadContent(GraphicsDevice gd)
        {
            using (var s = TitleContainer.OpenStream(AssetName))
                texture = Texture2D.FromStream(gd, s);
            UpdateSize();
        }

        public void Update(GameTime gt)
        {
            Animation?.Update(gt);
        }

        public void Draw(SpriteBatch sb)
        {
            if (texture == null)
                return;
            if (Animation != null)
            {
                sb.Draw(texture, Position, sourceRectangle: Animation.CurrentFrameRectangle, scale: Scale);
            }
            else
                sb.Draw(texture, Position, scale: Scale);
        }

        public FAFSprite(string assetName, FAFSpriteAnimation animation = null)
        {
            Position = Vector2.Zero;
            AssetName = assetName;
            Scale = new Vector2(1f);
            Animation = animation;
        }
    }
}
