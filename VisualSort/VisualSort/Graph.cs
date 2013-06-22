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
        /* OPÇÕES DE CÂMERA */
        //public static Vector3 Camera = new Vector3(0f, 0f, 1f);
        public static float DefaultNodeSize = 32f;
        public static PrimitiveRenderer DPrimitives;

        // Definição de um Nodo que será printado na tela
        public class TDrawNodo
        {
            public Vector2 Pos;
            public Color Color;
            public bool Selected, Drawable;
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
            public void Update(GameTime gameTime)
            {
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
                // Desenha o nodo
                float Size = DefaultNodeSize;
                //if (!this.Selected)
                //    Size = DefaultNodeSize * (1.0f+(Camera.Z * 0.8f));
                spriteBatch.Draw(Program.NodoTex, 
                    new Rectangle(
                        (int)(this.Pos.X),
                        (int)(this.Pos.Y),
                        (int)(Size),
                        (int)(Size)),
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
                    NodoL.Pos = NodoS.Pos + new Vector2(
                        NodeDisplace.X * cosRadians - NodeDisplace.Y * sinRadians,
                        NodeDisplace.X * sinRadians + NodeDisplace.Y * cosRadians);
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
