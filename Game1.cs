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
            scrollingBackground.LoadContent(GraphicsDevice);
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

            base.Update(gameTime);
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
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
