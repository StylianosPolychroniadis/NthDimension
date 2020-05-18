using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public enum LightType {
        Directional,
        Point,
        Spot
    }

    public class Light {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Color4 Diffuse { get; set; }
        public Color4 Specular { get; set; }

        public LightType Type { get; set; }
        public LightName Name { get; set; }
        public bool Enabled { get; set; }

        public Light(
            LightName name, 
            LightType type, 
            Vector3 position, 
            Vector3 direction, 
            Color4 diffuse, 
            Color4 specular) {

            Name = name;
            Type = type;
            Position = position;
            if (type == LightType.Spot) {
                Direction = direction;
            }

            Diffuse = diffuse;
            Specular = specular;
            Enabled = true;
        }

        public Light(LightName name, LightType type, Vector3 position, Vector3 direction) 
            : this(name, type, position, direction, Color4.White, Color4.White) {
        }

        public Light(LightName name, LightType type) : this(name, type, new Vector3(), new Vector3()) {
        }
    }
}
