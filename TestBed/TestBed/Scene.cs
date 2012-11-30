using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using TestBed.Tiles;

namespace TestBed
{
    public class Scene
    {
        private Game1 m_game;
        private List<StaticCollidableTile> staticTileList;
        private DynamicCollidableTile dynamicTileOne;
        private Texture2D Green;
        private Texture2D Red;

        Model m_shipModel;
        Model m_boxModel;

        public Scene(Game1 game)
        {
            this.m_game = game;
        }

        public void InitializeScene(SpriteBatch spriteBatch, World physicsWorld, Camera2D camera2D)
        {
            m_shipModel = m_game.Content.Load<Model>("Models\\ship1");
            m_boxModel = m_game.Content.Load<Model>("Models\\wc1");
            Green = m_game.Content.Load<Texture2D>("Images/Green_Front");
            Red = m_game.Content.Load<Texture2D>("Images/Red_Front");
            dynamicTileOne = new DynamicCollidableTile(spriteBatch, Red, new Vector2(250.0f, 160.0f), new Rectangle(0, 0, 5, 5), 0.0f, physicsWorld);
            staticTileList = new List<StaticCollidableTile>();

            for (int i = 0; i < 10; i++)
            {
                staticTileList.Add(new StaticCollidableTile(spriteBatch, Green, new Vector2(250.0f + (i * 100), 180.0f), new Rectangle(0, 0, 100, 5), 0.0f, physicsWorld));
            }
            
            camera2D.TrackingBody = ((DynamicCollidableTile)dynamicTileOne).PhysicsBody;
        }

        public void Draw2DShit()
        {
            staticTileList.ForEach(tile => tile.Draw());
            dynamicTileOne.Draw();
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


        public void DrawScene(Camera camera, GameTime gameTime)
        {
            m_game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            m_game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            m_game.GraphicsDevice.BlendState = BlendState.Opaque;

            for (int i = 0; i < 10; i++)
            {
                Matrix position = Matrix.CreateTranslation(-100 * i, 470, -800);
                DrawModel(m_shipModel, position, camera);

            }
        }
    }
}
