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
    public class Graph
    {
        /* OPÇÕES DE CÂMERA */
        public static Vector2 CameraPos = new Vector2(0f, 0f);
        public static float Zoom = 1.0f;
        public static float DefaultNodeSize = 32f;

        // Definição de um Nodo que será printado na tela
        public class TDrawNodo
        {
            public Vector2 Pos;
            public Color Color;
            public bool Selected;
            public float RotAngle;

            public TDrawNodo()
            {
                Pos = new Vector2(0, 0);
                Color = Color.White * 0.8f;
                Selected = false;
                RotAngle = 0f;
            }
            public void Update(GameTime gameTime)
            {
                if (Selected)
                    RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 2.56f;
                else
                    RotAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.64f;
                RotAngle = RotAngle % (MathHelper.Pi * 4);
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                // Desenha o nodo
                spriteBatch.Draw(Program.NodoTex, 
                    new Rectangle(
                        (int)(this.Pos.X), 
                        (int)(this.Pos.Y),
                        (int)(DefaultNodeSize * Zoom),
                        (int)(DefaultNodeSize * Zoom)),
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
                            (int)(DefaultNodeSize * 1.28f * Zoom),
                            (int)(DefaultNodeSize * 1.28f * Zoom)),
                        null,
                        this.Color,
                        RotAngle,
                        new Vector2(64, 64), SpriteEffects.None, 0f);

                // Desenha cada Ligação
                // Pessoas
                if (this is TPessoa)
                {
                    // Cada pessoa
                }
            }
            public bool a()
            {
                if (this is TConferência)
                    return true;
                return false;
            }
        }
    }
}
