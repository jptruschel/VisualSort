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
        // GUI
        GUI MainGUI;
        GUI.TGUIPanel LeftPanel, InfoPanel;
        GUI.TGUIImgBtn LeftPanelHideButton1, LeftPanelHideButton2,
            InfoPanelHideButton1, InfoPanelHideButton2, imgDivider1;
        GUI.TGUIList LeftResultList;

        // Retorna true se o Foco (mouse) está no Grafo (meio da tela)
        public bool isFocusOnGraph()
        {
            return ((!LeftPanel.MouseOver || LeftPanel.Hidden) && (!InfoPanel.MouseOver || InfoPanel.Hidden));
        }

        // Funções de Clique da GUI
        // HideButtons
        public void InitializeHideButtons()
        {
            LeftPanelHideButton1.AutoSize = false;
            LeftPanelHideButton1.backgroundColor = Color.White * 0;
            LeftPanelHideButton1.BorderSize = 0;
            LeftPanelHideButton1.Image = GUI.ArrowBtns[0];
            LeftPanelHideButton1.HotImage = GUI.ArrowBtns[1];
            LeftPanelHideButton1.ClickImage = GUI.ArrowBtns[2];
            LeftPanelHideButton1.Size = new Vector2(GUI.ArrowBtns[0].Width * 0.36f, GUI.ArrowBtns[0].Height * 0.36f);
            LeftPanelHideButton1.DrawImageResized = true;
            LeftPanelHideButton1.spriteEffect = SpriteEffects.FlipHorizontally;
            LeftPanelHideButton1.useImage = true;
            LeftPanelHideButton1.useHotImage = true;
            LeftPanelHideButton1.useClickImage = true;
            LeftPanelHideButton1.OnClick = OnHideButtonLeft;
            LeftPanelHideButton1.OnMouseEnter = OnHideButtonMouseEnterLeft;
            LeftPanelHideButton1.OnMouseLeave = OnHideButtonMouseLeaveLeft;

            InfoPanelHideButton1.AutoSize = false;
            InfoPanelHideButton1.backgroundColor = Color.White * 0;
            InfoPanelHideButton1.BorderSize = 0;
            InfoPanelHideButton1.Image = GUI.ArrowBtns[0];
            InfoPanelHideButton1.HotImage = GUI.ArrowBtns[1];
            InfoPanelHideButton1.ClickImage = GUI.ArrowBtns[2];
            InfoPanelHideButton1.Size = new Vector2(GUI.ArrowBtns[0].Width * 0.36f, GUI.ArrowBtns[0].Height * 0.36f);
            InfoPanelHideButton1.DrawImageResized = true;
            InfoPanelHideButton1.useImage = true;
            InfoPanelHideButton1.useHotImage = true;
            InfoPanelHideButton1.useClickImage = true;
            InfoPanelHideButton1.OnClick = OnHideButtonInfo;
            InfoPanelHideButton1.OnMouseEnter = OnHideButtonMouseEnterInfo;
            InfoPanelHideButton1.OnMouseLeave = OnHideButtonMouseLeaveInfo;

            LeftPanelHideButton2.AutoSize = false;
            LeftPanelHideButton2.backgroundColor = Color.White * 0;
            LeftPanelHideButton2.BorderSize = 0;
            LeftPanelHideButton2.Image = GUI.ArrowBtns[0];
            LeftPanelHideButton2.HotImage = GUI.ArrowBtns[1];
            LeftPanelHideButton2.ClickImage = GUI.ArrowBtns[2];
            LeftPanelHideButton2.Size = new Vector2(GUI.ArrowBtns[0].Width * 0.36f, GUI.ArrowBtns[0].Height * 0.36f);
            LeftPanelHideButton2.DrawImageResized = true;
            LeftPanelHideButton2.spriteEffect = SpriteEffects.FlipHorizontally;
            LeftPanelHideButton2.useImage = true;
            LeftPanelHideButton2.useHotImage = true;
            LeftPanelHideButton2.useClickImage = true;
            LeftPanelHideButton2.OnClick = OnHideButtonLeft;
            LeftPanelHideButton2.OnMouseEnter = OnHideButtonMouseEnterLeft;
            LeftPanelHideButton2.OnMouseLeave = OnHideButtonMouseLeaveLeft;

            InfoPanelHideButton2.AutoSize = false;
            InfoPanelHideButton2.backgroundColor = Color.White * 0;
            InfoPanelHideButton2.BorderSize = 0;
            InfoPanelHideButton2.Image = GUI.ArrowBtns[0];
            InfoPanelHideButton2.HotImage = GUI.ArrowBtns[1];
            InfoPanelHideButton2.ClickImage = GUI.ArrowBtns[2];
            InfoPanelHideButton2.Size = new Vector2(GUI.ArrowBtns[0].Width * 0.36f, GUI.ArrowBtns[0].Height * 0.36f);
            InfoPanelHideButton2.DrawImageResized = true;
            InfoPanelHideButton2.useImage = true;
            InfoPanelHideButton2.useHotImage = true;
            InfoPanelHideButton2.useClickImage = true;
            InfoPanelHideButton2.OnClick = OnHideButtonInfo;
            InfoPanelHideButton2.OnMouseEnter = OnHideButtonMouseEnterInfo;
            InfoPanelHideButton2.OnMouseLeave = OnHideButtonMouseLeaveInfo;
        }
        public bool OnHideButtonLeft()
        {
            if (LeftPanel.Hidden)
            {
                LeftPanel.Show(10f);
            }
            else
            {
                LeftPanel.Hide(new Vector2(-LeftPanel.Size.X + 26, 0), 8f);
            }
            return true;
        }
        public bool OnHideButtonInfo()
        {
            if (InfoPanel.Hidden)
            {
                InfoPanel.Show(10f);
            }
            else
            {
                InfoPanel.Hide(new Vector2(InfoPanel.Size.X - 26, 0), 8f);
            }
            return true;
        }
        public bool OnHideButtonMouseEnterLeft()
        {
            LeftPanelHideButton1.Highlighted = true;
            LeftPanelHideButton2.Highlighted = true;
            return true;
        }
        public bool OnHideButtonMouseEnterInfo()
        {
            InfoPanelHideButton1.Highlighted = true;
            InfoPanelHideButton2.Highlighted = true;
            return true;
        }
        public bool OnHideButtonMouseLeaveLeft()
        {
            LeftPanelHideButton1.Highlighted = false;
            LeftPanelHideButton2.Highlighted = false;
            return true;
        }
        public bool OnHideButtonMouseLeaveInfo()
        {
            InfoPanelHideButton1.Highlighted = false;
            InfoPanelHideButton2.Highlighted = false;
            return true;
        }

        // Inicializa o XNA
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
        // Inicializa tudo necessário
        protected override void Initialize()
        {
            isLoading = true;

            // Inicializa a entrada de input do teclado para Edits da GUI
            EventInput.Initialize(this.Window);
            
            // Inicializa a GUI
            MainGUI = new GUI();

            LeftPanel = MainGUI.NewPanel(
                new Vector2(0, 0),
                new Vector2(200,
                    graphics.PreferredBackBufferHeight));
            InfoPanel = MainGUI.NewPanel(
                new Vector2(graphics.PreferredBackBufferWidth - (200), 0),
                new Vector2(200, graphics.PreferredBackBufferHeight));
            //LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(50,300), new Vector2(100, 50), "Button1"));
            //LeftPanel.AddComponent(new GUI.TGUILabel(new Vector2(50, 350), new Vector2(100, 50), "Label1"));
            LeftResultList = LeftPanel.AddComponent(new GUI.TGUIList(new Vector2(50, 450), new Vector2(150, 400))) as GUI.TGUIList;
            for (int i = 0; i < 500; i++)
                (LeftResultList as GUI.TGUIList).Add("elemento " + i);
            //LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(50, 250), new Vector2(100, 50), "Checkbox1"));
            LeftPanel.AddComponent(new GUI.TGUILabel(new Vector2(8, 5), new Vector2(100, 50), "Visual Sort")).Font = 0;
            LeftPanelHideButton1 = (LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(LeftPanel.Size.X - 12, 8), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            InfoPanelHideButton1 = (InfoPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(1, 8), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            LeftPanelHideButton2 = (LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(LeftPanel.Size.X - 12, LeftPanel.Size.Y - 12), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            InfoPanelHideButton2 = (InfoPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(1, InfoPanel.Size.Y - 12), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            LeftPanel.AddComponent(new GUI.TGUIEditBox(new Vector2(50, 200), new Vector2(100,25), "Edit1"));
            InfoPanel.Hide(new Vector2(InfoPanel.Size.X - 26, 0), 1f);
            imgDivider1 = LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 42), new Vector2(LeftPanel.Size.X - 4, 8), "")) as GUI.TGUIImgBtn;
            imgDivider1.ImageColor = Color.Black;
            imgDivider1.BorderSize = 0;
            imgDivider1.AutoSize = false;
            imgDivider1.backgroundColor = Color.Black * 0f;
            imgDivider1.DrawImageResized = true;

            // Inicializa o modo de visualização
            Program.ViewMode = 0;
            // Inicializa as listas de nodos a serem desenhados
            AppGraphics.DrawNodos = new List<TDrawNodo>();
            AppGraphics.MaxNodos = new List<TDrawMaxNodo>();
            Program.maxNodoSelecionado = 0;
            
            // Renderizador de primitivas
            AppGraphics.DPrimitives = new PrimitiveRenderer(GraphicsDevice);

            // Calcula o centro da tela
            AppGraphics.ScreenCenter = new Vector2(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.5f);

            // Inicializa a Câmera
            AppGraphics.Camera = new Camera2d(graphics.GraphicsDevice);
            AppGraphics.Camera.Pos = new Vector2(0f, 0f);

            // Estado do Mouse
            mouseState = new MouseState();

            base.Initialize();
            isLoading = false;
        }

        // Carrega texturas, fontes, sons, etc
        protected override void LoadContent()
        {
            isLoading = true;
  
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Carrega a GUI
            GUI.ArrowBtns = new Texture2D[3];
            GUI.ArrowBtns[0] = Content.Load<Texture2D>("ArrowBtn");
            GUI.ArrowBtns[1] = Content.Load<Texture2D>("ArrowBtnMouse");
            GUI.ArrowBtns[2] = Content.Load<Texture2D>("ArrowBtnClick");
            GUI.Arrow2 = Content.Load<Texture2D>("Arrow2");
            GUI.wBox = Content.Load<Texture2D>("wbox");
            GUI.stLine = Content.Load<Texture2D>("Line");
            GUI.CheckTex = Content.Load<Texture2D>("Check");
            GUI.Fonts = new SpriteFont[4];
            GUI.Fonts[0] = Content.Load<SpriteFont>("TitleFont");
            GUI.Fonts[1] = Content.Load<SpriteFont>("TextFont");
            GUI.Fonts[2] = Content.Load<SpriteFont>("ListFont");
            GUI.Fonts[3] = Content.Load<SpriteFont>("BigTextFont");
            InitializeHideButtons();
            imgDivider1.Image = GUI.stLine;
            imgDivider1.useImage = true;

            // Load the Sprites
            AppGraphics.NodoTex = Content.Load<Texture2D>("Nodo512");
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

            // Carrega o XML

            // Example
            Program.mPessoas.NovoNodo(new TInfoNodo("Leandro Krug Wives", new BPos(0,0)));
          //  Program.mPessoas.NovoNodo(new TInfoNodo("Aline", new BPos(0, 1)));
          //  Program.mPessoas.NovoNodo(new TInfoNodo("Mara Abel", new BPos(0, 2)));
           // Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(1,0));
           // Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(2, 0));
            for (int i = 1; i < 500; i++)
            {
                Program.mPessoas.NovoNodo(new TInfoNodo("n" + i, new BPos(0, 0)));
                //Program.GetNodoFromLists(new TPNodo(0, 0)).AdicionaLigaçãoCom(new TPNodo(i, 0));
                for (int j = 0; j < i; j++)
                {
                    Program.GetNodoFromLists(new TPNodo(j, 0)).AdicionaLigaçãoCom(new TPNodo(i, 0));
                    Program.GetNodoFromLists(new TPNodo(i, 0)).AdicionaLigaçãoCom(new TPNodo(j, 0));
                }
            }
            AppGraphics.SelecionaNodoComVoltar(Program.mPessoas[0].Nodo, -1);
            isLoading = false;
        }

        // Descarrega tudo - não necessário
        protected override void UnloadContent()
        {
        }

        // Atualiza o programa (em uma taxa constante, invariável do fps)
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
            // Somente considera algo se o mouse não está em um dos panels (e ele não está escondido)
            if (isFocusOnGraph())
            {
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
                                {
                                    AppGraphics.SelecionaNodoComVoltar(Program.NodoMouse.InfoNodo.Nodo, Program.MaxNodoSelecionado);
                                }
                }
                if (mouseTimer < 15)
                    mouseTimer++;
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
            }
            oldMouseState = mouseState;
            oldKeyboardState = keyboardState;

            // Update GUI
            LeftPanel.Update(mouseState);
            InfoPanel.Update(mouseState);

            // Update Camer Info (mouse and limits)
            AppGraphics.Camera.UpdateCameraInfo(mouseState);

            base.Update(gameTime);
        }

        // Desenha tudo que deve ser desenhado na tela a cada frame. Tenta ser constante a 60fps
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

            // Desenha todos os maxNodos
            foreach (TDrawMaxNodo dMaxNodo in AppGraphics.MaxNodos)
            {
                if (dMaxNodo.Drawable)
                {
                    dMaxNodo.Update(gameTime, isFocusOnGraph());
                    dMaxNodo.Draw(spriteBatch);
                }
            }

            spriteBatch.End();
            // Desenho sem transformação da câmera

            spriteBatch.Begin();

            // Nome do nodo com mouse em cima
            if ((Program.NodoMouse != null) && (Program.NodoMouse.MouseOver == true))
            {
                Vector2 DPos = Vector2.Transform(Program.NodoMouse.Pos, AppGraphics.Camera.get_transformation());
                Vector2 NameSize = GUI.Fonts[3].MeasureString(Program.NodoMouse.InfoNodo.Nome);
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
                spriteBatch.DrawString(GUI.Fonts[3], Program.NodoMouse.InfoNodo.Nome, DPos, Color.White);
            }

            // Desenha a GUI
            LeftPanel.Draw(spriteBatch);
            InfoPanel.Draw(spriteBatch);

            // Draws the FPS
            spriteBatch.DrawString(DefaultFont, "FPS: " + (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds), new Vector2(LeftPanel.Pos.X + LeftPanel.Size.X + 2, 0f), Color.White);
            spriteBatch.DrawString(DefaultFont, "Camera: X=" + AppGraphics.Camera.Pos.X.ToString() + " Y=" + AppGraphics.Camera.Pos.Y.ToString() + " Z=" + AppGraphics.Camera.Zoom.ToString() + " R=" + AppGraphics.Camera.Rotation.ToString(), new Vector2(LeftPanel.Pos.X + LeftPanel.Size.X + 2, 20f), Color.White);
            spriteBatch.DrawString(DefaultFont, "Mouse: X=" + AppGraphics.Camera.mousePos.X.ToString() + " Y=" + AppGraphics.Camera.mousePos.Y.ToString() + " l:" + AppGraphics.DPrimitives.Lines.Count, new Vector2(LeftPanel.Pos.X + LeftPanel.Size.X + 2, 40f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
