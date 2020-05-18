using System;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class Viewport {
        public int Width { get; set; }
        public int Height { get; set; }

        public float AspectRatio {
            get {
                return Width / (float)Height;
            }
        }

        public Viewport(int width, int height) {
            Width = width;
            Height = height;
        }

        public void MakeCurrent() {
            GL.Viewport(0, 0, Width, Height);
        }
    }
}
