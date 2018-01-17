using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL
{
    public class GlProgram
    {
        private readonly int _hProgram;

        public GlProgram(string vertexShader, string fragmentShader)
        {
            //load, compile and link shaders
            //see https://www.khronos.org/opengl/wiki/Vertex_Shader
            var vertexShaderSource = File.ReadAllText(vertexShader);

            var hVertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(hVertexShader, vertexShaderSource);
            GL.CompileShader(hVertexShader);
            GL.GetShader(hVertexShader, ShaderParameter.CompileStatus, out int status);
            if (status != 1)
                throw new Exception(GL.GetShaderInfoLog(hVertexShader));

            //see https://www.khronos.org/opengl/wiki/Fragment_Shader
            var fragmentShaderSource = File.ReadAllText(fragmentShader);

            var hFragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(hFragmentShader, fragmentShaderSource);
            GL.CompileShader(hFragmentShader);
            GL.GetShader(hFragmentShader, ShaderParameter.CompileStatus, out status);
            if (status != 1)
                throw new Exception(GL.GetShaderInfoLog(hFragmentShader));

            //link shaders to a program
            _hProgram = GL.CreateProgram();
            GL.AttachShader(_hProgram, hFragmentShader);
            GL.AttachShader(_hProgram, hVertexShader);
            GL.LinkProgram(_hProgram);
            GL.GetProgram(_hProgram, GetProgramParameterName.LinkStatus, out status);
            if (status != 1)
                throw new Exception(GL.GetProgramInfoLog(_hProgram));
        }

        public void Use()
        {
            GL.UseProgram(_hProgram);
        }

        public static implicit operator int(GlProgram p)
        {
            return p._hProgram;
        }
    }
}
