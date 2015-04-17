// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Project1
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;

    public class Project1Game : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private GameObject model;
        private KeyboardManager keyboardManager;
        public KeyboardState keyboardState;
        private MouseManager mouseManager;
        public MouseState mouseState;

        // Represents the camera's position and orientation
        public Camera camera;

        // World boundaries that indicate where the edge of the screen is for the camera.
        public static float boundaryFront;
        public static float boundaryRight;
        public static float boundaryTop;
        public static float boundaryLeft;
        public static float boundaryBack;
        
        // Positions mouse in middle of window
        public const float MOUSEX = 0.5f;
        public const float MOUSEY = 0.5f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project1Game" /> class.
        /// </summary>
        public Project1Game()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            keyboardManager = new KeyboardManager(this);
            mouseManager = new MouseManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            boundaryFront = 0;
            boundaryLeft = 0;
            boundaryRight = Landscape.LENGTH - 1;
            boundaryTop = Landscape.MAXSEED*3;
            boundaryBack = Landscape.LENGTH - 1;
        }

        protected override void LoadContent()
        {
            model = new Landscape(this);

            // Create an input layout from the vertices

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 1";
            camera = new Camera(this);
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = keyboardManager.GetState();
            mouseState = mouseManager.GetState();
            camera.Update(gameTime);
            model.Update(gameTime);

            // Resets mouse position so it can't go outside the window
            mouseManager.SetPosition(new Vector2(MOUSEX, MOUSEY));

            // Escape key will exit game
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.SkyBlue);

            model.Draw(gameTime);

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
