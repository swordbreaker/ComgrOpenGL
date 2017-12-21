using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL
{
    public class Mesh
    {
        private readonly int _vboTriangleIndices;
        private readonly int _vaoTriangle;
        private readonly int _triangleLengt;
        private readonly int _hProgramm;

        public Matrix4 ViewModel = Matrix4.Identity;
        
        public Mesh(float [] verts, int[] idx, float[] colors, float[] normals, float[] uvs, int hProgram)
        {
            _triangleLengt = idx.Length;
            _hProgramm = hProgram;

            var vboTriangleVertices = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboTriangleVertices);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);

            _vboTriangleIndices = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vboTriangleIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, idx.Length * sizeof(int), idx, BufferUsageHint.StaticDraw);

            var vboColor = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboColor);
            GL.BufferData(BufferTarget.ArrayBuffer, colors.Length * sizeof(float), colors, BufferUsageHint.StaticDraw);

            var vboNormals = GL.GenBuffer();
            if (normals != null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
                GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);
            }


            var vboUvs = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvs);
            GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * sizeof(float), uvs, BufferUsageHint.StaticDraw);


            //set up a vao
            _vaoTriangle = GL.GenVertexArray();
            GL.BindVertexArray(_vaoTriangle);

            GL.EnableVertexAttribArray(GL.GetAttribLocation(hProgram, "pos"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboTriangleVertices);
            GL.VertexAttribPointer(GL.GetAttribLocation(hProgram, "pos"), 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(GL.GetAttribLocation(hProgram, "v_color"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboColor);
            GL.VertexAttribPointer(GL.GetAttribLocation(hProgram, "v_color"), 4, VertexAttribPointerType.Float, false, 0, 0);

            if (normals != null)
            {
                GL.EnableVertexAttribArray(GL.GetAttribLocation(hProgram, "v_normal"));
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
                GL.VertexAttribPointer(GL.GetAttribLocation(hProgram, "v_normal"), 3, VertexAttribPointerType.Float, true, 0, 0);
            }

            GL.EnableVertexAttribArray(GL.GetAttribLocation(hProgram, "v_uv"));
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboUvs);
            GL.VertexAttribPointer(GL.GetAttribLocation(hProgram, "v_uv"), 2, VertexAttribPointerType.Float, false, 0, 0);
        }

        public void Render(PrimitiveType primitiveType)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(_hProgramm, "m"), false, ref ViewModel);

            GL.BindVertexArray(_vaoTriangle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vboTriangleIndices);

            GL.DrawElements(primitiveType, _triangleLengt, DrawElementsType.UnsignedInt, 0);
        }
    }
}
