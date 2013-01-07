#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace TestBed
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        private float cameraArc = -30;

        public float CameraArc
        {
            get { return cameraArc; }
            set { cameraArc = value; }
        }

        private float cameraRotation = 0;

        public float CameraRotation
        {
            get { return cameraRotation; }
            set { cameraRotation = value; }
        }

        private float cameraDistance = 1000;

        public float CameraDistance
        {
            get { return cameraDistance; }
            set { cameraDistance = value; }
        }
        private Matrix view3D;
        private Matrix projection;

        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
	
        public Matrix View3D
        {
            get
            {
                
                return view3D;
            }
        }

        //public Matrix View2D
        //{
        //    get
        //    {

        //        return view2D;
        //    }
        //}

        public Matrix Projection
        {
            get
            {
                return projection;
            }
        }

        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public void SetTrackingBody(Body trackingBody)
        {
            m_trackingBody = trackingBody;
        }
        Body m_trackingBody = null;

        public Matrix get_transformation(GraphicsDevice graphics)
        {
            Matrix _transform = 
            Matrix.CreateTranslation(Position) *
                //screen view
            Matrix.CreateTranslation(new Vector3(graphics.Viewport.Width * 0.5f, graphics.Viewport.Height * 0.5f, 0));
            return _transform;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            //// TODO: Add your update code here

            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (m_trackingBody != null)
            {
                var pos = ConvertUnits.ToDisplayUnits(m_trackingBody.Position);
                Position = new Vector3(-pos.X,
                                       0,
                                        0);
            }

            view3D = Matrix.CreateTranslation(Position) *
                     //Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                     //Matrix.CreateScale(0.01f);
                     //Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                     Matrix.CreateLookAt(new Vector3(0, 0, CameraDistance), new Vector3(0, 0, 0),
                                           Vector3.Up);

            Position = Vector3.Transform(Vector3.Zero,Matrix.Invert(View3D));

            float aspectRatio = (float)Game.Window.ClientBounds.Width /
                                (float)Game.Window.ClientBounds.Height;

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1,
                                                                    3000);
            base.Update(gameTime);
        }

        public Vector2 ConvertScreenToWorld(Vector2 location, GraphicsDevice graphics)
        {
            Vector3 t = new Vector3(location, 0);


            t = graphics.Viewport.Unproject(t, Projection, View3D, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }

        public Vector2 ConvertWorldToScreen(Vector2 location, GraphicsDevice graphics)
        {
            Vector3 t = new Vector3(location, 0);

            t = graphics.Viewport.Project(t, Projection, View3D, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }
    }
}