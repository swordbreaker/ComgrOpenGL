using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace OpenGL
{
    public class ImageTexture
    {
        //internal static readonly object Lock = new object();
        //internal static int Count;
        //public readonly int Id;

        private readonly Texture<byte> _tex;

        public ImageTexture(string path)
        {
            var bmp = (Bitmap)Image.FromFile(path);

            var width = bmp.Width;
            var height = bmp.Height;
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var data = new byte[width * height * 4];

            unsafe
            {
                var p = (byte*)bitmapData.Scan0;
                var i = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, i += 4)
                    {
                        var r = *(p++);
                        var g = *(p++);
                        var b = *(p++);
                        var a = *(p++);

                        data[i + 0] = b;
                        data[i + 1] = g;
                        data[i + 2] = r;
                        data[i + 3] = a;
                    }
                }
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            _tex = new Texture<byte>(data, width, height, PixelInternalFormat.Srgb8Alpha8, PixelType.UnsignedByte, PixelFormat.Rgba);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            //lock (Lock)
            //{
            //    var textruesId = new int[1];
            //    GL.GenTextures(1, textruesId);

            //    Id = Count;
            //    Count++;
            //    GL.ActiveTexture(TextureUnit.Texture0 + Id);
            //    GL.BindTexture(TextureTarget.Texture2D, textruesId[0]);

            //    var bmp = (Bitmap)Image.FromFile(path);

            //    var width = bmp.Width;
            //    var height = bmp.Height;
            //    var bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
            //        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //    var data = new byte[width * height * 4];

            //    unsafe
            //    {
            //        var p = (byte*)bitmapData.Scan0;
            //        var i = 0;
            //        for (int y = 0; y < height; y++)
            //        {
            //            for (int x = 0; x < width; x++, i += 4)
            //            {
            //                var r = *(p++);
            //                var g = *(p++);
            //                var b = *(p++);
            //                var a = *(p++);

            //                data[i + 0] = b;
            //                data[i + 1] = g;
            //                data[i + 2] = r;
            //                data[i + 3] = a;
            //            }
            //        }
            //    }
            //    bmp.UnlockBits(bitmapData);

            //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            //    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Srgb8Alpha8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            //    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            //}
        }

        public static implicit operator int(ImageTexture imageTexture)
        {
            return imageTexture._tex.Id;
        }
    }
}
