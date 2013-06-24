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
        MouseState mouseState, oldMouseState;
        int mouseTimer;
        float prevWheelValue;
        float currWheelValue;

        public Renderer()
        {
            graphics = new GraphicsDeviceManager(this);
            // Deixemos assim até que coloquemos full screen com toda a parte de resolução certinho
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            //graphics.IsFullScreen = true;
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
            
            // Inicializa o modo de visualização
            Program.ViewMode = 0;
            AppGraphics.MaxNodos = new List<TDrawMaxNodo>();
            AppGraphics.MaxNodos.Add(new TDrawMaxNodo(null));
            Program.maxNodoSelecionado = 0;

            AppGraphics.DPrimitives = new PrimitiveRenderer(GraphicsDevice);

            AppGraphics.ScreenCenter = new Vector2(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.5f);

            AppGraphics.Camera = new Camera2d(graphics.GraphicsDevice);
            AppGraphics.Camera.Pos = new Vector2(0f, 0f);

            mouseState = new MouseState();
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
            AppGraphics.NodoTex = Content.Load<Texture2D>("Nodo");
            AppGraphics.BoxTex = Content.Load<Texture2D>("box");
            // Initializes the Loading Circle
            AppGraphics.LoadingTexture = new Texture2D[5];
            AppGraphics.LoadingTexture[0] = Content.Load<Texture2D>("Loading1");
            AppGraphics.LoadingTexture[1] = Content.Load<Texture2D>("Loading2");
            AppGraphics.LoadingTexture[2] = Content.Load<Texture2D>("Loading3");
            AppGraphics.LoadingTexture[3] = Content.Load<Texture2D>("Loading4");
            AppGraphics.LoadingTexture[4] = Content.Load<Texture2D>("loading5");
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            LoadingOrigin.X = AppGraphics.LoadingTexture[0].Width / 2;
            LoadingOrigin.Y = AppGraphics.LoadingTexture[1].Height / 2;
            LoadingScale = 0.64f;
            LoadingAlpha = 0.8f;
            LoadingPos.X = viewport.Width / 2f;// - (LoadingScale * LoadingTexture1.Width * 0.64f);
            LoadingPos.Y = viewport.Height * 0.4f;//LoadingScale * LoadingTexture1.Height * 0.64f;

            // Font
            DefaultFont = Content.Load<SpriteFont>("DefaultFont");
            AppGraphics.TextFont = Content.Load<SpriteFont>("TextFont");

            // Carrega o XML

            // Example
            Program.mPessoas.NovoNodo(new TInfoNodo("Leandro Krug Wives", new BPos(0,0)));
          //  Program.mPessoas.NovoNodo(new TInfoNodo("Aline", new BPos(0, 1)));
          //  Program.mPessoas.NovoNodo(new TInfoNodo("Mara Abel", new BPos(0, 2)));
           // Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(1,0));
           // Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(2, 0));
            for (int i = 1; i < 50; i++)
            {
                Program.mPessoas.NovoNodo(new TInfoNodo("n" + i, new BPos(0, 0)));
                //Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(i, 0));
                for (int j = 0; j < i; j++)
                {
                    Program.GetNodoFromLists(new TPNodo(j, 0)).AdicionaLigaçãoCom(new TPNodo(i, 0));
                    Program.GetNodoFromLists(new TPNodo(i, 0)).AdicionaLigaçãoCom(new TPNodo(j, 0));
                }
            }
            AppGraphics.SelecionaNodo(new TPNodo(0, 0), 0);

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
            mouseState = Mouse.GetState();
            prevWheelValue = currWheelValue;
            currWheelValue = mouseState.ScrollWheelValue;
            AppGraphics.Camera.Zoom = AppGraphics.Camera.Zoom + (currWheelValue - prevWheelValue) * 0.001f * (AppGraphics.Camera.Zoom);
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (oldMouseState.RightButton == ButtonState.Pressed)
                {
                    AppGraphics.Camera.Move(
                        (new Vector2(
                            oldMouseState.X,
                            oldMouseState.Y) - 
                         new Vector2(
                             mouseState.X,
                             mouseState.Y))
                         * (1 / AppGraphics.Camera.Zoom));
                }
            }
            // Double Click Timer
            if (mouseState.LeftButton == ButtonState.Released)
            {
                if (oldMouseState.LeftButton == ButtonState.Pressed)
                {
                    mouseTimer = 0;
                }
            }
            // Verifica Double Click - seleciona, se houver
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (oldMouseState.LeftButton == ButtonState.Released)
                    if (mouseTimer < 10f)
                        if (Program.NodoMouse != null)
                            if (Program.NodoMouse.MouseOver == true)
                                AppGraphics.SelecionaNodo(Program.NodoMouse.Nodo, 0);
            }
            if (mouseTimer < 15)
                mouseTimer++;
            oldMouseState = mouseState;
            // Move camera
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    AppGraphics.Camera.Rotation += 0.02f;
                else
                {
                    AppGraphics.Camera.Move(
                        new Vector2(
                            -1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Cos(AppGraphics.Camera.Rotation)),
                            1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Sin(AppGraphics.Camera.Rotation))));
                }
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    AppGraphics.Camera.Rotation -= 0.02f;
                else
                {
                    AppGraphics.Camera.Move(
                        new Vector2(
                            1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Cos(AppGraphics.Camera.Rotation)),
                            -1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Sin(AppGraphics.Camera.Rotation))));
                }
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    AppGraphics.Camera.Zoom += 0.01f;
                else
                {
                    AppGraphics.Camera.Move(
                        new Vector2(
                            -1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Sin(AppGraphics.Camera.Rotation)),
                            -1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Cos(AppGraphics.Camera.Rotation))));
                }
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                    AppGraphics.Camera.Zoom -= 0.01f;
                else
                {
                    AppGraphics.Camera.Move(
                        new Vector2(
                            1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Sin(AppGraphics.Camera.Rotation)),
                            1.0f * (10 / AppGraphics.Camera.Zoom) * (float)(Math.Cos(AppGraphics.Camera.Rotation))));
                }
            }
            oldKeyboardState = keyboardState;
            //Graph.Camera.Rotation += 0.01f;

            // Update Camer Info (mouse and limits)
            AppGraphics.Camera.UpdateCameraInfo(mouseState);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(new Color(10, 35, 114));//Color.DarkSlateGray);
            //GraphicsDevice.Clear(new Color(0, 25, 014));
            GraphicsDevice.Clear(new Color(0, 14, 25));
            spriteBatch.Begin();

            // Draws the Loading Circle
            if (isLoading)
            {
                spriteBatch.Draw(AppGraphics.LoadingTexture[2], LoadingPos, null, Color.White * LoadingAlpha * 0.6f, 0.0f,
                    LoadingOrigin, LoadingScale*0.96f, SpriteEffects.None, 0f);
                spriteBatch.Draw(AppGraphics.LoadingTexture[3], LoadingPos, null, Color.White * LoadingAlpha, -LoadingAngle * 0.5f,
                    LoadingOrigin, LoadingScale * 0.96f, SpriteEffects.None, 0f);
                spriteBatch.Draw(AppGraphics.LoadingTexture[0], LoadingPos, null, Color.White * LoadingAlpha, -LoadingAngle,
                    LoadingOrigin, LoadingScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(AppGraphics.LoadingTexture[1], LoadingPos, null, Color.White * LoadingAlpha, LoadingAngle,
                    LoadingOrigin, LoadingScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(DefaultFont, "Loading", LoadingPos + new Vector2(-(DefaultFont.MeasureString("Loading")).X * 0.5f, AppGraphics.LoadingTexture[0].Width * 0.56f * LoadingScale), Color.White);
            }

            // Ends normal drawing and begins drawing with Camera
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        AppGraphics.Camera.get_transformation());

            // Draws all the node connections
            AppGraphics.DPrimitives.Draw();

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
            // Desenho sem transformação da câmera

            spriteBatch.Begin();

            // Nome do nodo com mouse em cima
            if ((Program.NodoMouse != null) && (Program.NodoMouse.MouseOver == true))
            {
                Vector2 DPos = Vector2.Transform(Program.NodoMouse.Pos, AppGraphics.Camera.get_transformation());
                Vector2 NameSize = AppGraphics.TextFont.MeasureString(Program.NodoMouse.Nome);
                DPos.X += (AppGraphics.DefaultNodeSize*0.5f * AppGraphics.Camera.Zoom);
                DPos.Y -= NameSize.Y * 0.5f;
                spriteBatch.Draw(AppGraphics.BoxTex,
                    new Rectangle(
                        (int)(DPos.X),
                        (int)(DPos.Y),
                        (int)(NameSize.X),
                        (int)(NameSize.Y)),
                    null,
                    Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
                spriteBatch.DrawString(AppGraphics.TextFont, Program.NodoMouse.Nome, DPos, Color.White);
            }
            // Draws the FPS
            spriteBatch.DrawString(DefaultFont, "FPS: " + (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds), new Vector2(0f, 0f), Color.White);
            spriteBatch.DrawString(DefaultFont, "Camera: X=" + AppGraphics.Camera.Pos.X.ToString() + " Y=" + AppGraphics.Camera.Pos.Y.ToString() + " Z=" + AppGraphics.Camera.Zoom.ToString() + " R=" + AppGraphics.Camera.Rotation.ToString(), new Vector2(00f, 20f), Color.White);
            spriteBatch.DrawString(DefaultFont, "Mouse: X=" + AppGraphics.Camera.mousePos.X.ToString() + " Y=" + AppGraphics.Camera.mousePos.Y.ToString(), new Vector2(00f, 40f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
