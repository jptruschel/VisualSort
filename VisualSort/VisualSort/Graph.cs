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
            public Matrix inverse;
            public Vector2 mousePos;
            public Vector2 _pos; // Camera Position
            protected float _rotation; // Camera Rotation

            public Camera2d()
            {
                _zoom = 1.0f;
                _rotation = 0.0f;
                _pos = Vector2.Zero;
            }
            // Sets and gets zoom
            public float Zoom
            {
                get { return _zoom; }
                set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
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
            // Get set position
            public Vector2 Pos
            {
                get { return _pos; }
                set { _pos = value; }
            }
            public Matrix get_transformation(GraphicsDevice graphicsDevice)
            {
                _transform =       // Thanks to o KB o for this solution
                  Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                             Matrix.CreateRotationZ(Rotation) *
                                             Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                             Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
                return _transform;
            }
            public void UpdateMouse(GraphicsDevice graphicsDevice, MouseState mouseStateCurrent)
            {
                this.inverse = Matrix.Invert(get_transformation(graphicsDevice));
                mousePos = Vector2.Transform(
                   new Vector2(mouseStateCurrent.X, mouseStateCurrent.Y), inverse);
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
                    Lines.Clear();
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
                        (int)(DefaultNodeSize),
                        (int)(DefaultNodeSize)),
                     new Rectangle(
                         (int)Graph.Camera.mousePos.X,
                         (int)Graph.Camera.mousePos.Y,
                         1, 1
                         )).IsEmpty)
                    MouseOver = true;
                else
                    MouseOver = false;
                // Recalcula as linhas
                for (int i = 0; i < Lines.Count; i++)
                {
                    Graph.DPrimitives.Lines[Lines[i]].Point1 = Pos;
                    Graph.DPrimitives.Lines[Lines[i]].Point2 =
                        Program.GetNodoFromLists((this as TInfoNodo).Ligações[i]).Pos;
                    Graph.DPrimitives.Lines[Lines[i]].Color2 =
                        Program.GetNodoFromLists((this as TInfoNodo).Ligações[i]).Color * 0.5f;
                }
                // Calcula os ângulos e cores
                if (Selected)
                    RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2.56f;
                else
                    RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.64f;
                RotAngle = RotAngle % (MathHelper.Pi * 4);
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                if (MouseOver)
                    this.Color = Color.Red;
                else
                    this.Color = Color.White;
                // Desenha o nodo
                spriteBatch.Draw(Program.NodoTex, 
                    new Rectangle(
                        (int)(this.Pos.X),
                        (int)(this.Pos.Y),
                        (int)(DefaultNodeSize),
                        (int)(DefaultNodeSize)),
                    new Rectangle(0,0,64,64),
                    this.Color,
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
                        this.Color,
                        RotAngle,
                        new Vector2(64, 64), SpriteEffects.None, 0f);
            }
        }

        // Seleciona um nodo - coloca-o no centro e faz todos os ligados serem desenhados
        public static void SelecionaNodo(TPNodo Nodo)
        {
            TInfoNodo NodoS = Program.GetNodoFromLists(Nodo);
            Vector2 NodeDisplace = new Vector2(82f, 0.0f);
            Vector2 Farest;
            float NodeAngleDisplace = 0.0f;
            int Loops = 0;

            if (NodoS != null)
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

                // Seleciona o dado
                NodoS.Drawable = true;
                NodoS.Selected = true;
                Farest = NodoS.Pos;

                // Varre todos os nodos que tem ligação, colocando eles em lugares próprios de serem desenhados
                for (int i = 0; i < NodoS.Ligações.Count; i++)
                {
                    TInfoNodo NodoL = Program.GetNodoFromLists(NodoS.Ligações[i]);
                    float cosRadians = (float)Math.Cos(NodeAngleDisplace);
                    float sinRadians = (float)Math.Sin(NodeAngleDisplace);
                    NodoL.Drawable = true;
                    NodoL.MoveTo(NodoS.Pos + new Vector2(
                        NodeDisplace.X * cosRadians - NodeDisplace.Y * sinRadians,
                        NodeDisplace.X * sinRadians + NodeDisplace.Y * cosRadians));
                    if (Vector2.Distance(NodoL.Pos, NodoS.Pos) > Vector2.Distance(NodoS.Pos, Farest))
                        Farest = NodoL.Pos;

                    NodeAngleDisplace += (float)(-Math.PI / 8);
                    if (NodeAngleDisplace <= -(Math.PI))
                    {
                        NodeDisplace += new Vector2(6f, 1f + ((Math.Min(150, Loops)) * (1.5f))); //* ((Math.Min(10, Loops)))));
                    }
                    if (NodeAngleDisplace <= -(2 * Math.PI))
                    {
                        //NodeDisplace += new Vector2(48f, 16f);
                        NodeAngleDisplace = 0.0f;
                        Loops++;
                    }
                }
               // Camera = new Vector3(NodoS.Pos, 1.0f);
                // Update the camera zoom 
                //Zoom = 2.0f;//(Vector2.Distance(NodoS.Pos, Farest)) / (720 / 1280);
            }
        }
    }
}
