using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using TestBed.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;

namespace TestBed
{
    public class Character : AbstractCollidableTile
    {
        public Character(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle tileSize, float rotation, World physicsWorld)
            : base(spriteBatch, texture, position, tileSize, rotation, physicsWorld, 1.0f)
        {
            position = ConvertUnits.ToSimUnits(position);

            //Create body that is half size of etire object
            float upperBody = ConvertUnits.ToSimUnits(tileSize.Height) - (ConvertUnits.ToSimUnits(tileSize.Width) / 2.0f);
            PhysicsBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(tileSize.Height), upperBody, 10.0f);
            //shift it up a tiny bit to keep the new objects center correct
            PhysicsBody.Position = position - Vector2.UnitY * (ConvertUnits.ToSimUnits(tileSize.Width) / 4);
            float centerOffset = position.Y - PhysicsBody.Position.Y;

            //Force the upper body to stay upright
            var fixedAngleJoint = JointFactory.CreateFixedAngleJoint(physicsWorld, PhysicsBody);
            
            //Create a wheel as wide as PhysicsBody
            WheelBody = BodyFactory.CreateCircle(physicsWorld, ConvertUnits.ToSimUnits(tileSize.Width) / 2.0f, 1.0f);
            //Position its center at the bottom of the upper body
            WheelBody.Position = PhysicsBody.Position + Vector2.UnitY * (upperBody / 2.0f);

            var motor = JointFactory.CreateRevoluteJoint(physicsWorld, PhysicsBody, WheelBody, WheelBody.Position);
            motor.MotorEnabled = true;
            motor.MaxMotorTorque = 1000f;
            motor.MotorSpeed = 2f;
            WheelBody.IgnoreCollisionWith(PhysicsBody);
            WheelBody.Friction = float.MaxValue;
            PhysicsBody.IgnoreCollisionWith(WheelBody);
            PhysicsBody.BodyType = BodyType.Dynamic;
            WheelBody.BodyType = BodyType.Dynamic;
        }

        public Body WheelBody
        {
            get;
            private set;
        }
    }
}
