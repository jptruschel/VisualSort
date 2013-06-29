﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace VisualSort
{
    // Definição de um Nodo que será printado na tela - base para todos os nodos utilizados no programa
    public class TDrawNodo
    {
        public Vector2 Pos;         // posição atual 
        public Vector2 Speed;       // velocidade
        public Vector2 FinalPos;    // posição final (=Pos, exceto quando está animando)
        public float SpeedAlpha;
        public Color Color;
        public bool Selected, MouseOver;
        public float RotAngle;
        public TInfoNodo InfoNodo;

        public TDrawNodo(TInfoNodo InfoNodo)
        {
            Pos = new Vector2(0f, 0f);
            Color = Color.White * 0.8f;
            Selected = false;
            RotAngle = 0f;
            this.InfoNodo = InfoNodo;
        }
        public void MoveTo(Vector2 Pos)
        {
            this.Pos = Pos;
            this.Speed = new Vector2(0f, 0f);
            //this.FinalPos = new Vector2(0f, 0f);
        }
        public void AccelerateTo(Vector2 NewPos, float Alpha)
        {
            this.Speed = new Vector2((NewPos.X - Pos.X) / Alpha, (NewPos.Y - Pos.Y) / Alpha);
            SpeedAlpha = Alpha;
            this.FinalPos = NewPos;
        }
        public void Update(GameTime gameTime, bool UpdateMouse)
        {
            // Move o nodo, caso haja velocidade
            if ((Math.Abs(Speed.X) > 0.01f) || (Math.Abs(Speed.Y) > 0.01f))
            {
                Pos += Speed;
                Speed = new Vector2((FinalPos.X - Pos.X) / SpeedAlpha, (FinalPos.Y - Pos.Y) / SpeedAlpha);
            }
            MouseOver = false;
            if (UpdateMouse)
                // Verifica se está com o mouse em cima
                if (!Rectangle.Intersect(
                    new Rectangle(
                        (int)(this.Pos.X - (AppGraphics.DefaultNodeSize * 0.4f)),
                        (int)(this.Pos.Y - (AppGraphics.DefaultNodeSize * 0.4f)),
                        (int)(AppGraphics.DefaultNodeSize * 0.9f),
                        (int)(AppGraphics.DefaultNodeSize * 0.9f)),
                     new Rectangle(
                         (int)AppGraphics.Camera.mousePos.X,
                         (int)AppGraphics.Camera.mousePos.Y,
                         1, 1
                         )).IsEmpty)
                    MouseOver = true;
                else
                {
                    MouseOver = false;
                    if (Program.NodoMouse == this)
                        Program.NodoMouse = null;
                }
            // Ãngulos
            if (Selected)
            {
                // Calcula os ângulos e cores
                RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2.56f;
            }
            else
                RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.64f;
            RotAngle = RotAngle % (MathHelper.Pi * 4);

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (AppGraphics.Camera.isInCameraView(Pos))
            {
                if (MouseOver)
                    if (this.Selected)
                        this.Color = Color.GreenYellow;
                    else
                        this.Color = Color.Yellow;
                else
                    if (this.Selected)
                        if (AppGraphics.MaxNodos[Program.maxNodoSelecionado].Loops > 0)
                            this.Color = Color.White; //* (1 / AppGraphics.MaxNodos[Program.maxNodoSelecionado].Loops);
                        else
                            this.Color = Color.White;
                    else
                        this.Color = Color.Azure;
                Color cor = this.Color;
            //    if (this.Selected)
             //       cor = Color.Green;
                // Desenha o nodo
                spriteBatch.Draw(AppGraphics.NodoTex,
                    new Rectangle(
                        (int)(this.Pos.X),
                        (int)(this.Pos.Y),
                        (int)(AppGraphics.DefaultNodeSize),
                        (int)(AppGraphics.DefaultNodeSize)),
                    new Rectangle(0, 0, AppGraphics.NodoTex.Width, AppGraphics.NodoTex.Height),
                    cor,
                    0f,
                    new Vector2(AppGraphics.NodoTex.Width * 0.5f, AppGraphics.NodoTex.Height * 0.5f), SpriteEffects.None, 0f);
                // Desenha o círculo de seleção
                if (this.Selected)
                    spriteBatch.Draw(AppGraphics.LoadingTexture[0],
                        new Rectangle(
                            (int)(this.Pos.X),
                            (int)(this.Pos.Y),
                            (int)(AppGraphics.DefaultNodeSize * 1.28f),
                            (int)(AppGraphics.DefaultNodeSize * 1.28f)),
                        null,
                        cor,
                        RotAngle,
                        new Vector2(128, 128), SpriteEffects.None, 0f);
                if (InfoNodo.MesmoNome("Krug", false))
                {
                    spriteBatch.Draw(AppGraphics.LoadingTexture[4],
                    new Rectangle(
                        (int)(this.Pos.X-16f),
                        (int)(this.Pos.Y-28f),
                        (int)(AppGraphics.LoadingTexture[4].Width * 0.25f),
                        (int)(AppGraphics.LoadingTexture[4].Height * 0.25f)),
                    new Rectangle(0, 0, AppGraphics.LoadingTexture[4].Width, AppGraphics.LoadingTexture[4].Height),
                    Color.White, 0f, new Vector2(0,0), SpriteEffects.None, 0f);
                }
                // Se o mouse estiver em cima, desenhar uma caixinha com o nome
                if (MouseOver)
                {
                    Program.NodoMouse = this;
                }
            }
        }
    }
    // Definição de MaxNodo - um nodo constituído de outros nodos
    public class TDrawMaxNodo
    {
        public TInfoNodo MainNodo;      // Nodo central
        public Vector2 Pos;             // Posição (em worldPos)
        public List<TDrawNodo> Nodos;   // Todos os nodos desenhados nesse maxNodo (todos desenhados ao redor do central)
                                        // Considera-se que todos nodos aqui estão ligados ao principal
        public List<int> Ligações;      // Ligações com outros maxNodos
        public int Loops;               // Número de loops
        public Vector2 Size;            // Tamanho (em worldSize) 
        public List<int> Lines;         // As linhas (com o ponteiro aqui para poderem ser movidas caso o elemento se mova)
        public bool Drawable;           // Se pode desenhar
        // Inicializador de um maxNodo, automaticamente colocando todos os nodos ligados
        public TDrawMaxNodo(TPNodo Nodo, Vector2 Pos)
        {
            MainNodo = Program.GetNodoFromLists(Nodo);
            Ligações = new List<int>();
            Nodos = new List<TDrawNodo>();
            Lines = new List<int>();
            Loops = 0;
            Size = new Vector2(0, 0);
            this.Pos = Pos;
            Drawable = true;
        }
        // Adiciona uma maxLigação
        public void AddLigaçãoCom(int Índice)
        {
            if (!Ligações.Contains(Índice))
                Ligações.Add(Índice);
        }
        // Esconde - não deleta da memória, mas deleta as primitivas e os nodos
        public void Esconder()
        {
            foreach (int Lin in Lines)
                AppGraphics.DPrimitives.Lines[Lin].Drawable = false;
            Nodos.Clear();
            Drawable = false;
        }
        // Ressurgir - quase igual a inicializar, mas algumas coisas já estão instanciadas
        public void Ressurgir()
        {
            // Reinicializa o MainNodo para todos os Elementos a serem desenhados
            MainNodo.DrawNodo = new TDrawNodo(MainNodo);
            MainNodo.DrawNodo.MouseOver = false;
            MainNodo.DrawNodo.Selected = true;
            MainNodo.DrawNodo.MoveTo(Pos);
            MainNodo.DrawNodo.Color = Color.White;

            // Cria todos os drawnodos
            Size = new Vector2(0, 0);
            Loops = 0;

            Vector2 NodeDisplace = new Vector2(82f, 0.0f);
            float NodeAngleDisplace = 0f;
            // Varre todos os nodos que tem ligação, colocando eles em lugares próprios de serem desenhados
            for (int i = 0; i < MainNodo.Ligações.Count; i++)
            {
                TInfoNodo NodoL = Program.GetNodoFromLists(MainNodo.Ligações[i]);
                NodoL.DrawNodo = new TDrawNodo(NodoL);
                float cosRadians = (float)Math.Cos(NodeAngleDisplace);
                float sinRadians = (float)Math.Sin(NodeAngleDisplace);
                Vector2 NewPosDis = new Vector2(
                        NodeDisplace.X * cosRadians - NodeDisplace.Y * sinRadians,
                        NodeDisplace.X * sinRadians + NodeDisplace.Y * cosRadians);
                NodoL.DrawNodo.MoveTo(Pos);
                NodoL.DrawNodo.AccelerateTo(
                    Pos + NewPosDis, 10f);
                if (Vector2.Distance(NewPosDis, Pos) > Vector2.Distance(NewPosDis, Size))
                    Size = NewPosDis;

                NodeAngleDisplace += (float)(-Math.PI / (8/**/ * (Loops + 1)));
                if (NodeAngleDisplace <= -(Math.PI))
                {
                    NodeDisplace += new Vector2(10f / (Loops + 1), 1f);//new Vector2(6f, 1f + ((Math.Min(50, Loops)) * (1.0f))); //* ((Math.Min(10, Loops)))));
                }
                if (NodeAngleDisplace < -(1.999 * Math.PI))
                {
                    //NodeDisplace += new Vector2(48f, 16f);
                    NodeAngleDisplace = 0.0f;
                    Loops++;
                }
                // Adiciona a linha
                AppGraphics.DPrimitives.Lines[Lines[i]].Drawable = true;
                // Adiciona o nodo na lista de nodos
                Nodos.Add(NodoL.DrawNodo);
            }
        }
        // Inicializa todos os nodos para desenhar e cria as linhas ligando todos
        // Se chamado após um Esconde, Ressurgir será chamado
        public void Inicializa()
        {
            if (!Drawable)
            {
                Ressurgir();
                return;
            }
            Nodos.Clear();
            Lines.Clear();
            // Inicializa o DrawNodo para todos os Elementos a serem desenhados
            // Cria o DrawNodo principal
            MainNodo.DrawNodo = new TDrawNodo(MainNodo);
            MainNodo.DrawNodo.MouseOver = false;
            MainNodo.DrawNodo.Selected = true;
            MainNodo.DrawNodo.MoveTo(Pos);
            MainNodo.DrawNodo.Color = Color.White;

            // Cria todos os drawnodos
            Size = new Vector2(0, 0);
            Loops = 0;

            Vector2 NodeDisplace = new Vector2(82f, 0.0f);
            float NodeAngleDisplace = 0f;
            // Varre todos os nodos que tem ligação, colocando eles em lugares próprios de serem desenhados
            for (int i = 0; i < MainNodo.Ligações.Count; i++)
            {
                TInfoNodo NodoL = Program.GetNodoFromLists(MainNodo.Ligações[i]);
                NodoL.DrawNodo = new TDrawNodo(NodoL);
                float cosRadians = (float)Math.Cos(NodeAngleDisplace);
                float sinRadians = (float)Math.Sin(NodeAngleDisplace);
                Vector2 NewPosDis = new Vector2(
                        NodeDisplace.X * cosRadians - NodeDisplace.Y * sinRadians,
                        NodeDisplace.X * sinRadians + NodeDisplace.Y * cosRadians);
                NodoL.DrawNodo.MoveTo(Pos);
                NodoL.DrawNodo.AccelerateTo(
                    Pos + NewPosDis, 10f);
                //if (Vector2.Distance(NewPosDis, Pos) > Vector2.Distance(NewPosDis, Size))

                NodeAngleDisplace += (float)(-Math.PI / (8/**/ * (Loops + 1)));
                if (NodeAngleDisplace <= -(Math.PI))
                {
                    NodeDisplace += new Vector2(10f / (Loops + 1), 1f);//new Vector2(6f, 1f + ((Math.Min(50, Loops)) * (1.0f))); //* ((Math.Min(10, Loops)))));
                }
                if (NodeAngleDisplace < -(1.999 * Math.PI))
                {
                    //NodeDisplace += new Vector2(48f, 16f);
                    NodeAngleDisplace = 0.0f;
                    Loops++;
                }
                Size = new Vector2((NewPosDis.Length()*2) + (AppGraphics.DefaultNodeSize),
                    (NewPosDis.Length() * 2) + (AppGraphics.DefaultNodeSize));
                // Adiciona a linha
                Lines.Add((int)AppGraphics.DPrimitives.AddLine(
                    Pos,
                    NodoL.DrawNodo.Pos,
                    MainNodo.DrawNodo.Color * 0.5f,
                    NodoL.DrawNodo.Color * 0.5f));
                // Adiciona o nodo na lista de nodos
                Nodos.Add(NodoL.DrawNodo);
            }
        }
        // Atualiza a posição das linhas
        public void Update(GameTime gameTime, bool UpdateNodesMouse)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                AppGraphics.DPrimitives.Lines[Lines[i]].Point1 = Pos;
                AppGraphics.DPrimitives.Lines[Lines[i]].Color1 = MainNodo.DrawNodo.Color * 0.1f;
                AppGraphics.DPrimitives.Lines[Lines[i]].Point2 =
                    Program.GetNodoFromLists(MainNodo.Ligações[i]).DrawNodo.Pos;
                AppGraphics.DPrimitives.Lines[Lines[i]].Color2 =
                    Program.GetNodoFromLists(MainNodo.Ligações[i]).DrawNodo.Color * 0.5f;
            }
            // Atualiza tudo
            MainNodo.DrawNodo.Update(gameTime, UpdateNodesMouse);
            foreach (TDrawNodo dNodo in Nodos)
                dNodo.Update(gameTime, UpdateNodesMouse);
        }
        // Desenha todo o maxNodo
        public void Draw(SpriteBatch spriteBatch)
        {
            // Desenha o nodo principal
            
            MainNodo.DrawNodo.Draw(spriteBatch);
            // Todos os nodos
            foreach (TDrawNodo dNodo in Nodos)
            {
                dNodo.Draw(spriteBatch);
            }
            // Desenha como nodo, dependendo do zoom
            spriteBatch.Draw(AppGraphics.NodoTex,
                new Rectangle(
                    (int)(this.Pos.X),
                    (int)(this.Pos.Y),
                    (int)(Size.X),
                    (int)(Size.Y)),
                null,
                Color.Azure * (float)(0.1 / AppGraphics.Camera.Zoom),
                0,
                new Vector2(AppGraphics.NodoTex.Width * 0.5f, AppGraphics.NodoTex.Height*0.5f), SpriteEffects.None, 0f);
        }
    }

    // Câmera 2D - XY, Zoom e Rotação
    public class Camera2d
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 ScreenSize;
        public Matrix inverse;
        public Vector2 mousePos;
        public Vector2 TopLeft, BotRight;
        public Vector2[] Bounds;
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera2d(GraphicsDevice graphicsDevice)
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
            Bounds = new Vector2[4];
            ScreenSize = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        }
        // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.001f) _zoom = 0.001f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        public void LookAt(Vector2 pos)
        {
            _pos = pos;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public Matrix get_transformation()
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                //                       new Vector3(-_pos.X * Zoom, -_pos.Y * Zoom, 0)  
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(ScreenSize.X * 0.5f, ScreenSize.Y * 0.5f, 0));
            return _transform;
        }
        public void UpdateCameraInfo(MouseState mouseStateCurrent)
        {
            this.inverse = Matrix.Invert(get_transformation());
            mousePos = Vector2.Transform(
               new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y), inverse);

            TopLeft = Pos - (new Vector2(ScreenSize.X * 0.5f, ScreenSize.Y * 0.5f));
            BotRight = Pos + (new Vector2(ScreenSize.X * 0.5f, ScreenSize.Y * 0.5f));
            Bounds[0] = TopLeft;
            Bounds[1] = new Vector2(BotRight.X, TopLeft.Y);
            Bounds[2] = BotRight;
            Bounds[3] = new Vector2(TopLeft.X, BotRight.Y);
            Matrix trans = get_transformation();
            for (int i = 0; i < 4; i++)
                Bounds[i] = Vector2.Transform(Bounds[i], inverse);

            TopLeft.X = Math.Min(Math.Min(Math.Min(Bounds[0].X, Bounds[1].X), Bounds[2].X), Bounds[3].X);
            TopLeft.Y = Math.Min(Math.Min(Math.Min(Bounds[0].Y, Bounds[1].Y), Bounds[2].Y), Bounds[3].Y);
            BotRight.X = Math.Max(Math.Max(Math.Max(Bounds[0].X, Bounds[1].X), Bounds[2].X), Bounds[3].X);
            BotRight.Y = Math.Max(Math.Max(Math.Max(Bounds[0].Y, Bounds[1].Y), Bounds[2].Y), Bounds[3].Y);
        }
        public bool isInCameraView(Vector2 Position)
        {
            return true;
            Vector2 iPosition = Vector2.Transform(Position, inverse);

            if (Rectangle.Intersect(
                new Rectangle(
                    (int)((TopLeft.X)), (int)((TopLeft.Y)),
                    (int)((BotRight.X - TopLeft.X)), (int)((BotRight.Y - TopLeft.Y))),
                new Rectangle(
                    (int)iPosition.X, (int)iPosition.Y,
                    (int)AppGraphics.DefaultNodeSize, (int)AppGraphics.DefaultNodeSize))
                    .IsEmpty)
                return false;
            else
                return true;
        }
    }
    // Uma definição de Linha
    public class DrawLine
    {
        public Vector2 Point1, Point2;
        public Color Color1, Color2;
        public bool Drawable;
        public DrawLine(Vector2 P1, Vector2 P2, Color Color1, Color Color2)
        {
            Point1 = P1;
            Point2 = P2;
            Drawable = false;
            this.Color1 = Color1;
            this.Color2 = Color2;
        }
    }
    // Renderizador de Primitivas para desenhar rapidamente todas as linhas
    public class PrimitiveRenderer
    {
        GraphicsDevice graphicsDevice;

        public List<DrawLine> Lines;

        Matrix projection;
        public BasicEffect basicEffect;

        public PrimitiveRenderer(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            Lines = new List<DrawLine>();

            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            basicEffect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            basicEffect.Alpha = 1.0f;
            basicEffect.VertexColorEnabled = true;

            /*Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 30.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f); // Look back at the origin

            float fovAngle = MathHelper.ToRadians(45);  // convert 45 degrees to radians
            float aspectRatio = graphicsDevice.Viewport.Width / graphicsDevice.Viewport.Height;
            float near = 0.01f; // the near clipping plane distance
            float far = 100f; // the far clipping plane distance
            */
            //projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, aspectRatio, near, far);
            projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 20);
            basicEffect.Projection = projection;
            /*basicEffect.World = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);
            basicEffect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);*/
        }

        public void Draw()
        {
            if (Lines.Count > 0)
            {
                VertexPositionColor[] aD = new VertexPositionColor[Lines.Count * 2];
                int i = 0;
                while ((i < Lines.Count))
                {
                    if (Lines[i].Drawable)
                    {
                        aD[i * 2] =
                            new VertexPositionColor(
                                new Vector3(
                                    new Vector2(
                                        Lines[i].Point1.X,
                                        Lines[i].Point1.Y), 0),
                                        Lines[i].Color1);
                        aD[(i * 2) + 1] =
                            new VertexPositionColor(
                                new Vector3(
                                    new Vector2(
                                        Lines[i].Point2.X,
                                        Lines[i].Point2.Y), 0),
                                        Lines[i].Color2);
                    }
                    i++;
                }
                basicEffect.View = AppGraphics.Camera._transform;
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, aD, 0, (int)(Math.Min(aD.Length * 0.5, 1048570)));
                }
            }
        }

        public Int64 AddLine(Vector2 point1, Vector2 point2, Color Color)
        {
            //  for (int i = 0; i < Lines.Count; i++)
            //     if ((Lines[i].Point1 == point1) && (Lines[i].Point2 == point2))
            //         return i;
            Lines.Add(new DrawLine(point1, point2, Color, Color));
            return Lines.Count - 1;
        }
        public Int64 AddLine(Vector2 point1, Vector2 point2, Color Color1, Color Color2)
        {
            //for (int i = 0; i < Lines.Count; i++)
            //   if ((Lines[i].Point1 == point1) && (Lines[i].Point2 == point2))
            //     return i;
            Lines.Add(new DrawLine(point1, point2, Color1, Color2));
            Lines[Lines.Count - 1].Drawable = true;
            return Lines.Count - 1;
        }
        public void SetDrawability(int Index, bool Drawable)
        {
            Lines[Index].Drawable = Drawable;
        }
        public void ResetDrawability()
        {
            for (int i = 0; i < Lines.Count; i++)
                Lines[i].Drawable = false;
        }
        public void RemoveLine(int Index)
        {
            Lines.RemoveAt(Index);
        }
    }

    // Define variáveis e funções para a parte gráfica do aplicativo
    public static class AppGraphics
    {
        // Câmera
        public static Camera2d Camera;
        // Tamanho default de um nodo
        public static float DefaultNodeSize = 32f;
        // Instância do Renderer de Primitivas (linhas)
        public static PrimitiveRenderer DPrimitives;

        // Todos os MaxNodos sendo desenhados
        public static List<TDrawMaxNodo> MaxNodos;
        public static List<TDrawNodo> DrawNodos;

        // Texturas
        public static Texture2D NodoTex;
        public static Texture2D[] LoadingTexture;
        public static Texture2D BoxTex;

        // O centro da tela
        public static Vector2 ScreenCenter;

        // Reseta a visualização
        public static void ResetView()
        {
            DrawNodos.Clear();
            MaxNodos.Clear();
            DPrimitives.Lines.Clear();
        }
        // Sleciona um nodo / maxNodo, dependendo do ViewMode
        public static void SelecionaNodoComVoltar(TPNodo Nodo, int Anterior)
        {
            if (Anterior > -1)
                if (MaxNodos[Anterior].MainNodo.Nodo == Nodo)
                    return;
            if (Anterior != -1)
            {
                MaxNodos[Anterior].Esconder();
                // Apaga todo os depois (apaga a linha seguinte de nodos)
                for (int i = Anterior + 1; i < MaxNodos.Count; i++)
                    MaxNodos.RemoveAt(Anterior+1);
            }
            // Cria o novo na posição 0,0
            AppGraphics.MaxNodos.Add(
                new TDrawMaxNodo(Nodo, new Vector2(0, 0)));
            // Inicializa
            AppGraphics.MaxNodos[AppGraphics.MaxNodos.Count - 1].Inicializa();
            // Olha
            AppGraphics.Camera.LookAt(new Vector2(0, 0));
            Program.MaxNodoSelecionado = AppGraphics.MaxNodos.Count - 1;
        }
        // Seleciona um nodo - coloca-o no centro e faz todos os ligados serem desenhados
   /*     public static void SelecionaNodo(TPNodo Nodo, int iMaxNodo)
        {
            Program.NodoSelecionado = Program.GetNodoFromLists(Nodo);
            
            Vector2 Farest;
            float NodeAngleDisplace = 0.0f;

            if (Program.NodoSelecionado != null)
            {
                // Apaga os antigos
                foreach (TInfoNodo nodo in Program.mPessoas.ProcuraNodo(true))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = AppGraphics.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mProduções.ProcuraNodo(true))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = AppGraphics.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mInstituições.ProcuraNodo(true))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = AppGraphics.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mPeridódicos.ProcuraNodo(true))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = AppGraphics.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mConferências.ProcuraNodo(true))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = AppGraphics.ScreenCenter;
                }

                // Seleciona o nodo
                Program.NodoSelecionado.Drawable = true;
                Program.NodoSelecionado.Selected = true;
                Program.NodoSelecionado.SetAllLinesDrawability(true);
                Farest = Program.NodoSelecionado.Pos;
                MaxNodos[iMaxNodo].Loops = 0;
                Program.maxNodoSelecionado = iMaxNodo;

                Vector2 NodeDisplace = new Vector2(82f, 0.0f);
                // Varre todos os nodos que tem ligação, colocando eles em lugares próprios de serem desenhados
                for (int i = 0; i < Program.NodoSelecionado.Ligações.Count; i++)
                {
                    TInfoNodo NodoL = Program.GetNodoFromLists(Program.NodoSelecionado.Ligações[i]);
                    float cosRadians = (float)Math.Cos(NodeAngleDisplace);
                    float sinRadians = (float)Math.Sin(NodeAngleDisplace);
                    NodoL.Drawable = true;
                    NodoL.MoveTo(Program.NodoSelecionado.Pos + new Vector2(
                        NodeDisplace.X * cosRadians - NodeDisplace.Y * sinRadians,
                        NodeDisplace.X * sinRadians + NodeDisplace.Y * cosRadians));
                    if (Vector2.Distance(NodoL.Pos, Program.NodoSelecionado.Pos) > Vector2.Distance(Program.NodoSelecionado.Pos, Farest))
                        Farest = NodoL.Pos;

                    NodeAngleDisplace += (float)(-Math.PI / (8/**/ //* (MaxNodos[iMaxNodo].Loops +1)));
             /*       if (NodeAngleDisplace <= -(Math.PI))
                    {
                        NodeDisplace += new Vector2(10f / (MaxNodos[iMaxNodo].Loops + 1), 1f);//new Vector2(6f, 1f + ((Math.Min(50, Loops)) * (1.0f))); //* ((Math.Min(10, Loops)))));
                    }
                    if (NodeAngleDisplace < -(1.999 * Math.PI))
                    {
                        //NodeDisplace += new Vector2(48f, 16f);
                        NodeAngleDisplace = 0.0f;
                        MaxNodos[iMaxNodo].Loops++;
                    }
                }
                if (MaxNodos[iMaxNodo].Loops > 0)
                    Program.NodoSelecionado.Color = Color.White * (1 / MaxNodos[iMaxNodo].Loops);
                else
                    Program.NodoSelecionado.Color = Color.White;
                // Move Camera
                Camera.LookAt(Program.NodoSelecionado.Pos);
            }
        }*/
    }
}
