using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Vector4 = OpenTK.Vector4;

namespace OpenGL
{
    internal class TransparenceProgram : IProgram
    {
        private ImageTexture _imageTextureGlasses;
        private ImageTexture _imageTextureSolid;
        private Mesh _transparentCube;
        private Mesh _solidCube;

        private GlProgram _tranparentProgram;
        private GlProgram _solidProgram;

        private Matrix4 _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, 1, 0.1f, 50);

        public void Run()
        {
            using (var w = new GameWindow(720, 480, null, "ComGr", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible))
            {
                var alpha = 0f;
                var beta = 1f;
                var gamma = 2f;

                w.Load += (o, ea) =>
                {
                    //set up opengl
                    GL.ClearColor(0.5f, 0.5f, 0.5f, 0);
                    //GL.ClearDepth(1);
                    GL.Enable(EnableCap.DepthTest);
                    GL.DepthMask(true);
                    //GL.DepthFunc(DepthFunction.Less);
                    GL.DepthRange(0, 50);
                    GL.Disable(EnableCap.CullFace);
                    GL.Enable(EnableCap.FramebufferSrgb);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                    _solidProgram = new GlProgram(@"Shaders\VertexShader.glsl", @"Shaders\FragmentShader.glsl");
                    _tranparentProgram = new GlProgram(@"Shaders\VertexShader.glsl", @"Shaders\FragmentShaderTransparen.glsl");

                    _transparentCube = Figures.CubeTransparent(_tranparentProgram);
                    _solidCube = Figures.Cube(_solidProgram);

                    //Textures
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                    _imageTextureGlasses = new ImageTexture("Textures\\glasses.png");
                    _imageTextureSolid = new ImageTexture("Textures\\05.JPG");

                    {
                        //check for errors during all previous calls
                        var error = GL.GetError();
                        if (error != ErrorCode.NoError)
                            throw new Exception(error.ToString());
                    }

                };

                w.UpdateFrame += (o, fea) =>
                {
                    alpha += 0.005f;
                    beta += 0.006f;
                    gamma += 0.008f;
                };

                w.RenderFrame += (o, fea) =>
                {
                    //clear screen and z-buffer
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    DrawSolid(alpha, beta, gamma);
                    DrawTransparent(alpha, beta, gamma);

                    GL.DepthMask(true);

                    //display
                    w.SwapBuffers();

                    var error = GL.GetError();
                    if (error != ErrorCode.NoError)
                        throw new Exception(error.ToString());
                };

                w.Resize += (o, ea) =>
                {
                    var r = w.Width / (float)w.Height;
                    _projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, r, 0.1f, 50);
                    GL.Viewport(w.ClientRectangle);
                };

                w.Run();
            }
        }

        private void DrawSolid(float alpha, float beta, float gamma)
        {
            _solidProgram.Use();

            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            GL.UniformMatrix4(GL.GetUniformLocation(_solidProgram, "p"), false, ref _projection);

            GL.Uniform4(GL.GetUniformLocation(_tranparentProgram, "enviroment"), new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            GL.Uniform1(GL.GetUniformLocation(_solidProgram, "texture1"), _imageTextureSolid);

            var rotaM = Matrix4x4.CreateFromYawPitchRoll(beta, gamma, alpha).ToGlMatrix();
            var m = rotaM * Matrix4.CreateTranslation(-0.8f, -0.8f, -9.5f);

            _solidCube.ViewModel = m;
            _solidCube.Render(PrimitiveType.Triangles);
        }

        private void DrawTransparent(float alpha, float beta, float gamma)
        {
            _tranparentProgram.Use();

            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);

            GL.UniformMatrix4(GL.GetUniformLocation(_tranparentProgram, "p"), false, ref _projection);

            GL.Uniform4(GL.GetUniformLocation(_tranparentProgram, "enviroment"), new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            GL.Uniform1(GL.GetUniformLocation(_tranparentProgram, "texture1"), _imageTextureGlasses);

            var rotaM = Matrix4x4.CreateFromYawPitchRoll(gamma, alpha, gamma).ToGlMatrix();
            var m = rotaM * Matrix4.CreateTranslation(0f, 0f, -10f);

            _transparentCube.ViewModel = m;
            _transparentCube.Render(PrimitiveType.Triangles);
        }
    }
}