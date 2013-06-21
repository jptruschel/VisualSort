// LinesDemo.cs
// Copyright 2006 Michael Anderson
// Version 1.0 -- January 7, 2007


#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion


namespace LinesDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (LinesDemo game = new LinesDemo())
            {
                game.Run();
            }
        }
    }

    
    /// <summary>
    /// Main "game" class
    /// </summary>
    public class LinesDemo : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        Matrix viewMatrix;
        Matrix projMatrix;
        float xCamera = 0.0f;
        float yCamera = 0.0f;
        float rotCamera = 0.0f;
        const float defaultZoomCamera = 10.0f;
        float zoomCamera = defaultZoomCamera;

        bool wireframeButtonDown = false;
        bool showWireframe = false;

        bool useOtherStyleButtonDown = false;
        bool useOtherStyle = true;

        LineManager lineManager = new LineManager(); // Handles drawing lines

        // Various lists of lines to draw
        List<Line> gridLineList = new List<Line>();
        List<Line> triangleLineList = new List<Line>();
        List<Line> octLineList = new List<Line>();
        List<Line> starLineList = new List<Line>();
        List<Line> miscLineList = new List<Line>();

        public LinesDemo()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            // Create a grid of lines, with the major axes a little different
            for (int i = -10; i <= 10; i++)
            {
                if (i == 0)
                    continue; // major axes are added later
                gridLineList.Add(new Line(new Vector2(i, -10), new Vector2(i, 10), 0.05f, Color.DarkBlue));
                gridLineList.Add(new Line(new Vector2(-10, i), new Vector2(10, i), 0.05f, Color.DarkBlue));
            }
            // Add major axes to gridLineList last so they draw on top of other lines
            gridLineList.Add(new Line(new Vector2(0, -20), new Vector2(0, 20), 0.05f, Color.Cyan));
            gridLineList.Add(new Line(new Vector2(-20, 0), new Vector2(20, 0), 0.05f, Color.Cyan));

            // Create a triangle from three lines
            triangleLineList.Add(new Line(new Vector2(2, 2), new Vector2(8, 2)));
            triangleLineList.Add(new Line(new Vector2(8, 2), new Vector2(5, 8)));
            triangleLineList.Add(new Line(new Vector2(5, 8), new Vector2(2, 2)));

            // Use AddPolyLine to more easily add several connected lines
            AddPolyLine(octLineList,
                new Vector2(-6, 2),
                new Vector2(-4, 2),
                new Vector2(-2, 4),
                new Vector2(-2, 6),
                new Vector2(-4, 8),
                new Vector2(-6, 8),
                new Vector2(-8, 6),
                new Vector2(-8, 4),
                new Vector2(-6, 2));

            // Make a (somewhat uneven) star
            AddPolyLine(starLineList,
                new Vector2(-2.5f, -7.5f),
                new Vector2(-8, -4),
                new Vector2(-2, -4),
                new Vector2(-7.5f, -7.5f),
                new Vector2(-5, -2),
                new Vector2(-2.5f, -7.5f)
                );

            // Make a list of some miscellaneous lines with different attributes
            miscLineList.Add(new Line(new Vector2(2, -2), new Vector2(8, -2), 0.10f, Color.Salmon));
            miscLineList.Add(new Line(new Vector2(4, -4), new Vector2(6, -4), 0.20f, Color.Gainsboro));
            miscLineList.Add(new Line(new Vector2(5, -6), new Vector2(5, -6), 0.50f, Color.Firebrick));
            Color myColor = new Color(108, 35, 108, 200); // note transparency
            miscLineList.Add(new Line(new Vector2(2, -7), new Vector2(8, -9), 0.30f, myColor));
            miscLineList.Add(new Line(new Vector2(2, -9), new Vector2(8, -7), 0.30f, myColor));
        }


        /// <summary>
        /// Helper function to add a series of connected line segments to a line list
        /// </summary>
        void AddPolyLine(List<Line> lineList, params Vector2[] vectors)
        {
            Vector2 p1;
            Vector2 p2 = vectors[0];
            for (int iVec = 1; iVec < vectors.Length; iVec++)
            {
                p1 = p2;
                p2 = vectors[iVec];
                lineList.Add(new Line(p1, p2));
            }
        }


        /// <summary>
        /// Load graphics content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                lineManager.Init(graphics.GraphicsDevice, content);
                Create2DProjectionMatrix();
            }
        }


        /// <summary>
        /// Unload graphics content.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
            {
                content.Unload();
            }
        }


        /// <summary>
        /// Create a simple 2D projection matrix
        /// </summary>
        public void Create2DProjectionMatrix()
        {
            // Projection matrix ignores Z and just squishes X or Y to balance the upcoming viewport stretch
            float projScaleX;
            float projScaleY;
            float width = graphics.GraphicsDevice.Viewport.Width;
            float height = graphics.GraphicsDevice.Viewport.Height;
            if (width > height)
            {
                // Wide window
                projScaleX = height / width;
                projScaleY = 1.0f;
            }
            else
            {
                // Tall window
                projScaleX = 1.0f;
                projScaleY = width / height;
            }
            projMatrix = Matrix.CreateScale(projScaleX, projScaleY, 0.0f);
            projMatrix.M43 = 0.5f;
        }


        /// <summary>
        /// Read input and update state
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gamePadState.Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (gamePadState.Buttons.Start == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Space))
            {
                // Reset camera
                zoomCamera = defaultZoomCamera;
                rotCamera = 0.0f;
                xCamera = 0.0f;
                yCamera = 0.0f;
            }

            if (gamePadState.Buttons.A == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.W))
            {
                if (!wireframeButtonDown)
                {
                    wireframeButtonDown = true;
                    showWireframe = !showWireframe;
                }
            }
            else
            {
                wireframeButtonDown = false;
            }

            if (gamePadState.Buttons.B == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.B))
            {
                if (!useOtherStyleButtonDown)
                {
                    useOtherStyleButtonDown = true;
                    useOtherStyle = !useOtherStyle;
                }
            }
            else
            {
                useOtherStyleButtonDown = false;
            }

            float leftX = gamePadState.ThumbSticks.Left.X;
            if (keyboardState.IsKeyDown(Keys.Left))
                leftX -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.Right))
                leftX += 1.0f;

            float leftY = -gamePadState.ThumbSticks.Left.Y;
            if (keyboardState.IsKeyDown(Keys.Up))
                leftY -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.Down))
                leftY += 1.0f;

            float rightX = gamePadState.ThumbSticks.Right.X;
            if (keyboardState.IsKeyDown(Keys.A))
                rightX -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.Z))
                rightX += 1.0f;

            // Zoom the camera 
            float fZoom = 0.0f;
            if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.S))
            {
                fZoom += 1;
            }
            if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.X))
            {
                fZoom -= 1;
            }
            zoomCamera *= (1.0f + elapsed * fZoom * 0.5f);

            // Rotate the camera
            rotCamera += -rightX * elapsed;

            // Move the camera, taking current rotation and zoom level into account
            Vector4 motionVector = new Vector4(leftX * elapsed * zoomCamera * 2, leftY * elapsed * zoomCamera * 2, 0, 1);
            Matrix matRot = Matrix.CreateRotationZ(-rotCamera);
            Vector4 rotatedMotionVector4 = Vector4.Transform(motionVector, matRot);
            xCamera += rotatedMotionVector4.X;
            yCamera -= rotatedMotionVector4.Y;

            viewMatrix = Matrix.CreateTranslation(-xCamera, -yCamera, 0) * Matrix.CreateRotationZ(-rotCamera) * Matrix.CreateScale(1.0f / zoomCamera, 1.0f / zoomCamera, 1.0f);

            base.Update(gameTime);
        }


        /// <summary>
        /// Draw the scene
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.DarkSlateBlue);

            if (showWireframe)
                graphics.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            else
                graphics.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            float time = (float)gameTime.TotalGameTime.TotalSeconds;

            lineManager.Draw(gridLineList, 0, Color.TransparentBlack, viewMatrix, projMatrix, time, (useOtherStyle ? null : "NoBlur"));
            lineManager.Draw(triangleLineList, 0.25f, Color.Yellow, viewMatrix, projMatrix, time, (useOtherStyle ? null : "NoBlur"));
            lineManager.Draw(octLineList, 0.25f, Color.Red, viewMatrix, projMatrix, time, (useOtherStyle ? "Modern" : "Glow"));
            lineManager.Draw(starLineList, 0.25f, Color.Green, viewMatrix, projMatrix, time, (useOtherStyle ? "AnimatedLinear" : "AnimatedRadial"));
            lineManager.Draw(miscLineList, 0, Color.TransparentBlack, viewMatrix, projMatrix, time, (useOtherStyle ? "Tubular" : "Standard" ));

            base.Draw(gameTime);
        }
    }
}
