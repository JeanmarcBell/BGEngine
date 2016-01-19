using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGEngine
{
    public static class RectangleExtensions
    {
        public static Vector2 Center(this RectangleF rectangle)
        {
            return new Vector2(rectangle.Left + rectangle.Width / 2f, rectangle.Top + rectangle.Height / 2f);
        }

        public static Vector2 Center(this Rectangle rectangle)
        {
            return new Vector2(rectangle.Left + rectangle.Width / 2, rectangle.Top + rectangle.Height / 2);
        }

        public static float CalculateAspectFitScalingFactor(Vector2 sourceSize, Vector2 destinationSize)
        {
            float sourceAR = sourceSize.X / sourceSize.Y;
            float destAR = destinationSize.X / destinationSize.Y;

            float scale = 1f;

            if (sourceAR > destAR)
            {
                scale = destinationSize.X / sourceSize.X;
            }
            else
            {
                scale = destinationSize.Y / sourceSize.Y;
            }
            return scale;
        }
    }
}
