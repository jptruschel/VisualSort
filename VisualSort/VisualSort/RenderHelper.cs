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
    public class DrawLine
    {
        public Vector2 Point1, Point2;
        public Color Color1, Color2;
        public DrawLine(Vector2 P1, Vector2 P2, Color Color1, Color Color2)
        {
            Point1 = P1;
            Point2 = P2;
            this.Color1 = Color1;
            this.Color2 = Color2;
        }
    }
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
            projection =  Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 20);
            basicEffect.Projection = projection;
            /*basicEffect.World = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);
            basicEffect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);*/
        }

        public void Draw()
        {
            if (Lines.Count > 0)
            {
                VertexPositionColor[] aD = new VertexPositionColor[Lines.Count * 2];
                for (int i = 0; i < Lines.Count; i++)
                {
                    aD[i * 2] = 
                        new VertexPositionColor(
                            new Vector3(
                                new Vector2(
                                    Lines[i].Point1.X,// + (Lines[i].Point1.X - Graph.Camera.X) * Graph.Camera.Z,
                                    Lines[i].Point1.Y),0),// + (Lines[i].Point1.Y - Graph.Camera.Y) * Graph.Camera.Z), 0), 
                                    Lines[i].Color1);
                    aD[(i * 2) + 1] = 
                        new VertexPositionColor(
                            new Vector3(
                                new Vector2(
                                    Lines[i].Point2.X,// + (Lines[i].Point2.X - Graph.Camera.X) * Graph.Camera.Z,
                                    Lines[i].Point2.Y),0),// + (Lines[i].Point2.Y - Graph.Camera.Y) * Graph.Camera.Z), 0),
                                    Lines[i].Color2);
                }
                foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, aD, 0, (int)(aD.Length * 0.5));
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
            return Lines.Count - 1;
        }
    }
    class RenderHelper
    {
    }
}
