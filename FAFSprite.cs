using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace FAF
{
    public class FAFSpriteAnimationFrame
    {
        
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

        public void Update(GameTime gt, Vector2 speed, Vector2 direction)
        {
            Position += direction * speed * (float)gt.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Position, scale: Scale);
        }

        public FAFSprite(string assetName)
        {
            Position = Vector2.Zero;
            AssetName = assetName;
            Scale = new Vector2(1f);
        }
    }
}
