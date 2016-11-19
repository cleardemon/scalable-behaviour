using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;

namespace FAF
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        enum GameState
        {
            Title,
            InGame,
            GameOverBad,
            GameOverGood
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont fontCAFNormal, fontCAFMassive;
        Texture2D uiBackgroundGrey, uiBackgroundShadow;
        Texture2D uiFingerGlyph;

        FAFSprite spritePlayer;
        FAFSpriteAnimation animPlayerRunning;
        FAFSpriteAnimation animPlayerJumping;
        bool playerJumping;
        int playerJumpSpeed = 0;
        int playerStartY, playerMinY;
        int playerPoints;
        GameState playerGameState;
        const int PointDegredation = 50; // happens when an influencer spawns

        Random rand;
        readonly Dictionary<InfluencerType, FAFInfluencer> influencers = new Dictionary<InfluencerType, FAFInfluencer>();
        readonly List<FAFInfluencer> visibleInfluencers = new List<FAFInfluencer>();

        FAFScrollingBackground scrollingBackground;

        public Game1()
        {
            rand = new Random();

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

            // player
            animPlayerRunning = FAFSpriteAnimation.FromFrameCount(250, 358, 12, frameRate: 0.04);
            animPlayerJumping = FAFSpriteAnimation.FromFrameCount(250, 358, 1, 358); // one jump frame
            spritePlayer = new FAFSprite("Content/CharacterPlayer.png", animPlayerRunning);
            playerStartY = GraphicsDevice.Viewport.Height - 400;
            playerMinY = -GraphicsDevice.Viewport.Height;
            spritePlayer.Position = new Vector2(50, playerStartY);
            spritePlayer.LoadContent(GraphicsDevice);

            // influencers
            FAFInfluencer.Init(GraphicsDevice);
            influencers.Add(InfluencerType.Kimble, new FAFInfluencer(new FAFSprite("Content/InfluencerKimble.png") { Scale = new Vector2(2, 2) }, GraphicsDevice) 
            { 
                PointsOnCollision = -192, 
                Speed = 1 
            });
            influencers.Add(InfluencerType.FixedPriceSales, new FAFInfluencer(new FAFSprite("Content/InfluencerSales.png"), GraphicsDevice) 
            { 
                PointsOnCollision = -192, 
                Speed = 1 
            });
            influencers.Add(InfluencerType.Nandos, new FAFInfluencer(new FAFSprite("Content/InfluencerNandos.png"), GraphicsDevice) 
            { 
                PointsOnCollision = 192, 
                Speed = 2, 
                VariableYAmount = 20 
            });
            influencers.Add(InfluencerType.ProjectLaunch, new FAFInfluencer(new FAFSprite("Content/InfluencerLaunch.png"), GraphicsDevice) 
            { 
                PointsOnCollision = 192, 
                Speed = 1 
            });
            influencers.Add(InfluencerType.Whitfield, new FAFInfluencer(new FAFSprite("Content/InfluencerSteve.png"), GraphicsDevice)
            { 
                PointsOnCollision = -192, 
                Speed = 3, 
                VariableYAmount = 5 
            });
            influencers.Add(InfluencerType.SalaryIncrease, new FAFInfluencer(new FAFSprite("Content/InfluencerSalary.png"), GraphicsDevice) 
            { 
                PointsOnCollision = 192, 
                Speed = 1 
            });

            RestartGame();
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

        TimeSpan nextSpawnTime;
        TimeSpan gameRoundStartTime, gameRoundTime;

        void RestartGame()
        {
            playerPoints = 1927;
            nextSpawnTime = TimeSpan.Zero;
            gameRoundStartTime = TimeSpan.Zero;
            spritePlayer.Position = new Vector2(spritePlayer.Position.X, playerStartY);
            playerJumping = false;
            visibleInfluencers.Clear();

            playerGameState = GameState.InGame;
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
            if (playerGameState == GameState.InGame)
            {
                if (gameRoundStartTime == TimeSpan.Zero)
                    gameRoundStartTime = gameTime.TotalGameTime;
                gameRoundTime = gameTime.TotalGameTime - gameRoundStartTime;
                if (gameRoundTime.TotalSeconds > 60)
                {
                    // game over! good

                }
                if (playerPoints < 0)
                {
                    // game over! bad
                }

                // check for player jump
                var ts = TouchPanel.GetState();
                if (ts.Count > 0)
                {
                    var touch = ts[0];
                    if (touch.State == TouchLocationState.Pressed)
                    {
                        playerJumpSpeed = -14;
                        playerJumping = true;
                        spritePlayer.Animation = animPlayerJumping;
                    }
                }

                // move player if jumping
                if (playerJumping)
                {
                    spritePlayer.Move(0, playerJumpSpeed, playerMinY);
                    playerJumpSpeed++;
                    if (spritePlayer.Position.Y >= playerStartY)
                    {
                        spritePlayer.Position = new Vector2(spritePlayer.Position.X, playerStartY);
                        spritePlayer.Animation = animPlayerRunning;
                        playerJumping = false;
                    }
                }

                // add an influncer?
                var addNPC = gameTime.TotalGameTime >= nextSpawnTime;
                if (addNPC)
                {
                    nextSpawnTime = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(1));
                    var npcType = (InfluencerType)rand.Next(0, influencers.Count - 1);
                    var inf = (FAFInfluencer)influencers[npcType].Clone();
                    // start it off the screen
                    inf.Sprite.Position = new Vector2(GraphicsDevice.Viewport.Width, rand.Next(20, playerStartY + 200));
                    visibleInfluencers.Add(inf);

                    // reduce points
                    playerPoints -= PointDegredation;
                }

                // game logic for influencers
                for (var ii = 0; ii < visibleInfluencers.Count; ii++)
                {
                    var i = visibleInfluencers[ii];
                    // is kill?
                    if (i.IsKilled(gameTime))
                    {
                        visibleInfluencers.Remove(i);
                        ii--;
                        continue;
                    }

                    // move it
                    i.Sprite.Move(-i.Speed, i.GetYDelta());

                    // check collision with player
                    if (!i.PointsAwarded && i.Sprite.HasCollision(spritePlayer))
                    {
                        i.SetKilled(gameTime);
                        // award points
                        playerPoints += i.PointsOnCollision;
                        i.PointsAwarded = true;
                    }

                    i.Sprite.Update(gameTime);
                }
            }

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

            // influencers
            foreach (var i in visibleInfluencers)
            {
                i.Sprite.Draw(spriteBatch);
            }
            // player
            spritePlayer.Draw(spriteBatch);

            // level time (we show from 9am to 6pm)
            var hours = 9 + (gameRoundTime.TotalSeconds / (60 / 9));
            var hoursText = ((int)hours) + (hours < 12 ? "am" : "pm");
            DrawRectangleText(fontCAFMassive, 20, 20, hoursText, Color.White);
            // level score
            DrawRectangleText(fontCAFMassive, -40, 20, playerPoints.ToString(), Color.White, uiFingerGlyph);

            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
