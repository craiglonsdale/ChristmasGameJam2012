using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TestBed.Lighting
{
    public class PointLight
    {
        public Vector3 LightPosition {get; set;} 
        public Color Colour {get; set;} 
        public float LightRadius {get; set;}
        public float LightIntensity {get; set;}
    }
}
