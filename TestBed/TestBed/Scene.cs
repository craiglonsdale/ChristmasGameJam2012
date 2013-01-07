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
using GLEED2D;

namespace TestBed
{
    public class Scene
    {
        private Game1 m_game;
        private DynamicCollidableTile dynamicTileOne;
        private Texture2D Green;
        private Texture2D Red;
        private SpriteFont CourierNew;
        private Renderer m_particleRenderer;
        private Level m_level;
        private Item m_playerStart;
        private Body[] collidableBodies;

        Model m_shipModel;
        Model m_floorModel;

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

        private void InitializeLightingScene()
        {
            //All lighting objects are requires to be in a layer named Lighting
            var lighting = m_level.getLayerByName("Lighting").Items;

            for (int i = 0; i < lighting.Count; ++i)
            {
                Vector3 lightPosition = new Vector3();
                lightPosition.X = lighting[i].Position.X;
                lightPosition.Y = lighting[i].Position.Y;
                lightPosition.Z = 0;

                PointLights.Add(new PointLight()
                {
                    Colour = Color.Red,
                    LightIntensity = 4,
                    LightRadius = 50,
                    LightPosition = lightPosition
                });
            }
        }

        private void InitializePhysicsScene(World physicsWorld)
        {
            //All collidable objects are requires to be in a layer named COllidable
            var collisionItems = m_level.getLayerByName("Collidable").Items;
            var collidablePositions = new Vector2[collisionItems.Count];
            collidableBodies = new Body[collisionItems.Count];

            for (int i = 0; i < collisionItems.Count; ++i)
            {
                collidablePositions[i].X = ConvertUnits.ToSimUnits(collisionItems[i].Position.X) + ConvertUnits.ToSimUnits(((RectangleItem)collisionItems[i]).Width) / 2;
                collidablePositions[i].Y = ConvertUnits.ToSimUnits(collisionItems[i].Position.Y) + ConvertUnits.ToSimUnits(((RectangleItem)collisionItems[i]).Height) / 2;
                collidableBodies[i] = BodyFactory.CreateRectangle(
                                            physicsWorld,
                                            ConvertUnits.ToSimUnits(((RectangleItem)collisionItems[i]).Height),
                                            ConvertUnits.ToSimUnits(((RectangleItem)collisionItems[i]).Width),
                                            1f,
                                            collidablePositions[i]);
                collidableBodies[i].BodyType = BodyType.Static;
                collidableBodies[i].Restitution = 0.3f;
                collidableBodies[i].Friction = 0.5f;
            }
        }

        public void InitializeScene(SpriteBatch spriteBatch, World physicsWorld, Camera camera)
        {
            
            m_level = Level.FromFile("Content\\Levels\\level1.xml", m_game.Content);
            //m_playerStart = m_level.getItemByName("heroStart");
            InitializePhysicsScene(physicsWorld);
            InitializeLightingScene();
            CourierNew = m_game.Content.Load<SpriteFont>("Text");
            m_shipModel = m_game.Content.Load<Model>("Models\\ship1");
            m_floorModel = m_game.Content.Load<Model>("Models\\Ground");
            Green = m_game.Content.Load<Texture2D>("Images/Green_Front");
            Red = m_game.Content.Load<Texture2D>("Images/Red_Front");

            dynamicTileOne = new DynamicCollidableTile(spriteBatch, Red, new Vector2(0.0f, 0.0f), new Rectangle(0, 0, 5, 5), 0.0f, physicsWorld);
            camera.SetTrackingBody(dynamicTileOne.PhysicsBody);

            m_particleRenderer.LoadContent(m_game.Content);
            ParticleEffect pEffect = m_game.Content.Load<ParticleEffect>("ParticleEffects\\FlameThrower");

            AddParticleEffect(pEffect, dynamicTileOne);

            DirectionalLights.Add(new Lighting.DirectionalLight()
            {
                Colour = Color.White,
                LightDirection = new Vector3(-1, -1, 0)
            });
            
        }

        public void Draw2DShit(SpriteBatch spriteBatch)
        {
            foreach (var layer in m_level.Layers)
            {
                if(layer.Name  == "Textures")
                    layer.draw(spriteBatch);
            }

            dynamicTileOne.Draw();
        }

        public void WriteText(SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {
            spriteBatch.DrawString(CourierNew, camera.Position.ToString(), new Vector2(graphics.Viewport.Width/2, graphics.Viewport.Height/2), 
                                   Color.White, 0, new Vector2(0, 0), 5, SpriteEffects.None, 0);
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
                    effect.Parameters["View"].SetValue(camera.View3D);
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
               LightRadius = 25,
               LightPosition = new Vector3(300, 160, -100)
            }, trackingObject)
            {
                Name = "FlameThrower",
                Enabled = false
            };

            ParticleEffects.Add(entry);
        }

        public void DrawParticles(Camera camera, GraphicsDevice graphics)
        {
            ParticleEffects.ForEach(entity => entity.Render(camera.get_transformation(graphics)));
        }

        public void DrawScene(Camera camera, GameTime gameTime)
        {
            m_game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            m_game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            m_game.GraphicsDevice.BlendState = BlendState.Opaque;
            var rotation = (3.141592654f / 2f);
            for (int i = 0; i < 5; i++)
            {
                DrawModel(m_floorModel, Matrix.CreateRotationX(rotation) * Matrix.CreateTranslation(200 * i, 0, 0), camera);
            }
        }

        public List<PointLight> PointLights {get; private set;}
        public List<TestBed.Lighting.DirectionalLight> DirectionalLights {get; private set;}
        public List<LightCausingParticleObject> ParticleEffects { get; private set; }
    }
}
