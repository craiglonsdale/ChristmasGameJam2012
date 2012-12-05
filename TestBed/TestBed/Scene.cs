using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using TestBed.Tiles;
using TestBed.Lighting;
using ProjectMercury;
using ProjectMercury.Renderers;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Factories;
using TestBed.Interface;

namespace TestBed
{
    public class Scene
    {
        private Game1 m_game;
        private List<StaticCollidableTile> staticTileList;
        private DynamicCollidableTile dynamicTileOne;
        private Texture2D Green;
        private Texture2D Red;
        private SpriteFont CourierNew;
        private Renderer m_particleRenderer;

        Model m_shipModel;

        public Scene(Game1 game)
        {
            this.m_game = game;
            DirectionalLights = new List<TestBed.Lighting.DirectionalLight>();
            PointLights = new List<PointLight>();
            ParticleEffects = new List<LightCausingParticleObject>();

            m_particleRenderer = new SpriteBatchRenderer()
            {
                GraphicsDeviceService = m_game.graphics
            };
        }

        public void InitializeScene(SpriteBatch spriteBatch, World physicsWorld, Camera2D camera2D)
        {
            CourierNew = m_game.Content.Load<SpriteFont>("Text");
            m_shipModel = m_game.Content.Load<Model>("Models\\ship1");
            Green = m_game.Content.Load<Texture2D>("Images/Green_Front");
            Red = m_game.Content.Load<Texture2D>("Images/Red_Front");

            dynamicTileOne = new DynamicCollidableTile(spriteBatch, Red, new Vector2(250.0f, 160.0f), new Rectangle(0, 0, 5, 5), 0.0f, physicsWorld);
            staticTileList = new List<StaticCollidableTile>();

            m_particleRenderer.LoadContent(m_game.Content);
            ParticleEffect pEffect = m_game.Content.Load<ParticleEffect>("ParticleEffects\\BasicFireball");

            AddParticleEffect(pEffect, dynamicTileOne);

            for (int i = 0; i < 10; i++)
            {
                staticTileList.Add(new StaticCollidableTile(spriteBatch, Green, new Vector2(250.0f + (i * 100), 180.0f), new Rectangle(0, 0, 100, 5), 0.0f, physicsWorld));
            }
            
            camera2D.TrackingBody = ((DynamicCollidableTile)dynamicTileOne).PhysicsBody;


            PointLights.Add(new PointLight()
            {
               Colour = Color.Red,
               LightIntensity = 4,
               LightRadius = 50,
               LightPosition = new Vector3(300, 160, -100)
            });

            PointLights.Add(new PointLight()
            {
                Colour = Color.Green,
                LightIntensity = 4,
                LightRadius = 50,
                LightPosition = new Vector3(500, 160, -100)
            });

            PointLights.Add(new PointLight()
            {
                Colour = Color.Yellow,
                LightIntensity = 4,
                LightRadius = 50,
                LightPosition = new Vector3(700, 160, -100)
            });

            PointLights.Add(new PointLight()
            {
                Colour = Color.Blue,
                LightIntensity = 4,
                LightRadius = 50,
                LightPosition = new Vector3(900, 160, -100)
            });

            DirectionalLights.Add(new Lighting.DirectionalLight()
            {
                Colour = Color.White,
                LightDirection = new Vector3(-1, -1, 0)
            });
            
        }

        public void Draw2DShit()
        {
            staticTileList.ForEach(tile => tile.Draw());
            dynamicTileOne.Draw();
        }

        public void WriteText(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(CourierNew,
                                   "\nCharacter Position: " + dynamicTileOne.PositionIn3DDisplay.ToString()
                                   + "\nLight Position: " + PointLights[0].LightPosition, 
                                   ConvertUnits.ToDisplayUnits(dynamicTileOne.Position), 
                                   Color.White, 0, new Vector2(100, 0), 0.33f, SpriteEffects.None, 0);
        }

        public DynamicCollidableTile Character
        {
            get
            {
                return dynamicTileOne;
            }
        }

        private void DrawModel(Model model, Matrix world, Camera camera)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["Projection"].SetValue(camera.Projection);
                }
                mesh.Draw();
            }
        }

        private void AddParticleEffect(ParticleEffect particleEffect, ICollidableTile trackingObject)
        {
            particleEffect.LoadContent(m_game.Content);
            particleEffect.Initialise();

            var entry = new LightCausingParticleObject(particleEffect, m_particleRenderer,
            new PointLight()
            {
               Colour = Color.Orange,
               LightIntensity = 4,
               LightRadius = 50,
               LightPosition = new Vector3(300, 160, -100)
            }, trackingObject);

            ParticleEffects.Add(entry);
        }

        public void DrawParticles(Camera2D camera)
        {
            ParticleEffects.ForEach(entity => entity.Render(camera.get_transformation()));
        }

        public void DrawScene(Camera camera, GameTime gameTime)
        {
            m_game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            m_game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            m_game.GraphicsDevice.BlendState = BlendState.Opaque;

            for (int i = 0; i < 10; i++)
            {
                Matrix position = Matrix.CreateRotationY(3.141592654f) * Matrix.CreateTranslation((100 * i), 180, -100);//3.141592654f);
                DrawModel(m_shipModel, position, camera);

            }
        }

        public List<PointLight> PointLights {get; private set;}
        public List<TestBed.Lighting.DirectionalLight> DirectionalLights {get; private set;}
        public List<LightCausingParticleObject> ParticleEffects { get; private set; }
    }
}
