using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace OpenGL
{
    public class Texture<T> where T:struct 
    {
        internal static readonly object Lock = new object();
        internal static int Count;
        public readonly int Id;

        public Texture(T[] data, int width, int height, PixelInternalFormat pixelInternalFormat, PixelType pixelType, PixelFormat pixelFormat)
        {
            lock (Lock)
            {
                var textruesId = new int[1];
                GL.GenTextures(1, textruesId);

                Id = Count;
                Count++;
                GL.ActiveTexture(TextureUnit.Texture0 + Id);
                GL.BindTexture(TextureTarget.Texture2D, textruesId[0]);

                GL.TexImage2D(TextureTarget.Texture2D, 0, pixelInternalFormat, width, height, 0, pixelFormat, pixelType, data);
                
            }
        }

        public static implicit operator int(Texture<T> imageTexture)
        {
            return imageTexture.Id;
        }
    }
}
