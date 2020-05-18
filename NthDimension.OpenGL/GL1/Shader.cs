using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NthDimension.Rasterizer.GL1
{
    public class Shader
    {
        protected RendererBase _renderer;
        public int ID { get; private set; }
        public ShaderType Type { get; private set; }

        public Shader(RendererBase renderer, string source, ShaderType type)
        {
            Type = type;
            ID = -1;

            int id = _renderer.SHADER_CreateShader(type);
            _renderer.SHADER_ShaderSource(id, source);

            ErrorCode code = _renderer.GetError();

            if (code != ErrorCode.NoError)
                throw new Exception(code.ToString());

            _renderer.SHADER_CompileShader(id);

            int status = 0;
            _renderer.SHADER_GetShader(id, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                string info = _renderer.SHADER_GetShaderInfoLog(id);
                throw new Exception("In " + type.ToString() + ": " + Environment.NewLine + info);
            }

            code = _renderer.GetError();
            if (code != ErrorCode.NoError)
                throw new Exception("In " + type.ToString() + ": " + Environment.NewLine + code.ToString());

            ID = id;
        }

        public void Dispose()
        {
            _renderer.SHADER_DeleteShader(ID);
        }
    }
}
