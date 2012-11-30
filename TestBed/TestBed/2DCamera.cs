using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace TestBed
{
    public class Camera2D
    {
        private const float MIN_ZOOM = 0.02f;
        private const float MAX_ZOOM = 20.0f;
        private static GraphicsDevice m_graphics;

        private Body m_trackingBody;

        private Vector2 m_currentPosition;
        private Vector2 m_targetPosition;
        private Vector2 m_minPosition;
        private Vector2 m_maxPosition;
        private Vector2 m_translateCenter;

        private float m_minRotation;
        private float m_maxRotation;
        private float m_currentRotation;
        private float m_targetRotation;
        private float m_currentZoom;

        public Camera2D(GraphicsDevice graphics)
        {
            m_graphics = graphics;
            
            ProjectionMatrix = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(m_graphics.Viewport.Width),
                                                                  ConvertUnits.ToSimUnits(m_graphics.Viewport.Height), 0f, 0f, 1f);

            ViewMatrix = Matrix.Identity;
            BatchViewMatrix = Matrix.Identity;

            m_translateCenter = new Vector2(ConvertUnits.ToSimUnits(m_graphics.Viewport.Width / 2.0f),
                                            ConvertUnits.ToSimUnits(m_graphics.Viewport.Height / 2.0f));

            ResetCamera();
        }

        /// <summary>
        /// Resets the camera to default values.
        /// </summary>
        public void ResetCamera()
        {
            m_currentPosition = Vector2.Zero;
            m_targetPosition = Vector2.Zero;
            m_minPosition = Vector2.Zero;
            m_maxPosition = Vector2.Zero;

            m_currentRotation = 0.0f;
            m_targetRotation = 0f;
            m_minRotation = -MathHelper.Pi;
            m_maxRotation = MathHelper.Pi;

            PositionTracking = false;
            RotationTracking = false;

            m_currentZoom = 1f;

            SetView();
        }

        private void SetView()
        {
            Matrix matRotation = Matrix.CreateRotationZ(m_currentRotation);
            Matrix matZoom = Matrix.CreateScale(m_currentZoom);
            Vector3 translateCenter = new Vector3(m_translateCenter, 0f);
            Vector3 translateBody = new Vector3(-m_currentPosition, 0f);

            ViewMatrix = Matrix.CreateTranslation(translateBody) *
                            matRotation *
                            matZoom *
                            Matrix.CreateTranslation(translateCenter);

            translateCenter = ConvertUnits.ToDisplayUnits(translateCenter);
            translateBody = ConvertUnits.ToDisplayUnits(translateBody);

            BatchViewMatrix = Matrix.CreateTranslation(translateBody) *
                                 matRotation *
                                 matZoom *
                                 Matrix.CreateTranslation(translateCenter);
        }

        public bool PositionTracking
        {
            get;
            private set;
        }

        public bool RotationTracking
        {
            get;
            private set;
        }

        public Matrix ProjectionMatrix
        {
            get;
            private set;
        }

        public Matrix ViewMatrix
        {
            get;
            private set;
        }

        public Matrix BatchViewMatrix
        {
            get;
            private set;
        }

        public Vector2 Position
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(m_currentPosition);
            }
            set
            {
                m_targetPosition = ConvertUnits.ToSimUnits(value);
                if(m_minPosition != m_maxPosition)
                {
                    Vector2.Clamp(ref m_targetPosition, ref m_minPosition, ref m_maxPosition, out m_targetPosition);
                }

            }
        }

        /// <summary>
        /// Current Rotation in Radians
        /// </summary>
        public float Rotation
        {
            get
            {
                return m_currentRotation;
            }
            set
            {
                m_targetRotation = value % MathHelper.TwoPi;

                if(m_minRotation != m_maxRotation)
                {
                    m_targetRotation = MathHelper.Clamp(m_targetRotation, m_minRotation, m_maxRotation);
                }
            }
        }

        public float Zoom
        {
            get
            {
                return m_currentZoom;
            }
            set
            {
                m_currentZoom = value;
                m_currentZoom = MathHelper.Clamp(m_currentZoom, MIN_ZOOM, MAX_ZOOM); 
            }
        }

        public Vector2 MinPosition
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(m_minPosition);
            }
            set
            {
                m_minPosition = ConvertUnits.ToSimUnits(value);
            }
        }

        public Vector2 MaxPosition
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(m_maxPosition);
            }
            set
            {
                m_maxPosition = ConvertUnits.ToSimUnits(value);
            }
        }

        /// <summary>
        /// Max Rotation in Radians
        /// </summary>
        public float MaxRotation
        {
            get
            {
                return m_maxRotation;
            }
            set
            {
                m_maxRotation = MathHelper.Clamp(value, 0.0f, MathHelper.Pi);
            }
        }

        /// <summary>
        /// Min Rotation in Radians
        /// </summary>
        public float MinRotation
        {
            get
            {
                return m_minRotation;
            }
            set
            {
                m_minRotation = MathHelper.Clamp(value, -MathHelper.Pi, 0.0f);
            }
        }

        public Body TrackingBody
        {
            get
            {
                return m_trackingBody;
            }
            set
            {
                m_trackingBody = value;

                if (m_trackingBody != null)
                {
                    PositionTracking = true;
                }
            }
        }

        public bool EnablePositionTracking
        {
            get
            {
                return PositionTracking;
            }
            set
            {
                if (value && TrackingBody != null)
                {
                    PositionTracking = true;
                }
                else
                {
                    PositionTracking = false;
                }
            }
        }

        public bool EnableRotationTracking
        {
            get
            {
                return RotationTracking;
            }
            set
            {
                if (value && TrackingBody != null)
                {
                    RotationTracking = true;
                }
                else
                {
                    RotationTracking = false;
                }
            }
        }

        /// <summary>
        /// Sets both Position and Rotation tracking to true if we are currently Tracking a Body object.
        /// </summary>
        public bool EnableTracking
        {
            set
            {
                EnablePositionTracking = true;
                EnableRotationTracking = true;
            }
        }

        public void MoveCamera(Vector2 movement)
        {
            m_currentPosition += movement;
            if (m_minPosition != m_maxPosition)
            {
                Vector2.Clamp(ref m_currentPosition, ref m_minPosition, ref m_maxPosition, out m_currentPosition);
            }
            m_targetPosition = m_currentPosition;
            PositionTracking = false;
            RotationTracking = false;
        }

        public void RotateCamera(float rotation)
        {
            m_currentRotation += rotation;
            if (m_minRotation != m_maxRotation)
            {
                m_currentRotation = MathHelper.Clamp(m_currentRotation, m_minRotation, m_maxRotation);
            }
            m_targetRotation = m_currentRotation;
            PositionTracking = false;
            RotationTracking = false;
        }

        public void JumpToTarget()
        {
            m_currentRotation = m_targetRotation;
            m_currentPosition = m_targetPosition;

            SetView();
        }

        public void Update(GameTime gameTime)
        {
            if (TrackingBody != null)
            {
                if (PositionTracking)
                {
                    m_targetPosition = TrackingBody.Position;

                    if (m_minPosition != m_maxPosition)
                    {
                        Vector2.Clamp(ref m_targetPosition, ref m_minPosition, ref m_maxPosition, out m_targetPosition);
                    }
                }
                if (RotationTracking)
                {
                    m_targetRotation = -TrackingBody.Rotation % MathHelper.TwoPi;
                    if (m_minRotation != m_maxRotation)
                    {
                        m_targetRotation = MathHelper.Clamp(m_targetRotation, m_minRotation, m_maxRotation);
                    }
                }
            }
            Vector2 delta = m_targetPosition - m_currentPosition;
            float distance = delta.Length();
            if (distance > 0f)
            {
                delta /= distance;
            }
            float inertia;
            if (distance < 10f)
            {
                inertia = (float)Math.Pow(distance / 10.0, 2.0);
            }
            else
            {
                inertia = 1f;
            }

            float rotDelta = m_targetRotation - m_currentRotation;

            float rotInertia;
            if (Math.Abs(rotDelta) < 5f)
            {
                rotInertia = (float)Math.Pow(rotDelta / 5.0, 2.0);
            }
            else
            {
                rotInertia = 1f;
            }
            if (Math.Abs(rotDelta) > 0f)
            {
                rotDelta /= Math.Abs(rotDelta);
            }

            m_currentPosition += 100f * delta * inertia * Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f));
            m_currentRotation += 80f * rotDelta * rotInertia * Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f));

            SetView();
        }

        public Vector2 ConvertScreenToWorld(Vector2 location)
        {
            Vector3 t = new Vector3(location, 0);

            t = m_graphics.Viewport.Unproject(t, ProjectionMatrix, ViewMatrix, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }

        public Vector2 ConvertWorldToScreen(Vector2 location)
        {
            Vector3 t = new Vector3(location, 0);

            t = m_graphics.Viewport.Project(t, ProjectionMatrix, ViewMatrix, Matrix.Identity);

            return new Vector2(t.X, t.Y);
        }
    }
}
