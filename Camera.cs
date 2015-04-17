using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
namespace Project1
{
    using SharpDX.Toolkit.Input;
    public class Camera
    {
        public Matrix View;
        public Matrix Projection;
        public Project1Game game;
        public Vector3 pos;
        public Vector3 oldPos;
        public Vector3 oldView;
        public Vector3 viewPoint;
        public Vector3 up;

        private Vector3 moveVec;
        private MouseState currentMouseState;
        private float rotateUpDown;
        private float rotateLeftRight;
        private float moveForward;
        private float moveRight;
        private float moveUp;
        private Vector3 viewDir;

        // Controls how quickly camera rotates
        public const float ROTATESENSITIVITY = 2;
        // Controls how fast camera moves
        public const float MOVESPEED = 50;

        // Sets bounding sphere radious for camera
        private readonly float COLLISIONRADIUS = 1;
        // Initial view direction of camera
        private readonly Vector3 ORIGINALDIR = Vector3.ForwardLH;
        // Original up vector
        private readonly Vector3 ORIGINALUP = Vector3.Up;


        public Camera(Project1Game game) {
            currentMouseState = game.mouseState;
            rotateLeftRight = 0;
            rotateUpDown = 0;
            viewDir = ORIGINALDIR;
            pos = new Vector3(1, Landscape.MAXSEED+1, 1);
            viewPoint = pos + viewDir;
            up = Vector3.UnitY;
            View = Matrix.LookAtLH(pos, viewPoint, up);
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);
            this.game = game;
        }


        public void Update(GameTime gameTime)
        {
            oldPos = pos;
            oldView = viewPoint;
            readChanges(gameTime);
            moveCamera();
            collisionCheck();
            rotateCamera();
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 1000f);
            View = Matrix.LookAtLH(pos, viewPoint, up); 
            
        }


        // Reads input from keyboard and mouse
        public void readChanges(GameTime gameTime)
        {
            moveForward = 0;
            moveRight = 0;
            moveUp = 0;

            // Keyboard input
            if (game.keyboardState.IsKeyDown(Keys.Up) && (rotateUpDown - (float)gameTime.ElapsedGameTime.TotalSeconds * ROTATESENSITIVITY) > -Math.PI/2) 
            {
                rotateUpDown -= (float) gameTime.ElapsedGameTime.TotalSeconds * ROTATESENSITIVITY;
            }
            if (game.keyboardState.IsKeyDown(Keys.Down) && (rotateUpDown + (float)gameTime.ElapsedGameTime.TotalSeconds * ROTATESENSITIVITY) < Math.PI/2)
            {
                rotateUpDown += (float)gameTime.ElapsedGameTime.TotalSeconds * ROTATESENSITIVITY;
            }
            if (game.keyboardState.IsKeyDown(Keys.Right))
            {
                rotateLeftRight += (float)gameTime.ElapsedGameTime.TotalSeconds * ROTATESENSITIVITY;
            }
            if (game.keyboardState.IsKeyDown(Keys.Left))
            {
                rotateLeftRight -= (float)gameTime.ElapsedGameTime.TotalSeconds * ROTATESENSITIVITY;
            }
            if (game.keyboardState.IsKeyDown(Keys.W))
            {
                moveForward += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (game.keyboardState.IsKeyDown(Keys.S))
            {
                moveForward -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (game.keyboardState.IsKeyDown(Keys.D))
            {
                moveRight += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (game.keyboardState.IsKeyDown(Keys.A))
            {
                moveRight -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (game.keyboardState.IsKeyDown(Keys.Q))
            {
                moveUp += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (game.keyboardState.IsKeyDown(Keys.E))
            {
                moveUp -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            
            // Mouse input
            currentMouseState = game.mouseState;
            if (currentMouseState.X != Project1Game.MOUSEX || currentMouseState.Y != Project1Game.MOUSEY)
            {
                rotateLeftRight += (currentMouseState.X - Project1Game.MOUSEX) * ROTATESENSITIVITY;
                rotateUpDown += (currentMouseState.Y - Project1Game.MOUSEY) * ROTATESENSITIVITY;
            }
        }


        // Applies camera translations
        public void moveCamera()
        {
            Matrix newRotation = Matrix.RotationX(rotateUpDown) * Matrix.RotationY(rotateLeftRight);
            moveVec = new Vector3(moveRight, moveUp, moveForward);
            Vector3.Normalize(moveVec);
            moveVec = (Vector3)Vector3.Transform(moveVec, newRotation);
            pos += moveVec * MOVESPEED;
        }


        // Applies camera rotations
        public void rotateCamera()
        {
            Matrix newRotation = Matrix.RotationX(rotateUpDown) * Matrix.RotationY(rotateLeftRight);
            viewDir = (Vector3) Vector3.Transform(ORIGINALDIR, newRotation);
            viewPoint = pos + viewDir;
            up = (Vector3)Vector3.Transform(ORIGINALUP, newRotation);
        }


        // Handles collisions with boundary of world and landscape surface
        public void collisionCheck()
        {
            //Check collision with world boundary
            if (pos.X <= Project1Game.boundaryLeft || pos.X >= Project1Game.boundaryRight)
            {
                pos.X = oldPos.X;
            }
            if (pos.Z <= Project1Game.boundaryFront || pos.Z >= Project1Game.boundaryBack)
            {
                pos.Z = oldPos.Z;
            }
            if (pos.Y >= Project1Game.boundaryTop)
            {
                pos.Y = oldPos.Y;
            }
            
            //Check collisions with walls
            int x;
            int z;
            if (pos.X == 0)
            {
                x = 0; 
            }
            else if (pos.X == Project1Game.boundaryRight)
            {
                x = (int)pos.X - 1;
            }
            else
            { 
                x = (int)Math.Round(pos.X - 0.5, MidpointRounding.AwayFromZero);
            }
            
            if (pos.Z == 0)
            {
                z = 0; 
            }
            else if(pos.Z == Project1Game.boundaryBack)
            {
                z = (int)pos.Z - 1;
            }
            else 
            {
                z = (int)Math.Round(pos.Z - 0.5, MidpointRounding.AwayFromZero);
            }
            
            // Can be implemented 2 ways
            // If collision can just revert back to old pos and prevent movement
            // Gives behaviour of sticky walls
            // Or if collision can increase pos to be above vertex's y pos outside of collision radius
            // Can give bumpy impression but allows movement across surface even with collisions.
            Vector3 bottomLeft = new Vector3(x, Landscape.heightMap[x, z], z);
            Vector3 topLeft = new Vector3(x, Landscape.heightMap[x, z + 1], z);
            Vector3 topRight = new Vector3(x + 1, Landscape.heightMap[x + 1, z + 1], z + 1);
            Vector3 bottomRight = new Vector3(x + 1, Landscape.heightMap[x + 1, z], z);
            if ((pos - bottomLeft).LengthSquared() <= Math.Pow(2 * COLLISIONRADIUS, 2))
            {
                pos.Y = bottomLeft.Y + (float) Math.Pow(2 * COLLISIONRADIUS, 2);
                //pos = oldPos;
            }else if ((pos - topLeft).LengthSquared() <= Math.Pow(2 * COLLISIONRADIUS, 2))
            {
                pos.Y = topLeft.Y + (float)Math.Pow(2 * COLLISIONRADIUS, 2);
                //pos = oldPos;
            }
            else if ((pos - bottomRight).LengthSquared() <= Math.Pow(2 * COLLISIONRADIUS, 2))
            {
                pos.Y = bottomRight.Y + (float)Math.Pow(2 * COLLISIONRADIUS, 2);
                //pos = oldPos;
            }
            if ((pos - topRight).LengthSquared() <= Math.Pow(2 * COLLISIONRADIUS, 2))
            {
                pos.Y = topRight.Y + (float)Math.Pow(2 * COLLISIONRADIUS, 2);
                //pos = oldPos;
            }

        }
    }
}
