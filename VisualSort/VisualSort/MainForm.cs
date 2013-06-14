using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VisualSort
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainForm : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D LoadingTexture1, LoadingTexture2, LoadingTexture3;
        private Vector2 LoadingOrigin, LoadingPos;
        private float LoadingAngle, LoadingScale, LoadingAlpha;
        private bool isLoading;

        public MainForm()
        {
            graphics = new GraphicsDeviceManager(this);
            // Deixemos assim até que coloquemos full screen com toda a parte de resolução certinho
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
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
            isLoading = true;
            // TODO: Add your initialization logic here

            base.Initialize();
            isLoading = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            TElemento teste = new TElemento("Um a dois bla we adsdwd");
            Console.WriteLine(teste.Nome);
            Console.WriteLine(teste.Sigla);

            isLoading = true;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initializes the Loading Circle
            LoadingTexture1 = Content.Load<Texture2D>("Loading1");
            LoadingTexture2 = Content.Load<Texture2D>("Loading2");
            LoadingTexture3 = Content.Load<Texture2D>("Loading3");
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            LoadingOrigin.X = LoadingTexture1.Width / 2;
            LoadingOrigin.Y = LoadingTexture1.Height / 2;
            LoadingScale = 0.42f;
            LoadingAlpha = 0.9f;
            LoadingPos.X = viewport.Width - (LoadingScale * LoadingTexture1.Width * 0.64f);
            LoadingPos.Y = LoadingScale * LoadingTexture1.Height * 0.64f;

            isLoading = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Makes the Loading Circle Rotate
            LoadingAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2.5f;
            LoadingAngle = LoadingAngle % (MathHelper.Pi * 4);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkCyan);

            // Draws the Loading Circle
            spriteBatch.Begin();
            if (!isLoading)
            {
                spriteBatch.Draw(LoadingTexture3, LoadingPos, null, Color.White * LoadingAlpha * 0.8f, LoadingAngle * 0.5f,
                    LoadingOrigin, LoadingScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(LoadingTexture1, LoadingPos, null, Color.White * LoadingAlpha, -LoadingAngle,
                    LoadingOrigin, LoadingScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(LoadingTexture2, LoadingPos, null, Color.White * LoadingAlpha, LoadingAngle,
                    LoadingOrigin, LoadingScale, SpriteEffects.None, 0f);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
