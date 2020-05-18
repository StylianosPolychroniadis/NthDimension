using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLTerrain {
    public class ShaderProgram : IDisposable {
        public int ID { get; private set; }
        public bool IsAttached { get; private set; }

        public Shader VertexShader { get; private set; }
        public Shader FragmentShader { get; private set; }

        public ShaderProgram(string vertex, string fragment) {
            int id = GL.CreateProgram();

            if (!String.IsNullOrEmpty(vertex)) {
                VertexShader = new Shader(vertex, ShaderType.VertexShader);
                GL.AttachShader(id, VertexShader.ID);
            }

            if (!String.IsNullOrEmpty(fragment)) {
                FragmentShader = new Shader(fragment, ShaderType.FragmentShader);
                GL.AttachShader(id, FragmentShader.ID);
            }

            if (VertexShader == null && FragmentShader == null) {
                throw new ArgumentException("Shader source must not be empty/null!");
            }

            GL.LinkProgram(id);
            GL.ValidateProgram(id);

            int status = 0;
            GL.GetProgram(id, ProgramParameter.LinkStatus, out status);
            if (status == 0) {
                string info = GL.GetProgramInfoLog(id);
                throw new Exception(info);
            }

            ErrorCode code = GL.GetError();
            if (code != ErrorCode.NoError) {
                throw new Exception(code.ToString());
            }

            ID = id;
        }

        public void SetUniform(string name, int value) {
            bool attached = false;
            if (!IsAttached) {
                attached = IsAttached;
                Attach();
            }

            int location = GL.GetUniformLocation(ID, name);
            GL.Uniform1(location, value);

            if (!attached) {
                Detach();
            }
        }

        public void SetUniform(string name, float value) {
            bool attached = false;
            if (!IsAttached) {
                attached = IsAttached;
                Attach();
            }

            int location = GL.GetUniformLocation(ID, name);
            GL.Uniform1(location, value);

            if (!attached) {
                Detach();
            }
        }

        public void SetUniform(string name, Vector3 value) {
            bool attached = false;
            if (!IsAttached) {
                attached = IsAttached;
                Attach();
            }

            int location = GL.GetUniformLocation(ID, name);
            GL.Uniform3(location, ref value);

            if (!attached) {
                Detach();
            }
        }

        public void SetUniform(string name, bool value) {
            bool attached = false;
            if (!IsAttached) {
                attached = IsAttached;
                Attach();
            }

            int location = GL.GetUniformLocation(ID, name);
            GL.Uniform1(location, value ? 1 : 0);

            if (!attached) {
                Detach();
            }
        }

        public unsafe void SetUniform(string name, float[] values) {
            bool attached = false;
            if (!IsAttached) {
                attached = IsAttached;
                Attach();
            }

            int location = GL.GetUniformLocation(ID, name);
            fixed (float * valuePtr = values) {
                GL.Uniform1(location, values.Length, valuePtr);
            }

            if (!attached) {
                Detach();
            }
        }

        public void Attach() {
            GL.UseProgram(ID);
            IsAttached = true;
        }

        public void Detach() {
            GL.UseProgram(0);
            IsAttached = false;
        }

        public void Dispose() {
            GL.DeleteProgram(ID);
        }
    }

    public class Shader  : IDisposable {
        public int ID { get; private set; }
        public ShaderType Type { get; private set; }

        public Shader(string source, ShaderType type) {
            Type = type;
            ID = -1;

            int id = GL.CreateShader(type);
            GL.ShaderSource(id, source);

            ErrorCode code = GL.GetError();
            if (code != ErrorCode.NoError) {
                throw new Exception(code.ToString());
            }

            GL.CompileShader(id);

            int status = 0;
            GL.GetShader(id, ShaderParameter.CompileStatus, out status);
            if (status == 0) {
                string info = GL.GetShaderInfoLog(id);
                throw new Exception("In " + type.ToString() + ": " + Environment.NewLine + info);
            }

            code = GL.GetError();
            if (code != ErrorCode.NoError) {
                throw new Exception("In " + type.ToString() + ": " + Environment.NewLine + code.ToString());
            }

            ID = id;
        }

        public void Dispose() {
            GL.DeleteShader(ID);
        }
    }
}
