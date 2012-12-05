using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using TestBed.Lighting;

namespace TestBed
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class DeferredRenderer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Camera m_camera;
        private Camera2D m_camera2D;
        private QuadRenderComponent m_quadRenderer;
        private Scene m_scene;
        private Model m_sphere;
        private Input m_input;

        private RenderTarget2D m_colourRT; //Holds colour and specular intensity
        private RenderTarget2D m_normalRT; //Holds the normals and specular power
        private RenderTarget2D m_depthRT;  //Holds the depth.
        private RenderTarget2D m_lightRT;  //Holds the light...stuff...
        
        private Effect m_clearBufferEffect;
        private Effect m_directionalLightEffect;
        private Effect m_combineFinalEffect;
        private Effect m_pointLightEffect;

        private SpriteBatch m_spriteBatch;
        private World m_physicsWorld;
        private Vector2 m_halfPixel;

        public DeferredRenderer(Game1 game)
            : base(game)
        {
            m_scene = new Scene(game);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            m_camera = new Camera(Game);
            m_quadRenderer = new QuadRenderComponent(Game);

            Game.Components.Add(m_camera);
            Game.Components.Add(m_quadRenderer);

            m_camera.SetTrackingCamera(m_camera2D);
        }

        private void LoadKeyBindings()
        {
            m_input.BindChordToAction(new Chord { Keys = new List<Keys> { Keys.D }, State = ChordState.Pressed }, () =>
            {
                var body = m_scene.Character;
                if (body.PhysicsBody.LinearVelocity.X < 5f)
                {
                    body.ApplyForce(new Vector2(0.005f, 0f));
                }
            });

            m_input.BindChordToAction(new Chord { Keys = new List<Keys> { Keys.A }, State = ChordState.Pressed }, () =>
            {
                var body = m_scene.Character;
                if (body.PhysicsBody.LinearVelocity.X > -5f)
                {
                    body.ApplyForce(new Vector2(-0.005f, 0f));
                }
            });
        }

        protected override void LoadContent()
        {
            m_input = new Input(Game);
            LoadKeyBindings();

            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            m_physicsWorld = new World(new Vector2(0, 20));
            m_camera2D = new Camera2D(GraphicsDevice);
            m_camera2D.Zoom = 4f;

            m_scene.InitializeScene(m_spriteBatch, m_physicsWorld, m_camera2D);
            m_sphere = Game.Content.Load<Model>(@"Models\sphere");

            int backBufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int backBufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            m_colourRT = new RenderTarget2D(GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            m_normalRT = new RenderTarget2D(GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
            m_depthRT = new RenderTarget2D(GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Single, DepthFormat.None);
            m_lightRT = new RenderTarget2D(GraphicsDevice, backBufferWidth, backBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

            m_clearBufferEffect = Game.Content.Load<Effect>(@"Shaders/ClearGBuffer");
            m_directionalLightEffect = Game.Content.Load<Effect>(@"Shaders/DirectionalLight");
            m_combineFinalEffect = Game.Content.Load<Effect>(@"Shaders/CombineFinal");
            m_pointLightEffect = Game.Content.Load<Effect>(@"Shaders/PointLight");

            m_halfPixel.X = 0.5f / (float)GraphicsDevice.PresentationParameters.BackBufferWidth;
            m_halfPixel.Y = 0.5f / (float)GraphicsDevice.PresentationParameters.BackBufferHeight;

            base.LoadContent();
        }

        private void SetGBuffer()
        {
            GraphicsDevice.SetRenderTargets(m_colourRT, m_normalRT, m_depthRT);
        }

        private void ResolveGBuffer()
        {
            GraphicsDevice.SetRenderTarget(null);
        }

        private void ClearGBuffer()
        {
            m_clearBufferEffect.Techniques[0].Passes[0].Apply();   
            m_quadRenderer.Render(Vector2.One * -1, Vector2.One);
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw Deferred Render Scene
            SetGBuffer();
            m_scene.DrawScene(m_camera, gameTime);
            ResolveGBuffer();
            DrawLights(gameTime);

            //Draw 2D Scene
            m_spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, m_camera2D.BatchViewMatrix);
                m_scene.Draw2DShit();
                //m_scene.WriteText(m_spriteBatch);
                m_scene.DrawParticles(m_camera2D);
            m_spriteBatch.End();

            


            //=================================================
            // Deferred Debugging Stuff
            //=================================================
            //int halfWidth = GraphicsDevice.Viewport.Width / 2;
            //int halfHeight = GraphicsDevice.Viewport.Height / 2;

            //m_spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            //    m_spriteBatch.Draw(m_colourRT, new Rectangle(0, 0, halfWidth, halfHeight), Color.White);
            //    m_spriteBatch.Draw(m_normalRT, new Rectangle(0, halfHeight, halfWidth, halfHeight), Color.White);
            //    m_spriteBatch.Draw(m_lightRT, new Rectangle(halfWidth, 0, halfWidth, halfHeight), Color.White);

            //m_spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            m_physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds / 10f);
            m_camera2D.Update(gameTime);
            m_camera.Update(gameTime);
            m_input.Update();

            m_scene.ParticleEffects.ForEach(entry => 
            {
                entry.Update(gameTime, m_camera2D);
            });

            base.Update(gameTime);
        }

        private void DrawPointLight(Vector3 lightPosition, Color colour, float lightRadius, float lightIntensity)
        {
            //Set the GBuffer params
            m_pointLightEffect.Parameters["colourMap"].SetValue(m_colourRT);
            m_pointLightEffect.Parameters["normalMap"].SetValue(m_normalRT);
            m_pointLightEffect.Parameters["depthMap"].SetValue(m_depthRT);

            //Compute the light world matrix
            //Scale according to light radius, and translate to the light position
            Matrix sphereWorldMatrix = Matrix.CreateScale(lightRadius) * Matrix.CreateTranslation(lightPosition);

            m_pointLightEffect.Parameters["World"].SetValue(sphereWorldMatrix);
            m_pointLightEffect.Parameters["View"].SetValue(m_camera.View);
            m_pointLightEffect.Parameters["Projection"].SetValue(m_camera.Projection);
            m_pointLightEffect.Parameters["lightPosition"].SetValue(lightPosition);
            m_pointLightEffect.Parameters["Colour"].SetValue(colour.ToVector3());
            m_pointLightEffect.Parameters["lightRadius"].SetValue(lightRadius);
            m_pointLightEffect.Parameters["lightIntensity"].SetValue(lightIntensity);
            m_pointLightEffect.Parameters["cameraPosition"].SetValue(m_camera.Position);
            m_pointLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(m_camera.View * m_camera.Projection));
            m_pointLightEffect.Parameters["halfPixel"].SetValue(m_halfPixel);

            //Calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(m_camera.Position, lightPosition);

            //if we are inside the light volume, draw the sphere's inside face
            if (cameraToCenter < lightRadius)
            {
                GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            }
            else
            {
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }

            m_pointLightEffect.Techniques[0].Passes[0].Apply();

            foreach (ModelMesh mesh in m_sphere.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    GraphicsDevice.Indices = meshPart.IndexBuffer;
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                    0,
                    0,
                    meshPart.NumVertices,
                    meshPart.StartIndex,
                    meshPart.PrimitiveCount);
                }
            }

            //Return the cull mode back default
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        private void DrawLights(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(m_lightRT);

            //Clear all components to 0
            GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            foreach (var light in m_scene.DirectionalLights)
            {
                DrawDirectionalLight(light.LightDirection, light.Colour);
            }

            foreach (var light in m_scene.PointLights)
            {
                DrawPointLight(light.LightPosition, light.Colour, light.LightRadius, light.LightIntensity);
            }

            foreach (var particleEffect in m_scene.ParticleEffects)
            {
                DrawPointLight(particleEffect.Light.LightPosition, particleEffect.Light.Colour, particleEffect.Light.LightRadius, particleEffect.Light.LightIntensity);
            }

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            GraphicsDevice.SetRenderTarget(null);

            m_combineFinalEffect.Parameters["colourMap"].SetValue(m_colourRT);
            m_combineFinalEffect.Parameters["lightMap"].SetValue(m_lightRT);
            m_combineFinalEffect.Parameters["halfPixel"].SetValue(m_halfPixel);

            m_combineFinalEffect.Techniques[0].Passes[0].Apply();
            m_quadRenderer.Render(Vector2.One * -1, Vector2.One);
        }

        private void DrawDirectionalLight(Vector3 lightDirection, Color colour)
        {
            m_directionalLightEffect.Parameters["colourMap"].SetValue(m_colourRT);
            m_directionalLightEffect.Parameters["normalMap"].SetValue(m_normalRT);
            m_directionalLightEffect.Parameters["depthMap"].SetValue(m_depthRT);

            m_directionalLightEffect.Parameters["lightDirection"].SetValue(lightDirection);
            m_directionalLightEffect.Parameters["Colour"].SetValue(colour.ToVector3());
            m_directionalLightEffect.Parameters["cameraPosition"].SetValue(m_camera.Position);
            m_directionalLightEffect.Parameters["halfPixel"].SetValue(m_halfPixel);

            m_directionalLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(m_camera.View * m_camera.Projection));

            m_directionalLightEffect.Techniques[0].Passes[0].Apply();
            m_quadRenderer.Render(Vector2.One * -1, Vector2.One);
        }
    }
}
