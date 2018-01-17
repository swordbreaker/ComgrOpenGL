using System;
using OpenTK;
using OpenTK.Input;
using Vector3 = OpenTK.Vector3;
using Vector4 = OpenTK.Vector4;

namespace OpenGL
{
    public class CameraHelper
    {
        public float Elevation { get; set; }
        public float Azimut { get; set; }
        public float Radius { get; set; }

        public CameraHelper(float elevation = 0f, float azimut = 0f, float radius = 20f)
        {
            Elevation = elevation;
            Azimut = azimut;
            Radius = radius;
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            Radius -= mouseWheelEventArgs.Delta * 0.2f;
            Radius = Math.Max(Radius, 0.001f);
        }

        public void OnKeyDown(object sender, KeyboardKeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Key)
            {
                case Key.Up:
                case Key.W:
                    Elevation += 0.05f;
                    //keyEventArgs.Handled = true;
                    break;
                case Key.Down:
                case Key.S:
                    Elevation -= 0.05f;
                    //keyEventArgs.Handled = true;
                    break;
                case Key.Left:
                case Key.A:
                    Azimut -= 0.05f;
                    //keyEventArgs.Handled = true;
                    break;
                case Key.Right:
                case Key.D:
                    Azimut += 0.05f;
                    //keyEventArgs.Handled = true;
                    break;
            }
        }

        public Matrix4 CameraMatrix
        {
            get
            {
                var a = new Vector3(0, 0, Radius);
                var b = new Vector3(0, 0, 0);
                var up = new Vector3(0, 1, 0);

                var r1 = Matrix4.CreateRotationX(Elevation);
                var r2 = Matrix4.CreateRotationY(-Azimut);
                var r = r1 * r2;

                var eye = Vector4.Transform(new Vector4(a, 1), r).Xyz;
                var upt = Vector4.Transform(new Vector4(up, 1), r).Xyz;

                return Matrix4.LookAt(eye, b, upt);
            }
        }
    }
}