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
    public class Renderer : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Loading Circle stuff
        private Vector2 LoadingOrigin, LoadingPos;
        private float LoadingAngle, LoadingScale, LoadingAlpha;
        private bool isLoading;
        // Default Font
        SpriteFont DefaultFont;
        // Inputs
        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;
        MouseState ms;
        float prevWheelValue;
        float currWheelValue;

        Graph.TDrawNodo teste = new Graph.TDrawNodo();

        public Renderer()
        {
            graphics = new GraphicsDeviceManager(this);
            // Deixemos assim até que coloquemos full screen com toda a parte de resolução certinho
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
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
            Graph.DPrimitives = new PrimitiveRenderer(GraphicsDevice);

            Program.ScreenCenter = new Vector2(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.5f);

            Graph.Camera = new Graph.Camera2d();
            Graph.Camera.Pos = new Vector2(500.0f, 200.0f);

            ms = new MouseState();
            base.Initialize();
            isLoading = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            isLoading = true;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the Sprites
            Program.NodoTex = Content.Load<Texture2D>("Nodo64");
            // Initializes the Loading Circle
            Program.LoadingTexture = new Texture2D[4];
            Program.LoadingTexture[0] = Content.Load<Texture2D>("Loading1");
            Program.LoadingTexture[1] = Content.Load<Texture2D>("Loading2");
            Program.LoadingTexture[2] = Content.Load<Texture2D>("Loading3");
            Program.LoadingTexture[3] = Content.Load<Texture2D>("Loading4");
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            LoadingOrigin.X = Program.LoadingTexture[0].Width / 2;
            LoadingOrigin.Y = Program.LoadingTexture[1].Height / 2;
            LoadingScale = 0.64f;
            LoadingAlpha = 0.8f;
            LoadingPos.X = viewport.Width / 2f;// - (LoadingScale * LoadingTexture1.Width * 0.64f);
            LoadingPos.Y = viewport.Height * 0.4f;//LoadingScale * LoadingTexture1.Height * 0.64f;

            // Font
            DefaultFont = Content.Load<SpriteFont>("DefaultFont");

            // Loads XML File

            // Example
            Program.mPessoas.NovoNodo(new TInfoNodo("Krug", new BPos(0,0)));
          //  Program.mPessoas.NovoNodo(new TInfoNodo("Aline", new BPos(0, 1)));
          //  Program.mPessoas.NovoNodo(new TInfoNodo("Mara Abel", new BPos(0, 2)));
           // Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(1,0));
           // Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(2, 0));
            for (int i = 1; i < 500; i++)
            {
                Program.mPessoas.NovoNodo(new TInfoNodo("n" + i, new BPos(0, 0)));
                Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(i, 0));
            }
            Graph.SelecionaNodo(new TPNodo(0, 0));

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
            LoadingAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2.56f;
            LoadingAngle = LoadingAngle % (MathHelper.Pi * 4);

            // Inputs
            //Zoom
            ms = Mouse.GetState();
            prevWheelValue = currWheelValue;
            currWheelValue = ms.ScrollWheelValue;
            Graph.Camera.Zoom = Graph.Camera.Zoom + (currWheelValue - prevWheelValue) * 0.001f * (Graph.Camera.Zoom);
            // Move camera
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Graph.Camera.Move(new Vector2(-1.0f * (10/Graph.Camera.Zoom), 0f));
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                Graph.Camera.Move(new Vector2(1.0f * (10 / Graph.Camera.Zoom), 0f));
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Graph.Camera.Move(new Vector2(0f, -1.0f * (10 / Graph.Camera.Zoom)));
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Graph.Camera.Move(new Vector2(0f, 1.0f * (10 / Graph.Camera.Zoom)));
            }
            oldKeyboardState = keyboardState;
            // Update Mouse Position
            Graph.Camera.UpdateMouse(graphics.GraphicsDevice, ms);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 35, 114));//Color.DarkSlateGray);

            
            spriteBatch.Begin();

            // Draws the Loading Circle
            if (isLoading)
            {
                spriteBatch.Draw(Program.LoadingTexture[2], LoadingPos, null, Color.White * LoadingAlpha * 0.6f, 0.0f,
                    LoadingOrigin, LoadingScale*0.96f, SpriteEffects.None, 0f);
                spriteBatch.Draw(Program.LoadingTexture[3], LoadingPos, null, Color.White * LoadingAlpha, -LoadingAngle * 0.5f,
                    LoadingOrigin, LoadingScale * 0.96f, SpriteEffects.None, 0f);
                spriteBatch.Draw(Program.LoadingTexture[0], LoadingPos, null, Color.White * LoadingAlpha, -LoadingAngle,
                    LoadingOrigin, LoadingScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(Program.LoadingTexture[1], LoadingPos, null, Color.White * LoadingAlpha, LoadingAngle,
                    LoadingOrigin, LoadingScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(DefaultFont, "Loading", LoadingPos + new Vector2(-(DefaultFont.MeasureString("Loading")).X * 0.5f, Program.LoadingTexture[0].Width * 0.56f * LoadingScale), Color.White);
            }

            // Ends normal drawing and begins drawing with Camera
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        Graph.Camera.get_transformation(graphics.GraphicsDevice));

            // Draws all the node connections
            Graph.DPrimitives.Draw();

            // Draws all the nodes
            foreach(TInfoNodo nodo in Program.mPessoas.ProcuraNodo(true))
            {
                nodo.Update(gameTime);
                nodo.Draw(spriteBatch);
            }
            foreach (TInfoNodo nodo in Program.mProduções.ProcuraNodo(true))
            {
                nodo.Update(gameTime);
                nodo.Draw(spriteBatch);
            }
            foreach (TInfoNodo nodo in Program.mInstituições.ProcuraNodo(true))
            {
                nodo.Update(gameTime);
                nodo.Draw(spriteBatch);
            }
            foreach (TInfoNodo nodo in Program.mPeridódicos.ProcuraNodo(true))
            {
                nodo.Update(gameTime);
                nodo.Draw(spriteBatch);
            }
            foreach (TInfoNodo nodo in Program.mConferências.ProcuraNodo(true))
            {
                nodo.Update(gameTime);
                nodo.Draw(spriteBatch);
            }


            spriteBatch.End();
            spriteBatch.Begin();
            // Draws the FPS
            spriteBatch.DrawString(DefaultFont, "FPS: " + (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds), new Vector2(0f, 0f), Color.White);
            spriteBatch.DrawString(DefaultFont, "Camera: X=" + Graph.Camera.Pos.X.ToString() + " Y=" + Graph.Camera.Pos.Y.ToString() + " Z=" + Graph.Camera.Zoom.ToString(), new Vector2(00f, 20f), Color.White);
            spriteBatch.DrawString(DefaultFont, "Mouse: X=" + Graph.Camera.mousePos.X.ToString() + " Y=" + Graph.Camera.mousePos.Y.ToString(), new Vector2(00f, 40f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
