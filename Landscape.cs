using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Landscape : ColoredGameObject
    {
        // The max value the seed can be set to
        public static readonly float MAXSEED = 100;
        // How big the world is. For memory restrictions value between 1-9
        public static readonly int SIZE = 9;
        // Length of world
        public static readonly int LENGTH = (int)Math.Pow(2, SIZE) + 1;

        // Height relative to MAXSEED snow will occur at
        private const float SNOWLEVEL = 0.8f;
        // Height for rocky level
        private const float ROCKLEVEL = 0.7f;
        // Height for grass
        private const float GRASSLEVEL = 0.1f;
        
        // Sets how bumpy landscape is.  Higher number gives smoother landscape
        // Between 0.8 and 1.0 give pretty good results
        private const float SMOOTHNESS = 1.0f;
        private readonly float H = (float)Math.Pow(2, -SMOOTHNESS);
        // Sets water transparency
        private const float TRANSPARENCY = 0.3f;

        public static float[,] heightMap;
        private Vector3[,] vertexNorms;

        public Landscape(Project1Game game)
        {
            this.game = game;
            // Create the height map and vertex norms
            heightMap = createRandomMap();
            vertexNorms = calcNorms();

            // Set the array to hold all the vertices
            VertexPositionNormalColor[] pointList = new VertexPositionNormalColor[6 * (LENGTH - 1) * (LENGTH - 1) + 6];
            int counter = 0;
            for (int x = 0; x < LENGTH - 1; x++)
            {
                for (int y = 1; y < LENGTH; y++)
                {
                    // Taking a point (x,y) from the height map it draws the square with the corners (x,y) (x+1,y) (x+1,y+1) and (x+1,y)
                    Vector3 pointA = new Vector3(x, heightMap[x, y-1], y-1);
                    pointList[counter] = new VertexPositionNormalColor(pointA, vertexNorms[x,y-1], chooseColor(heightMap[x,y-1]));
                    counter++;
                    Vector3 pointB = new Vector3(x, heightMap[x,y], y);
                    pointList[counter] = new VertexPositionNormalColor(pointB, vertexNorms[x,y], chooseColor(heightMap[x, y]));
                    counter++;
                    Vector3 pointC = new Vector3(x+1, heightMap[x+1, y], y);
                    pointList[counter] = new VertexPositionNormalColor(pointC, vertexNorms[x+1,y], chooseColor(heightMap[x+1, y]));
                    counter++;
                    Vector3 pointD = new Vector3(x, heightMap[x, y-1], y-1);
                    pointList[counter] = new VertexPositionNormalColor(pointD, vertexNorms[x,y-1], chooseColor(heightMap[x, y - 1]));
                    counter++;
                    Vector3 pointE = new Vector3(x+1, heightMap[x+1, y], y);
                    pointList[counter] = new VertexPositionNormalColor(pointE, vertexNorms[x+1,y], chooseColor(heightMap[x+1, y]));
                    counter++;
                    Vector3 pointF = new Vector3(x+1, heightMap[x+1, y-1], y-1);
                    pointList[counter] = new VertexPositionNormalColor(pointF, vertexNorms[x+1,y-1], chooseColor(heightMap[x+1, y - 1]));
                    counter++;
                }
            }

            // Adds a plane of water which we will make sea level eg. y=0
            Color water = new Color(new Vector3(0,0,50), TRANSPARENCY);
            Vector3 wpointA = new Vector3(-0.1f, -0.1f, -0.1f);
            pointList[counter] = new VertexPositionNormalColor(wpointA, Vector3.Down, water);
            counter++;
            Vector3 wpointB = new Vector3(-0.1f, -0.1f, LENGTH-0.9f);
            pointList[counter] = new VertexPositionNormalColor(wpointB, Vector3.Down, water);
            counter++;
            Vector3 wpointC = new Vector3(LENGTH-0.9f, -0.1f, LENGTH-0.9f);
            pointList[counter] = new VertexPositionNormalColor(wpointC, Vector3.Down, water);
            counter++;
            Vector3 wpointD = new Vector3(-0.1f, -0.1f, -0.1f);
            pointList[counter] = new VertexPositionNormalColor(wpointD, Vector3.Down, water);
            counter++;
            Vector3 wpointE = new Vector3(LENGTH-0.9f, -0.1f, LENGTH-0.9f);
            pointList[counter] = new VertexPositionNormalColor(wpointE, Vector3.Down, water);
            counter++;
            Vector3 wpointF = new Vector3(LENGTH-0.9f, -0.1f, -0.1f);
            pointList[counter] = new VertexPositionNormalColor(wpointF, Vector3.Down, water);
            counter++;

            // Add the vertices to the landscape
            vertices = Buffer.Vertex.New(game.GraphicsDevice, pointList);

            // Set the lighting of the landscape
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                View = game.camera.View,
                Projection = game.camera.Projection,
                VertexColorEnabled = true,
                World = Matrix.Identity,
                LightingEnabled = true,
                AmbientLightColor = new Vector3(0.1f,0.1f,0.1f),
            };
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.Direction = Vector3.Right;
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.1f,0.1f,0.1f);
            basicEffect.DirectionalLight0.SpecularColor = new Vector3(1.0f,1.0f,1.0f);

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        // Given a height returns the color for that height
        private Color chooseColor(float height)
        {
            if (height<GRASSLEVEL*MAXSEED) 
            {
                return Color.SandyBrown;
            }
            else if (height < ROCKLEVEL*MAXSEED)
            {
                return Color.ForestGreen;
            }
            else if (height < SNOWLEVEL*MAXSEED)
            {
                return Color.DarkSlateGray;
            }
            else
            {
                return Color.Snow;
            }
        }

        // Uses the Diamond Square Algorithm to create a height map array
        private float[,] createRandomMap()
        {
            float[,] map = new float[LENGTH, LENGTH];
            Random rand = new Random();
            float seed = rand.NextFloat(0,1) * MAXSEED;
            float displaceRange = MAXSEED;
            map[0, 0] = map[LENGTH - 1, 0] = map[LENGTH-1, LENGTH-1] = map[0, LENGTH-1] = seed;
            for (int sideLength = LENGTH - 1; sideLength > 1; sideLength /= 2)
            {
                int halfway = sideLength / 2;

                //Diamond step
                //Produces diamonds with the new calculated points
                for (int x = 0; x < LENGTH - 1; x += sideLength)
                {
                    for (int y = 0; y < LENGTH - 1; y += sideLength)
                    {
                        float topLeft = map[x, y];
                        float topRight = map[x + sideLength, y];
                        float bottomLeft = map[x, y + sideLength];
                        float bottomRIght = map[x + sideLength, y + sideLength];

                        float average = (topLeft + topRight + bottomLeft + bottomRIght) / 4;

                        map[x + halfway, y + halfway] = average + rand.NextFloat(-1, 1) * displaceRange;

                    }
                }

                //Square step
                //Produces squares with the new calculated points
                for (int x = 0; x < LENGTH - 1; x += halfway)
                {
                    for (int y = (halfway + x) % sideLength; y < LENGTH - 1; y += sideLength)
                    {
                        float top = map[x, (y - halfway + LENGTH) % LENGTH];
                        float bottom = map[x, (y + halfway) % LENGTH];
                        float left = map[(x - halfway + LENGTH) % LENGTH, y];
                        float right = map[(x + halfway) % LENGTH, y];

                        float average = (top + bottom + left + right) / 4;

                        map[x, y] = average + rand.NextFloat(-1, 1) * displaceRange;

                        if (x == 0)
                        {
                            map[LENGTH - 1, y] = map[x, y];
                        }
                        if (y == 0)
                        {
                            map[x, LENGTH - 1] = map[x, y];
                        }
                    }
                }

                displaceRange *= H;
            }
            return map;
        }

        // Calculates the vertex normals
        Vector3[,] calcNorms()
        {
            Vector3[,] norms = new Vector3[LENGTH, LENGTH];
            Vector3 top;
            Vector3 bottom;
            Vector3 left;
            Vector3 right;
            Vector3 current;
            for (int x = 0; x < LENGTH; x++)
            {
                for (int y = 0; y < LENGTH; y++)
                {
                    current = new Vector3(x,heightMap[x,y],y);
                    if (x == 0)
                    {
                        left = current;
                    }
                    else
                    {
                        left = new Vector3(x - 1, heightMap[x - 1, y], y);
                    }
                    if (x == LENGTH - 1)
                    {
                        right = current;
                    }
                    else
                    {
                        right = new Vector3(x + 1, heightMap[x + 1, y], y);
                    }
                    if (y == 0)
                    {
                        top = current;
                    }
                    else
                    {
                        top = new Vector3(x, heightMap[x, y-1], y-1);
                    }
                    if (y == LENGTH - 1)
                    {
                        bottom = current;
                    }
                    else
                    {
                        bottom = new Vector3(x, heightMap[x, y+1], y+1);
                    }

                    Vector3 xslope = right - left;
                    Vector3 zslope = bottom - top;
                    Vector3 norm = Vector3.Cross(xslope, zslope);
                    Vector3.Normalize(norm);
                    norms[x, y] = norm;
                }
            }
            return norms;
        }

        // Update step
        public override void Update(GameTime gameTime)
        {
            // Sets daylength WARNING higher value makes quicker day
            // For some reason it couldn't handle passing this value as a float < 1 (light would remain stationary)
            int daylength = 10000;
            double sunTime = gameTime.TotalGameTime.TotalMilliseconds * Math.PI / daylength;
            basicEffect.DirectionalLight0.Direction = new Vector3((float) Math.Cos(sunTime)*10, (float) Math.Sin(sunTime)*10, 0);
            basicEffect.View = game.camera.View;
            basicEffect.Projection = game.camera.Projection;

        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Sets the BlendState for the water   
            game.GraphicsDevice.SetBlendState(game.GraphicsDevice.BlendStates.AlphaBlend);

            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
