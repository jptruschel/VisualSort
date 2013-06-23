using System;
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
    public static class Graph
    {

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
                for (int i = 0; i < 4;i++ )
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
                        (int)((BotRight.X-TopLeft.X)), (int)((BotRight.Y-TopLeft.Y))),
                    new Rectangle(
                        (int)iPosition.X, (int)iPosition.Y,
                        (int)DefaultNodeSize, (int)DefaultNodeSize))
                        .IsEmpty)
                    return false;
                else
                    return true;
            }
        }
        public static Camera2d Camera;
        public static float DefaultNodeSize = 32f;
        public static PrimitiveRenderer DPrimitives;

        // Definição de um Nodo que será printado na tela
        public class TDrawNodo
        {
            public Vector2 Pos;
            public Color Color;
            public bool Selected, Drawable, MouseOver;
            public float RotAngle;
            public List<int> Lines;

            public TDrawNodo()
            {
                Pos = new Vector2(50, 20);
                Color = Color.White * 0.8f;
                Lines = new List<int>();
                Selected = false;
                Drawable = false;
                RotAngle = 0f;
            }
            public void InitializeLines()
            {
                if (this is TInfoNodo)
                {
                    for (int i = 0; i < (this as TInfoNodo).Ligações.Count; i++)
                    {
                        Lines.Add((int)Graph.DPrimitives.AddLine(
                            Pos,
                            Program.GetNodoFromLists((this as TInfoNodo).Ligações[i]).Pos,
                            Color * 0.5f,
                            Program.GetNodoFromLists((this as TInfoNodo).Ligações[i]).Color * 0.5f));
                    }
                }
            }
            public void NewLine(int LigaçãoIndex)
            {
                if (this is TInfoNodo)
                {
                    Lines.Add((int)Graph.DPrimitives.AddLine(
                        Pos,
                        Program.GetNodoFromLists((this as TInfoNodo).Ligações[LigaçãoIndex]).Pos,
                        Color * 0.5f,
                        Program.GetNodoFromLists((this as TInfoNodo).Ligações[LigaçãoIndex]).Color * 0.5f));
                }
            }
            public void ResetLines()
            {
                for (int i = 0; i < Lines.Count; i++)
                {
                    Graph.DPrimitives.SetDrawability(Lines[i], false);
                }
                Lines.Clear();
            }
            public void SetAllLinesDrawability(bool Drawable)
            {
                for (int i = 0; i < Lines.Count; i++)
                    Graph.DPrimitives.SetDrawability(Lines[i], Drawable);
            }
            public void MoveTo(Vector2 Pos)
            {
                this.Pos = Pos;
            }
            public void Update(GameTime gameTime)
            {
                // Verifica se está com o mouse em cima
                if (!Rectangle.Intersect(
                    new Rectangle(
                        (int)(this.Pos.X - (DefaultNodeSize / 2)),
                        (int)(this.Pos.Y - (DefaultNodeSize / 2)),
                        (int)(DefaultNodeSize * 0.9f),
                        (int)(DefaultNodeSize * 0.9f)),
                     new Rectangle(
                         (int)Graph.Camera.mousePos.X,
                         (int)Graph.Camera.mousePos.Y,
                         1, 1
                         )).IsEmpty)
                    MouseOver = true;
                else
                {
                    MouseOver = false;
                    if (this is TInfoNodo)
                        if (Program.NodoMouse == (this as TInfoNodo))
                            Program.NodoMouse = null;
                }
                // Recalcula as linhas - se selecionado
                if (Selected)
                {
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        Graph.DPrimitives.Lines[Lines[i]].Point1 = Pos;
                        Graph.DPrimitives.Lines[Lines[i]].Color1 = Color * 0.1f;
                        Graph.DPrimitives.Lines[Lines[i]].Point2 =
                            Program.GetNodoFromLists((this as TInfoNodo).Ligações[i]).Pos;
                        Graph.DPrimitives.Lines[Lines[i]].Color2 =
                            Program.GetNodoFromLists((this as TInfoNodo).Ligações[i]).Color * 0.5f;
                    }
                    // Calcula os ângulos e cores
                    RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2.56f;
                }
                else
                    RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.64f;
                RotAngle = RotAngle % (MathHelper.Pi * 4);

            }
            public void Draw(SpriteBatch spriteBatch)
            {
                if (Camera.isInCameraView(Pos))
                {
                    if (MouseOver)
                        if (this.Selected)
                            this.Color = Color.Green;
                        else
                            this.Color = Color.Yellow;
                    else
                        this.Color = Color.White;
                    Color cor = this.Color;
                    if (this.Selected)
                        cor = Color.Green;
                    // Desenha o nodo
                    spriteBatch.Draw(Program.NodoTex, 
                        new Rectangle(
                            (int)(this.Pos.X),
                            (int)(this.Pos.Y),
                            (int)(DefaultNodeSize),
                            (int)(DefaultNodeSize)),
                        new Rectangle(0,0,64,64),
                        cor,
                        0f,
                        new Vector2(32,32), SpriteEffects.None, 0f);
                    // Desenha o círculo de seleção
                    if (this.Selected)
                        spriteBatch.Draw(Program.LoadingTexture[0],
                            new Rectangle(
                                (int)(this.Pos.X),
                                (int)(this.Pos.Y),
                                (int)(DefaultNodeSize * 1.28f),
                                (int)(DefaultNodeSize * 1.28f)),
                            null,
                            cor,
                            RotAngle,
                            new Vector2(64, 64), SpriteEffects.None, 0f);
                    // Se o mouse estiver em cima, desenhar uma caixinha com o nome
                    if ((MouseOver) && (this is TInfoNodo))
                    {
                        Program.NodoMouse = (this as TInfoNodo);
                    }
                }
            }
        }

        // Seleciona um nodo - coloca-o no centro e faz todos os ligados serem desenhados
        public static void SelecionaNodo(TPNodo Nodo)
        {
            // Deseleciona último selecionado
            if (Program.NodoSelecionado != null)
            {
                Program.NodoSelecionado.Drawable = false;
                Program.NodoSelecionado.Selected = false;
                Program.NodoSelecionado.SetAllLinesDrawability(false);
            }

            Program.NodoSelecionado = Program.GetNodoFromLists(Nodo);
            Vector2 NodeDisplace = new Vector2(82f, 0.0f);
            Vector2 Farest;
            float NodeAngleDisplace = 0.0f;
            int Loops = 0;

            if (Program.NodoSelecionado != null)
            {
                // Apaga os antigos
                foreach (TInfoNodo nodo in Program.mPessoas.ProcuraNodo(false))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = Program.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mProduções.ProcuraNodo(false))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = Program.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mInstituições.ProcuraNodo(false))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = Program.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mPeridódicos.ProcuraNodo(false))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = Program.ScreenCenter;
                }
                foreach (TInfoNodo nodo in Program.mConferências.ProcuraNodo(false))
                {
                    nodo.Drawable = false;
                    nodo.Selected = false;
                    nodo.Pos = Program.ScreenCenter;
                }

                // Seleciona o nodo
                Program.NodoSelecionado.Drawable = true;
                Program.NodoSelecionado.Selected = true;
                Program.NodoSelecionado.SetAllLinesDrawability(true);
                Farest = Program.NodoSelecionado.Pos;

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

                    NodeAngleDisplace += (float)(-Math.PI / (8 * (Loops+1)));
                    if (NodeAngleDisplace <= -(Math.PI))
                    {
                        NodeDisplace += new Vector2(10f / (Loops+1), 1f);//new Vector2(6f, 1f + ((Math.Min(50, Loops)) * (1.0f))); //* ((Math.Min(10, Loops)))));
                    }
                    if (NodeAngleDisplace <= -(2 * Math.PI))
                    {
                        //NodeDisplace += new Vector2(48f, 16f);
                        NodeAngleDisplace = 0.0f;
                        Loops++;
                    }
                }
                // Move Camera
                Camera.LookAt(Program.NodoSelecionado.Pos);
            }
        }
    }
}
