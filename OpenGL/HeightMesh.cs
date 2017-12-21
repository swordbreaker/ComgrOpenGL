using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace OpenGL
{
    public class HeightMesh
    {
        private readonly Mesh _mesh;
        private readonly HeightMap _heightMap;
        private readonly int _hPorgram;

        public Matrix4 ViewModel
        {
            get => _mesh.ViewModel;
            set => _mesh.ViewModel = value;
        }

        public HeightMesh(string path, int gridSize, int hProgram)
        {
            _hPorgram = hProgram;
            _heightMap = new HeightMap(path);

            var verts = new float[gridSize * gridSize * 3];
            var colors = new float[gridSize * gridSize * 4];
            var uvs = new float[gridSize * gridSize * 2];
            var idx = new int[(gridSize - 1) * (gridSize - 1) * 2 * 3];
            var normals = new float[gridSize * gridSize * 3];

            CalculateVertex(gridSize, verts, colors, uvs);

            CalculateTriangleIndices(gridSize, idx);

            _mesh = new Mesh(verts, idx, colors, null, uvs, hProgram);
        }

        private void CalculateVertex(int gridSize, float[] verts, float[] colors, float[] uvs)
        {
            var xStep = 2f / (gridSize - 1);
            var yStep = 2f / (gridSize - 1);

            float x = -1;
            float y = -1;

            var vi = 0;
            var ci = 0;
            var ui = 0;

            for (int i = 0; i < gridSize; i++)
            {
                for (int k = 0; k < gridSize; k++, vi += 3, ui += 2, ci += 4)
                {
                    verts[vi + 0] = x;
                    verts[vi + 1] = y;

                    colors[ci + 0] = 1;
                    colors[ci + 1] = 1;
                    colors[ci + 2] = 1;
                    colors[ci + 3] = 1;

                    uvs[ui + 0] = (x + 1) / 2;
                    uvs[ui + 1] = (y + 1) / 2;

                    x += xStep;
                }
                y += yStep;
                x = -1;
            }
        }

        private void CalculateTriangleIndices(int gridSize, int[] idx)
        {
            int i1 = 0;
            int i2 = gridSize;

            int i = -1;

            for (int yi = 0; yi < (gridSize - 1); yi++)
            {
                for (int xi = 0; xi < (gridSize - 1); xi++)
                {
                    idx[++i] = i1;
                    idx[++i] = i2;
                    idx[++i] = i2 + 1;

                    i2++;

                    idx[++i] = i1;
                    idx[++i] = i2;
                    idx[++i] = i1 + 1;

                    i1++;
                }
                i1++;
                i2++;
            }
        }

        private void CalculateNormals(int gridSize, float[] normals, float[] verts)
        {
            var delta = 0.0001f;

            Vector3 ClacNormal(float x1, float y1, float x2, float y2)
            {
                var v1 = new Vector3(2 * delta, 0, x1 - y1); // f(x + d, y) - f(x - d, y)
                var v2 = new Vector3(0, 2 * delta, ); //f(x, y + d) - f(x, y - d)
            }


            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {

                }
            }
        }


        public void Render(PrimitiveType primitiveType)
        {
            GL.Uniform1(GL.GetUniformLocation(_hPorgram, "heightMap"), _heightMap);
            GL.Uniform1(GL.GetUniformLocation(_hPorgram, "scale"), 5f);

            _mesh.Render(primitiveType);
        }
    }
}
