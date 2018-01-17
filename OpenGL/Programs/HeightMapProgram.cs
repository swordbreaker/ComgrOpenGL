using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Vector4 = OpenTK.Vector4;

namespace OpenGL
{
    internal class HeightMapProgram : IProgram
    {
        private ImageTexture _imageTexture;
        private HeightMesh _heigthMesh;

        private CameraHelper _cameraHelper = new CameraHelper(0, 0, 5);

        private GlProgram _program;

        public void Run()
        {
            using (var w = new GameWindow(720, 480, null, "ComGr", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible))
            {
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

                    _program = new GlProgram(@"Shaders\VertexShaderHeightMap.glsl", @"Shaders\FragmentShader.glsl");

                    //Textures
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                    _heigthMesh = new HeightMesh(@"Textures\MatterhornHeightMap.png", 500, _program);
                    _imageTexture = new ImageTexture(@"Textures\05.JPG");

                    {
                        //check for errors during all previous calls
                        var error = GL.GetError();
                        if (error != ErrorCode.NoError)
                            throw new Exception(error.ToString());
                    }

                    _program.Use();

                    GL.Uniform4(GL.GetUniformLocation(_program, "enviroment"), new Vector4(0f, 0f, 0f, 1));
                    GL.Uniform1(GL.GetUniformLocation(_program, "texture1"), _imageTexture);
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

                    GL.UniformMatrix4(GL.GetUniformLocation(_program, "p"), false, ref projection);

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
