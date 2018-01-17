using System;
using System.Numerics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Vector4 = OpenTK.Vector4;

namespace OpenGL
{
    internal class RotatingCubes : IProgram
    {
        private ImageTexture _imageTexture;
        private Mesh _cube;

        private GlProgram _program;

        public void Run()
        {
            using (var w = new GameWindow(720, 480, null, "ComGr", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible))
            {
                var projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 1, 0.1f, 100);

                var alpha = 0f;
                var beta = 1f;
                var gamma = 2f;

                w.Load += (o, ea) =>
                {
                    //set up opengl
                    GL.ClearColor(0.5f, 0.5f, 0.5f, 0);
                    //GL.ClearDepth(1);
                    GL.Enable(EnableCap.DepthTest);
                    //GL.DepthFunc(DepthFunction.Less);
                    GL.Disable(EnableCap.CullFace);
                    GL.Enable(EnableCap.FramebufferSrgb);
                    GL.Enable(EnableCap.Blend);

                    _program = new GlProgram(@"Shaders\VertexShader.glsl", @"Shaders\FragmentShader.glsl");

                    _cube = Figures.Cube(_program);

                    //Textures
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                    _imageTexture = new ImageTexture("Textures\\05.JPG");

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

                    //time += fea.Time;

                    alpha += 0.01f;
                    beta += 0.02f;
                    gamma += 0.03f;
                };

                w.RenderFrame += (o, fea) =>
                {
                    //clear screen and z-buffer
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    //switch to our shader
                    _program.Use();

                    GL.Uniform4(GL.GetUniformLocation(_program, "enviroment"), new Vector4(0.1f,0.1f,0.1f, 1));
                    GL.Uniform1(GL.GetUniformLocation(_program, "texture1"), _imageTexture);

                    GL.UniformMatrix4(GL.GetUniformLocation(_program, "p"), false, ref projection);

                    var rotaM = Matrix4x4.CreateFromYawPitchRoll(alpha, alpha, alpha).ToGlMatrix();
                    var translationM = Matrix4.CreateTranslation(0f, 0f, -10f);

                    var m = rotaM * translationM;

                    _cube.ViewModel = m;
                    _cube.Render(PrimitiveType.Triangles);

                    rotaM = Matrix4x4.CreateFromYawPitchRoll(beta, gamma, beta).ToGlMatrix();
                    m = rotaM * Matrix4.CreateTranslation(3f, 3f, -10f);

                    _cube.ViewModel = m;
                    _cube.Render(PrimitiveType.Triangles);

                    rotaM = Matrix4x4.CreateFromYawPitchRoll(gamma, alpha, gamma).ToGlMatrix();
                    m = rotaM * Matrix4.CreateTranslation(-3f, -3f, -10f);

                    _cube.ViewModel = m;
                    _cube.Render(PrimitiveType.Triangles);

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