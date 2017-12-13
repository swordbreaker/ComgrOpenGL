using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using OpenTK;                  //add "OpenTK" as NuGet reference
using OpenTK.Graphics.OpenGL4; //add "OpenTK" as NuGet reference

namespace OpenGL
{
    static class Program
    {
        static void Main()
        {
            using (var w = new GameWindow(720, 480, null, "ComGr", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible))
            {
                int hProgram = 0;

                int vaoTriangle = 0;
                int[] triangleIndices = null;
                int vboTriangleIndices = 0;

                double time = 0;

                var projection = Matrix4.CreateOrthographicOffCenter(-10, 10, -10, 10, 0.1f, 100);

                w.Load += (o, ea) =>
                {
                    //set up opengl
                    GL.ClearColor(0.5f, 0.5f, 0.5f, 0);
                    //GL.ClearDepth(1);
                    GL.Enable(EnableCap.DepthTest);
                    //GL.DepthFunc(DepthFunction.Less);
                    //GL.Disable(EnableCap.CullFace);

                    //load, compile and link shaders
                    //see https://www.khronos.org/opengl/wiki/Vertex_Shader


                    var VertexShaderSource = File.ReadAllText(@"Shaders\VertexShader.glsl");

                    var s = File.ReadAllText(@"Shaders\VertexShader.glsl");
                    //@"
                    //    #version 400 core

                    //    in vec3 pos;
                    //    uniform float time;
                    //    out float someFloat;

                    //    void main()
                    //    {
                    //      gl_Position = vec4(pos, 1.0) + vec4(sin(time) * 0.5, cos(time) * 0.5, 0.0, 0.0);
                    //      someFloat = pos.x + 0.5;
                    //    }
                    //    ";
                    var hVertexShader = GL.CreateShader(ShaderType.VertexShader);
                    GL.ShaderSource(hVertexShader, VertexShaderSource);
                    GL.CompileShader(hVertexShader);
                    GL.GetShader(hVertexShader, ShaderParameter.CompileStatus, out int status);
                    if (status != 1)
                        throw new Exception(GL.GetShaderInfoLog(hVertexShader));

                    //see https://www.khronos.org/opengl/wiki/Fragment_Shader
                    var FragmentShaderSource = File.ReadAllText(@"Shaders\FragmentShader.glsl");
                    //    #version 400 core

                    //    out vec4 colour;
                    //    in float someFloat;

                    //void main()
                    //{
                    //    colour = vec4(someFloat, 0.75, 0.0, 1.0);
                    //}
                    //";
                    var hFragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                    GL.ShaderSource(hFragmentShader, FragmentShaderSource);
                    GL.CompileShader(hFragmentShader);
                    GL.GetShader(hFragmentShader, ShaderParameter.CompileStatus, out status);
                    if (status != 1)
                        throw new Exception(GL.GetShaderInfoLog(hFragmentShader));

                    //link shaders to a program
                    hProgram = GL.CreateProgram();
                    GL.AttachShader(hProgram, hFragmentShader);
                    GL.AttachShader(hProgram, hVertexShader);
                    GL.LinkProgram(hProgram);
                    GL.GetProgram(hProgram, GetProgramParameterName.LinkStatus, out status);
                    if (status != 1)
                        throw new Exception(GL.GetProgramInfoLog(hProgram));

                    //upload model vertices to a vbo
                    var triangleVertices = new float[]
                    {
                        //-1, -1, -1,
                        //+1, -1, -1,
                        //+1, +1, -1,
                        //-1, +1, -1,

                        //-1, -1, +1,
                        //+1, -1, +1,
                        //+1, +1, +1,
                        //-1, +1, +1,

                        -1, -1, -1,
                        +1, -1, -1,
                        +1, +1, -1,
                        -1, +1, -1,

                        -1, -1, +1,
                        +1, -1, +1,
                        +1, +1, +1,
                        -1, +1, +1,

                        -1, -1, -1,
                        -1, +1, -1,
                        -1, +1, +1,
                        -1, -1, +1,

                        +1, -1, +1,
                        +1, +1, +1,
                        +1, +1, -1,
                        +1, -1, -1,

                        -1, +1, -1,
                        +1, +1, -1,
                        +1, +1, +1,
                        -1, +1, +1,

                        -1, -1, +1,
                        +1, -1, +1,
                        +1, -1, -1,
                        -1, -1, -1,
                    };
                    var vboTriangleVertices = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vboTriangleVertices);
                    GL.BufferData(BufferTarget.ArrayBuffer, triangleVertices.Length * sizeof(float), triangleVertices, BufferUsageHint.StaticDraw);

                    // upload model indices to a vbo
                    triangleIndices = new int[]
                    {
                        0,   1,  2,
                        0,   2,  3,
                        7,   6,  5,
                        7,   5,  4,
                        8,   9, 10,
                        8,  10, 11,
                        12, 13, 14,
                        14, 15, 12,
                        16, 18, 19,
                        16, 17, 18,
                        20, 21, 22,
                        22, 23, 20,
                    };
                    vboTriangleIndices = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboTriangleIndices);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, triangleIndices.Length * sizeof(int), triangleIndices, BufferUsageHint.StaticDraw);

                    var colors = new float[]
                    {
                        1,0,0,1,
                        0,1,0,1,
                        0,0,1,1,
                        1,1,0,0,

                        1,0,0,1,
                        0,1,0,1,
                        0,0,1,1,
                        1,1,0,0,

                        1,0,0,1,
                        0,1,0,1,
                        0,0,1,1,
                        1,1,0,0,

                        1,0,0,1,
                        0,1,0,1,
                        0,0,1,1,
                        1,1,0,0,

                        1,0,0,1,
                        0,1,0,1,
                        0,0,1,1,
                        1,1,0,0,

                        1,0,0,1,
                        0,1,0,1,
                        0,0,1,1,
                        1,1,0,0,
                    };

                    var vboColor = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vboColor);
                    GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(float), colors, BufferUsageHint.StaticDraw);


                    var normals = new float[]
                    {
                        0,0,-1,
                        0,0,-1,
                        0,0,-1,
                        0,0,-1,
                        //Top
                        0,0,1,
                        0,0,1,
                        0,0,1,
                        0,0,1,
                        //Left
                        -1,0,0,
                        -1,0,0,
                        -1,0,0,
                        -1,0,0,
                        //Right
                        1,0,0,
                        1,0,0,
                        1,0,0,
                        1,0,0,
                        //Front
                        0,1,0,
                        0,1,0,
                        0,1,0,
                        0,1,0,
                        //Back
                        0,-1,0,
                        0,-1,0,
                        0,-1,0,
                        0,-1,0,
                    };

                    var vboNormals = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
                    GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);

                    //set up a vao
                    vaoTriangle = GL.GenVertexArray();
                    GL.BindVertexArray(vaoTriangle);

                    GL.EnableVertexAttribArray(GL.GetAttribLocation(hProgram, "pos"));
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vboTriangleVertices);
                    GL.VertexAttribPointer(GL.GetAttribLocation(hProgram, "pos"), 3, VertexAttribPointerType.Float, false, 0, 0);

                    GL.EnableVertexAttribArray(GL.GetAttribLocation(hProgram, "v_color"));
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vboColor);
                    GL.VertexAttribPointer(GL.GetAttribLocation(hProgram, "v_color"), 4, VertexAttribPointerType.Float, false, 0, 0);

                    //GL.EnableVertexAttribArray(GL.GetAttribLocation(hProgram, "v_normal"));
                    //GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
                    //GL.VertexAttribPointer(GL.GetAttribLocation(hProgram, "v_normal"), 3, VertexAttribPointerType.Float, true, 0, 0);

                    {
                        //check for errors during all previous calls
                        var error = GL.GetError();
                        if (error != ErrorCode.NoError)
                            throw new Exception(error.ToString());
                    }

                };

                w.UpdateFrame += (o, fea) =>
                {
                    //perform logic

                    time += fea.Time;
                };

                var alpha = 0f;
                var beta = 1f;
                var gamma = 2f;

                w.RenderFrame += (o, fea) =>
                {
                    //clear screen and z-buffer
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    //switch to our shader
                    GL.UseProgram(hProgram);

                    //var projection = Matrix4.CreatePerspectiveOffCenter(-10, 10, 10, -10, 0.1f, 100);
                    var scaleM = Matrix4.CreateScale(1f);
                    var rotaM = Matrix4x4.CreateFromYawPitchRoll(alpha, alpha, alpha).ToGlMatrix();
                    rotaM.Transpose();
                    var translationM = Matrix4.CreateTranslation(0f, 0f, -5f);

                    var vm = rotaM * scaleM * translationM * projection;
                    //var vm = projection * (Matrix4.CreateRotationY((float)(10/180f * Math.PI)) * scaleM * translationM);
                    
       
                    GL.UniformMatrix4(GL.GetUniformLocation(hProgram, "viewModel"), false, ref vm);

                    //render our model
                    GL.BindVertexArray(vaoTriangle);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboTriangleIndices);

                    GL.DrawElements(PrimitiveType.Triangles, triangleIndices.Length, DrawElementsType.UnsignedInt, 0);

                    rotaM = Matrix4x4.CreateFromYawPitchRoll(beta, gamma, beta).ToGlMatrix();
                    vm = rotaM * scaleM * Matrix4.CreateTranslation(3f, 3f, -5f) * projection;
                    GL.UniformMatrix4(GL.GetUniformLocation(hProgram, "viewModel"), false, ref vm);
                    
                    GL.DrawElements(PrimitiveType.Triangles, triangleIndices.Length, DrawElementsType.UnsignedInt, 0);

                    rotaM = Matrix4x4.CreateFromYawPitchRoll(gamma, alpha, gamma).ToGlMatrix();
                    vm = rotaM * scaleM * Matrix4.CreateTranslation(-3f, -3f, -5f) * projection;
                    GL.UniformMatrix4(GL.GetUniformLocation(hProgram, "viewModel"), false, ref vm);

                    GL.DrawElements(PrimitiveType.Triangles, triangleIndices.Length, DrawElementsType.UnsignedInt, 0);

                    //display
                    w.SwapBuffers();

                    alpha += 0.01f;
                    beta += 0.02f;
                    gamma += 0.03f;

                    var error = GL.GetError();
                    if (error != ErrorCode.NoError)
                        throw new Exception(error.ToString());
                };

                w.Resize += (o, ea) =>
                {
                    var r = w.Height / (float)w.Width;
                    projection = Matrix4.CreateOrthographicOffCenter(10, -10, r * 10f, -r * 10f, 0.1f, 100);
                    GL.Viewport(w.ClientRectangle);
                };

                w.Run();
            }
        }
    }
}