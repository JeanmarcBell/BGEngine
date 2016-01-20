using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public class Camera2D
    {
        public Vector2 Offset { get; set; }

        public Camera2D()
        {
            Offset = Vector2.Zero;
        }

        public void MoveCamera(Vector2 moveAmount)
        {
            Offset += moveAmount;
        }

        public void CenterCameraAt(Vector2 location, float screenWidth, float screenHeight)
        {
            Offset = -location + new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
        }

        public void ApplyCameraTransformation(float parallaxLevel = 1.0f)
        {
            GL.PushMatrix();
            GL.Translate(parallaxLevel * Offset.X, parallaxLevel * Offset.Y, 0.0);
        }

        public void UnapplyCameraTransformation()
        {
            GL.PopMatrix();
        }
    }
}
