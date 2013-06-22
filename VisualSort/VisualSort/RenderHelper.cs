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
    public class LinesList
    {
        GraphicsDevice graphicsDevice;

        List<VertexPositionColor> vertices;

        Matrix projection;
        public BasicEffect basicEffect;

        public LinesList(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;

            vertices = new List<VertexPositionColor>();

            basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            basicEffect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            basicEffect.Alpha = 0.64f;
            basicEffect.LightingEnabled = false;

            Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 30.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f); // Look back at the origin

            float fovAngle = MathHelper.ToRadians(45);  // convert 45 degrees to radians
            float aspectRatio = graphicsDevice.Viewport.Width / graphicsDevice.Viewport.Height;
            float near = 0.01f; // the near clipping plane distance
            float far = 100f; // the far clipping plane distance

            projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, aspectRatio, near, far);
           // projection =  Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            basicEffect.Projection = projection;
            basicEffect.World = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);
            basicEffect.View = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        }

        public void Draw()
        {
            VertexPositionColor[] aD = new VertexPositionColor[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                aD[i] = new VertexPositionColor(vertices[i].Position, vertices[i].Color);
            }
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, aD, 0, (int)(vertices.Count * 0.5));
            }
        }

        public void AddLine(Vector2 point1, Vector2 point2)
        {
            VertexPositionColor vertex1 = new VertexPositionColor(new Vector3(point1.X, point1.Y, 0), Color.White);
            VertexPositionColor vertex2 = new VertexPositionColor(new Vector3(point2.X, point2.Y, 0), Color.White);
            vertices.Add(vertex1);
            vertices.Add(vertex2);
        }
    }
    class RenderHelper
    {
    }
}
