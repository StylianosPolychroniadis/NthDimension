using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NthDimension.Algebra;
using NthDimension.Graphics.Renderer;


namespace NthDimension.Rasterizer.GL1
{
    public partial class RendererGL1x : RendererBase
    {
        public override string GetProgramInfoLog(int programId)
        {
            string ret = OpenTK.Graphics.OpenGL.GL.GetProgramInfoLog(programId); GetError(true);
            return ret; 
        }
        public override void DrawElements(NthDimension.Graphics.Geometry.enuPolygonMode mode, int count, NthDimension.Graphics.DrawElementsType type, int indices)
        {
            OpenTK.Graphics.OpenGL.GL.DrawElements(mode.ToOpenGL(),
                                                    count,
                                                    type.ToOpenGL(),
                                                    indices); GetError(true);
        }

        #region [ Shader GLSL ]

        //public override void SHADER_LOAD(ref Shaders.ShaderBase shader)
        //{
        //    shader.ShaderID = OpenTK.Graphics.OpenGL.GL.CreateShader(shader.ShaderType.ToOpenGL());
        //    OpenTK.Graphics.OpenGL.GL.ShaderSource(shader.ShaderID, shader.ShaderProgramText);
        //    OpenTK.Graphics.OpenGL.GL.CompileShader(shader.ShaderID);

        //    OpenTK.Graphics.OpenGL.GL.GetShader(shader.ShaderID, OpenTK.Graphics.OpenGL.ShaderParameter.CompileStatus, out shader.compiled);
        //    //this.PrintPrettyShaderInfoLog(shader);

        //    #region PrintPrettyShaderInfoLog
        //    shader.programLines = shader.ShaderProgramText.Split('\n');
        //    shader.log_lines = OpenTK.Graphics.OpenGL.GL.GetShaderInfoLog(shader.ShaderID).Split('\n');
        //    #endregion

        //    shader.glAll = (int)OpenTK.Graphics.OpenGL.All.False;
        //}

        //protected void PrintPrettyShaderInfoLog(Shaders.ShaderBase shader)
        //{

        //}
        public override void EnableVertexAttribArray(int attrib)
        {
            OpenTK.Graphics.OpenGL.GL.EnableVertexAttribArray(attrib); GetError(true);
        }
        public override void DisableVertexAttribArray(int attrib)
        {
            OpenTK.Graphics.OpenGL.GL.DisableVertexAttribArray(attrib); GetError(true);
        }

        public override int SHADER_CreateProgram()
        {
            return OpenTK.Graphics.OpenGL.GL.CreateProgram(); GetError(true);
        }
        public override int SHADER_CreateShader(ShaderType type)
        {
            int ret = OpenTK.Graphics.OpenGL.GL.CreateShader(type.ToOpenGL()); GetError(true);
            return ret;  
        }
        public override void SHADER_ShaderSource(int programId, string code)
        {
            OpenTK.Graphics.OpenGL.GL.ShaderSource(programId, code); GetError(true);
        }
        public override void SHADER_CompileShader(int programId)
        {
            OpenTK.Graphics.OpenGL.GL.CompileShader(programId); GetError(true);
        }
        public override void SHADER_UseProgram(int programId)
        {
            OpenTK.Graphics.OpenGL.GL.UseProgram(programId); GetError(true);
        }
        public override void SHADER_LinkProgram(int programId)
        {
            OpenTK.Graphics.OpenGL.GL.LinkProgram(programId); GetError(true);
        }
        public override void SHADER_ValidateProgram(int programId)
        {
            OpenTK.Graphics.OpenGL.GL.ValidateProgram(programId); GetError(true);
        }
        public override void SHADER_DeleteProgram(int program)
        {
            OpenTK.Graphics.OpenGL.GL.DeleteProgram(program); GetError(true);
        }
        public override void SHADER_DeleteShader(int shaderId)

        {
            OpenTK.Graphics.OpenGL.GL.DeleteShader(shaderId); GetError(true);
        }
        public override void SHADER_GetShader(int shader, ShaderParameter pname, out int @params)
        {
            OpenTK.Graphics.OpenGL.GL.GetShader(shader, pname.ToOpenGL(), out @params); GetError(true);
        }
        public override string SHADER_GetShaderInfoLog(int shader)
        {
            return OpenTK.Graphics.OpenGL.GL.GetShaderInfoLog(shader); GetError(true);
        }
        public override void SHADER_AttachShader(int programId, int shaderId)
        {
            OpenTK.Graphics.OpenGL.GL.AttachShader(programId, shaderId); GetError(true);
        }
        public override void SHADER_DetachShader(int programId, int shaderId)
        {
            OpenTK.Graphics.OpenGL.GL.DetachShader(programId, shaderId); GetError(true);
        }
        public override void SHADER_ExtProgramParameter(int program, ExtGeometryShader4 pname, int value)
        {
            OpenTK.Graphics.OpenGL.GL.Ext.ProgramParameter(program, pname.ToOpenGL(), value); GetError(true);
        }
        public override string SHADER_GetProgramLog(int programId)
        {
            string ret = OpenTK.Graphics.OpenGL.GL.GetProgramInfoLog(programId); GetError(true);
            return ret;
        }
        //public override void SHADER_Uniform1(int location, double x)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform1(location, x);
        //}
        public override void SHADER_Uniform1(int location, float v0)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, v0); GetError(true);
        }
        public override void SHADER_Uniform1(int location, int v0)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, v0); GetError(true);
        }
        public override void SHADER_Uniform1(int location, uint v0)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, v0); GetError(true);
        }
        public override unsafe void SHADER_Uniform1(int location, int count, float* v0)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, v0); GetError(true);
        }
        //public override void SHADER_Uniform1(int location, int count, double[] value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, value);
        //}
        public override void SHADER_Uniform1(int location, int count, float[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, value); GetError(true);
        }
        public override void SHADER_Uniform1(int location, int count, int[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, value); GetError(true);
        }
        //public override void SHADER_Uniform1(int location, int count, ref double value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, ref value);
        //}
        public override void SHADER_Uniform1(int location, int count, ref float value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform1(int location, int count, ref int value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform1(int location, int count, ref uint value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform1(int location, int count, uint[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform1(location, count, value); GetError(true);
        }
        public override void SHADER_Uniform2(int location, ref Vector2 vector)
        {
            OpenTK.Vector2 v = vector.ToOpenGL();
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, ref v); GetError(true);
            vector = v.ToSyscon();
        }
        public override void SHADER_Uniform2(int location, Vector2 vector)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, vector.ToOpenGL()); GetError(true);
        }
        //public override void SHADER_Uniform2(int location, double x, double y)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform2(location, x, y);
        //}
        public override void SHADER_Uniform2(int location, float v0, float v1)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, v0, v1); GetError(true);
        }
        //public override void SHADER_Uniform2(int location, int count, double[] value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform2(location, count, value);
        //}
        public override void SHADER_Uniform2(int location, int count, float[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, count, value); GetError(true);
        }
        public override void SHADER_Uniform2(int location, int v0, int v1)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, v0, v1); GetError(true);
        }
        public override void SHADER_Uniform2(int location, int count, int[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, count, value); GetError(true);
        }
        //public override void SHADER_Uniform2(int location, int count, ref double value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform2(location, count, ref value);
        //}
        public override void SHADER_Uniform2(int location, int count, ref float value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform2(int location, int count, ref uint value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform2(int location, int count, uint[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, count, value); GetError(true);
        }
        public override void SHADER_Uniform2(int location, uint v0, uint v1)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform2(location, v0, v1); GetError(true);
        }
        public override void SHADER_Uniform3(int location, ref Vector3 vector)
        {
            OpenTK.Vector3 v = vector.ToOpenGL();
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, ref v); GetError(true);
            vector = v.ToSyscon();
        }
        public override void SHADER_Uniform3(int location, Vector3 vector)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, vector.ToOpenGL()); GetError(true);
        }
        //public override void SHADER_Uniform3(int location, int count, double[] value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, value);
        //}
        public override void SHADER_Uniform3(int location, int count, float[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, value); GetError(true);
        }
        public override void SHADER_Uniform3(int location, int count, int[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, value); GetError(true);
        }
        //public override void SHADER_Uniform3(int location, int count, ref double value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, ref value);
        //}
        public override void SHADER_Uniform3(int location, int count, ref float value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform3(int location, int count, ref int value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform3(int location, int count, ref uint value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform3(int location, int count, uint[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, count, value); GetError(true);
        }
        //public override void SHADER_Uniform3(int location, double x, double y, double z)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform3(location, x, y, z);
        //}
        public override void SHADER_Uniform3(int location, float v0, float v1, float v2)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, v0, v1, v2); GetError(true);
        }
        public override void SHADER_Uniform3(int location, int v0, int v1, int v2)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, v0, v1, v2); GetError(true);
        }
        public override void SHADER_Uniform3(int location, uint v0, uint v1, uint v2)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform3(location, v0, v1, v2); GetError(true);
        }
        public override void SHADER_Uniform4(int location, Color4 color)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, new OpenTK.Graphics.Color4(color.R, color.G, color.B, color.A)); GetError(true);
        }
        public override void SHADER_Uniform4(int location, Quaternion quaternion)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, quaternion.ToOpenGL()); GetError(true);
        }
        public override void SHADER_Uniform4(int location, ref Vector4 vector)
        {
            OpenTK.Vector4 v = vector.ToOpenGL();
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, ref v); GetError(true);
            vector = v.ToSyscon();
        }
        public override void SHADER_Uniform4(int location, Vector4 vector)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, vector.ToOpenGL()); GetError(true);
        }
        //public override void SHADER_Uniform4(int location, int count, double[] value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, value);
        //}
        public override void SHADER_Uniform4(int location, int count, float[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, value); GetError(true);
        }
        public override void SHADER_Uniform4(int location, int count, int[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, value); GetError(true);
        }
        //public override void SHADER_Uniform4(int location, int count, ref double value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, ref value);
        //}
        public override void SHADER_Uniform4(int location, int count, ref float value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform4(int location, int count, ref int value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform4(int location, int count, ref uint value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, ref value); GetError(true);
        }
        public override void SHADER_Uniform4(int location, int count, uint[] value)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, count, value); GetError(true);
        }
        //public override void SHADER_Uniform4(int location, double x, double y, double z, double w)
        //{
        //    OpenTK.Graphics.OpenGL.GL.Uniform4(location, x, y, z, w);
        //}
        public override void SHADER_Uniform4(int location, float v0, float v1, float v2, float v3)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, v0, v1, v2, v3); GetError(true);
        }
        public override void SHADER_Uniform4(int location, int v0, int v1, int v2, int v3)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, v0, v1, v2, v3); GetError(true);
        }
        public override void SHADER_Uniform4(int location, uint v0, uint v1, uint v2, uint v3)
        {
            OpenTK.Graphics.OpenGL.GL.Uniform4(location, v0, v1, v2, v3); GetError(true);
        }
        public override void SHADER_UniformBlockBinding(int program, int uniformBlockIndex, int uniformBlockBinding)
        {
            OpenTK.Graphics.OpenGL.GL.UniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding); GetError(true);
        }
        public override void SHADER_UniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding)
        {
            OpenTK.Graphics.OpenGL.GL.UniformBlockBinding(program, uniformBlockIndex, uniformBlockBinding); GetError(true);
        }
        //public override void SHADER_UniformMatrix3(int location, bool transpose, ref Matrix3 matrix)
        //{
        //    OpenTK.Matrix3 m = matrix.ToOpenGL();
        //    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, transpose, ref m);
        //    matrix = m.ToSyscon();
        //}
        //public override void SHADER_UniformMatrix3(int location, bool transpose, ref Matrix3d matrix)
        //{
        //    OpenTK.Matrix3d m = matrix.ToOpenGL();
        //    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, transpose, ref m);
        //    matrix = m.ToSyscon();
        //}
        //public override void SHADER_UniformMatrix3(int location, int count, bool transpose, double[] value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, count, transpose, value);
        //}
        public override void SHADER_UniformMatrix3(int location, int count, bool transpose, float[] value)
        {
            OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, count, transpose, value); GetError(true);
        }
        //public override void SHADER_UniformMatrix3(int location, int count, bool transpose, ref double value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, count, transpose, ref value);
        //}
        public override void SHADER_UniformMatrix3(int location, int count, bool transpose, ref float value)
        {
            OpenTK.Graphics.OpenGL.GL.UniformMatrix3(location, count, transpose, ref value); GetError(true);
        }
        public override void SHADER_UniformMatrix4(int location, bool transpose, ref Matrix4 matrix)
        {
            OpenTK.Matrix4 m = matrix.ToOpenGL();
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, transpose, ref m); GetError(true);
            matrix = m.ToSyscon();
        }
        //public override void SHADER_UniformMatrix4(int location, bool transpose, ref Matrix4d matrix)
        //{
        //    OpenTK.Matrix4d m = matrix.ToOpenGL();
        //    OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, transpose, ref m);
        //    matrix = m.ToSyscon();
        //}
        //public override void SHADER_UniformMatrix4(int location, int count, bool transpose, double[] value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, count, transpose, value);
        //}
        public override void SHADER_UniformMatrix4(int location, int count, bool transpose, float[] value)
        {
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, count, transpose, value); GetError(true);
        }
        //public override void SHADER_UniformMatrix4(int location, int count, bool transpose, ref double value)
        //{
        //    OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, count, transpose, ref value);
        //}
        public override void SHADER_UniformMatrix4(int location, int count, bool transpose, ref float value)
        {
            OpenTK.Graphics.OpenGL.GL.UniformMatrix4(location, count, transpose, ref value); GetError(true);
        }


        public override void SHADER_GetActiveAttrib(int program, int index, int bufSize, out int length, out int size, out ActiveAttribType type, System.Text.StringBuilder name)
        {
            OpenTK.Graphics.OpenGL.ActiveAttribType att = OpenTK.Graphics.OpenGL.ActiveAttribType.Float;
            OpenTK.Graphics.OpenGL.GL.GetActiveAttrib(program, index, bufSize, out length, out size, out att, name); GetError(true);
            type = att.ToSyscon();
        }
        public override int SHADER_GetAttribLocation(int programId, string name)
        {
            int ret = OpenTK.Graphics.OpenGL.GL.GetAttribLocation(programId, name); GetError(true);
            return ret; 
        }
        public override void SHADER_GetProgram(int programId, ProgramParameter pName, out int output)
        {
            OpenTK.Graphics.OpenGL.GL.GetProgram(programId, pName.ToOpenGL(), out output); GetError(true);
        }
        public override int SHADER_GetUniformLocation(int programId, string uniformName)
        {
            int ret = OpenTK.Graphics.OpenGL.GL.GetUniformLocation(programId, uniformName); GetError(true);
            return ret;
        }

        public override void SHADER_VertexAttribPointer(int index, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
        {
            OpenTK.Graphics.OpenGL.GL.VertexAttribPointer(index, size, type.ToOpenGL(), normalized, stride, offset); GetError(true);
        }
        #endregion
    }
}
