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
        bool mouseTimerClicked;
        float prevWheelValue;
        float currWheelValue;
        // GUI
        GUI MainGUI;
        public static GUI.TGUIPanel LeftPanel, RightPanel, FilterPanel;
        GUI.TGUIImgBtn LeftPanelHideButton1, LeftPanelHideButton2,
            InfoPanelHideButton1, InfoPanelHideButton2, imgDivider1,
            imgDivider2, imgDivider11, LeftSearchButton/*, LeftSeachButtonGraphar*/;
        public static GUI.TGUICheckBox[] GraphViewCheckBoxs;
        public static GUI.TGUIImgBtn FilterButton;
        GUI.TGUICheckBox[] FilterCheckBoxs;
        GUI.TGUIList LeftResultList;
        GUI.TGUIEditBox LeftSearchEdit;
        public static GUI.TGUICheckBox LeftSearchCheckBoxExato, LeftSearchCheckBoxShowOnGraph;
        public static List<TInfoNodo> SearchResults;

        public static GUI.TGUILabel rInfoTipoLista;
        public static GUI.TGUIList rInfoLista;
        public static GUI.TGUIMemo rInfoInfos;
        public static GUI.TGUIImgBtn rInfoOrgOpenPanel;
        public static GUI.TGUIPanel rInfoOrganPan;
        public static GUI.TGUIImgBtn[] rInfoOrganBut;
        public static List<TFArtigo> ArtigosSorts;
        public static List<TInfoNodo> ArtigosSortsI;
       
        // Retorna true se o Foco (mouse) está no Grafo (meio da tela)
        public bool isFocusOnGraph()
        {
            return ((!LeftPanel.MouseOver || LeftPanel.Hidden) && 
                (!RightPanel.MouseOver || RightPanel.Hidden) &&
                (!LeftSearchEdit.hasFocus) && (!LeftSearchEdit.mouseOver) &&
                (LeftResultList.xOffsetTextSize == 0) && (!rInfoOrganPan.Visible));
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
        public bool OnHideButtonLeft(int Tag)
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
        public bool OnHideButtonInfo(int Tag)
        {
            if (RightPanel.Hidden)
            {
                RightPanel.Show(10f);
            }
            else
            {
                RightPanel.Hide(new Vector2(RightPanel.Size.X - 26, 0), 8f);
            }
            return true;
        }
        public bool OnHideButtonMouseEnterLeft(int Tag)
        {
            LeftPanelHideButton1.Highlighted = true;
            LeftPanelHideButton2.Highlighted = true;
            return true;
        }
        public bool OnHideButtonMouseEnterInfo(int Tag)
        {
            InfoPanelHideButton1.Highlighted = true;
            InfoPanelHideButton2.Highlighted = true;
            return true;
        }
        public bool OnHideButtonMouseLeaveLeft(int Tag)
        {
            LeftPanelHideButton1.Highlighted = false;
            LeftPanelHideButton2.Highlighted = false;
            return true;
        }
        public bool OnHideButtonMouseLeaveInfo(int Tag)
        {
            InfoPanelHideButton1.Highlighted = false;
            InfoPanelHideButton2.Highlighted = false;
            return true;
        }
        // Botão de procura
        public bool SearchButtonOnClick(int Tag)
        {
            foreach (TInfoNodo aNodo in SearchResults)
                aNodo.Pesquisado = false;
            LeftResultList.Items.Clear();
            LeftSearchCheckBoxShowOnGraph.Enabled = (SearchResults.Count > 0);
            //LeftSeachButtonGraphar.Enabled = (SearchResults.Count > 0);
            isLoading = true;
            LeftResultList.ItemIndex = -1;
            SearchResults.Clear();

            for (int Tip = 0; Tip < 6; Tip++)
                if (FilterCheckBoxs[Tip].Checked)
                    foreach (TInfoNodo nNodo in Program.mListas[Tip].ProcuraNodo(LeftSearchEdit.Text, LeftSearchCheckBoxExato.Checked, true, true, true))
                    {
                        SearchResults.Add(nNodo);
                        nNodo.Pesquisado = true;
                    }

            // Listar
            foreach (TInfoNodo nNodo in SearchResults)
            {
                LeftResultList.Items.Add(nNodo.Nome + " (" + nNodo.Nodo.Tipo.ToString() + ")");
            }
            LeftSearchCheckBoxShowOnGraph.Enabled = (SearchResults.Count > 0);
            //LeftSeachButtonGraphar.Enabled = (SearchResults.Count > 0);
            isLoading = false;
            return true;
        }
        // Clicar na lista
        public bool SearchResultsListOnClick(int arg)
        {
            if (arg != -1)
            {
                AppGraphics.SelecionaNodoComVoltar(SearchResults[arg].Nodo, -1);
                MouseNodeHandler.UnMouseOver();
            }
            return true;
        }
        // Mostrar/esconder panel de filtros
        public static bool ToggleFilterPanel(int Tag)
        {
            if (FilterPanel.Visible)
            {
                FilterPanel.Visible = false;
                FilterButton.Text = "Filtros  | ";
            }
            else
            {
                FilterPanel.Visible = true;
                FilterButton.Text = "Filtros  | ";
            }
            return true;
        }
        // Mostrar/esconder panel de organização
        public static bool ToggleInfoListOrgPanel(int Tag)
        {
            if (rInfoOrganPan.Visible)
            {
                rInfoOrganPan.Visible = false;
            }
            else
            {
                rInfoOrganPan.Visible = true;
            }
            return true;
        }
        // Organizar a lista
        public static bool OrganizeLeftOrgPanel(int Tag)
        {
            rInfoOrganPan.Visible = false;
            rInfoLista.Items.Clear();

            switch (Tag)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        break;
                    }
                case 3:
                    {
                        TFArtigo.SortListQualis(ArtigosSorts);
                        foreach (TFArtigo sAr in ArtigosSorts)
                            if (sAr.Qualis == "i")
                                rInfoLista.Add("XX- "+ sAr.Título);
                            else
                                rInfoLista.Add(sAr.Qualis + "- " + sAr.Título);
                        break;
                    }
                case 4:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return true;
        }
        // Redesenha o grafo
        public static bool LeftChangeGraphFilterCheckClick(int Tag)
        {
            bool Do = false;
            for (int i = 0; i < 6; i++)
                if (GraphViewCheckBoxs[i].Changed == true)
                {
                    GraphViewCheckBoxs[i].Changed = false;
                    Do = true;
                }
            if (Do)
            {
                AppGraphics.MaxNodos[MouseNodeHandler.MaxNodoSelecionado].Esconder();
                AppGraphics.MaxNodos[MouseNodeHandler.MaxNodoSelecionado].Ressurgir();
            }
                //AppGraphics.MaxNodos[MouseNodeHandler.MaxNodoSelecionado].Inicializa();
            return true;
        }

        // Inicializa o XNA
        public Renderer()
        {
            graphics = new GraphicsDeviceManager(this);
            // Deixemos assim até que coloquemos full screen com toda a parte de resolução certinho
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
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

            // Listas
            Program.mListas = new TBigList[6];
            Program.mListas[0] = Program.mPessoas;
            Program.mListas[1] = Program.mArtigos;
            Program.mListas[2] = Program.mLivros;
            Program.mListas[3] = Program.mPeriódicos;
            Program.mListas[4] = Program.mCapítulos;
            Program.mListas[5] = Program.mConferências;
            SearchResults = new List<TInfoNodo>();
            
            // Inicializa a GUI
            MainGUI = new GUI();

            LeftPanel = MainGUI.NewPanel(
                new Vector2(0, 0),
                new Vector2(250,
                    graphics.PreferredBackBufferHeight));
            RightPanel = MainGUI.NewPanel(
                new Vector2(graphics.PreferredBackBufferWidth - (250), 0),
                new Vector2(250, graphics.PreferredBackBufferHeight));
            RightPanel.color = Color.CornflowerBlue;
            // LeftPanel
            LeftPanel.AddComponent(new GUI.TGUILabel(new Vector2(5, 55), Vector2.Zero, "Mostrar Ligações com: "));
            GraphViewCheckBoxs = new GUI.TGUICheckBox[6];
            GraphViewCheckBoxs[0] = LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(7, 80), Vector2.Zero, " Pessoas")) as GUI.TGUICheckBox;
            GraphViewCheckBoxs[1] = LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(125, 80), Vector2.Zero, " Artigos")) as GUI.TGUICheckBox;
            GraphViewCheckBoxs[2] = LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(7, 102), Vector2.Zero, " Livros")) as GUI.TGUICheckBox;
            GraphViewCheckBoxs[3] = LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(125, 102), Vector2.Zero, " Periódicos")) as GUI.TGUICheckBox;
            GraphViewCheckBoxs[4] = LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(7, 124), Vector2.Zero, " Capítulos")) as GUI.TGUICheckBox;
            GraphViewCheckBoxs[5] = LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(125, 124), Vector2.Zero, " Conferências")) as GUI.TGUICheckBox;
            for (int i = 0; i < 6; i++)
            {
                GraphViewCheckBoxs[i].Checked = true;
                GraphViewCheckBoxs[i].transparentBackground = false;
                GraphViewCheckBoxs[i].backgroundColor = Color.White * 0.2f;
            }
            LeftResultList = LeftPanel.AddComponent(new GUI.TGUIList(new Vector2(10, 312-65), new Vector2(LeftPanel.Size.X - 15, LeftPanel.Size.Y-353+65/*547*/))) as GUI.TGUIList;
            LeftResultList.Font = 5;
            LeftResultList.OnClick = SearchResultsListOnClick;
            LeftSearchEdit = LeftPanel.AddComponent(new GUI.TGUIEditBox(new Vector2(5, 185), new Vector2(LeftPanel.Size.X - 10, 25), "Termos da pesquisa")) as GUI.TGUIEditBox;
            LeftSearchButton = LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(LeftPanel.Size.X - 155, 214), new Vector2(), "      Pesquisar!  ")) as GUI.TGUIImgBtn;
            LeftSearchButton.BorderSize = 1;
            LeftSearchButton.AutoSize = false;
            LeftSearchButton.Size = new Vector2(150, 22);
            LeftSearchButton.Font = 4;
            LeftSearchButton.OnClick = SearchButtonOnClick;
            LeftSearchButton.backgroundColor = Color.GreenYellow;
            FilterButton = LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(5, 279-65), new Vector2(0, 0), " Filtros  | ")) as GUI.TGUIImgBtn;
            FilterButton.BorderSize = 1;
            FilterButton.backgroundColor = Color.OrangeRed * 0.86f;
            FilterButton.OnClick = ToggleFilterPanel;

            LeftPanel.AddComponent(new GUI.TGUILabel(new Vector2(8, 5), new Vector2(100, 50), "Visual Sort")).Font = 0;
            LeftPanelHideButton1 = (LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(LeftPanel.Size.X - 12, 8), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            InfoPanelHideButton1 = (RightPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(1, 8), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            LeftPanelHideButton2 = (LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(LeftPanel.Size.X - 12, LeftPanel.Size.Y - 12), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            InfoPanelHideButton2 = (RightPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(1, RightPanel.Size.Y - 12), new Vector2(100, 50), "")) as GUI.TGUIImgBtn);
            //RightPanel.Hide(new Vector2(RightPanel.Size.X - 26, 0), 1f);
            imgDivider1 = LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 42), new Vector2(LeftPanel.Size.X - 4, 8), "")) as GUI.TGUIImgBtn;
            imgDivider1.ImageColor = Color.Black;
            imgDivider1.BorderSize = 0;
            imgDivider1.AutoSize = false;
            imgDivider1.backgroundColor = Color.Black * 0f;
            imgDivider1.DrawImageResized = true;

            imgDivider11 = LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 170), new Vector2(LeftPanel.Size.X - 4, 8), "")) as GUI.TGUIImgBtn;
            imgDivider11.ImageColor = Color.Black;
            imgDivider11.BorderSize = 0;
            imgDivider11.AutoSize = false;
            imgDivider11.backgroundColor = Color.Black * 0f;
            imgDivider11.DrawImageResized = true;

            FilterPanel = MainGUI.NewPanel(
                new Vector2(5, 305) + new Vector2(FilterButton.Size.X,  -65),
                new Vector2(125, 161));
            FilterPanel.Visible = false;
            FilterPanel.color = new Color(195, 80, 0);//Color.DarkOrange;
            FilterPanel.backgroundAlpha = 0.99f;
            FilterCheckBoxs = new GUI.TGUICheckBox[6];
            FilterCheckBoxs[0] = FilterPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(2, 0), Vector2.Zero, "Pessoas")) as GUI.TGUICheckBox;
            FilterCheckBoxs[1] = FilterPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(2, 22), Vector2.Zero, "Artigos")) as GUI.TGUICheckBox;
            FilterCheckBoxs[2] = FilterPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(2, 44), Vector2.Zero, "Livros")) as GUI.TGUICheckBox;
            FilterCheckBoxs[3] = FilterPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(2, 66), Vector2.Zero, "Periódicos")) as GUI.TGUICheckBox;
            FilterCheckBoxs[4] = FilterPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(2, 88), Vector2.Zero, "Capítulos")) as GUI.TGUICheckBox;
            FilterCheckBoxs[5] = FilterPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(2, 110), Vector2.Zero, "Conferências")) as GUI.TGUICheckBox;
            for (int i = 0; i < 6; i++)
                FilterCheckBoxs[i].Checked = true;
            LeftSearchCheckBoxExato = FilterPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(2, 138), new Vector2(LeftPanel.Size.X - 10, 25), "Busca exata")) as GUI.TGUICheckBox;
            LeftSearchCheckBoxExato.Font = 1;
            LeftSearchCheckBoxExato.TextColor = Color.Black;
            LeftSearchCheckBoxExato.Checked = false;
            LeftSearchCheckBoxShowOnGraph = LeftPanel.AddComponent(new GUI.TGUICheckBox(new Vector2(5, LeftResultList.Pos.Y + LeftResultList.Size.Y + 3), new Vector2(0, 0), "Highlight")) as GUI.TGUICheckBox;
            LeftSearchCheckBoxShowOnGraph.Font = 1;
            LeftSearchCheckBoxShowOnGraph.backgroundColor = Color.Azure * 0.5f;
            LeftSearchCheckBoxShowOnGraph.transparentBackground = false;
            LeftSearchCheckBoxShowOnGraph.Enabled = false;
            //LeftSeachButtonGraphar = LeftPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(LeftPanel.Size.X - 145 - 5, LeftResultList.Pos.Y + LeftResultList.Size.Y + 3), new Vector2(145, 25), "Grafar Pesquisa")) as GUI.TGUIImgBtn;
            //LeftSeachButtonGraphar.AutoSize = false;
            //LeftSeachButtonGraphar.Font = 4;
            //LeftSeachButtonGraphar.Enabled = false;

            // RightPanel
            RightPanel.AddComponent(new GUI.TGUILabel(new Vector2(30, 5), new Vector2(100, 50), "Informações")).Font = 0;
            imgDivider2 = RightPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 42), new Vector2(RightPanel.Size.X - 4, 8), "")) as GUI.TGUIImgBtn;
            imgDivider2.ImageColor = Color.Black;
            imgDivider2.BorderSize = 0;
            imgDivider2.AutoSize = false;
            imgDivider2.backgroundColor = Color.Black * 0f;
            imgDivider2.DrawImageResized = true;
            rInfoInfos = RightPanel.AddComponent(new GUI.TGUIMemo(new Vector2(5, 60), new Vector2(RightPanel.Size.X - 10, 345))) as GUI.TGUIMemo;
            rInfoInfos.Font = 5;
            rInfoTipoLista = RightPanel.AddComponent(new GUI.TGUILabel(new Vector2(5, 415), Vector2.Zero, "N/A")) as GUI.TGUILabel;
            rInfoTipoLista.transparentBackground = true;
            rInfoLista = RightPanel.AddComponent(new GUI.TGUIList(new Vector2(5, 435), new Vector2(RightPanel.Size.X - 10, RightPanel.Size.Y - 470))) as GUI.TGUIList;
            rInfoLista.ResizeOnMouse = false;
            rInfoLista.Font = 5;
            rInfoOrgOpenPanel = RightPanel.AddComponent(new GUI.TGUIImgBtn(new Vector2(15, RightPanel.Size.Y - 30), new Vector2(), "|  Chave de Organização")) as GUI.TGUIImgBtn;
            rInfoOrgOpenPanel.BorderSize = 1;
            rInfoOrgOpenPanel.backgroundColor = Color.DarkOrange * 1.0f;
            rInfoOrgOpenPanel.OnClick = ToggleInfoListOrgPanel;
            rInfoOrganPan = MainGUI.NewPanel(
                new Vector2(RightPanel.Pos.X - RightPanel.Size.X+130, rInfoOrgOpenPanel.Pos.Y - 90),
                new Vector2(120, 110));
            rInfoOrganPan.Visible = false;
            rInfoOrganPan.color = new Color(195, 80, 0);//Color.DarkOrange;
            rInfoOrganPan.backgroundAlpha = 0.99f;
            rInfoOrganBut = new GUI.TGUIImgBtn[5];
            rInfoOrganBut[0] = rInfoOrganPan.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 0), Vector2.Zero, "Tipo")) as GUI.TGUIImgBtn;
            rInfoOrganBut[1] = rInfoOrganPan.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 22), Vector2.Zero, "Natureza")) as GUI.TGUIImgBtn;
            rInfoOrganBut[2] = rInfoOrganPan.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 44), Vector2.Zero, "Nº Coautores")) as GUI.TGUIImgBtn;
            rInfoOrganBut[3] = rInfoOrganPan.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 66), Vector2.Zero, "QUALIS")) as GUI.TGUIImgBtn;
            rInfoOrganBut[4] = rInfoOrganPan.AddComponent(new GUI.TGUIImgBtn(new Vector2(2, 88), Vector2.Zero, "Ano Publicação")) as GUI.TGUIImgBtn;
            for (int i = 0; i < 5; i++)
            {
                rInfoOrganBut[i].BorderSize = 1;
                rInfoOrganBut[i].OnClick = OrganizeLeftOrgPanel;
                rInfoOrganBut[i].Tag = i;
            }

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
            GUI.Fonts = new SpriteFont[6];
            GUI.Fonts[0] = Content.Load<SpriteFont>("TitleFont");
            GUI.Fonts[1] = Content.Load<SpriteFont>("TextFont");
            GUI.Fonts[2] = Content.Load<SpriteFont>("ListFont");
            GUI.Fonts[3] = Content.Load<SpriteFont>("BigTextFont");
            GUI.Fonts[4] = Content.Load<SpriteFont>("BoldTextFont");
            GUI.Fonts[5] = Content.Load<SpriteFont>("SmallListFont");
            InitializeHideButtons();
            imgDivider1.Image = GUI.stLine;
            imgDivider1.useImage = true;
            imgDivider11.Image = GUI.stLine;
            imgDivider11.useImage = true;
            imgDivider2.Image = GUI.stLine;
            imgDivider2.useImage = true;

            // Load the Sprites
            AppGraphics.NodoTex = Content.Load<Texture2D>("Nodo512");
            AppGraphics.BoxTex = Content.Load<Texture2D>("box");
            AppGraphics.GlowTex = Content.Load<Texture2D>("Glow");
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
            LoadingPos.X = viewport.Width / 2f; //- (LoadingScale * AppGraphics.LoadingTexture[1].Width * 0.64f);
            LoadingPos.Y = 75;

            // Font
            DefaultFont = Content.Load<SpriteFont>("DefaultFont");

            MouseNodeHandler.MaxNodoSelecionado = -1;

           // AppGraphics.SelecionaNodoComVoltar(Program.mPessoas[0].Nodo, -1);
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
            // Somente considera algo se o foco ta no grafo
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
                        mouseTimerClicked = true;
                    }
                }
                // Verifica Double Click - seleciona, se houver
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (oldMouseState.LeftButton == ButtonState.Released)
                    {
                        if ((mouseTimer < 10f) && (mouseTimerClicked))
                            if (MouseNodeHandler.NodoMouse != null)
                            {
                                AppGraphics.SelecionaNodoComVoltar(MouseNodeHandler.NodoMouse.InfoNodo.Nodo, MouseNodeHandler.MaxNodoSelecionado);
                                MouseNodeHandler.UnMouseOver();
                            }
                    }
                }
                if ((mouseTimer < 10) && (mouseTimerClicked))
                {
                    mouseTimer++;
                }
                else
                {
                    if (MouseNodeHandler.NodoMouse != null)
                    {
                        //AppGraphics.CarregaInformações(MouseNodeHandler.NodoMouse.InfoNodo.Nodo);
                        //MouseNodeHandler.UnMouseOver();
                    }
                    mouseTimerClicked = false;
                }
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
            if (FilterPanel.Visible == false)
                foreach (GUI.TGUIPanel Panel in GUI.Components)
                    Panel.Update(mouseState);
            FilterPanel.Update(mouseState);
            FilterButton.Update(mouseState);
            LeftChangeGraphFilterCheckClick(0);

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
                    new Vector2(AppGraphics.LoadingTexture[2].Width * 0.5f, AppGraphics.LoadingTexture[2].Height * 0.5f), LoadingScale * 0.96f, SpriteEffects.None, 0f);
                spriteBatch.Draw(AppGraphics.LoadingTexture[3], LoadingPos, null, Color.White * LoadingAlpha, -LoadingAngle * 0.5f,
                    new Vector2(AppGraphics.LoadingTexture[3].Width * 0.5f, AppGraphics.LoadingTexture[3].Height * 0.5f), LoadingScale * 0.96f, SpriteEffects.None, 0f);
                spriteBatch.Draw(AppGraphics.LoadingTexture[0], LoadingPos, null, Color.White * LoadingAlpha, -LoadingAngle,
                    new Vector2(AppGraphics.LoadingTexture[0].Width * 0.5f, AppGraphics.LoadingTexture[0].Height * 0.5f), LoadingScale * 0.5f, SpriteEffects.None, 0f);
                spriteBatch.Draw(AppGraphics.LoadingTexture[1], LoadingPos, null, Color.White * LoadingAlpha, LoadingAngle,
                    new Vector2(AppGraphics.LoadingTexture[1].Width * 0.5f, AppGraphics.LoadingTexture[1].Height * 0.5f), LoadingScale, SpriteEffects.None, 0f);
                spriteBatch.DrawString(DefaultFont, "Loading", LoadingPos + new Vector2(-(DefaultFont.MeasureString("Loading")).X * 0.5f, AppGraphics.LoadingTexture[1].Height * 0.56f * LoadingScale), Color.White);
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

            // Nome do nodo central
            if ((MouseNodeHandler.NodoMouse == null) &&
                (MouseNodeHandler.MaxNodoSelecionado != -1) && 
                (AppGraphics.MaxNodos[MouseNodeHandler.MaxNodoSelecionado].MainNodo != null) &&
                (((MouseNodeHandler.NodoMouse == null) || (MouseNodeHandler.NodoMouse.InfoNodo != (AppGraphics.MaxNodos[MouseNodeHandler.MaxNodoSelecionado].MainNodo)))))
            {
                MouseNodeHandler.SelectNodo(MouseNodeHandler.NodoSelecionado.DrawNodo);
                Vector2 Pos = Vector2.Transform(AppGraphics.MaxNodos[MouseNodeHandler.MaxNodoSelecionado].MainNodo.DrawNodo.Pos, AppGraphics.Camera.get_transformation());

                spriteBatch.Draw(AppGraphics.BoxTex,
                    new Rectangle(
                        (int)(MouseNodeHandler.NSTextPos.X),
                        (int)(MouseNodeHandler.NSTextPos.Y),
                        (int)(MouseNodeHandler.NSTextSize.X),
                        (int)(MouseNodeHandler.NSTextSize.Y)),
                    null,
                    Color.White * 0.92f, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
                for (int i = 0; i < MouseNodeHandler.NSTexts.Count; i++)
                {
                    spriteBatch.DrawString(GUI.Fonts[2], MouseNodeHandler.NSTexts[i], MouseNodeHandler.NSTextPos + new Vector2(1, i * (MouseNodeHandler.NSTextSize.Y / MouseNodeHandler.NSTexts.Count)), Color.White * 0.92f);
                }
            }
            // Nome do nodo com mouse em cima
            if ((MouseNodeHandler.NodoMouse != null))
            {
                spriteBatch.Draw(AppGraphics.BoxTex,
                    new Rectangle(
                        (int)(MouseNodeHandler.MOTextPos.X),
                        (int)(MouseNodeHandler.MOTextPos.Y),
                        (int)(MouseNodeHandler.MOTextSize.X + 2),
                        (int)(MouseNodeHandler.MOTextSize.Y)),
                    null,
                    Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
                for (int i = 0; i < MouseNodeHandler.MOTexts.Count; i++)
                {
                    spriteBatch.DrawString(GUI.Fonts[2], MouseNodeHandler.MOTexts[i], MouseNodeHandler.MOTextPos + new Vector2(1, (int)(i * (MouseNodeHandler.MOTextSize.Y / MouseNodeHandler.MOTexts.Count))), Color.White);
                }
            }

            // Desenha a GUI
            //LeftPanel.Draw(spriteBatch);
            //RightPanel.Draw(spriteBatch);
            foreach (GUI.TGUIPanel Panel in GUI.Components)
                Panel.Draw(spriteBatch);

            // Draws the FPS
            //spriteBatch.DrawString(DefaultFont, "FPS: " + (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds), new Vector2(LeftPanel.Pos.X + LeftPanel.Size.X + 2, 0f), Color.White);
            //spriteBatch.DrawString(DefaultFont, "Camera: X=" + AppGraphics.Camera.Pos.X.ToString() + " Y=" + AppGraphics.Camera.Pos.Y.ToString() + " Z=" + AppGraphics.Camera.Zoom.ToString() + " R=" + AppGraphics.Camera.Rotation.ToString(), new Vector2(LeftPanel.Pos.X + LeftPanel.Size.X + 2, 20f), Color.White);
            //spriteBatch.DrawString(DefaultFont, "Mouse: X=" + AppGraphics.Camera.mousePos.X.ToString() + " Y=" + AppGraphics.Camera.mousePos.Y.ToString() + " l:" + AppGraphics.DPrimitives.Lines.Count, new Vector2(LeftPanel.Pos.X + LeftPanel.Size.X + 2, 40f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
