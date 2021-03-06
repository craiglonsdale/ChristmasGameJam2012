﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMercury;
using TestBed.Lighting;
using TestBed.Interface;
using Microsoft.Xna.Framework;
using ProjectMercury.Renderers;
using Microsoft.Xna.Framework.Graphics;

namespace TestBed
{
    public class LightCausingParticleObject
    {
        private Renderer m_particleRenderer = null;

        public LightCausingParticleObject(ParticleEffect effect, Renderer partilceRenderer, PointLight pointLight, ICollidableTile trackingObject)
        {
            Enabled = true;
            Effect = effect;
            Light = pointLight;
            TrackingObject = trackingObject;
            m_particleRenderer = partilceRenderer;
        }

        public void Update(GameTime gameTime, Camera camera, GraphicsDevice graphics)
        {
            var position = camera.ConvertWorldToScreen(TrackingObject.PhysicsBody.Position, graphics);
            if (Enabled)
            {
                Effect.Trigger(position);
            }

            Effect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            Vector2 lightPosition =  ConvertUnits.ToDisplayUnits(TrackingObject.Position);
            Light.LightPosition =  new Vector3(lightPosition.X, lightPosition.Y, 0);

        }

        public string Name
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public void Render(Matrix transformMatrix)
        {
            m_particleRenderer.RenderEffect(Effect, ref transformMatrix);
        }

        public ParticleEffect Effect
        {
            get;
            private set;
        }

        public PointLight Light
        {
            get;
            private set;
        }

        public ICollidableTile TrackingObject
        {
            get
            {
                return m_trackingObject;
            }
            private set
            {
                m_trackingObject = value;
            }
        }

        ICollidableTile m_trackingObject = null;
    }
}
