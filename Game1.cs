using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FAF
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont fontCAFNormal, fontCAFMassive;
        Texture2D uiBackgroundGrey, uiBackgroundShadow;
        Texture2D uiFingerGlyph;

        FAFSprite spritePlayer;

        FAFScrollingBackground scrollingBackground;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // init scrolling background
            scrollingBackground = new FAFScrollingBackground(GraphicsDevice.Viewport);
            scrollingBackground.AddBackground("Content/Background01.png");
            scrollingBackground.AddBackground("Content/Background02.png");
            scrollingBackground.AddBackground("Content/Background03.png");
            scrollingBackground.AddBackground("Content/Background04.png");
            scrollingBackground.AddBackground("Content/Background05.png");
            scrollingBackground.AddBackground("Content/Background06.png");
            scrollingBackground.AddBackground("Content/Background07.png");
            scrollingBackground.AddBackground("Content/Background08.png");
            scrollingBackground.AddBackground("Content/Background09.png");
            scrollingBackground.LoadContent(GraphicsDevice);

            // init font
            fontCAFNormal = Content.Load<SpriteFont>("CAFFont");
            fontCAFMassive = Content.Load<SpriteFont>("CAFFontMassive");

            // ui
            uiBackgroundGrey = new Texture2D(GraphicsDevice, 1, 1);
            uiBackgroundShadow = new Texture2D(GraphicsDevice, 1, 1);
            uiBackgroundGrey.SetData(new[] { new Color(49, 49, 49) });
            uiBackgroundShadow.SetData(new[] { new Color(0f, 0f, 0f, 0.5f) });
            uiFingerGlyph = Content.Load<Texture2D>("MiddleFinger");

            spritePlayer = new FAFSprite("Content/CharacterPlayer.png", FAFSpriteAnimation.FromFrameCount(250, 358, 12, frameRate: 0.04));
            spritePlayer.Position = new Vector2(50, GraphicsDevice.Viewport.Height - 400);
            spritePlayer.LoadContent(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            if (uiBackgroundGrey != null)
            {
                uiBackgroundGrey.Dispose();
                uiBackgroundGrey = null;
            }
            if (uiBackgroundShadow != null)
            {
                uiBackgroundShadow.Dispose();
                uiBackgroundShadow = null;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            // Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
#endif

            scrollingBackground.Update(gameTime, 160, FAFScrollingBackground.ScrollDirection.Left);
            spritePlayer.Update(gameTime);

            base.Update(gameTime);
        }

        void DrawShadowRectangle(Rectangle rectPos)
        {
            const int ShadowOffset = 5;
            spriteBatch.Draw(uiBackgroundShadow, destinationRectangle: new Rectangle(rectPos.X + ShadowOffset, rectPos.Y + ShadowOffset, rectPos.Width + ShadowOffset, rectPos.Height + ShadowOffset));
            spriteBatch.Draw(uiBackgroundGrey, destinationRectangle: rectPos);
        }

        void DrawRectangleText(SpriteFont font, int x, int y, string text, Color colour, Texture2D glyph = null)
        {
            const int shadowExtraWidth = 40, shadowExtraHeight = 20;
            var fontSize = font.MeasureString(text);
            if (glyph != null)
            {
                fontSize.X += glyph.Width + (shadowExtraWidth / 2);
                fontSize.Y = Math.Max(fontSize.Y, glyph.Height);
            }            
            int rx, ry;
            if (x < 0)
                rx = GraphicsDevice.Viewport.Width - (-x) - (int)fontSize.X - shadowExtraWidth;
            else
                rx = x;
            if (y < 0)
                ry = GraphicsDevice.Viewport.Height - (-y) - (int)fontSize.Y - shadowExtraHeight;
            else
                ry = y;

            DrawShadowRectangle(new Rectangle(rx, ry, (int)fontSize.X + shadowExtraWidth, (int)fontSize.Y + shadowExtraHeight));
            if (glyph == null)
                spriteBatch.DrawString(font, text, new Vector2(rx + (shadowExtraWidth / 2), ry + (shadowExtraHeight / 2)), colour);
            else
            {
                spriteBatch.Draw(glyph, destinationRectangle: new Rectangle(rx + (shadowExtraWidth / 2), ry + (shadowExtraHeight / 2), glyph.Width, glyph.Height));
                spriteBatch.DrawString(font, text, new Vector2(rx + glyph.Width + ((shadowExtraWidth / 2) * 2), ry + (shadowExtraHeight / 2)), colour);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            scrollingBackground.Draw(spriteBatch);

            // level time
            DrawRectangleText(fontCAFMassive, 20, 20, "12pm", Color.White);
            // level score
            DrawRectangleText(fontCAFMassive, -40, 20, "1927", Color.White, uiFingerGlyph);
            // player
            spritePlayer.Draw(spriteBatch);

            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
