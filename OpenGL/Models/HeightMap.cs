﻿using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace OpenGL
{
    public class HeightMap
    {
        //public readonly int Id;
        private readonly Texture<byte> _tex;

        public HeightMap(string path)
        {
            //lock (Texture.Lock)
            //{
            //    var textruesId = new int[1];
            //    GL.GenTextures(1, textruesId);

            //    Id = Texture.Count;
            //    Texture.Count++;
            //    GL.ActiveTexture(TextureUnit.Texture0 + Id);
            //    GL.BindTexture(TextureTarget.Texture2D, textruesId[0]);

            var bmp = (Bitmap)Image.FromFile(path);

            var width = bmp.Width;
            var height = bmp.Height;
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var data = new byte[width * height * 3];

            unsafe
            {
                var p = (byte*)bitmapData.Scan0;
                var i = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++, i += 3)
                    {
                        var r = *(p++);
                        var g = *(p++);
                        var b = *(p++);

                        data[i + 0] = b;
                        data[i + 1] = g;
                        data[i + 2] = r;
                    }
                }
            }
            bmp.UnlockBits(bitmapData);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            _tex = new Texture<byte>(data, width, height, PixelInternalFormat.Srgb8, PixelType.UnsignedByte, PixelFormat.Rgb);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public static implicit operator int(HeightMap heightMap)
        {
            return heightMap._tex.Id;
        }
    }
}