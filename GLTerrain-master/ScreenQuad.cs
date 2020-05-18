using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class ScreenQuad {
        public Geometry Quad { get; private set; }
        private Matrix4 orthoMatrix = Matrix4.Identity;

        public ScreenQuad() {
            Quad = new Geometry(BeginMode.Quads,
                new VertexBuffer(new Vertex[] {
                    new Vertex() {X = 0, Y = 0, Z = 0, S = 0, T = 0},
                    new Vertex() {X = 0, Y = 1, Z = 0, S = 0, T = 1},
                    new Vertex() {X = 1, Y = 1, Z = 0, S = 1, T = 1},
                    new Vertex() {X = 1, Y = 0, Z = 0, S = 1, T = 0}
                }, new ushort[] {
                    0, 1, 2, 3
                }));

            Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -1, 1, out orthoMatrix);
        }

        public void Draw() {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();

            GL.LoadMatrix(ref orthoMatrix);

            GL.PushAttrib(AttribMask.CurrentBit | AttribMask.DepthBufferBit | AttribMask.LightingBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            {
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.DepthTest);
                GL.LoadIdentity();

                Quad.Draw();
            }
            GL.PopMatrix();
            GL.PopAttrib();

            // pop ortho projection and set mode back to modelview
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
        }
    }
}
