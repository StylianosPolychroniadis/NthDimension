using System;
using System.Text.RegularExpressions;
//using SYSCON.Graphics.Shaders;
using NthDimension.Rasterizer.GL1;
using OpenTK.Graphics.OpenGL;

namespace NthDimension.Rasterizer.GL1.GL1.Shaders
{
    //public class Shader : ShaderBase
    //{

    //    public Shader(ShaderType type, string name, string source)
    //        : base(type, name, source)
    //    {
    //    }

    //    public override void LoadShader()
    //    {
    //        ShaderID = GL.CreateShader(Extensions.ToOpenGL(this.ShaderType));
    //        GL.ShaderSource(ShaderID, this.ShaderProgramText);
    //        GL.CompileShader(ShaderID);
    //        int compiled;
    //        GL.GetShader(ShaderID, ShaderParameter.CompileStatus, out compiled);
    //        this.PrintPrettyShaderInfoLog();
    //        if (compiled == (int)OpenTK.Graphics.OpenGL.All.False)
    //        {
    //            Console.WriteLine("** Shader Compile Failed **");
    //            ShaderID = 0;
    //            throw new ShaderLoadException(this.ShaderName);
    //        }
    //    }

    //    public override void PrintPrettyShaderInfoLog()
    //    {
    //        string[] programLines = this.ShaderProgramText.Split('\n');
    //        string[] log_lines = GL.GetShaderInfoLog(ShaderID).Split('\n');

    //        // example line:			
    //        // ERROR: 0:36: Use of undeclared identifier 'diffuseMaterial'

    //        var regex = new System.Text.RegularExpressions.Regex(@"([0-9]+):([0-9]+):");
    //        var regex2 = new System.Text.RegularExpressions.Regex(@"([0-9]+)\(([0-9]+)\)");

    //        if (log_lines.Length > 0)
    //        {
    //            Console.WriteLine("-- {0} --", this.ShaderName);
    //        }
    //        foreach (var line in log_lines)
    //        {
    //            // print log line
    //            Console.WriteLine(line);

    //            // try to print the source-line
    //            var match = regex.Match(line);
    //            if (!match.Success)
    //            {
    //                match = regex2.Match(line);
    //            }
    //            if (match.Success)
    //            {
    //                int lineno = int.Parse(match.Groups[2].Value);
    //                Console.WriteLine("   > " + programLines[lineno - 1]);
    //            }
    //        }
    //    }    
    //}
}
