using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Vector4 = OpenTK.Vector4;

namespace OpenGL
{
    internal static class Program2
    {
        private static Texture _texture;
        private static HeightMesh _heigthMesh;

        private static CameraHelper _cameraHelper = new CameraHelper(0, 0, 5);

        private static void Main()
        {
            using (var w = new GameWindow(720, 480, null, "ComGr", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible))
            {
                int hProgram = 0;
                var projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 1, 0.1f, 100);

                w.KeyDown += _cameraHelper.OnKeyDown;
                w.MouseWheel += _cameraHelper.OnMouseWheel;

                w.Load += (o, ea) =>
                {
                    //set up opengl
                    GL.ClearColor(0.5f, 0.5f, 0.5f, 0);
                    //GL.ClearDepth(1);
                    GL.Enable(EnableCap.DepthTest);
                    //GL.DepthFunc(DepthFunction.Less);
                    GL.Disable(EnableCap.CullFace);
                    GL.Enable(EnableCap.FramebufferSrgb);

                    //load, compile and link shaders
                    //see https://www.khronos.org/opengl/wiki/Vertex_Shader
                    var VertexShaderSource = File.ReadAllText(@"Shaders\VertexShaderHeightMap.glsl");

                    var hVertexShader = GL.CreateShader(ShaderType.VertexShader);
                    GL.ShaderSource(hVertexShader, VertexShaderSource);
                    GL.CompileShader(hVertexShader);
                    GL.GetShader(hVertexShader, ShaderParameter.CompileStatus, out int status);
                    if (status != 1)
                        throw new Exception(GL.GetShaderInfoLog(hVertexShader));

                    //see https://www.khronos.org/opengl/wiki/Fragment_Shader
                    var FragmentShaderSource = File.ReadAllText(@"Shaders\FragmentShader.glsl");

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

                    //Textures
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                    _heigthMesh = new HeightMesh(@"Textures\MatterhornHeightMap.png", 500, hProgram);
                    _texture = new Texture(@"Textures\05.JPG");

                    {
                        //check for errors during all previous calls
                        var error = GL.GetError();
                        if (error != ErrorCode.NoError)
                            throw new Exception(error.ToString());
                    }

                    GL.UseProgram(hProgram);

                    GL.Uniform4(GL.GetUniformLocation(hProgram, "enviroment"), new Vector4(0f, 0f, 0f, 1));
                    GL.Uniform1(GL.GetUniformLocation(hProgram, "texture1"), _texture);
                };

                w.UpdateFrame += (o, fea) =>
                {
                    //perform logic

                    //time += fea.Time;
                };

                w.RenderFrame += (o, fea) =>
                {                    
                    //clear screen and z-buffer
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.UniformMatrix4(GL.GetUniformLocation(hProgram, "p"), false, ref projection);

                    _heigthMesh.ViewModel = Matrix4.CreateRotationX(-(float)Math.PI/2) *  _cameraHelper.CameraMatrix;

                    _heigthMesh.Render();

                    //display
                    w.SwapBuffers();

                    var error = GL.GetError();
                    if (error != ErrorCode.NoError)
                        throw new Exception(error.ToString());
                };

                w.Resize += (o, ea) =>
                {
                    var r = w.Width / (float)w.Height;
                    projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, r, 0.1f, 100);
                    GL.Viewport(w.ClientRectangle);
                };

                w.Run();
            }
        }
    }
}
