using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGL
{
    public class HeightMesh
    {
        private readonly Mesh _mesh;

        public HeightMesh(string path, int hProgram)
        {
            var bmp = (Bitmap) Image.FromFile(path);

            var width = bmp.Width;
            var height = bmp.Height;
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);

            var verts = new float[width/5 * height/5 * 3];
            var normals  = new float[width / 5 * height / 5 * 3];
            var idx = new float[width / 5 * height / 5 * 3];


            var xStep = 1 / (width*2);
            var yStep = 1 / (height*2);

            unsafe
            {
                var p = (byte*)bitmapData.Scan0;
                var i = 0;
                var x = -1;
                var y = -1;

                for (int k = 0; k < height; k += 5)
                {
                    for (int j = 0; j < width; j += 5, i += 3)
                    {
                        var gray = *(p++);
                        var v = gray / 255f;

                        verts[i + 0] = x;
                        verts[i + 1] = v;
                        verts[i + 2] = y;

                        normals[i + 0] = 0;
                        normals[i + 1] = 1;
                        normals[i + 2] = 0;

                        x += xStep;
                    }

                    y += yStep;
                }
            }

            //for (int k = 0; k < verts.Length; k++)
            //{
            //    idx
            //}
            //_mesh = new Mesh(verts, );
        }
    }
}
