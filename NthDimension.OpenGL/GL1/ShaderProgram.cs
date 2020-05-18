using NthDimension.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NthDimension.Rasterizer.GL1
{
    public class ShaderProgram : IDisposable
    {
        protected RendererBase          _renderer; 
        public int                      ID { get; private set; }
        public bool                     IsAttached { get; private set; }

        public Shader                   VertexShader { get; private set; }
        public Shader                   FragmentShader { get; private set; }

        public ShaderProgram(RendererBase renderer, string vertex, string fragment)
        {
            _renderer = renderer;
            int id = _renderer.SHADER_CreateProgram();

            if (!String.IsNullOrEmpty(vertex))
            {
                VertexShader = new Shader(_renderer, vertex, ShaderType.VertexShader);
                _renderer.SHADER_AttachShader(id, VertexShader.ID);
            }

            if (!String.IsNullOrEmpty(fragment))
            {
                FragmentShader = new Shader(_renderer, fragment, ShaderType.FragmentShader);
                _renderer.SHADER_AttachShader(id, FragmentShader.ID);
            }

            if (VertexShader == null && FragmentShader == null)
            {
                throw new ArgumentException("Shader source must not be empty/null!");
            }

            _renderer.SHADER_LinkProgram(id);
            _renderer.SHADER_ValidateProgram(id);

            int status = 0;
            _renderer.SHADER_GetProgram(id, ProgramParameter.LinkStatus, out status);
            if (status == 0)
            {
                string info = _renderer.GetProgramInfoLog(id);
                throw new Exception(info);
            }

            ErrorCode code = _renderer.GetError();
            if (code != ErrorCode.NoError)
            {
                throw new Exception(code.ToString());
            }

            ID = id;
        }

        public void SetUniform(string name, int value)
        {
            bool attached = false;
            if (!IsAttached)
            {
                attached = IsAttached;
                Attach();
            }

            int location = _renderer.SHADER_GetUniformLocation(ID, name);
            _renderer.SHADER_Uniform1(location, value);

            if (!attached)
            {
                Detach();
            }
        }
        public void SetUniform(string name, float value)
        {
            bool attached = false;
            if (!IsAttached)
            {
                attached = IsAttached;
                Attach();
            }

            int location = _renderer.SHADER_GetUniformLocation(ID, name);
            _renderer.SHADER_Uniform1(location, value);

            if (!attached)
            {
                Detach();
            }
        }
        public void SetUniform(string name, Vector3 value)
        {
            bool attached = false;
            if (!IsAttached)
            {
                attached = IsAttached;
                Attach();
            }

            int location = _renderer.SHADER_GetUniformLocation(ID, name);
            _renderer.SHADER_Uniform3(location, ref value);

            if (!attached)
            {
                Detach();
            }
        }
        public void SetUniform(string name, bool value)
        {
            bool attached = false;
            if (!IsAttached)
            {
                attached = IsAttached;
                Attach();
            }

            int location = _renderer.SHADER_GetUniformLocation(ID, name);
            _renderer.SHADER_Uniform1(location, value ? 1 : 0);

            if (!attached)
            {
                Detach();
            }
        }
        public unsafe void SetUniform(string name, float[] values)
        {
            bool attached = false;
            if (!IsAttached)
            {
                attached = IsAttached;
                Attach();
            }

            int location = _renderer.SHADER_GetUniformLocation(ID, name);
            fixed (float* valuePtr = values)
            {
                _renderer.SHADER_Uniform1(location, values.Length, valuePtr);
            }

            if (!attached)
            {
                Detach();
            }
        }

        public void Attach()
        {
            _renderer.SHADER_UseProgram(ID);
            IsAttached = true;
        }
        public void Detach()
        {
            _renderer.SHADER_UseProgram(0);
            IsAttached = false;
        }

        public void Dispose()
        {
            _renderer.SHADER_DeleteProgram(ID);
        }
    }
}
