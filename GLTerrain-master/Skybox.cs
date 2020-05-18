using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class Skybox : IDisposable {
        private Texture[] texture = null;
        private Geometry mesh = null;

        private const float size = 10.0f;
        private const float size2 = size / 2.0f;

        public Skybox(string frontFile, string leftFile, string rightFile, string backFile, string topFile, string bottomFile) {
            texture = new Texture[String.IsNullOrEmpty(bottomFile) ? 5 : 6];

            texture[0] = new Texture(frontFile, false);
            texture[0].Wrap = false;

            texture[1] = new Texture(leftFile, false);
            texture[1].Wrap = false;

            texture[2] = new Texture(rightFile, false);
            texture[2].Wrap = false;

            texture[3] = new Texture(backFile, false);
            texture[3].Wrap = false;

            texture[4] = new Texture(topFile, false);
            texture[4].Wrap = false;

            if (texture.Length > 5) {
                texture[5] = new Texture(bottomFile, false);
                texture[5].Wrap = false;
            }

            mesh = new Geometry(BeginMode.Quads,
                new VertexBuffer(new Vertex[] {
                new Vertex() {X = -size2, Y = -size2, Z = 0, S = 0.0f, T = 1},
                new Vertex() {X = size2, Y = -size2, Z = 0, S = 1.0f, T = 1},
                new Vertex() {X = size2, Y = size2, Z = 0, S = 1.0f, T = 0.0f},
                new Vertex() {X = -size2, Y = size2, Z = 0, S = 0.0f, T = 0.0f}
                }, new ushort[] {
                    0, 1, 2, 3
                }));

        }

        public void Draw(Camera cam, bool flip) {
            GL.PushAttrib(AttribMask.EnableBit | AttribMask.FogBit | AttribMask.LightingBit);
            GL.PushMatrix();
            {
                GL.Disable(EnableCap.Fog);
                GL.Disable(EnableCap.Lighting);
                GL.Disable(EnableCap.DepthTest);
                GL.DepthMask(false);

                Matrix4 mat = MatrixHelper.MatrixFromQuat(Quaternion.Invert(cam.Orientation));

                GL.LoadMatrix(ref mat);
                if (flip) {
                    GL.Scale(1, -1, 1);
                }

                // front
                mesh.Scale = new Vector3(1, 1, 1);
                mesh.Orientation = Quaternion.Identity;
                mesh.Position = new Vector3(0, 0, -size2);
                texture[0].Bind();
                mesh.Draw();
                texture[0].Unbind();

                //back
                mesh.Scale = new Vector3(-1, 1, 1);
                mesh.Position = new Vector3(0, 0, size2);
                texture[3].Bind();
                mesh.Draw();
                texture[3].Unbind();

                // left
                mesh.Orientation = Quaternion.FromAxisAngle(new Vector3(0, 1, 0), -MathHelper.PiOver2);
                texture[1].Bind();
                mesh.Draw();
                texture[1].Unbind();

                // right
                mesh.Scale = new Vector3(1, 1, 1);
                mesh.Position = new Vector3(0, 0, -size2);
                texture[2].Bind();
                mesh.Draw();
                texture[2].Unbind();

                // top
                mesh.Scale = new Vector3(1, 1, 1);
                mesh.Orientation = Quaternion.FromAxisAngle(new Vector3(1, 0, 0), MathHelper.PiOver2) * 
                    Quaternion.FromAxisAngle(new Vector3(0, 0, 1), -MathHelper.PiOver2);
                texture[4].Bind();
                mesh.Draw();
                texture[4].Unbind();

                GL.DepthMask(true);
            }
            GL.PopMatrix();
            GL.PopAttrib();
        }

        public void Dispose() {
            mesh.Dispose();
            foreach (Texture tex in texture) {
                tex.Dispose();
            }
        }
    }
}
